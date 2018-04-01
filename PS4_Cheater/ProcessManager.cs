using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;

namespace PS4_Cheater
{

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct Pointer
    {
        public ulong Address { get; }
        public ulong PointerValue { get; }

        public Pointer(ulong Address, ulong PointerValue)
        {
            this.Address = Address;
            this.PointerValue = PointerValue;
        }
    }

    public class PointerPath
    {
        public List<ulong> pointerPath = new List<ulong>();

        public void AddRange(List<ulong> pointerList)
        {
            pointerPath.AddRange(pointerList);
        }

        public void Add(ulong pointer)
        {
            pointerPath.Add(pointer);
        }

        public ulong this[int index]
        {
            get {
                return pointerPath[index];
            }
        }

        public int Count { get { return pointerPath.Count; } }
    }

    public class PointerList
    {
        private List<Pointer> pointer_list_order_by_address;
        private List<Pointer> pointer_list_order_by_pointer_value;

        public bool Stop { get; set; }

        public PointerList()
        {
            pointer_list_order_by_address = new List<Pointer>();
            pointer_list_order_by_pointer_value = new List<Pointer>();
        }

        public delegate void NewPathGeneratedHandler(PointerList pointerList, List<long> path_offset, List<Pointer> path_address);

        public event NewPathGeneratedHandler NewPathGeneratedEvent;

