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
        public ulong Address;
        public ulong PointerValue;

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
            get
            {
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
                else if (x.Address < y.Address)
                {
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
                    if (x.Address == y.Address)
                    {
                        return 0;
                    }
                    else if (x.Address < y.Address)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
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

        private static int BinarySearchByValue(List<Pointer> pointerList, ulong pointerValue)
        {
            int low = 0;
            int high = pointerList.Count - 1;
            int middle;

            while (low <= high)
            {
                middle = (low + high) / 2;
                if (pointerValue > pointerList[middle].PointerValue)
                {
                    low = middle + 1;
                }
                else if (pointerValue < pointerList[middle].PointerValue)
                {
                    high = middle - 1;
                }
                else
                {
                    return middle;
                }
            }

            return -1;
        }

        public ulong GetTailAddress(PointerResult pointerResult, MappedSectionList mappedSectionList)
        {
            ulong tailAddress = pointerResult.GetBaseAddress(mappedSectionList);

            if (pointerResult.Offsets.Length > 0)
            {
                int j = 0;
                Pointer pointer = new Pointer();
                int index = GetPointerByAddress(tailAddress, ref pointer);
                if (index < 0) return 0;
                tailAddress = pointer.PointerValue;
                for (j = 0; j < pointerResult.Offsets.Length - 1; ++j)
                {
                    index = GetPointerByAddress((ulong)((long)tailAddress + pointerResult.Offsets[j]), ref pointer);
                    if (index < 0) return 0;
                    tailAddress = pointer.PointerValue;
                }

                tailAddress = (ulong)((long)tailAddress + pointerResult.Offsets[j]);
            }

            return tailAddress;
        }

        private List<Pointer> GetPointerListByValue(ulong pointerValue)
        {
            List<Pointer> pointerList = new List<Pointer>();
            int index = BinarySearchByValue(pointer_list_order_by_pointer_value, pointerValue);

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
            ulong address, List<int> range, int level)
        {

            if (Stop)
            {
                return;
            }
            
            if (level < range.Count)
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

                    if ((long)pointer_list_order_by_address[i].Address + range[level] < (long)address)
                    {
                        break;
                    }

                    List<Pointer> pointerList = GetPointerListByValue(pointer_list_order_by_address[i].Address);

                    if (pointerList.Count > 0)
                    {
                        path_offset.Add((long)(address - pointer_list_order_by_address[i].Address));
                        const int max_pointer_count =  15;
                        int cur_pointer_counter = 0;

                        bool in_new_level = false;

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

                            in_new_level = true;
                            if (cur_pointer_counter >= max_pointer_count) break;

                            ++cur_pointer_counter;

                            path_address.Add(pointerList[j]);
                            PointerFinder(path_offset, path_address, pointerList[j].Address, range, level + 1);
                            path_address.RemoveAt(path_address.Count - 1);
                        }

                        path_offset.RemoveAt(path_offset.Count - 1);

                        if (counter >= 1)
                        {
                            break;
                        }

                        if (in_new_level) ++counter;
                    }
                }
            }

            if (Stop)
            {
                return;
            }

            NewPathGeneratedEvent?.Invoke(this, path_offset, path_address);
        }

        public void Save()
        {
            string ADDRESS_NAME = "D:\\name.txt";

            string[] lines = new string[pointer_list_order_by_address.Count];

            for (int i = 0; i < pointer_list_order_by_address.Count; ++i)
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
            PointerFinder(path_offset, path_address, address, range, 0);
        }

        public int Count { get { return pointer_list_order_by_address.Count; } }
    }

}
