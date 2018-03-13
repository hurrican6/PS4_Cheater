using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;

namespace PS4_Cheater
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct Address
    {
        public uint AddressOffset;

        public uint MemoryValue;
    }

    public class AddressList
    {
        private List<Address> address_list = new List<Address>();

        public void Add(Address address)
        {
            address_list.Add(address);
        }

        public void Clear()
        {
            address_list.Clear();
        }

        public void RemoveAt(int idx)
        {
            address_list.RemoveAt(idx);
        }

        public Address this[int index]
        {
            get
            {
                return address_list[index];
            }
            set
            {
                address_list[index] = value;
            }
        }

        public AddressList Intersect(MemoryHelper memoryHelper, AddressList filteredList, string default_compare_value)
        {

            AddressList new_address_list = new AddressList();
            int idx_i = 0;
            int idx_j = 0;

            while (idx_i < address_list.Count && idx_j < filteredList.Count)
            {
                Address address = address_list[idx_i];
                Address filtered = filteredList[idx_j];

                if (address.AddressOffset == filtered.AddressOffset)
                {
                    byte[] value = memoryHelper.UintToBytes(filtered.MemoryValue);

                    byte[] compare_value = memoryHelper.GetCompareBytes(address, default_compare_value);

                    if (memoryHelper.Compare(compare_value, value))
                    {
                        new_address_list.Add(filtered);
                    }

                    idx_j++;
                    idx_i++;
                }
                else if (address.AddressOffset > filtered.AddressOffset)
                {
                    idx_j++;
                }
                else
                {
                    idx_i++;
                }
            }

            return new_address_list;
        }
        
        public int Count { get { return address_list.Count; } }
    }

    public class MappedSection
    {
        public ulong Start { get; set; }
        public int Length { get; set; }
        public string Name { get; set; }
        public bool Check { set; get; }
        public uint Prot { get; set; }

        public AddressList AddressList { get; set; }

        public MappedSection()
        {
            AddressList = new AddressList();
        }

        public AddressList getFilteredAddressList(ProcessManager processManager,
            string value, BackgroundWorker worker, ref ulong percent_len, int start, float percent)
        {
            AddressList filtered_list = new AddressList();
            worker.ReportProgress(start);

            if (!Check)
            {
                return filtered_list;
            }

            ulong address = this.Start;
            int length = this.Length;

            const int block_length = 1024 * 1024 * 128;

            while (length != 0)
            {
                int cur_length = block_length;

                if (cur_length > length)
                {
                    cur_length = length;
                    length = 0;
                }
                else
                {
                    length -= cur_length;
                }

                percent_len += (ulong)cur_length;
                worker.ReportProgress(start + (int)(((float)percent_len / processManager.TotalMemorySize) * 100 * percent));

                byte[] buffer = processManager.MemoryHelper.ReadMemory(address, (int)cur_length);

                byte[] match_value = processManager.MemoryHelper.StringToBytes(value);

                processManager.MemoryHelper.CompareWithFilterList(match_value, address, buffer, filtered_list);

                address += (ulong)cur_length;
            }
            return filtered_list;
        }
    }

    public class ProcessManager
    {
        public ulong TotalMemorySize { get; set; }
        public int ProcessID { get; set; }

        public MemoryHelper MemoryHelper { get; set; }

        public List<MappedSection> mapped_section_list { get; set; }

        public ProcessManager()
        {
            mapped_section_list = new List<MappedSection>();
        }

        public MappedSection GetMappedSection(int idx)
        {
            return mapped_section_list[idx];
        }

        public int GetSectionInfoCount()
        {
            return mapped_section_list.Count;
        }

        public int GetSectionInfoIdx(ulong address)
        {
            for (int i = 0; i < mapped_section_list.Count; ++i)
            {
                MappedSection sectionInfo = mapped_section_list[i];
                if (sectionInfo.Start <= address && (sectionInfo.Start + (ulong)sectionInfo.Length) >= address)
                {
                    return i;
                }
            }

            return -1;
        }

        public void SectionCheck(int idx, bool _checked)
        {
            mapped_section_list[idx].Check = _checked;
            if (mapped_section_list[idx].Check)
            {
                TotalMemorySize += (ulong)mapped_section_list[idx].Length;
            }
            else
            {
                TotalMemorySize -= (ulong)mapped_section_list[idx].Length;
            }
        }

        public ProcessInfo GetProcessInfo(string process_name)
        {
            //this.ps4.Connect();
            ProcessList processList = MemoryHelper.GetProcessList();
            ProcessInfo processInfo = null;
            foreach (Process process in processList.processes)
            {
                if (process.name == process_name)
                {
                    ProcessID = process.pid;
                    processInfo = MemoryHelper.GetProcessInfo(process.pid);
                    break;
                }
            }

            return processInfo;
        }

        public string GetProcessName(int idx)
        {
            ProcessList processList = MemoryHelper.GetProcessList();
            return processList.processes[idx].name;
        }

        public string GetSectionName(int section_idx)
        {
            MappedSection sectionInfo = mapped_section_list[section_idx];

            StringBuilder section_name = new StringBuilder();
            section_name.Append(sectionInfo.Name + "-");
            section_name.Append(String.Format("{0:X}", sectionInfo.Prot) + "-");
            section_name.Append(String.Format("{0:X}", sectionInfo.Start) + "-");
            section_name.Append((sectionInfo.Length / 1024).ToString() + "KB");

            return section_name.ToString();
        }

        public void InitMemorySectionList(ProcessInfo pi)
        {
            mapped_section_list.Clear();
            TotalMemorySize = 0;

            for (int i = 0; i < pi.entries.Length; i++)
            {
                MemoryEntry entry = pi.entries[i];
                if ((entry.prot & 0x1) == 0x1)
                {
                    ulong length = entry.end - entry.start;
                    ulong start = entry.start;
                    string name = entry.name;
                    int idx = 0;
                    ulong block_length = 1024 * 1024 * 128;

                    if ((entry.prot & 0x5) == 0x5)
                    {
                        block_length = length;
                    }

                    while (length != 0)
                    {
                        ulong cur_length = block_length;

                        if (cur_length > length)
                        {
                            cur_length = length;
                            length = 0;
                        }
                        else
                        {
                            length -= cur_length;
                        }

                        MappedSection section_info = new MappedSection();
                        section_info.Start = start;
                        section_info.Length = (int)cur_length;
                        section_info.Name = entry.name + "[" + idx + "]";
                        section_info.Check = false;
                        section_info.Prot = entry.prot;

                        mapped_section_list.Add(section_info);

                        start += cur_length;
                        ++idx;
                    }
                }
            }

        }

        public ulong TotalAddressCount()
        {
            ulong total_address_count = 0;
            for (int idx = 0; idx < mapped_section_list.Count; ++idx)
            {
                total_address_count += (ulong)mapped_section_list[idx].AddressList.Count;
            }
            return total_address_count;
        }

        public void ClearAddressList()
        {
            for (int idx = 0; idx < mapped_section_list.Count; ++idx)
            {
                mapped_section_list[idx].AddressList.Clear(); ;
            }
        }
    }
}
