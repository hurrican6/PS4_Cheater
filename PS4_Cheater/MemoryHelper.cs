using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;
using System.Threading;

namespace PS4_Cheater
{
    public enum CompareType
    {
        EXACT_VALUE,
        INCREASED_VALUE,
        DECREASED_VALUE,
        BIGGER_THAN_VALUE,
        SMALLER_THAN_VALUE,
        CHANGED_VALUE,
        UNCHANGED_VALUE,
        UNKNOWN_INITIAL_VALUE,
        NONE,
    }

    public enum ValueType
    {
        INT_TYPE,
        SHORT_TYPE,
        FLOAT_TYPE,
        HEX_TYPE,
        NONE,
    }

    public class MemoryHelper
    {
        private PS4RPC ps4;
        private Mutex mutex;
        public int ProcessID;

        public MemoryHelper(PS4RPC ps4)
        {
            mutex = new Mutex();
            this.ps4 = ps4;
        }

        public delegate string BytesToStringHandler(Byte[] value);
        public delegate Byte[] StringToBytesHandler(string value);
        public delegate Byte[] GetBytesByTypeHandler(ulong address);
        public delegate void CompareWithFilterListHandler(Byte[] match_value, ulong address, byte[] mem,
            AddressList filtered_lists);
        public delegate void SetBytesByTypeHandler(ulong address, byte[] value);
        public delegate uint BytesToUintHandler(byte[] value);
        public delegate byte[] UintToBytesHandler(uint value);
        public delegate bool CompareHandler(Byte[] match_value, Byte[] value);

        CompareType cur_compare_type = CompareType.UNCHANGED_VALUE;

        public GetBytesByTypeHandler GetBytesByType { get; set; }
        public SetBytesByTypeHandler SetBytesByType { get; set; }
        public BytesToStringHandler BytesToString { get; set; }
        public StringToBytesHandler StringToBytes { get; set; }
        public BytesToUintHandler BytesToUint { get; set; }
        public UintToBytesHandler UintToBytes { get; set; }
        public CompareHandler Compare { get; set; }

        public CompareHandler CompareInFilter { get; set; }

        public CompareWithFilterListHandler CompareWithFilterList { get; set; }

        public byte[] ReadMemory(ulong address, int length)
        {
            mutex.WaitOne();
            byte[] buf = ps4.ReadMemory(ProcessID, address, length);
            mutex.ReleaseMutex();
            return buf;
        }

        public void WriteMemory(ulong address, byte[] data)
        {
            mutex.WaitOne();
            ps4.WriteMemory(ProcessID, address, data);
            mutex.ReleaseMutex();
        }

        public ProcessList GetProcessList()
        {
            mutex.WaitOne();
            ProcessList processList = ps4.GetProcessList();
            mutex.ReleaseMutex();
            return processList;
        }

        public ProcessInfo GetProcessInfo(int processID)
        {
            mutex.WaitOne();
            ProcessInfo processInfo = ps4.GetProcessInfo(processID);
            mutex.ReleaseMutex();
            return processInfo;
        }

        public string PrintBytesByHex(byte[] bytes)
        {
            string str = string.Format("{0:X}", BitConverter.ToUInt32(bytes, 0));
            return str.ToUpper();
        }

