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
        FUZZY_VALUE,
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
        BYTE_TYPE,
        USHORT_TYPE,
        UINT_TYPE,
        ULONG_TYPE,
        FLOAT_TYPE,
        DOUBLE_TYPE,
        HEX_TYPE,
        NONE_TYPE,

    }

    public class MemoryHelper
    {
        public static PS4RPC ps4 = null;
        private static Mutex mutex;
        public static int ProcessID;

        static MemoryHelper()
        {
            mutex = new Mutex();
        }

        public static bool Connect(string ip)
        {
            ps4 = new PS4RPC(ip);
            ps4.Connect();
            return false;
        }

        public delegate string BytesToStringHandler(Byte[] value);
        public delegate Byte[] StringToBytesHandler(string value);
        public delegate Byte[] GetBytesByTypeHandler(ulong address);
        public delegate void CompareWithFilterListHandler(Byte[] match_value, ulong address, byte[] mem,
            AddressList filtered_lists);
        public delegate void SetBytesByTypeHandler(ulong address, byte[] value);
        public delegate ulong BytesToUlongHandler(byte[] value);
        public delegate byte[] UlongToBytesHandler(ulong value);
        public delegate bool CompareHandler(Byte[] match_value, Byte[] value);

        CompareType cur_compare_type = CompareType.UNCHANGED_VALUE;

        public GetBytesByTypeHandler GetBytesByType { get; set; }
        public SetBytesByTypeHandler SetBytesByType { get; set; }
        public BytesToStringHandler BytesToString { get; set; }
        public StringToBytesHandler StringToBytes { get; set; }
        public BytesToUlongHandler BytesToUlong { get; set; }
        public UlongToBytesHandler UlongToBytes { get; set; }
        public CompareHandler Compare { get; set; }

        public CompareHandler CompareInFilter { get; set; }

        public CompareWithFilterListHandler CompareWithFilterList { get; set; }

        public static byte[] ReadMemory(ulong address, int length)
        {
            mutex.WaitOne();
            byte[] buf = ps4.ReadMemory(ProcessID, address, length);
            mutex.ReleaseMutex();
            return buf;
        }

        public static void WriteMemory(ulong address, byte[] data)
        {
            mutex.WaitOne();
            ps4.WriteMemory(ProcessID, address, data);
            mutex.ReleaseMutex();
        }

        public static ProcessList GetProcessList()
        {
            mutex.WaitOne();
            ProcessList processList = ps4.GetProcessList();
            mutex.ReleaseMutex();
            return processList;
        }

        public static ProcessInfo GetProcessInfo(int processID)
        {
            mutex.WaitOne();
            ProcessInfo processInfo = ps4.GetProcessInfo(processID);
            mutex.ReleaseMutex();
            return processInfo;
        }
        public static string double_to_string(Byte[] value)
        {
            return BitConverter.ToDouble(value, 0).ToString();
        }
        public static string float_to_string(Byte[] value)
        {
            return BitConverter.ToSingle(value, 0).ToString();
        }
        public static string ulong_to_string(Byte[] value)
        {
            return BitConverter.ToUInt64(value, 0).ToString();
        }
        public static string uint_to_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString();
        }
        public static string ushort_to_string(Byte[] value)
        {
            return BitConverter.ToUInt16(value, 0).ToString();
        }
        public static string uchar_to_string(Byte[] value)
        {
            return value[0].ToString();
        }
        public static string bytes_to_hex_string(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        public static byte[] string_to_double(string value)
        {
            byte[] data = BitConverter.GetBytes(double.Parse(value));
            byte[] ret = new byte[8];
            Buffer.BlockCopy(data, 0, ret, 0, 8);
            return ret;
        }
        public static byte[] string_to_float(string value)
        {
            byte[] data = BitConverter.GetBytes(float.Parse(value));
            byte[] ret = new byte[8];
            Buffer.BlockCopy(data, 0, ret, 0, 4);
            return ret;
        }
        public static byte[] string_to_8_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }

        public static byte[] string_to_4_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }

        public static byte[] string_to_2_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }
        public static byte[] string_to_1_byte(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }
        public static byte[] string_to_hex(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += "0";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        byte[] get_bytes_8_bytes(ulong address)
        {
            byte[] buffer = new byte[8];
            buffer = ReadMemory(address, 8);
            return buffer;
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
        byte[] get_bytes_1_byte(ulong address)
        {
            byte[] buffer = new byte[1];
            buffer = ReadMemory(address, 1);
            return buffer;
        }

        public static ulong bytes_to_8_bytes(byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static ulong bytes_to_4_bytes(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static ulong bytes_to_2_bytes(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static ulong bytes_to_1_byte(byte[] bytes)
        {
            return bytes[0];
        }

        public static byte[] ulong_to_8_bytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] ulong_to_4_bytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] ulong_to_2_bytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] ulong_to_1_byte(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        void set_bytes_hex(ulong address, byte[] value)
        {
            WriteMemory(address, value);
        }

        void set_bytes_8_bytes(ulong address, byte[] value)
        {
            byte[] data = new byte[8];
            Buffer.BlockCopy(value, 0, data, 0, 8);
            WriteMemory(address, data);
        }

        void set_bytes_4_bytes(ulong address, byte[] value)
        {
            byte[] data = new byte[4];
            Buffer.BlockCopy(value, 0, data, 0, 4);
            WriteMemory(address, data);
        }
        void set_bytes_2_bytes(ulong address, byte[] value)
        {
            byte[] data = new byte[2];
            Buffer.BlockCopy(value, 0, data, 0, 2);
            WriteMemory(address, data);
        }
        void set_bytes_1_byte(ulong address, byte[] value)
        {
            byte[] data = new byte[1];
            Buffer.BlockCopy(value, 0, data, 0, 1);
            WriteMemory(address, data);
        }

        void compare_with_filter_list_8_bytes(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[8];
            for (int i = 0; i + 8 < mem.LongLength; i += 4)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 8);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUlong(bytes);
                    filtered_list.Add(addr);
                }
            }
        }

        void compare_with_filter_list_4_bytes(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[8];
            for (int i = 0; i + 4 < mem.LongLength; i += 4)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 4);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUlong(bytes);
                    filtered_list.Add(addr);
                }
            }
        }
        void compare_with_filter_list_2_bytes(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[8];
            for (int i = 0; i + 2 < mem.LongLength; i += 2)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 2);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUlong(bytes);
                    filtered_list.Add(addr);
                }
            }
        }

        void compare_with_filter_list_1_byte(byte[] match_value, ulong address, byte[] mem, AddressList filtered_list)
        {
            Byte[] bytes = new byte[8];
            for (int i = 0; i + 1 < mem.LongLength; i += 1)
            {
                Buffer.BlockCopy(mem, i, bytes, 0, 1);
                if (CompareInFilter(match_value, bytes))
                {
                    Address addr = new Address();
                    addr.AddressOffset = (uint)i;
                    addr.MemoryValue = BytesToUlong(bytes);
                    filtered_list.Add(addr);
                }
            }
        }

        bool scan_type_any_ulong(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt64(value, 0) != 0 ? true : false;
        }

        bool scan_type_bigger_ulong(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt64(value, 0) > BitConverter.ToUInt64(match_value, 0);
        }
        bool scan_type_less_ulong(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt64(value, 0) < BitConverter.ToUInt64(match_value, 0);
        }

        bool scan_type_equal_ulong(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt64(value, 0) == BitConverter.ToUInt64(match_value, 0);
        }
        bool scan_type_not_ulong(byte[] match_value, byte[] value)
        {
            return BitConverter.ToUInt64(value, 0) != BitConverter.ToUInt64(match_value, 0);
        }

        bool scan_type_any_double(byte[] match_value, byte[] value)
        {
            return BitConverter.ToDouble(value, 0) != 0 ? true : false;
        }
        bool scan_type_bigger_double(byte[] match_value, byte[] value)
        {
            return BitConverter.ToDouble(value, 0) > BitConverter.ToSingle(match_value, 0);
        }
        bool scan_type_less_double(byte[] match_value, byte[] value)
        {
            return BitConverter.ToDouble(value, 0) < BitConverter.ToSingle(match_value, 0);
        }
        bool scan_type_equal_double(byte[] match_value, byte[] value)
        {
            return Math.Abs(BitConverter.ToDouble(value, 0) -
                BitConverter.ToDouble(match_value, 0)) < 0.0001;
        }
        bool scan_type_fuzzy_equal_double(byte[] match_value, byte[] value)
        {
            return Math.Abs(BitConverter.ToDouble(value, 0) -
                BitConverter.ToDouble(match_value, 0)) < 1;
        }
        bool scan_type_not_double(byte[] match_value, byte[] value)
        {
            return !scan_type_equal_double(match_value, value);
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
        bool scan_type_fuzzy_equal_float(byte[] match_value, byte[] value)
        {
            return Math.Abs(BitConverter.ToSingle(value, 0) -
                BitConverter.ToSingle(match_value, 0)) < 1;
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
                compare_value = UlongToBytes(address.MemoryValue);
            }

            return compare_value;
        }

        public static ValueType GetValueTypeByString(string valueType)
        {
            ValueType _valueType = ValueType.NONE_TYPE;
            switch (valueType)
            {
                case "8 bytes":
                    _valueType = ValueType.ULONG_TYPE;
                    break;
                case "4 bytes":
                    _valueType = ValueType.UINT_TYPE;
                    break;
                case "2 bytes":
                    _valueType = ValueType.USHORT_TYPE;
                    break;
                case "1 byte":
                    _valueType = ValueType.BYTE_TYPE;
                    break;
                case "double":
                    _valueType = ValueType.DOUBLE_TYPE;
                    break;
                case "float":
                    _valueType = ValueType.FLOAT_TYPE;
                    break;
                case "hex":
                    _valueType = ValueType.HEX_TYPE;
                    break;
                default:
                    throw new Exception("GetValueTypeByString!!!");
            }
            return _valueType;
        }

        public static string GetStringOfValueType(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.ULONG_TYPE:
                    return "8 bytes";
                case ValueType.UINT_TYPE:
                    return "4 bytes";
                case ValueType.USHORT_TYPE:
                    return "2 bytes";
                case ValueType.BYTE_TYPE:
                    return "1 byte";
                case ValueType.DOUBLE_TYPE:
                    return "double";
                case ValueType.FLOAT_TYPE:
                    return "float";
                case ValueType.HEX_TYPE:
                    return "hex";
                default:
                    throw new Exception("GetStringOfValueType!!!");
            }
        }

        public void InitMemoryHandler(string valueType, CompareType compareType)
        {
            ValueType _valueType = GetValueTypeByString(valueType);
            InitMemoryHandler(_valueType, compareType);
        }

        public void InitMemoryHandler(ValueType valueType, CompareType compareType)
        {
            bool is_float = false;
            bool is_double = false;
            cur_compare_type = compareType;

            switch (valueType)
            {
                case ValueType.DOUBLE_TYPE:
                    SetBytesByType = set_bytes_8_bytes;
                    GetBytesByType = get_bytes_8_bytes;
                    CompareWithFilterList = compare_with_filter_list_8_bytes;
                    BytesToString = double_to_string;
                    StringToBytes = string_to_double;
                    BytesToUlong = bytes_to_8_bytes;
                    UlongToBytes = ulong_to_8_bytes;
                    is_double = true;
                    break;
                case ValueType.FLOAT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    CompareWithFilterList = compare_with_filter_list_4_bytes;
                    BytesToString = float_to_string;
                    StringToBytes = string_to_float;
                    BytesToUlong = bytes_to_4_bytes;
                    UlongToBytes = ulong_to_4_bytes;
                    is_float = true;
                    break;
                case ValueType.ULONG_TYPE:
                    SetBytesByType = set_bytes_8_bytes;
                    GetBytesByType = get_bytes_8_bytes;
                    CompareWithFilterList = compare_with_filter_list_8_bytes;
                    BytesToString = ulong_to_string;
                    StringToBytes = string_to_8_bytes;
                    BytesToUlong = bytes_to_8_bytes;
                    UlongToBytes = ulong_to_8_bytes;
                    break;
                case ValueType.UINT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    CompareWithFilterList = compare_with_filter_list_4_bytes;
                    BytesToString = uint_to_string;
                    StringToBytes = string_to_4_bytes;
                    BytesToUlong = bytes_to_4_bytes;
                    UlongToBytes = ulong_to_4_bytes;
                    break;
                case ValueType.USHORT_TYPE:
                    SetBytesByType = set_bytes_2_bytes;
                    GetBytesByType = get_bytes_2_bytes;
                    CompareWithFilterList = compare_with_filter_list_2_bytes;
                    BytesToString = ushort_to_string;
                    StringToBytes = string_to_2_bytes;
                    BytesToUlong = bytes_to_2_bytes;
                    UlongToBytes = ulong_to_2_bytes;
                    break;
                case ValueType.BYTE_TYPE:
                    SetBytesByType = set_bytes_1_byte;
                    GetBytesByType = get_bytes_1_byte;
                    CompareWithFilterList = compare_with_filter_list_1_byte;
                    BytesToString = uchar_to_string;
                    StringToBytes = string_to_1_byte;
                    BytesToUlong = bytes_to_1_byte;
                    UlongToBytes = ulong_to_1_byte;
                    break;
                case ValueType.HEX_TYPE:
                    SetBytesByType = set_bytes_hex;
                    GetBytesByType = null;
                    CompareWithFilterList = null;
                    BytesToString = bytes_to_hex_string;
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
                    else if (is_double)
                    {
                        Compare = scan_type_any_double;
                    }
                    else
                    {
                        Compare = scan_type_any_ulong;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.EXACT_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_equal_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_equal_double;
                    }
                    else
                    {
                        Compare = scan_type_equal_ulong;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.FUZZY_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_fuzzy_equal_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_fuzzy_equal_double;
                    }
                    else
                    {
                        Compare = scan_type_equal_ulong;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.CHANGED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_not_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_not_double;
                        CompareInFilter = scan_type_any_double;
                    }
                    else
                    {
                        Compare = scan_type_not_ulong;
                        CompareInFilter = scan_type_any_ulong;
                    }
                    break;
                case CompareType.UNCHANGED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_equal_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_equal_double;
                        CompareInFilter = scan_type_any_double;
                    }
                    else
                    {
                        Compare = scan_type_equal_ulong;
                        CompareInFilter = scan_type_any_ulong;
                    }
                    break;
                case CompareType.INCREASED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_bigger_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_bigger_double;
                        CompareInFilter = scan_type_any_double;
                    }
                    else
                    {
                        Compare = scan_type_bigger_ulong;
                        CompareInFilter = scan_type_any_ulong;
                    }
                    break;
                case CompareType.DECREASED_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_less_float;
                        CompareInFilter = scan_type_any_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_less_double;
                        CompareInFilter = scan_type_any_double;
                    }
                    else
                    {
                        Compare = scan_type_less_ulong;
                        CompareInFilter = scan_type_any_ulong;
                    }
                    break;
                case CompareType.BIGGER_THAN_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_bigger_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_bigger_double;
                    }
                    else
                    {
                        Compare = scan_type_bigger_ulong;
                    }
                    CompareInFilter = Compare;
                    break;
                case CompareType.SMALLER_THAN_VALUE:
                    if (is_float)
                    {
                        Compare = scan_type_less_float;
                    }
                    else if (is_double)
                    {
                        Compare = scan_type_less_double;
                    }
                    else
                    {
                        Compare = scan_type_less_ulong;
                    }
                    CompareInFilter = Compare;
                    break;
                default:
                    break;
            }
        }
    }
}
