using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;

namespace PS4_Cheater
{ 

    public class AddressList
    {
        private const int block_size = 4096 * 16;
        private List<byte[]> block_list = new List<byte[]>();

        private int block_tag_offset = 0;
        private int block_tag_elem_count = 0;
        private int block_id = 0;

        private int count = 0;
        private int iterator = 0;
        private int element_size = 0;
        private int element_alignment = 1;

        private const int OFFSET_SIZE = 4;
        private const int BIT_MAP_SIZE = 8;

        public AddressList(int element_size, int element_alignment)
        {
            block_list.Add(new byte[block_size]);
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

        public void Add(uint memoryAddress, byte[] memoryValue)
        {
            int o_block_id = block_id;
            int o_block_tag_offset = block_tag_offset;
            int o_block_tag_elem_count = block_tag_elem_count;

            if (memoryValue.Length != element_size)
            {
                throw new Exception("Invalid address!");
            }

            byte[] dense_buffer = block_list[block_id];
            uint address = memoryAddress;
            
            uint tag_address_offset_base = BitConverter.ToUInt32(dense_buffer, block_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, block_tag_offset + OFFSET_SIZE);

            if (tag_address_offset_base > address)
            {
                throw new Exception("Invalid address!");
            }

            if (bit_map == 0)
            {
                tag_address_offset_base = address;
                Buffer.BlockCopy(BitConverter.GetBytes(address), 0, dense_buffer, block_tag_offset, OFFSET_SIZE);
            }

            int offset_in_bit_map = (int)(address - tag_address_offset_base) / element_alignment;
            if (offset_in_bit_map < 64)
            {
                dense_buffer[block_tag_offset + OFFSET_SIZE + offset_in_bit_map / 8] |= (byte)(1 << (offset_in_bit_map % 8)); //bit map
                Buffer.BlockCopy(memoryValue, 0, dense_buffer, block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * block_tag_elem_count, element_size);//value
                ++block_tag_elem_count;
            }
            else
            {
                block_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * block_tag_elem_count;

                if (block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= block_size)
                {
                    block_list.Add(new byte[block_size]);
                    ++block_id;
                    block_tag_offset = 0;
                    block_tag_elem_count = 0;
                    dense_buffer = block_list[block_id];
                }

                dense_buffer[block_tag_offset + OFFSET_SIZE] = (byte)1; //bit map
                Buffer.BlockCopy(BitConverter.GetBytes(address), 0, dense_buffer, block_tag_offset, OFFSET_SIZE); //tag address base
                Buffer.BlockCopy(memoryValue, 0, dense_buffer, block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE, element_size); //value
                block_tag_elem_count = 1;
            }
            count++;
        }

        public void Clear()
        {
            count = 0;
            block_tag_offset = 0;
            block_tag_elem_count = 0;
            block_id = 0;
            block_list.Clear();
            block_list.Add(new byte[block_size]);
        }

        public void Get(ref uint addressOffset, ref byte[] memoryValue)
        {

            byte[] dense_buffer = block_list[block_id];
            memoryValue = new byte[element_size];

            uint offset_base = BitConverter.ToUInt32(dense_buffer, block_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, block_tag_offset + OFFSET_SIZE);
            addressOffset = (uint)(bit_position(bit_map, block_tag_elem_count) * element_alignment) + offset_base;
            Buffer.BlockCopy(dense_buffer, block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * block_tag_elem_count, memoryValue, 0, element_size);
            
        }

        public void Set(byte[] memoryValue)
        {
            byte[] dense_buffer = block_list[block_id];
            Buffer.BlockCopy(memoryValue, 0, dense_buffer, block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * block_tag_elem_count, element_size);
        }

        public void Begin()
        {
            iterator = 0;
            block_tag_offset = 0;
            block_tag_elem_count = 0;
            block_id = 0;
        }

        private int o_tag_offset = 0;
        private int o_tag_count = 0;
        private int o_bit_count = 0;

        public void Next()
        {
            ++iterator;

            byte[] dense_buffer = block_list[block_id];
            uint base_offset = BitConverter.ToUInt32(dense_buffer, block_tag_offset);
            ulong bit_map = BitConverter.ToUInt64(dense_buffer, block_tag_offset + 4);
            o_tag_offset = block_tag_offset;
            o_tag_count = block_tag_elem_count;
            o_bit_count = bit_count(bit_map, 63);
            ++block_tag_elem_count;

            if (bit_count(bit_map, 63) <= block_tag_elem_count)
            {
                block_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * block_tag_elem_count;
                if (block_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= block_size)
                {
                    ++block_id;
                    block_tag_offset = 0;
                    block_tag_elem_count = 0;
                }
                else
                {
                    block_tag_elem_count = 0;
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

        public AddressList AddressList { get; set; }

        public MappedSection()
        {
            AddressList = null;
        }

        public void UpdateAddressList(ProcessManager processManager, MemoryHelper memoryHelper,
            string default_value_0_str, string default_value_1_str, bool newScan)
        {
            if (!Check)
            {
                AddressList = null;
                return;
            }

            AddressList new_address_list = new AddressList(memoryHelper.Length, memoryHelper.Alignment);

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

                byte[] buffer = MemoryHelper.ReadMemory(address, (int)cur_length);

                byte[] default_value_0 = memoryHelper.StringToBytes(default_value_0_str);
                byte[] default_value_1 = memoryHelper.StringToBytes(default_value_1_str);

                if (newScan)
                {
                    memoryHelper.CompareWithMemoryBufferNewScanner(default_value_0, default_value_1, buffer, AddressList, new_address_list);
                }
                else
                {
                    memoryHelper.CompareWithMemoryBufferNextScanner(default_value_0, default_value_1, buffer, AddressList, new_address_list);
                }
                address += (ulong)cur_length;
            }
            AddressList = new_address_list;
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
                if (MappedSectionList[idx].Check)
                {
                    total_address_count += (ulong)MappedSectionList[idx].AddressList.Count;
                }
            }
            return total_address_count;
        }

        public void ClearAddressList()
        {
            for (int idx = 0; idx < MappedSectionList.Count; ++idx)
            {
                if (MappedSectionList[idx].AddressList != null)
                {
                    MappedSectionList[idx].AddressList.Clear();
                }
            }
        }
    }
}
