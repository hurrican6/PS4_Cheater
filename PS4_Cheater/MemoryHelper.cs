using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;
using System.Threading;
using System.Globalization;
using System.Collections;

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
        POINTER_VALUE,
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
        STRING_TYPE,
        HEX_TYPE,
        POINTER_TYPE,
        NONE_TYPE,
    }

    public class MemoryHelper
    {
        public static PS4RPC ps4 = null;
        private static Mutex mutex;
        private int SelfProcessID;
        private int ProcessID {
            get {
                if (DefaultProcessID)
                    return Util.DefaultProcessID;
                return SelfProcessID;
            }
        }
        private bool DefaultProcessID;

        static MemoryHelper()
        {
            mutex = new Mutex();
        }

        public MemoryHelper(bool defaultProcessID, int processID)
        {
            this.SelfProcessID = processID;
            this.DefaultProcessID = defaultProcessID;
        }

        public static bool Connect(string ip)
        {
            try
            {
                mutex.WaitOne();
                ps4 = new PS4RPC(ip);
                ps4.Connect();
                mutex.ReleaseMutex();
                return true;
            }
            catch
            {
                mutex.ReleaseMutex();
            }
            return false;
        }

        public static bool Disconnect()
        {

            try
            {
                mutex.WaitOne();
                if (ps4 != null)
                {
                    ps4.Disconnect();
                }
                mutex.ReleaseMutex();
                return true;
            }
            catch
            {
                mutex.ReleaseMutex();
            }
            return false;
        }

        public delegate string BytesToStringHandler(Byte[] value);
        public delegate Byte[] StringToBytesHandler(string value);
        public delegate bool ComparatorHandler(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value);
        
        public BytesToStringHandler BytesToString { get; set; }
        public BytesToStringHandler BytesToHexString { get; set; }
        public StringToBytesHandler StringToBytes { get; set; }
        public StringToBytesHandler HexStringToBytes { get; set; }
        public ComparatorHandler Comparer { get; set; }

        public int Length { get; set; }
        public int Alignment { get; set; }

        public ValueType ValueType { get; set; }
        public CompareType CompareType { get; set; }

        public bool ParseFirstValue { get; set; }
        public bool ParseSecondValue { get; set; }

        public byte[] ReadMemory(ulong address, int length)
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
            return new byte[length];
        }

        public void WriteMemory(ulong address, byte[] data)
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

        static string hex_to_string(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        static string string_to_string(Byte[] value)
        {
            return System.Text.Encoding.Default.GetString(value);
        }

        static string double_to_string(Byte[] value)
        {
            return BitConverter.ToDouble(value, 0).ToString();
        }
        static string float_to_string(Byte[] value)
        {
            return BitConverter.ToSingle(value, 0).ToString();
        }
        static string ulong_to_string(Byte[] value)
        {
            return BitConverter.ToUInt64(value, 0).ToString();
        }
        static string uint_to_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString();
        }
        static string uint16_to_string(Byte[] value)
        {
            return BitConverter.ToUInt16(value, 0).ToString();
        }
        static string uchar_to_string(Byte[] value)
        {
            return value[0].ToString();
        }
        static string double_to_hex_string(Byte[] value)
        {
            return BitConverter.ToUInt64(value, 0).ToString("X16");
        }
        static string float_to_hex_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString("X8");
        }
        static string ulong_to_hex_string(Byte[] value)
        {
            return BitConverter.ToUInt64(value, 0).ToString("X16");
        }
        static string uint_to_hex_string(Byte[] value)
        {
            return BitConverter.ToUInt32(value, 0).ToString("X8");
        }
        static string uint16_to_hex_string(Byte[] value)
        {
            return BitConverter.ToUInt16(value, 0).ToString("X4");
        }
        static string uchar_to_hex_string(Byte[] value)
        {
            return value[0].ToString("X2");
        }
        static string hex_to_hex_string(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        static string string_to_hex_string(Byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

        static byte[] string_to_double(string value)
        {
            return BitConverter.GetBytes(double.Parse(value));
        }
        static byte[] string_to_float(string value)
        {
            return BitConverter.GetBytes(float.Parse(value));
        }
        static byte[] string_to_8_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value));
        }

        static byte[] string_to_4_bytes(string value)
        {
            return BitConverter.GetBytes(uint.Parse(value));
        }

        static byte[] string_to_2_bytes(string value)
        {
            return BitConverter.GetBytes(UInt16.Parse(value));
        }
        static byte[] string_to_byte(string value)
        {
            return BitConverter.GetBytes(Byte.Parse(value));
        }
        public static byte[] string_to_hex_bytes(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        static byte[] string_to_string_bytes(string hexString)
        {
            byte[] buffer = System.Text.Encoding.Default.GetBytes(hexString);
            return buffer;
        }

        static byte[] hex_string_to_double(string value)
        {
            return BitConverter.GetBytes(double.Parse(value, NumberStyles.HexNumber));
        }
        static byte[] hex_string_to_float(string value)
        {
            return BitConverter.GetBytes(float.Parse(value, NumberStyles.HexNumber));
        }
        static byte[] hex_string_to_8_bytes(string value)
        {
            return BitConverter.GetBytes(ulong.Parse(value, NumberStyles.HexNumber));
        }

        static byte[] hex_string_to_4_bytes(string value)
        {
            return BitConverter.GetBytes(uint.Parse(value, NumberStyles.HexNumber));
        }

        static byte[] hex_string_to_2_bytes(string value)
        {
            return BitConverter.GetBytes(UInt16.Parse(value, NumberStyles.HexNumber));
        }
        static byte[] hex_string_to_byte(string value)
        {
            return BitConverter.GetBytes(Byte.Parse(value, NumberStyles.HexNumber));
        }

        public byte[] GetBytesByType(ulong address)
        {
            return ReadMemory(address, Length);
        }

        public void SetBytesByType(ulong address, byte[] data)
        {
            WriteMemory(address, data);
        }

        public void CompareWithMemoryBufferNextScanner(byte[] default_value_0, byte[] default_value_1, byte[] buffer,
            ResultList old_result_list, ResultList new_result_list)
        {
            int length = Length;

            Byte[] new_value = new byte[length];
            for (old_result_list.Begin(); !old_result_list.End(); old_result_list.Next())
            {
                uint address_offset = 0;
                Byte[] old_value = null;
                old_result_list.Get(ref address_offset, ref old_value);
                Buffer.BlockCopy(buffer, (int)address_offset, new_value, 0, length);
                if (Comparer(default_value_0, default_value_1, old_value, new_value))
                {
                    new_result_list.Add(address_offset, new_value);
                }
            }
        }

        public void CompareWithMemoryBufferNewScanner(byte[] default_value_0, byte[] default_value_1, byte[] buffer,
            ResultList new_result_list, uint base_address)
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
                    new_result_list.Add((uint)i + base_address, new_value);
                }
            }
        }

        public void CompareWithMemoryBufferPointerScanner(ProcessManager processManager, byte[] buffer,
            PointerList pointerList, ulong base_address)
        {
            Byte[] address_buf = new byte[8];
            for (int i = 0; i + 8 < buffer.LongLength; i += 8)
            {
                Buffer.BlockCopy(buffer, i, address_buf, 0, 8);
                ulong address = BitConverter.ToUInt64(address_buf, 0);
                int sectionID = processManager.MappedSectionList.GetMappedSectionID(address);
                if (sectionID != -1)
                {
                    Pointer pointer = new Pointer(base_address + (ulong)i, address);
                    pointerList.Add(pointer);
                }
            }
        }

        public bool scan_type_equal_hex(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            if (default_value_0.Length != new_value.Length)
            {
                throw new ArgumentException("Length!!!");
            }

            for (int i = 0; i < default_value_0.Length; ++i)
            {
                if (default_value_0[i] != new_value[i])
                {
                    return false;
                }
            }

            return true;
        }

        bool scan_type_equal_string(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            if (default_value_0.Length != new_value.Length)
            {
                throw new ArgumentException("Length!!!");
            }

            for (int i = 0; i < default_value_0.Length; ++i)
            {
                if (default_value_0[i] != new_value[i])
                {
                    return false;
                }
            }
            return true;
        }

        bool scan_type_pointer_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value)
        {
            return BitConverter.ToUInt64(new_value, 0) != 0 ? true : false;
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
                case CONSTANT.BYTE_8_TYPE:
                    _valueType = ValueType.ULONG_TYPE;
                    break;
                case CONSTANT.BYTE_4_TYPE:
                    _valueType = ValueType.UINT_TYPE;
                    break;
                case CONSTANT.BYTE_2_TYPE:
                    _valueType = ValueType.USHORT_TYPE;
                    break;
                case CONSTANT.BYTE_TYPE:
                case CONSTANT.BYTE_1_TYPE:
                    _valueType = ValueType.BYTE_TYPE;
                    break;
                case CONSTANT.BYTE_DOUBLE_TYPE:
                    _valueType = ValueType.DOUBLE_TYPE;
                    break;
                case CONSTANT.BYTE_FLOAT_TYPE:
                    _valueType = ValueType.FLOAT_TYPE;
                    break;
                case CONSTANT.BYTE_HEX_TYPE:
                    _valueType = ValueType.HEX_TYPE;
                    break;
                case CONSTANT.BYTE_STRING_TYPE:
                    _valueType = ValueType.STRING_TYPE;
                    break;
                case CONSTANT.BYTE_POINTER:
                    _valueType = ValueType.POINTER_TYPE;
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
                    return CONSTANT.BYTE_8_TYPE;
                case ValueType.UINT_TYPE:
                    return CONSTANT.BYTE_4_TYPE;
                case ValueType.USHORT_TYPE:
                    return CONSTANT.BYTE_2_TYPE;
                case ValueType.BYTE_TYPE:
                    return CONSTANT.BYTE_TYPE;
                case ValueType.DOUBLE_TYPE:
                    return CONSTANT.BYTE_DOUBLE_TYPE;
                case ValueType.FLOAT_TYPE:
                    return CONSTANT.BYTE_FLOAT_TYPE;
                case ValueType.HEX_TYPE:
                    return CONSTANT.BYTE_HEX_TYPE;
                case ValueType.STRING_TYPE:
                    return CONSTANT.BYTE_STRING_TYPE;
                case ValueType.POINTER_TYPE:
                    return CONSTANT.BYTE_POINTER;
                default:
                    throw new Exception("GetStringOfValueType!!!");
            }
        }

        public static CompareType GetCompareByString(string compareTypeString)
        {
            switch (compareTypeString)
            {
                case CONSTANT.EXACT_VALUE:
                    return CompareType.EXACT_VALUE;
                case CONSTANT.FUZZY_VALUE:
                    return CompareType.FUZZY_VALUE;
                case CONSTANT.INCREASED_VALUE:
                    return CompareType.INCREASED_VALUE;
                case CONSTANT.INCREASED_VALUE_BY:
                    return CompareType.INCREASED_VALUE_BY;
                case CONSTANT.DECREASED_VALUE:
                    return CompareType.DECREASED_VALUE;
                case CONSTANT.DECREASED_VALUE_BY:
                    return CompareType.DECREASED_VALUE_BY;
                case CONSTANT.BIGGER_THAN:
                    return CompareType.BIGGER_THAN_VALUE;
                case CONSTANT.SMALLER_THAN:
                    return CompareType.SMALLER_THAN_VALUE;
                case CONSTANT.CHANGED_VALUE:
                    return CompareType.CHANGED_VALUE;
                case CONSTANT.UNCHANGED_VALUE:
                    return CompareType.UNCHANGED_VALUE;
                case CONSTANT.BETWEEN_VALUE:
                    return CompareType.BETWEEN_VALUE;
                case CONSTANT.UNKNOWN_INITIAL_VALUE:
                    return CompareType.UNKNOWN_INITIAL_VALUE;
                default:
                    throw new Exception("compareType???");
            }
        }
        public void InitNextScanMemoryHandler(string compareType)
        {
            CompareType = GetCompareByString(compareType);

            if (ValueType == ValueType.HEX_TYPE)
            {
                Length *= 2;
            }

            InitMemoryHandler(ValueType, CompareType, Alignment != 1, Length);
        }

        public void InitNextScanMemoryHandler(CompareType compareType)
        {

            if (ValueType == ValueType.HEX_TYPE)
            {
                Length *= 2;
            }

            InitMemoryHandler(ValueType, compareType, Alignment != 1, Length);
        }

        public void InitMemoryHandler(string valueType, string compareType, bool is_alignment, int type_length = 0)
        {
            ValueType = GetValueTypeByString(valueType);
            CompareType = GetCompareByString(compareType);

            InitMemoryHandler(ValueType, CompareType, is_alignment, type_length);
        }

        public void InitMemoryHandler(ValueType valueType, CompareType compareType, bool is_alignment, int type_length = 0)
        {
            ValueType = valueType;
            CompareType = compareType;

            switch (valueType)
            {
                case ValueType.DOUBLE_TYPE:
                    BytesToString = double_to_string;
                    BytesToHexString = double_to_hex_string;
                    StringToBytes = string_to_double;
                    HexStringToBytes = hex_string_to_double;
                    Length = sizeof(double);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.FLOAT_TYPE:
                    BytesToString = float_to_string;
                    BytesToHexString = float_to_hex_string;
                    StringToBytes = string_to_float;
                    HexStringToBytes = hex_string_to_float;
                    Length = sizeof(float);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.ULONG_TYPE:
                    BytesToString = ulong_to_string;
                    BytesToHexString = ulong_to_hex_string;
                    StringToBytes = string_to_8_bytes;
                    HexStringToBytes = hex_string_to_8_bytes;
                    Length = sizeof(ulong);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.UINT_TYPE:
                    BytesToString = uint_to_string;
                    BytesToHexString = uint_to_hex_string;
                    StringToBytes = string_to_4_bytes;
                    HexStringToBytes = hex_string_to_4_bytes;
                    Length = sizeof(uint);
                    Alignment = (is_alignment) ? 4 : 1;
                    break;
                case ValueType.USHORT_TYPE:
                    BytesToString = uint16_to_string;
                    BytesToHexString = uint16_to_hex_string;
                    StringToBytes = string_to_2_bytes;
                    HexStringToBytes = hex_string_to_2_bytes;
                    Length = sizeof(ushort);
                    Alignment = (is_alignment) ? 2 : 1;
                    break;
                case ValueType.BYTE_TYPE:
                    BytesToString = uchar_to_string;
                    BytesToHexString = uchar_to_hex_string;
                    StringToBytes = string_to_byte;
                    HexStringToBytes = hex_string_to_byte;
                    Length = sizeof(byte);
                    Alignment = 1;
                    break;
                case ValueType.HEX_TYPE:
                    BytesToString = hex_to_string;
                    BytesToHexString = hex_to_hex_string;
                    StringToBytes = string_to_hex_bytes;
                    HexStringToBytes = null;
                    Alignment = 1;
                    Length = type_length / 2;
                    break;
                case ValueType.STRING_TYPE:
                    BytesToString = string_to_string;
                    BytesToHexString = string_to_hex_string;
                    StringToBytes = string_to_string_bytes;
                    HexStringToBytes = null;
                    Alignment = 1;
                    Length = type_length;
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
                    ParseFirstValue = false;
                    ParseSecondValue = false;
                    break;
                case CompareType.FUZZY_VALUE:
                    switch (valueType)
                    {
                        case ValueType.DOUBLE_TYPE:
                            Comparer = scan_type_fuzzy_equal_double;
                            break;
                        case ValueType.FLOAT_TYPE:
                            Comparer = scan_type_fuzzy_equal_float;
                            break;
                        default:
                            break;
                    }
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                        case ValueType.HEX_TYPE:
                            Comparer = scan_type_equal_hex;
                            break;
                        case ValueType.STRING_TYPE:
                            Comparer = scan_type_equal_string;
                            break;
                        default:
                            break;
                    }
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = false;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = false;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = false;
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
                    ParseFirstValue = true;
                    ParseSecondValue = true;
                    break;
                case CompareType.POINTER_VALUE:
                    switch (valueType)
                    {
                        case ValueType.ULONG_TYPE:
                            Comparer = scan_type_between_ulong;
                            break;
                        default:
                            throw new Exception("Pointer Type!!!");
                    }
                    ParseFirstValue = false;
                    ParseSecondValue = false;
                    break;
                default:
                    break;
                    //throw new Exception("Unknown compare type.");
            }
        }
    }
}
