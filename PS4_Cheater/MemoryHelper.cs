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
        INCREASED_VALUE_BY,
        DECREASED_VALUE,
        DECREASED_VALUE_BY,
        BIGGER_THAN_VALUE,
        SMALLER_THAN_VALUE,
        CHANGED_VALUE,
        UNCHANGED_VALUE,
        BETWEEN_VALUE,
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
        public delegate void SetBytesByTypeHandler(ulong address, byte[] value);
        public delegate bool ComparatorHandler(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value);

        CompareType cur_compare_type = CompareType.UNCHANGED_VALUE;

        public GetBytesByTypeHandler GetBytesByType { get; set; }
        public SetBytesByTypeHandler SetBytesByType { get; set; }
        public BytesToStringHandler BytesToString { get; set; }
        public StringToBytesHandler StringToBytes { get; set; }
        public ComparatorHandler Comparer { get; set; }

        public int Length { get; set; }
        public int Alignment { get; set; }

        public static byte[] ReadMemory(ulong address, int length)
        {
            mutex.WaitOne();
            try
            {
                byte[] buf = ps4.ReadMemory(ProcessID, address, length);
                mutex.ReleaseMutex();
                return buf;
            }
            catch
            {
                mutex.ReleaseMutex();
            }
            return new byte[1];
        }

        public static void WriteMemory(ulong address, byte[] data)
        {
            mutex.WaitOne();
            try
            {
                ps4.WriteMemory(ProcessID, address, data);
                mutex.ReleaseMutex();
            }
            catch
            {
                mutex.ReleaseMutex();
            }
        }

        public static ProcessList GetProcessList()
        {
            mutex.WaitOne();
            try
            {
                ProcessList processList = ps4.GetProcessList();
                mutex.ReleaseMutex();
                return processList;
            }
            catch
            {
                mutex.ReleaseMutex();
            }
            return null;
        }

        public static ProcessInfo GetProcessInfo(int processID)
        {
            mutex.WaitOne();
            try
            {
                ProcessInfo processInfo = ps4.GetProcessInfo(processID);
                mutex.ReleaseMutex();
                return processInfo;
            }
            catch
            {
                mutex.ReleaseMutex();
                return null;
            }
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
            return BitConverter.GetBytes(double.Parse(value));
        }
        public static byte[] string_to_float(string value)
        {
            return BitConverter.GetBytes(float.Parse(value));
        }
        public static byte[] string_to_8_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }

        public static byte[] string_to_4_bytes(string value)
        {
            return BitConverter.GetBytes(uint.Parse(value));
        }

        public static byte[] string_to_2_bytes(string value)
        {
            return BitConverter.GetBytes(UInt16.Parse(value));
        }
        public static byte[] string_to_1_byte(string value)
        {
            return BitConverter.GetBytes(Byte.Parse(value));
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
            return ReadMemory(address, 8);
        }

        byte[] get_bytes_4_bytes(ulong address)
        {
            return ReadMemory(address, 4);
        }

        byte[] get_bytes_2_bytes(ulong address)
        {
            return ReadMemory(address, 2);
        }
        byte[] get_bytes_1_byte(ulong address)
        {
            return ReadMemory(address, 1);
        }

        void set_bytes_hex(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }

        void set_bytes_8_bytes(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }

        void set_bytes_4_bytes(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }
        void set_bytes_2_bytes(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }
        void set_bytes_1_byte(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }

        public void CompareWithMemoryBufferNextScanner(byte[] default_value_0, byte[] default_value_1, byte[] buffer,
            AddressList old_address_list, AddressList new_address_list)
        {
            int alignment = Alignment;
            int length = Length;

            Byte[] new_value = new byte[length];
            for (old_address_list.Begin(); !old_address_list.End(); old_address_list.Next())
            {
                uint address_offset = 0;
                Byte[] old_value = null;
                old_address_list.Get(ref address_offset, ref old_value);
                Buffer.BlockCopy(buffer, (int)address_offset, new_value, 0, length);
                if (Comparer(default_value_0, default_value_1, old_value, new_value))
                {
                    new_address_list.Add(address_offset, new_value);
                }
            }
        }

        public void CompareWithMemoryBufferNewScanner(byte[] default_value_0, byte[] default_value_1, byte[] buffer,
            AddressList old_address_list, AddressList new_address_list)
        {
            int alignment = Alignment;
            int length = Length;

            Byte[] new_value = new byte[length];
            Byte[] dummy_value = new byte[length];
            for (int i = 0; i + length < buffer.LongLength; i += alignment)
            {
                Buffer.BlockCopy(buffer, i, new_value, 0, length);
                if (Comparer(default_value_0, default_value_1, dummy_value, new_value))
                {
                    new_address_list.Add((uint)i, new_value);
                }
            }
        }

        bool scan_type_any_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) != 0 ? true : false;
        }

        bool scan_type_between_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) <= BitConverter.ToUInt64(default_value_1, 0) &&
                BitConverter.ToUInt64(new_value, 0) >= BitConverter.ToUInt64(default_value_0, 0);
        }

        bool scan_type_increased_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) > BitConverter.ToUInt64(old_value, 0);
        }
        bool scan_type_increased_by_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) ==
                BitConverter.ToUInt64(old_value, 0) + BitConverter.ToUInt64(default_value_0, 0);
        }
        bool scan_type_bigger_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) > BitConverter.ToUInt64(default_value_0, 0);
        }
        bool scan_type_decreased_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) < BitConverter.ToUInt64(old_value, 0);
        }

        bool scan_type_decreased_by_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) ==
                BitConverter.ToUInt64(old_value, 0) - BitConverter.ToUInt64(default_value_0, 0);
        }

        bool scan_type_less_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) < BitConverter.ToUInt64(default_value_0, 0);
        }
        bool scan_type_unchanged_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(old_value, 0) == BitConverter.ToUInt64(new_value, 0);
        }
        bool scan_type_equal_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(default_value_0, 0) == BitConverter.ToUInt64(new_value, 0);
        }
        bool scan_type_changed_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(old_value, 0) != BitConverter.ToUInt64(new_value, 0);
        }
        bool scan_type_not_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(default_value_0, 0) != BitConverter.ToUInt64(new_value, 0);
        }


        bool scan_type_any_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) != 0 ? true : false;
        }

        bool scan_type_between_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) <= BitConverter.ToUInt32(default_value_1, 0) &&
                BitConverter.ToUInt32(new_value, 0) >= BitConverter.ToUInt32(default_value_0, 0);
        }

        bool scan_type_increased_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) > BitConverter.ToUInt32(old_value, 0);
        }
        bool scan_type_increased_by_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) ==
                BitConverter.ToUInt32(old_value, 0) + BitConverter.ToUInt32(default_value_0, 0);
        }
        bool scan_type_bigger_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) > BitConverter.ToUInt32(default_value_0, 0);
        }
        bool scan_type_decreased_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) < BitConverter.ToUInt32(old_value, 0);
        }

        bool scan_type_decreased_by_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) ==
                BitConverter.ToUInt32(old_value, 0) - BitConverter.ToUInt32(default_value_0, 0);
        }

        bool scan_type_less_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(new_value, 0) < BitConverter.ToUInt32(default_value_0, 0);
        }
        bool scan_type_unchanged_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(old_value, 0) == BitConverter.ToUInt32(new_value, 0);
        }
        bool scan_type_equal_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(default_value_0, 0) == BitConverter.ToUInt32(new_value, 0);
        }
        bool scan_type_changed_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(old_value, 0) != BitConverter.ToUInt32(new_value, 0);
        }
        bool scan_type_not_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt32(default_value_0, 0) != BitConverter.ToUInt32(new_value, 0);
        }

        bool scan_type_any_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) != 0 ? true : false;
        }

        bool scan_type_between_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) <= BitConverter.ToUInt16(default_value_1, 0) &&
                BitConverter.ToUInt16(new_value, 0) >= BitConverter.ToUInt16(default_value_0, 0);
        }

        bool scan_type_increased_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) > BitConverter.ToUInt16(old_value, 0);
        }
        bool scan_type_increased_by_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) ==
                BitConverter.ToUInt16(old_value, 0) + BitConverter.ToUInt16(default_value_0, 0);
        }
        bool scan_type_bigger_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) > BitConverter.ToUInt16(default_value_0, 0);
        }
        bool scan_type_decreased_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) < BitConverter.ToUInt16(old_value, 0);
        }

        bool scan_type_decreased_by_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) ==
                BitConverter.ToUInt16(old_value, 0) - BitConverter.ToUInt16(default_value_0, 0);
        }

        bool scan_type_less_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(new_value, 0) < BitConverter.ToUInt16(default_value_0, 0);
        }
        bool scan_type_unchanged_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(old_value, 0) == BitConverter.ToUInt16(new_value, 0);
        }
        bool scan_type_equal_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(default_value_0, 0) == BitConverter.ToUInt16(new_value, 0);
        }
        bool scan_type_changed_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(old_value, 0) != BitConverter.ToUInt16(new_value, 0);
        }
        bool scan_type_not_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt16(default_value_0, 0) != BitConverter.ToUInt16(new_value, 0);
        }

        bool scan_type_any_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] != 0 ? true : false;
        }

        bool scan_type_between_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] <= default_value_1[0] &&
                new_value[0] >= default_value_0[0];
        }

        bool scan_type_increased_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] > old_value[0];
        }
        bool scan_type_increased_by_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] ==
                old_value[0] + default_value_0[0];
        }
        bool scan_type_bigger_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] > default_value_0[0];
        }
        bool scan_type_decreased_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] < old_value[0];
        }

        bool scan_type_decreased_by_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] ==
                old_value[0] - default_value_0[0];
        }

        bool scan_type_less_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return new_value[0] < default_value_0[0];
        }
        bool scan_type_unchanged_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return old_value[0] == new_value[0];
        }
        bool scan_type_equal_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return default_value_0[0] == new_value[0];
        }
        bool scan_type_changed_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return old_value[0] != new_value[0];
        }

        bool scan_type_not_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return default_value_0[0] != new_value[0];
        }
        bool scan_type_any_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) != 0 ? true : false;
        }
        bool scan_type_fuzzy_equal_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToDouble(default_value_0, 0) -
                BitConverter.ToDouble(new_value, 0)) < 1;
        }
        bool scan_type_between_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) <= BitConverter.ToDouble(default_value_1, 0) &&
                BitConverter.ToDouble(new_value, 0) >= BitConverter.ToDouble(default_value_0, 0);
        }
        bool scan_type_increased_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) > BitConverter.ToDouble(old_value, 0);
        }
        bool scan_type_increased_by_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToDouble(new_value, 0) -
                (BitConverter.ToDouble(default_value_0, 0) + BitConverter.ToDouble(old_value, 0))) < 0.0001;
        }
        bool scan_type_bigger_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) > BitConverter.ToDouble(default_value_0, 0);
        }
        bool scan_type_decreased_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) < BitConverter.ToDouble(old_value, 0);
        }
        bool scan_type_decreased_by_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToDouble(new_value, 0) -
                (BitConverter.ToDouble(old_value, 0) - BitConverter.ToDouble(default_value_0, 0))) < 0.0001;
        }
        bool scan_type_less_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToDouble(new_value, 0) < BitConverter.ToDouble(default_value_0, 0);
        }
        bool scan_type_unchanged_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToDouble(old_value, 0) -
                BitConverter.ToDouble(new_value, 0)) < 0.0001;
        }
        bool scan_type_equal_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToDouble(default_value_0, 0) -
                BitConverter.ToDouble(new_value, 0)) < 0.0001;
        }
        bool scan_type_changed_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return !scan_type_unchanged_double(default_value_0, default_value_1, old_value, new_value);
        }
        bool scan_type_not_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return !scan_type_equal_double(default_value_0, default_value_1, old_value, new_value);
        }

        bool scan_type_any_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) != 0 ? true : false;
        }
        bool scan_type_fuzzy_equal_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToSingle(default_value_0, 0) -
                BitConverter.ToSingle(new_value, 0)) < 1;
        }
        bool scan_type_between_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) <= BitConverter.ToSingle(default_value_1, 0) &&
                BitConverter.ToSingle(new_value, 0) >= BitConverter.ToSingle(default_value_0, 0);
        }
        bool scan_type_increased_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) > BitConverter.ToSingle(old_value, 0);
        }
        bool scan_type_increased_by_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToSingle(new_value, 0) -
                (BitConverter.ToSingle(default_value_0, 0) + BitConverter.ToSingle(old_value, 0))) < 0.0001;
        }
        bool scan_type_bigger_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) > BitConverter.ToSingle(default_value_0, 0);
        }
        bool scan_type_decreased_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) < BitConverter.ToSingle(old_value, 0);
        }
        bool scan_type_decreased_by_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToSingle(new_value, 0) -
                (BitConverter.ToSingle(old_value, 0) - BitConverter.ToSingle(default_value_0, 0))) < 0.0001;
        }
        bool scan_type_less_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToSingle(new_value, 0) < BitConverter.ToSingle(default_value_0, 0);
        }
        bool scan_type_unchanged_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToSingle(old_value, 0) -
                BitConverter.ToSingle(new_value, 0)) < 0.0001;
        }

        bool scan_type_equal_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return Math.Abs(BitConverter.ToSingle(default_value_0, 0) -
                BitConverter.ToSingle(new_value, 0)) < 0.0001;
        }
        bool scan_type_changed_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return !scan_type_unchanged_float(default_value_0, default_value_1, old_value, new_value);
        }

        bool scan_type_not_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return !scan_type_equal_float(default_value_0, default_value_1, old_value, new_value);
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
                case "byte":
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
                    return "byte";
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

        public void InitMemoryHandler(string valueType, CompareType compareType, bool is_alignment)
        {
            ValueType _valueType = GetValueTypeByString(valueType);
            InitMemoryHandler(_valueType, compareType, is_alignment);
        }
        public void InitMemoryHandler(ValueType valueType, CompareType compareType, bool is_alignment)
        {
            cur_compare_type = compareType;

            switch (valueType)
            {
                case ValueType.DOUBLE_TYPE:
                    SetBytesByType = set_bytes_8_bytes;
                    GetBytesByType = get_bytes_8_bytes;
                    BytesToString = double_to_string;
                    StringToBytes = string_to_double;
                    Length = sizeof(double);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.FLOAT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    BytesToString = float_to_string;
                    StringToBytes = string_to_float;
                    Length = sizeof(float);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.ULONG_TYPE:
                    SetBytesByType = set_bytes_8_bytes;
                    GetBytesByType = get_bytes_8_bytes;
                    BytesToString = ulong_to_string;
                    StringToBytes = string_to_8_bytes;
                    Length = sizeof(ulong);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.UINT_TYPE:
                    SetBytesByType = set_bytes_4_bytes;
                    GetBytesByType = get_bytes_4_bytes;
                    BytesToString = uint_to_string;
                    StringToBytes = string_to_4_bytes;
                    Length = sizeof(uint);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.USHORT_TYPE:
                    SetBytesByType = set_bytes_2_bytes;
                    GetBytesByType = get_bytes_2_bytes;
                    BytesToString = ushort_to_string;
                    StringToBytes = string_to_2_bytes;
                    Length = sizeof(ushort);
                    Alignment = (is_alignment) ? 2 : 1;
                    break;
                case ValueType.BYTE_TYPE:
                    SetBytesByType = set_bytes_1_byte;
                    GetBytesByType = get_bytes_1_byte;
                    BytesToString = uchar_to_string;
                    StringToBytes = string_to_1_byte;
                    Length = sizeof(byte);
                    Alignment = 1;
                    break;
                case ValueType.HEX_TYPE:
                    SetBytesByType = set_bytes_hex;
                    GetBytesByType = null;
                    BytesToString = bytes_to_hex_string;
                    StringToBytes = string_to_2_bytes;
                    Alignment = 1;
                    break;
                default:
                    break;
            }

            switch (compareType)
            {
                case CompareType.UNKNOWN_INITIAL_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_any_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_any_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_any_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_any_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_any_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_any_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.FUZZY_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_equal_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_equal_float;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.EXACT_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_equal_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_equal_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_equal_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_equal_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_equal_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_equal_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.CHANGED_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_changed_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_changed_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_changed_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_changed_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_changed_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_changed_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.UNCHANGED_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_unchanged_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_unchanged_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_unchanged_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_unchanged_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_unchanged_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_unchanged_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.INCREASED_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_increased_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_increased_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_increased_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_increased_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_increased_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_increased_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.INCREASED_VALUE_BY:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_increased_by_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_increased_by_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_increased_by_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_increased_by_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_increased_by_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_increased_by_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.DECREASED_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_decreased_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_decreased_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_decreased_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_decreased_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_decreased_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_decreased_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.DECREASED_VALUE_BY:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_decreased_by_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_decreased_by_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_decreased_by_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_decreased_by_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_decreased_by_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_decreased_by_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.BIGGER_THAN_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_bigger_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_bigger_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_bigger_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_bigger_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_bigger_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_bigger_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.SMALLER_THAN_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_less_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_less_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_less_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_less_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_less_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_less_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.BETWEEN_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_between_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_between_float;
                            break;
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_between_ulong;
                            break;
                        case ValueType.UINT_TYPE:
                            Comparer = scan_type_between_uint;
                            break;
                        case ValueType.USHORT_TYPE:
                            Comparer = scan_type_between_uint16;
                            break;
                        case ValueType.BYTE_TYPE:
                            Comparer = scan_type_between_uint8;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
                    //throw new Exception("Unknown compare type.");
            }
        }
    }
}