        class ComparerByAddress : IComparer<Pointer>
        {
            public int Compare(Pointer x, Pointer y)
            {
                if (x.Address == y.Address)
                {
                    return 0;
                }
                else if (x.Address < y.Address) {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        class ComparerByPointerValue : IComparer<Pointer>
        {
            public int Compare(Pointer x, Pointer y)
            {
                if (x.PointerValue == y.PointerValue)
                {
                    return 0;
                }
                else if (x.PointerValue < y.PointerValue)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public void Add(Pointer pointer)
        {
            pointer_list_order_by_address.Add(pointer);
            pointer_list_order_by_pointer_value.Add(pointer);
        }

        public void Clear()
        {
            pointer_list_order_by_address.Clear();
            pointer_list_order_by_pointer_value.Clear();
        }

        private static int BinarySearchByAddress(List<Pointer> pointerList, int low, int high, ulong address)
        {
            int mid = (low + high) / 2;
            if (low > high)
            {
                return -1;
            }
            else
            {
                if (pointerList[mid].Address == address)
                    return mid;
                else if (pointerList[mid].Address > address)
                {
                    if (mid - 1 >= 0 && pointerList[mid - 1].Address <= address)
                    {
                        return mid - 1;
                    }
                    return BinarySearchByAddress(pointerList, low, mid - 1, address);
                }
                else
                {
                    if (mid + 1 < pointerList.Count && pointerList[mid + 1].Address >= address)
                    {
                        return mid + 1;
                    }
                    return BinarySearchByAddress(pointerList, mid + 1, high, address);
                }
            }
        }

        private static int BinarySearchByValue(List<Pointer> pointerList, int low, int high, ulong pointerValue)
        {
            int mid = (low + high) / 2;
            if (low > high)
            {
                return -1;
            }
            else
            {
                if (pointerList[mid].PointerValue == pointerValue)
                    return mid;
                else if (pointerList[mid].PointerValue > pointerValue)
                    return BinarySearchByValue(pointerList, low, mid - 1, pointerValue);
                else
                    return BinarySearchByValue(pointerList, mid + 1, high, pointerValue);
            }
        }

        private List<Pointer> GetPointerListByValue(ulong pointerValue)
        {
            List<Pointer> pointerList = new List<Pointer>();
            int index = BinarySearchByValue(pointer_list_order_by_pointer_value, 0, pointer_list_order_by_pointer_value.Count - 1, pointerValue);

            if (index < 0) return pointerList;

            int start = index;
            for (; start >= 0; --start)
            {
                if (pointer_list_order_by_pointer_value[start].PointerValue != pointerValue)
                {
                    break;
                }
            }

            bool find = false;
            for (int i = start; i < pointer_list_order_by_pointer_value.Count; ++i)
            {
                if (pointer_list_order_by_pointer_value[i].PointerValue == pointerValue)
                {
                    find = true;
                    pointerList.Add(pointer_list_order_by_pointer_value[i]);
                }
                else
                {
                    if (find) break;
                }
            }

            return pointerList;
        }

        private int GetPointerByAddress(ulong address, ref Pointer pointer)
        {
            int index = BinarySearchByAddress(pointer_list_order_by_address, 0, pointer_list_order_by_address.Count - 1, address);
            if (index < 0) return index;

            pointer = pointer_list_order_by_address[index];
            return index;
        }

        private void PointerFinder(List<long> path_offset, List<Pointer> path_address,
            ulong address, List<int> range, int level, ref bool changed)
        {

            if (level == range.Count)
            {
                changed = false;
                //NewPathGeneratedEvent?.Invoke(this, path_offset, path_address);
                return;
            }

            if (Stop)
            {
                return;
            }

            bool local_changed = false;
            if ((level + 1) < range.Count)
            {

                Pointer position = new Pointer();
                int index = GetPointerByAddress(address, ref position);
                int counter = 0;

                for (int i = index; i >= 0; i--)
                {
                    if (Stop)
                    {
                        break;
                    }

                    if ((long)pointer_list_order_by_address[i].Address + range[0] < (long)address)
                    {
                        break;
                    }

                    List<Pointer> pointerList = GetPointerListByValue(pointer_list_order_by_address[i].Address);

                    if (pointerList.Count > 0)
                    {
                        path_offset.Add((long)(address - pointer_list_order_by_address[i].Address));

                        for (int j = 0; j < pointerList.Count; ++j)
                        {
                            bool in_stack = false;
                            for (int k = 0; k < path_address.Count; ++k)
                            {
                                if (path_address[k].PointerValue == pointerList[j].PointerValue ||
                                    path_address[k].Address == pointerList[j].Address)
                                {
                                    in_stack = true;
                                    break;
                                }
                            }
                            if (in_stack)
                            {
                                continue;
                            }

                            bool sub_changed = false;

                            path_address.Add(pointerList[j]);
                            PointerFinder(path_offset, path_address, pointerList[j].Address, range, level + 1, ref sub_changed);
                            path_address.RemoveAt(path_address.Count - 1);

                            local_changed |= sub_changed;
                        }

                        path_offset.RemoveAt(path_offset.Count - 1);

                        if (counter > 1)
                        {
                            break;
                        }
                    }

                    ++counter;
                }
            }
            else
            {
                local_changed = true;
            }

            if (Stop)
            {
                return;
            }

            if (!local_changed)
            {
                changed = true;
                NewPathGeneratedEvent?.Invoke(this, path_offset, path_address);
            }
        }

        public void Save()
        {
            string ADDRESS_NAME = "D:\\name.txt";

            string[] lines = new string[pointer_list_order_by_address.Count];

            for (int i = 0; i< pointer_list_order_by_address.Count; ++i)
            {
                lines[i] = pointer_list_order_by_address[i].Address.ToString() + " " + pointer_list_order_by_address[i].PointerValue.ToString();
            }
            File.WriteAllLines(ADDRESS_NAME, lines);
        }

        public void Load()
        {
            string ADDRESS_NAME = "D:\\name.txt";

            string[] lines = File.ReadAllLines(ADDRESS_NAME);

            for (int i = 0; i < lines.Length; ++i)
            {
                string[] elems = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                ulong Address = UInt64.Parse(elems[0]);
                ulong PointerValue = UInt64.Parse(elems[1]);

                Pointer pointer = new Pointer(Address, PointerValue);
                pointer_list_order_by_address.Add(pointer);
                pointer_list_order_by_pointer_value.Add(pointer);
            }

            pointer_list_order_by_pointer_value.Sort(new ComparerByPointerValue());

        }

        public void Init()
        {
            pointer_list_order_by_address.Sort(new ComparerByAddress());
            pointer_list_order_by_pointer_value.Sort(new ComparerByPointerValue());
        }

        public void FindPointerList(ulong address, List<int> range)
        {
            List<long> path_offset = new List<long>();
            List<Pointer> path_address = new List<Pointer>();
            bool changed = true;
            PointerFinder(path_offset, path_address, address, range, 0, ref changed);
        }

        public int Count { get { return pointer_list_order_by_address.Count; } }
    }

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
            for (int i = 0; i <= 63; ++i)
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

                //Alloc new page
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
            string default_value_0_str, string default_value_1_str, bool is_hex, bool newScan)
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
                    if (is_hex)
                    {
                        default_value_0 = memoryHelper.HexStringToBytes(default_value_0_str);
                    }
                    else
                    {
                        default_value_0 = memoryHelper.StringToBytes(default_value_0_str);
                    }
                }

                byte[] default_value_1 = null;
                if (memoryHelper.ParseSecondValue)
                {
                    if (is_hex)
                    {
                        default_value_1 = memoryHelper.HexStringToBytes(default_value_1_str);
                    }
                    else
                    {
                        default_value_1 = memoryHelper.StringToBytes(default_value_1_str);
                    }
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

        public void PointerSearchInit(ProcessManager processManager, MemoryHelper memoryHelper, PointerList pointerList)
        {
            ulong address = this.Start;
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

                memoryHelper.CompareWithMemoryBufferPointerScanner(processManager, buffer, pointerList, address);

                address += (ulong)cur_length;
            }
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

        private int BinarySearch(int low, int high, ulong address)
        {
            int mid = (low + high) / 2;
            if (low > high)
                return -1;
            else
            {
                if ((MappedSectionList[mid].Start <= address) && (MappedSectionList[mid].Start + (ulong)(MappedSectionList[mid].Length) >= address))
                    return mid;
                else if (MappedSectionList[mid].Start > address)
                    return BinarySearch(low, mid - 1, address);
                else
                    return BinarySearch(mid + 1, high, address);
            }
        }

        public int GetMappedSectionID(ulong address)
        {
            ulong start = 0;
            ulong end = 0;

            if (MappedSectionList.Count > 0)
            {
                start = MappedSectionList[0].Start;
                end = MappedSectionList[MappedSectionList.Count - 1].Start + (ulong)MappedSectionList[MappedSectionList.Count - 1].Length;
            }

            if (start > address || end < address)
            {
                return -1;
            }

            return BinarySearch(0, MappedSectionList.Count - 1, address);
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
            if (section_idx < 0)
            {
                return "sectioni wrong!";
            }
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