        string float_to_string(Byte[] value)
        {
            return BitConverter.ToSingle(value, 0).ToString();
        }
        string uint_to_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString();
        }
        string ushort_to_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString();
        }
        string print_bytes_hex(Byte[] value)
        {
            return PrintBytesByHex(value);
        }

        byte[] string_to_float(string value)
        {
            return BitConverter.GetBytes(float.Parse(value));
        }
        byte[] string_to_4_bytes(string value)
        {
            return BitConverter.GetBytes(uint.Parse(value));
        }
        byte[] string_to_2_bytes(string value)
        {
            return BitConverter.GetBytes(uint.Parse(value));
        }

        byte[] string_to_hex(string value)
        {
            return BitConverter.GetBytes(ushort.Parse(value));
        }

        byte[] get_bytes_4_bytes(ulong address)
        {
            byte[] buffer = new byte[4];
            buffer = ReadMemory(address, 4);
            return buffer;
        }
        byte[] get_bytes_2_bytes(ulong address)
        {
            byte[] buffer = new byte[2];
            buffer = ReadMemory(address, 2);
            return buffer;
        }

        uint bytes_to_uint_4_bytes(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        uint bytes_to_uint_2_bytes(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }

        byte[] uint_to_bytes_4_bytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        byte[] uint_to_bytes_2_bytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        void set_bytes_hex(ulong address, byte[] value)
        {
            WriteMemory(address, value);
        }

        void set_bytes_4_bytes(ulong address, byte[] value)
        {
            byte[] data = { value[0], value[1], value[2], value[3] };
            WriteMemory(address, data);
        }
        void set_bytes_2_bytes(ulong address, byte[] value)
        {
            byte[] data = { value[0], value[1] };
            WriteMemory(address, data);
        }

        void compare_with_filter_list_4_bytes(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[4];
            for (int i = 0; i < mem.LongLength; i += 4)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 4);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUint(bytes);
                    filtered_list.Add(addr);
                }
            }
        }
        void compare_with_filter_list_2_bytes(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[4];
            for (int i = 0; i < mem.LongLength; i += 2)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 2);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUint(bytes);
                    filtered_list.Add(addr);
                }
            }
        }

        bool scan_type_any_uint(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt32(value, 0) != 0 ? true : false;
        }

        bool scan_type_bigger_uint(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt32(value, 0) > BitConverter.ToUInt32(match_value, 0);
        }
        bool scan_type_less_uint(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt32(value, 0) < BitConverter.ToUInt32(match_value, 0);
        }

        bool scan_type_equal_uint(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt32(value, 0) == BitConverter.ToUInt32(match_value, 0);
        }
        bool scan_type_not_uint(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt32(value, 0) != BitConverter.ToUInt32(match_value, 0);
        }

        bool scan_type_any_float(byte[] match_value, byte[] value)
        {
            return BitConverter.ToSingle(value, 0) != 0 ? true : false;
        }
        bool scan_type_bigger_float(byte[] match_value, byte[] value)
        {
            return BitConverter.ToSingle(value, 0) >  BitConverter.ToSingle(match_value, 0);
        }
        bool scan_type_less_float(byte[] match_value, byte[] value)
        {
            return BitConverter.ToSingle(value, 0) < BitConverter.ToSingle(match_value, 0);
        }
        bool scan_type_equal_float(byte[] match_value, byte[] value)
        {
            return Math.Abs(BitConverter.ToSingle(value, 0) -
                BitConverter.ToSingle(match_value, 0)) < 0.0001;
        }
        bool scan_type_not_float(byte[] match_value, byte[] value)
        {
            return !scan_type_equal_float(match_value, value);
        }

        public byte[] GetCompareBytes(Address address, string potential_value)
        {
            byte[] compare_value = StringToBytes(potential_value);
            if (CompareType.UNCHANGED_VALUE == cur_compare_type ||
                CompareType.CHANGED_VALUE == cur_compare_type ||
                CompareType.INCREASED_VALUE == cur_compare_type ||
                CompareType.DECREASED_VALUE == cur_compare_type)
            {
                compare_value = UintToBytes(address.MemoryValue);
            }

            return compare_value;
        }

        public void InitMemoryHandler(string valueType, CompareType compareType)
        {
            ValueType _valueType = ValueType.NONE;
            switch (valueType)
            {
                case "4 bytes":
                    _valueType = ValueType.INT_TYPE;
                    break;
                case "2 bytes":
                    _valueType = ValueType.SHORT_TYPE;
                    break;
                case "float":
                    _valueType = ValueType.FLOAT_TYPE;
                    break;
                case "hex":
                    _valueType = ValueType.HEX_TYPE;
                    break;
            }

            InitMemoryHandler(_valueType, compareType);
        }

        public void InitMemoryHandler(ValueType valueType, CompareType compareType)
        {
            bool is_float = false;
            cur_compare_type = compareType;

            switch (valueType)
            {
                case ValueType.FLOAT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    CompareWithFilterList = compare_with_filter_list_4_bytes;
                    BytesToString = float_to_string;
                    StringToBytes = string_to_float;
                    BytesToUint = bytes_to_uint_4_bytes;
                    UintToBytes = uint_to_bytes_4_bytes;
                    is_float = true;
                    break;
                case ValueType.INT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    CompareWithFilterList = compare_with_filter_list_4_bytes;
                    BytesToString = uint_to_string;
                    StringToBytes = string_to_4_bytes;
                    BytesToUint = bytes_to_uint_4_bytes;
                    UintToBytes = uint_to_bytes_4_bytes;
                    break;
                case ValueType.SHORT_TYPE:
                    SetBytesByType = set_bytes_2_bytes;
                    GetBytesByType = get_bytes_2_bytes;
                    CompareWithFilterList = compare_with_filter_list_2_bytes;
                    BytesToString = ushort_to_string;
                    StringToBytes = string_to_2_bytes;
                    BytesToUint = bytes_to_uint_2_bytes;
                    UintToBytes = uint_to_bytes_2_bytes;
                    break;
                case ValueType.HEX_TYPE:
                    SetBytesByType = set_bytes_hex;
                    GetBytesByType = null;
                    CompareWithFilterList = null;
                    BytesToString = print_bytes_hex;
                    StringToBytes = string_to_2_bytes;
                    break;
                default:
                    break;
            }

            switch(compareType)
            {
                case CompareType.UNKNOWN_INITIAL_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_any_float;
                    }
                    else
                    {
                        Compare = scan_type_any_uint;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.EXACT_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_equal_float;
                    }
                    else
                    {
                        Compare = scan_type_equal_uint;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.CHANGED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_not_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else
                    {
                        Compare = scan_type_not_uint;
                        CompareInFilter = scan_type_any_uint;
                    }
                    break;
                case CompareType.UNCHANGED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_equal_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else
                    {
                        Compare = scan_type_equal_uint;
                        CompareInFilter = scan_type_any_uint;
                    }
                    break;
                case CompareType.INCREASED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_bigger_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else
                    {
                        Compare = scan_type_bigger_uint;
                        CompareInFilter = scan_type_any_uint;
                    }
                    break;
                case CompareType.DECREASED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_less_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else
                    {
                        Compare = scan_type_less_uint;
                        CompareInFilter = scan_type_any_uint;
                    }
                    break;
                case CompareType.BIGGER_THAN_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_bigger_float;
                    }
                    else
                    {
                        Compare = scan_type_bigger_uint;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.SMALLER_THAN_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_less_float;
                    }
                    else
                    {
                        Compare = scan_type_less_uint;
                    }
                    CompareInFilter = Compare;
                    break;
                default:
                    break;
            }
        }
    }
}
