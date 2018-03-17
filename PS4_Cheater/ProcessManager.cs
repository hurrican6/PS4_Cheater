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

        public ulong MemoryValue;
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
                    byte[] value = memoryHelper.UlongToBytes(filtered.MemoryValue);

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

        public AddressList getFilteredAddressList(ProcessManager processManager, MemoryHelper memoryHelper,
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

                byte[] buffer = MemoryHelper.ReadMemory(address, (int)cur_length);

                byte[] match_value = memoryHelper.StringToBytes(value);

                memoryHelper.CompareWithFilterList(match_value, address, buffer, filtered_list);

                address += (ulong)cur_length;
            }
            return filtered_list;
        }
    }

    public class ProcessManager
    {
        public ulong TotalMemorySize { get; set; }
        //public int ProcessID { get; set; }

        public List<MappedSection> MappedSectionList { get; }

        public ProcessManager()
        {
            MappedSectionList = new List<MappedSection>();
        }

        public MappedSection GetMappedSection(int idx)
        {
            return MappedSectionList[idx];
        }

        public int GetSectionInfoCount()
        {
            return MappedSectionList.Count;
        }

        public int GetMappedSectionID(ulong address)
        {
            for (int i = 0; i < MappedSectionList.Count; ++i)
            {
                MappedSection sectionInfo = MappedSectionList[i];
                if (sectionInfo.Start <= address && (sectionInfo.Start + (ulong)sectionInfo.Length) >= address)
                {
                    return i;
                }
            }

            return -1;
        }

        public MappedSection GetMappedSection(ulong address)
        {
            int sectionID = GetMappedSectionID(address);
            if (sectionID < 0)
            {
                return null;
            }
            return MappedSectionList[sectionID];
        }

        public void SectionCheck(int idx, bool _checked)
        {
            MappedSectionList[idx].Check = _checked;
            if (MappedSectionList[idx].Check)
            {
                TotalMemorySize += (ulong)MappedSectionList[idx].Length;
            }
            else
            {
                TotalMemorySize -= (ulong)MappedSectionList[idx].Length;
            }
        }

        public ProcessInfo GetProcessInfo(string process_name)
        {
            ProcessList processList = MemoryHelper.GetProcessList();
            ProcessInfo processInfo = null;
            foreach (Process process in processList.processes)
            {
                if (process.name == process_name)
                {
                    processInfo = MemoryHelper.GetProcessInfo(process.pid);
                    MemoryHelper.ProcessID = process.pid;
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
            MappedSection sectionInfo = MappedSectionList[section_idx];

            StringBuilder section_name = new StringBuilder();
            section_name.Append(sectionInfo.Name + "-");
            section_name.Append(String.Format("{0:X}", sectionInfo.Prot) + "-");
            section_name.Append(String.Format("{0:X}", sectionInfo.Start) + "-");
            section_name.Append((sectionInfo.Length / 1024).ToString() + "KB");

            return section_name.ToString();
        }

        public void InitMemorySectionList(ProcessInfo pi)
        {
            MappedSectionList.Clear();
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

                    //Executable section
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

                        MappedSection mappedSection = new MappedSection();
                        mappedSection.Start = start;
                        mappedSection.Length = (int)cur_length;
                        mappedSection.Name = entry.name + "[" + idx + "]";
                        mappedSection.Check = false;
                        mappedSection.Prot = entry.prot;

                        MappedSectionList.Add(mappedSection);

                        start += cur_length;
                        ++idx;
                    }
                }
            }

        }

        public ulong TotalAddressCount()
        {
            ulong total_address_count = 0;
            for (int idx = 0; idx < MappedSectionList.Count; ++idx)
            {
                total_address_count += (ulong)MappedSectionList[idx].AddressList.Count;
            }
            return total_address_count;
        }

        public void ClearAddressList()
        {
            for (int idx = 0; idx < MappedSectionList.Count; ++idx)
            {
                MappedSectionList[idx].AddressList.Clear(); ;
            }
        }
    }
}
