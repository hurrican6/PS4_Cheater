using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;

namespace PS4_Cheater
{ 

    public class ResultList
    {
        private const int buffer_size = 4096 * 16;
        private List<byte[]> buffer_list = new List<byte[]>();

        private int buffer_tag_offset = 0;
        private int buffer_tag_elem_count = 0;
        private int buffer_id = 0;

        private int count = 0;
        private int iterator = 0;
        private int element_size = 0;
        private int element_alignment = 1;

        private const int OFFSET_SIZE = 4;
        private const int BIT_MAP_SIZE = 8;

        public ResultList(int element_size, int element_alignment)
        {
            buffer_list.Add(new byte[buffer_size]);
            this.element_size = element_size;
            this.element_alignment = element_alignment;
        }

        private int bit_count(ulong data, int end)
        {
            int sum = 0;
            for (int i = 0; i <= end; ++i)
            {
                if ((data & (1ul << i)) != 0)
                {
                    ++sum;
                }
            }
            return sum;
        }

        private int bit_position(ulong data, int pos)
        {
            int sum = 0;
            for (int i = 0; i <= 64; ++i)
            {
                if ((data & (1ul << i)) != 0)
                {
                    if (sum == pos)
                    {
                        return i;
                    }
                    ++sum;
                }
            }
            return -1;
        }

        public void Add(uint memoryAddressOffset, byte[] memoryValue)
        {
            if (memoryValue.Length != element_size)
            {
                throw new Exception("Invalid address!");
            }

            byte[] dense_buffer = buffer_list[buffer_id];
            
            uint tag_address_offset_base = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + OFFSET_SIZE);

            if (tag_address_offset_base > memoryAddressOffset)
            {
                throw new Exception("Invalid address!");
            }

            if (bit_map == 0)
            {
                tag_address_offset_base = memoryAddressOffset;
                Buffer.BlockCopy(BitConverter.GetBytes(memoryAddressOffset), 0, dense_buffer, buffer_tag_offset, OFFSET_SIZE);
            }

            int offset_in_bit_map = (int)(memoryAddressOffset - tag_address_offset_base) / element_alignment;
            if (offset_in_bit_map < 64)
            {
                dense_buffer[buffer_tag_offset + OFFSET_SIZE + offset_in_bit_map / 8] |= (byte)(1 << (offset_in_bit_map % 8)); //bit map
                Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, element_size);//value
                ++buffer_tag_elem_count;
            }
            else
            {
                buffer_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count;

                if (buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= buffer_size)
                {
                    buffer_list.Add(new byte[buffer_size]);
                    ++buffer_id;
                    buffer_tag_offset = 0;
                    buffer_tag_elem_count = 0;
                    dense_buffer = buffer_list[buffer_id];
                }

                Buffer.BlockCopy(BitConverter.GetBytes(memoryAddressOffset), 0, dense_buffer, buffer_tag_offset, OFFSET_SIZE); //tag address base
                dense_buffer[buffer_tag_offset + OFFSET_SIZE] = (byte)1; //bit map
                Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE, element_size); //value
                buffer_tag_elem_count = 1;
            }
            count++;
        }

        public void Clear()
        {
            count = 0;
            buffer_tag_offset = 0;
            buffer_tag_elem_count = 0;
            buffer_id = 0;
            buffer_list.Clear();
            buffer_list.Add(new byte[buffer_size]);
        }

        public void Get(ref uint memoryAddressOffset, ref byte[] memoryValue)
        {

            byte[] dense_buffer = buffer_list[buffer_id];
            memoryValue = new byte[element_size];

            uint offset_base = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + OFFSET_SIZE);
            memoryAddressOffset = (uint)(bit_position(bit_map, buffer_tag_elem_count) * element_alignment) + offset_base;
            Buffer.BlockCopy(dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, memoryValue, 0, element_size);
            
        }

        public void Set(byte[] memoryValue)
        {
            byte[] dense_buffer = buffer_list[buffer_id];
            Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, element_size);
        }

        public void Begin()
        {
            iterator = 0;
            buffer_tag_offset = 0;
            buffer_tag_elem_count = 0;
            buffer_id = 0;
        }

        public void Next()
        {
            ++iterator;

            byte[] dense_buffer = buffer_list[buffer_id];
            uint base_offset = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + 4);
            ++buffer_tag_elem_count;

            if (bit_count(bit_map, 63) <= buffer_tag_elem_count)
            {
                buffer_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count;
                if (buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= buffer_size)
                {
                    ++buffer_id;
                    buffer_tag_offset = 0;
                    buffer_tag_elem_count = 0;
                }
                else
                {
                    buffer_tag_elem_count = 0;
                }
            }
        }

        public bool End()
        {
            return (iterator == count);
        }

        public int Count { get { return count; } }
    }

    public class MappedSection
    {
        public ulong Start { get; set; }
        public int Length { get; set; }
        public string Name { get; set; }
        public bool Check { set; get; }
        public uint Prot { get; set; }

        public ResultList ResultList { get; set; }

        public MappedSection()
        {
            ResultList = null;
        }

        public void UpdateResultList(ProcessManager processManager, MemoryHelper memoryHelper,
            string default_value_0_str, string default_value_1_str, bool newScan)
        {
            if (!Check)
            {
                ResultList = null;
                return;
            }

            ResultList new_result_list = new ResultList(memoryHelper.Length, memoryHelper.Alignment);

            ulong address = this.Start;
            uint base_address = 0;
            int length = this.Length;

            const int buffer_length = 1024 * 1024 * 128;

            while (length != 0)
            {
                int cur_length = buffer_length;

                if (cur_length > length)
                {
                    cur_length = length;
                    length = 0;
                }
                else
                {
                    length -= cur_length;
                }

                byte[] buffer = MemoryHelper.ReadMemory(address, (int)cur_length);

                byte[] default_value_0 = null;
                if (memoryHelper.ParseFirstValue)
                {
                    default_value_0 = memoryHelper.StringToBytes(default_value_0_str);
                }

                byte[] default_value_1 = null;
                if (memoryHelper.ParseSecondValue)
                {
                    default_value_1 = memoryHelper.StringToBytes(default_value_1_str);
                }

                if (newScan)
                {
                    memoryHelper.CompareWithMemoryBufferNewScanner(default_value_0, default_value_1, buffer, new_result_list, base_address);
                }
                else
                {
                    memoryHelper.CompareWithMemoryBufferNextScanner(default_value_0, default_value_1, buffer, ResultList, new_result_list);
                }

                address += (ulong)cur_length;
                base_address += (uint)cur_length;
            }
            ResultList = new_result_list;
        }
    }

    public class ProcessManager
    {
        public ulong TotalMemorySize { get; set; }

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
                    ulong buffer_length = 1024 * 1024 * 128;

                    //Executable section
                    if ((entry.prot & 0x5) == 0x5)
                    {
                        buffer_length = length;
                    }

                    while (length != 0)
                    {
                        ulong cur_length = buffer_length;

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

        public ulong TotalResultCount()
        {
            ulong total_result_count = 0;
            for (int idx = 0; idx < MappedSectionList.Count; ++idx)
            {
                if (MappedSectionList[idx].Check && MappedSectionList[idx].ResultList != null)
                {
                    total_result_count += (ulong)MappedSectionList[idx].ResultList.Count;
                }
            }
            return total_result_count;
        }

        public void ClearResultList()
        {
            for (int idx = 0; idx < MappedSectionList.Count; ++idx)
            {
                if (MappedSectionList[idx].ResultList != null)
                {
                    MappedSectionList[idx].ResultList.Clear();
                }
            }
        }
    }
}
