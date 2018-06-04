using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PS4_Cheater
{
    public enum CheatType
    {
        DATA_TYPE,
        SIMPLE_POINTER_TYPE,
        NONE_TYPE,
    }

    public enum CheatOperatorType
    {
        DATA_TYPE,
        OFFSET_TYPE,
        ADDRESS_TYPE,
        SIMPLE_POINTER_TYPE,
        POINTER_TYPE,
        ARITHMETIC_TYPE,
    }

    public enum ToStringType
    {
        DATA_TYPE,
        ADDRESS_TYPE,
        ARITHMETIC_TYPE,
    }

    public class CheatOperator
    {
        public CheatOperator(ValueType valueType, ProcessManager processManager)
        {
            ProcessManager = processManager;
            ValueType = valueType;
        }

        private ValueType _valueType;

        protected MemoryHelper MemoryHelper = new MemoryHelper(true, 0);

        public ProcessManager ProcessManager { get; set; }

        public ValueType ValueType {
            get {
                return _valueType;
            }
            set {
                _valueType = value;
                MemoryHelper.InitMemoryHandler(ValueType, CompareType.NONE, false);
            }
        }
        public CheatOperatorType CheatOperatorType { get; set; }

        public virtual byte[] Get(int idx = 0) { return null; }

        public virtual byte[] GetRuntime() { return null; }

        public virtual void Set(CheatOperator SourceCheatOperator, int idx = 0) { }

        public virtual void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0) { }

        public virtual int GetSectionID() { return -1; }

        public virtual bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format) { return false; }

        public virtual string ToString(bool simple) { return null; }

        public virtual string Dump(bool simpleFormat) { return null; }

        public virtual string Display() { return null; }
    }

    public class DataCheatOperator : CheatOperator
    {
        private const int DATA_TYPE = 0;
        private const int DATA = 1;

        private byte[] data;

        public DataCheatOperator(string data, ValueType valueType, ProcessManager processManager)
            :base(valueType, processManager)
        {
            this.data = MemoryHelper.StringToBytes(data);
            CheatOperatorType = CheatOperatorType.DATA_TYPE;
        }

        public DataCheatOperator(byte[] data, ValueType valueType, ProcessManager processManager)
            :base(valueType, processManager)
        {
            this.data = data;
            CheatOperatorType = CheatOperatorType.DATA_TYPE;
        }

        public DataCheatOperator(ProcessManager processManager)
            :base(ValueType.NONE_TYPE, processManager)
        {
            CheatOperatorType = CheatOperatorType.DATA_TYPE;
        }

        public override byte[] Get(int idx = 0) { return data; }

        public override byte[] GetRuntime() { return data; }

        public override void Set(CheatOperator SourceCheatOperator, int idx = 0)
        {
            data = new byte[MemoryHelper.Length];
            Buffer.BlockCopy(SourceCheatOperator.Get(), 0, data, 0, MemoryHelper.Length);
        }

        public override void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0)
        {
            data = new byte[MemoryHelper.Length];
            Buffer.BlockCopy(SourceCheatOperator.GetRuntime(), 0, data, 0, MemoryHelper.Length);
        }

        public void Set(string data)
        {
            this.data = MemoryHelper.StringToBytes(data);
        }

        public void Set(byte[] data)
        {
            this.data = data;
        }

        public override string ToString(bool simple) {
            return MemoryHelper.BytesToString(data);
        }

        public override string Display()
        {
            return MemoryHelper.BytesToString(data);
        }

        public override bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format)
        {
            ValueType = MemoryHelper.GetValueTypeByString(cheat_elements[start_idx + DATA_TYPE]);
            data = MemoryHelper.StringToBytes(cheat_elements[start_idx + DATA]);
            start_idx += 2;
            return true;
        }

        public override string Dump(bool simpleFormat)
        {
            string save_buf = "";
            save_buf += MemoryHelper.GetStringOfValueType(ValueType) + "|";
            save_buf += MemoryHelper.BytesToString(data) + "|";
            return save_buf;
        }
    }

    public class OffsetCheatOperator : CheatOperator
    {
        public long Offset { get; set; }

        public OffsetCheatOperator(long offset, ValueType valueType, ProcessManager processManager)
            : base(valueType, processManager)
        {
            this.Offset = offset;
            CheatOperatorType = CheatOperatorType.OFFSET_TYPE;
        }

        public OffsetCheatOperator(ProcessManager processManager)
            : base(ValueType.NONE_TYPE, processManager)
        {
            CheatOperatorType = CheatOperatorType.OFFSET_TYPE;
        }

        public override byte[] Get(int idx = 0) { return BitConverter.GetBytes(Offset); }

        public override byte[] GetRuntime() { return BitConverter.GetBytes(Offset); }

        public override void Set(CheatOperator SourceCheatOperator, int idx = 0)
        {
            Offset =  BitConverter.ToInt64(SourceCheatOperator.Get(), 0);
        }

        public override void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0)
        {
            Offset = BitConverter.ToInt64(SourceCheatOperator.Get(), 0);
        }

        public void Set(long offset)
        {
            this.Offset = offset;
        }

        public override string ToString(bool simple)
        {
            return Offset.ToString("X16");
        }

        public override string Display()
        {
            return Offset.ToString("X16");
        }

        public override bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format)
        {
            Offset = Int64.Parse(cheat_elements[start_idx], NumberStyles.HexNumber);
            start_idx += 1;
            return true;
        }

        public override string Dump(bool simpleFormat)
        {
            string save_buf = "";
            save_buf += "+";
            save_buf += Offset.ToString("X");
            return save_buf;
        }
    }

    public class AddressCheatOperator : CheatOperator
    {
        private const int SECTION_ID = 0;
        private const int ADDRESS_OFFSET = 1;

        public ulong Address { get; set; }

        public AddressCheatOperator(ulong Address, ProcessManager processManager)
            : base(ValueType.ULONG_TYPE, processManager)
        {
            this.Address = Address;
            CheatOperatorType = CheatOperatorType.ADDRESS_TYPE;
        }

        public AddressCheatOperator(ProcessManager processManager)
            : base(ValueType.ULONG_TYPE, processManager)
        {
            CheatOperatorType = CheatOperatorType.ADDRESS_TYPE;
        }

        public override byte[] Get(int idx = 0)
        {
            return BitConverter.GetBytes(Address);
        }

        public override byte[] GetRuntime()
        {
            return MemoryHelper.ReadMemory(Address, MemoryHelper.Length);
        }

        public override int GetSectionID()
        {
            return ProcessManager.MappedSectionList.GetMappedSectionID(Address);
        }

        public override void Set(CheatOperator SourceCheatOperator, int idx = 0)
        {
            Address = BitConverter.ToUInt64(SourceCheatOperator.Get(), 0);
        }

        public override void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0)
        {
            MemoryHelper.WriteMemory(Address, SourceCheatOperator.GetRuntime());
        }

        public string DumpOldFormat()
        {
            string save_buf = "";

            int sectionID = ProcessManager.MappedSectionList.GetMappedSectionID(Address);
            MappedSection mappedSection = ProcessManager.MappedSectionList[sectionID];
            save_buf += sectionID + "|";
            save_buf += String.Format("{0:X}", Address - mappedSection.Start) + "|";
            return save_buf;
        }

        public override string Dump(bool simpleFormat)
        {
            string save_buf = "";

            int sectionID = ProcessManager.MappedSectionList.GetMappedSectionID(Address);
            MappedSection mappedSection = ProcessManager.MappedSectionList[sectionID];
            save_buf += String.Format("@{0:X}", Address) + "_";
            save_buf += sectionID + "_";
            save_buf += String.Format("{0:X}", Address - mappedSection.Start);
            return save_buf;
        }

        public override bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format)
        {
            if (simple_format)
            {
                string address = cheat_elements[start_idx++];
                string[] address_elements = address.Split('_');

                int sectionID = int.Parse(address_elements[1]);
                if (sectionID >= ProcessManager.MappedSectionList.Count || sectionID < 0)
                {
                    return false;
                }

                ulong addressOffset = ulong.Parse(address_elements[2], NumberStyles.HexNumber);

                Address = addressOffset + ProcessManager.MappedSectionList[sectionID].Start;
            }
            return false;
        }

        public bool ParseOldFormat(string[] cheat_elements, ref int start_idx)
        {
            int sectionID = int.Parse(cheat_elements[start_idx + SECTION_ID]);
            if (sectionID >= ProcessManager.MappedSectionList.Count || sectionID < 0)
            {
                return false;
            }

            ulong addressOffset = ulong.Parse(cheat_elements[start_idx + ADDRESS_OFFSET], NumberStyles.HexNumber);

            Address = addressOffset + ProcessManager.MappedSectionList[sectionID].Start;

            start_idx += 2;
            return true;
        }

        public override string Display()
        {
            return Address.ToString("X");
        }

        public override string ToString()
        {
            return Address.ToString("X");
        }
    }

    public class SimplePointerCheatOperator : CheatOperator
    {
        private AddressCheatOperator Address { get; set; }
        private List<OffsetCheatOperator> Offsets  { get; set; }

        public SimplePointerCheatOperator(AddressCheatOperator Address, List<OffsetCheatOperator> Offsets, ValueType valueType, ProcessManager processManager)
            :base(valueType, processManager)
        {
            this.Address = Address;
            this.Offsets = Offsets;
            CheatOperatorType = CheatOperatorType.SIMPLE_POINTER_TYPE;
        }

        public SimplePointerCheatOperator(ProcessManager processManager)
            : base(ValueType.NONE_TYPE, processManager)
        {
            Address = new AddressCheatOperator(ProcessManager);
            Offsets = new List<OffsetCheatOperator>();

            CheatOperatorType = CheatOperatorType.SIMPLE_POINTER_TYPE;
        }

        public override byte[] Get(int idx = 0)
        {
            return Address.Get();
        }

        public override byte[] GetRuntime()
        {
            return MemoryHelper.ReadMemory(GetAddress(), MemoryHelper.Length);
        }

        public override int GetSectionID()
        { 
            return ProcessManager.MappedSectionList.GetMappedSectionID(GetAddress());
        }

        public override void Set(CheatOperator SourceCheatOperator, int idx = 0)
        {
            throw new Exception("Pointer Set!!");
        }

        private ulong GetAddress()
        {
            ulong address = BitConverter.ToUInt64(Address.GetRuntime(), 0);
            int i = 0;
            for (; i < Offsets.Count - 1; ++i)
            {
                Byte[] new_address = MemoryHelper.ReadMemory((ulong)((long)address + Offsets[i].Offset), 8);
                address = BitConverter.ToUInt64(new_address, 0);
            }

            if (i < Offsets.Count)
            {
                address += (ulong)Offsets[i].Offset;
            }

            return address;
        }

        public override void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0)
        {
            byte[] buf = new byte[MemoryHelper.Length];
            Buffer.BlockCopy(SourceCheatOperator.GetRuntime(), 0, buf, 0, MemoryHelper.Length);

            MemoryHelper.WriteMemory(GetAddress(), buf);
        }

        public override bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format)
        {
            ValueType = MemoryHelper.GetValueTypeByString(cheat_elements[start_idx + 0]);
            string pointer_str = cheat_elements[start_idx + 1];
            int pointer_idx = 0;
            string[] pointer_list = pointer_str.Split('+');

            Address.Parse(pointer_list, ref pointer_idx, simple_format);

            for (int i = 1; i < pointer_list.Length; ++i)
            {
                OffsetCheatOperator offset = new OffsetCheatOperator(ProcessManager);
                offset.Parse(pointer_list, ref pointer_idx, simple_format);
                Offsets.Add(offset);
            }

            start_idx += 2;

            return true;
        }

        public override string Display()
        {
            return "p->" + GetAddress().ToString("X");
        }

        public override string Dump(bool simpleFormat)
        {
            string dump_buf = "";

            dump_buf += MemoryHelper.GetStringOfValueType(ValueType) + "|";
            dump_buf += Address.Dump(simpleFormat);
            for (int i = 0; i < Offsets.Count; ++i)
            {
                dump_buf += Offsets[i].Dump(simpleFormat);
            }
            return dump_buf;
        }
    }

    public enum ArithmeticType
    {
        ADD_TYPE,
        SUB_TYPE,
        MUL_TYPE,
        DIV_TYPE,
    }

    public class BinaryArithmeticCheatOperator : CheatOperator
    {
        public CheatOperator Left { get; set; }
        public CheatOperator Right { get; set; }

        private ArithmeticType ArithmeticType { get; set; }

        public BinaryArithmeticCheatOperator(CheatOperator left, CheatOperator right, ArithmeticType ArithmeticType,
            ProcessManager processManager)
            : base(left.ValueType, processManager)
        {
            Left = left;
            Right = right;
            this.ArithmeticType = ArithmeticType;
            CheatOperatorType = CheatOperatorType.ARITHMETIC_TYPE;
        }

        public override byte[] Get(int idx)
        {
            if (idx == 0) return Left.Get();
            return Right.Get();
        }

        public byte[] GetRuntime(int idx)
        {
            byte[] left_buf = new byte[MemoryHelper.Length];
            Buffer.BlockCopy(Left.Get(), 0, left_buf, 0, MemoryHelper.Length);
            byte[] right_buf = new byte[MemoryHelper.Length];
            Buffer.BlockCopy(Right.Get(), 0, right_buf, 0, MemoryHelper.Length);
            ulong left = BitConverter.ToUInt64(left_buf, 0);
            ulong right = BitConverter.ToUInt64(right_buf, 0);
            ulong result = 0;

            switch (ArithmeticType)
            {
                case ArithmeticType.ADD_TYPE:
                    result = left + right;
                    break;
                case ArithmeticType.SUB_TYPE:
                    result = left - right;
                    break;
                case ArithmeticType.MUL_TYPE:
                    result = left * right;
                    break;
                case ArithmeticType.DIV_TYPE:
                    result = left / right;
                    break;
                default:
                    throw new Exception("ArithmeticType!!!");
            }
            return MemoryHelper.StringToBytes(result.ToString());
        }

        public override void Set(CheatOperator SourceCheatOperator, int idx = 0)
        {
            throw new Exception("Set BinaryArithmeticCheatOperator");
        }

        public override void SetRuntime(CheatOperator SourceCheatOperator, int idx = 0)
        {
            throw new Exception("SetRuntime BinaryArithmeticCheatOperator");
        }

        public override bool Parse(string[] cheat_elements, ref int start_idx, bool simple_format)
        {
            if (Left.Parse(cheat_elements, ref start_idx, simple_format))
            {
                return false;
            }

            switch (cheat_elements[start_idx])
            {
                case "+":
                    ArithmeticType = ArithmeticType.ADD_TYPE;
                    break;
                case "-":
                    ArithmeticType = ArithmeticType.SUB_TYPE;
                    break;
                case "*":
                    ArithmeticType = ArithmeticType.MUL_TYPE;
                    break;
                case "/":
                    ArithmeticType = ArithmeticType.DIV_TYPE;
                    break;
                default:
                    throw new Exception("ArithmeticType parse!!!");
            }
            ++start_idx;

            if (Right.Parse(cheat_elements, ref start_idx, simple_format))
            {
                return false;
            }

            return true;
        }

        public override string Display()
        {
            return "";
        }

        public override string Dump(bool simpleFormat)
        {
            return Left.Dump(simpleFormat) + Right.Dump(simpleFormat);
        }
    }

    public class Cheat
    {

        public CheatType CheatType { get; set; }

        protected ProcessManager ProcessManager;

        public string Description { get; set; }

        public bool Lock { get; set; }

        public bool AllowLock { get; set; }

        public virtual bool Parse(string[] cheat_elements)
        {
            return false;
        }

        public Cheat(ProcessManager ProcessManager)
        {
            this.ProcessManager = ProcessManager;
        }

        protected CheatOperator Source { get; set; }
        protected CheatOperator Destination { get; set; }

        public CheatOperator GetSource()
        {
            return Source;
        }

        public CheatOperator GetDestination()
        {
            return Destination;
        }
    }

    public class DataCheat : Cheat
    {
        private const int CHEAT_CODE_DATA_TYPE_FLAG = 5;
        private const int CHEAT_CODE_DATA_TYPE_DESCRIPTION = 6;

        private const int CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT = CHEAT_CODE_DATA_TYPE_DESCRIPTION + 1;

        public DataCheat(DataCheatOperator source, AddressCheatOperator dest, bool lock_, string description, ProcessManager processManager)
            : base(processManager)
        {
            CheatType = CheatType.DATA_TYPE;
            AllowLock = true;
            Source = source;
            Destination = dest;
            Lock = lock_;
            Description = description;
        }

        public DataCheat(ProcessManager ProcessManager) :
            base(ProcessManager)
        {
            Source = new DataCheatOperator(ProcessManager);
            Destination = new AddressCheatOperator(ProcessManager);
            CheatType = CheatType.DATA_TYPE;
            AllowLock = true;
        }

        public override bool Parse(string[] cheat_elements)
        {
            if (cheat_elements.Length < CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT)
            {
                return false;
            }

            int start_idx = 1;
            AddressCheatOperator addressCheatOperator = (AddressCheatOperator)Destination;
            if (!(addressCheatOperator.ParseOldFormat(cheat_elements, ref start_idx)))
            {
                return false;
            }

            if (!Source.Parse(cheat_elements, ref start_idx, true))
            {
                return false;
            }

            ulong flag = ulong.Parse(cheat_elements[CHEAT_CODE_DATA_TYPE_FLAG], NumberStyles.HexNumber);

            Lock = flag == 1 ? true : false;

            Description = cheat_elements[CHEAT_CODE_DATA_TYPE_DESCRIPTION];

            Destination.ValueType = Source.ValueType;

            return true;
        }

        public override string ToString()
        {
            string save_buf = "";
            save_buf += "data|";
            save_buf += ((AddressCheatOperator)Destination).DumpOldFormat();
            save_buf += Source.Dump(true);
            save_buf += (Lock ? "1" : "0") + "|";
            save_buf += Description + "|";
            save_buf += Destination.ToString() + "\n";
            return save_buf;
        }
    }


    public class SimplePointerCheat : Cheat
    {
        public SimplePointerCheat(ProcessManager ProcessManager)
            : base(ProcessManager)
        {
            CheatType = CheatType.SIMPLE_POINTER_TYPE;
            AllowLock = true;
        }

        public SimplePointerCheat(CheatOperator source, CheatOperator dest, bool lock_, string description, ProcessManager processManager)
            : base(processManager)
        {
            CheatType = CheatType.DATA_TYPE;
            AllowLock = true;
            Source = source;
            Destination = dest;
            Lock = lock_;
            Description = description;
        }

        public override bool Parse(string[] cheat_elements)
        {
            int start_idx = 1;

            if (cheat_elements[start_idx] == "address")
            {
                Destination = new AddressCheatOperator(ProcessManager);
            }
            else if (cheat_elements[start_idx] == "pointer")
            {
                Destination = new SimplePointerCheatOperator(ProcessManager);
            }

            ++start_idx;
            Destination.Parse(cheat_elements, ref start_idx, true);

            if (cheat_elements[start_idx] == "data")
            {
                Source = new DataCheatOperator(ProcessManager);
            }
            else if (cheat_elements[start_idx] == "pointer")
            {
                Source = new SimplePointerCheatOperator(ProcessManager);
            }

            ++start_idx;
            Source.Parse(cheat_elements, ref start_idx, true);

            ulong flag = ulong.Parse(cheat_elements[start_idx], NumberStyles.HexNumber);

            Lock = flag == 1 ? true : false;

            Description = cheat_elements[start_idx + 1];

            return true;
        }

        public override string ToString()
        {
            string save_buf = "";
            save_buf += "simple pointer|";
            save_buf += "pointer|";
            save_buf += Destination.Dump(true) + "|";
            save_buf += "data|";
            save_buf += Source.Dump(true);
            save_buf += (Lock ? "1" : "0") + "|";
            save_buf += Description + "|";
            save_buf += "\n";
            return save_buf;
        }
    }

    class CheatList
    {
        private List<Cheat> cheat_list;

        private const int CHEAT_CODE_HEADER_VERSION = 0;
        private const int CHEAT_CODE_HEADER_PROCESS_NAME = 1;
        private const int CHEAT_CODE_HEADER_PROCESS_ID   = 2;
        private const int CHEAT_CODE_HEADER_PROCESS_VER  = 3;

        private const int CHEAT_CODE_HEADER_ELEMENT_COUNT = CHEAT_CODE_HEADER_PROCESS_NAME + 1;

        private const int CHEAT_CODE_TYPE = 0;
        public CheatList()
        {
            cheat_list = new List<Cheat>();
        }

        public void Add(Cheat cheat)
        {
            cheat_list.Add(cheat);
        }

        public void RemoveAt(int idx)
        {
            cheat_list.RemoveAt(idx);
        }

        public bool Exist(Cheat cheat)
        {
            return false;
        }

        public bool Exist(ulong destAddress)
        {
            return false;
        }

        public bool LoadFile(string path, ProcessManager processManager, ComboBox comboBox)
        {
            string[] cheats = File.ReadAllLines(path);

            if (cheats.Length < 2)
            {
                return false;
            }

            string header = cheats[0];
            string[] header_items = header.Split('|');

            if (header_items.Length < CHEAT_CODE_HEADER_ELEMENT_COUNT)
            {
                return false;
            }

            string[] version = (header_items[CHEAT_CODE_HEADER_VERSION]).Split('.');

            ulong major_version = 0;
            ulong secondary_version = 0;

            ulong.TryParse(version[0], out major_version);
            if (version.Length > 1)
            {
                ulong.TryParse(version[1], out secondary_version);
            }

            if (major_version > CONSTANT.MAJOR_VERSION || (major_version == CONSTANT.MAJOR_VERSION && secondary_version > CONSTANT.SECONDARY_VERSION))
            {
                return false;
            }

            string process_name = header_items[CHEAT_CODE_HEADER_PROCESS_NAME];
            if (process_name != (string)comboBox.SelectedItem)
            {
                comboBox.SelectedItem = process_name;
            }

            if (process_name != (string)comboBox.SelectedItem)
            {
                MessageBox.Show("Invalid process or refresh processes first.");
                return false;
            }

            string game_id = "";
            string game_ver = "";

            if (header_items.Length > CHEAT_CODE_HEADER_PROCESS_ID)
            {
                game_id = header_items[CHEAT_CODE_HEADER_PROCESS_ID];
                game_id = game_id.Substring(3);
            }

            if (header_items.Length > CHEAT_CODE_HEADER_PROCESS_VER)
            {
                game_ver = header_items[CHEAT_CODE_HEADER_PROCESS_VER];
                game_ver = game_ver.Substring(4);
            }

            if (game_id != "" && game_ver != "")
            {
                GameInfo gameInfo = new GameInfo();
                if (gameInfo.GameID != game_id)
                {
                    if (MessageBox.Show("Your Game ID(" + gameInfo.GameID + ") is different with cheat file(" + game_id + "), still load?",
                        "Invalid game ID", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return false;
                    }
                }

                if (gameInfo.Version != game_ver)
                {
                    if (MessageBox.Show("Your game version(" + gameInfo.Version + ") is different with cheat file(" + game_ver + "), still load?",
                        "Invalid game version", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return false;
                    }
                }
            }

            for (int i = 1; i < cheats.Length; ++i)
            {
                string cheat_tuple = cheats[i];
                string[] cheat_elements = cheat_tuple.Split(new string[] { "|" }, StringSplitOptions.None);

                if (cheat_elements.Length == 0)
                {
                    continue;
                }

                if (cheat_elements[CHEAT_CODE_TYPE] == "data")
                {
                    DataCheat cheat = new DataCheat(processManager);
                    if (!cheat.Parse(cheat_elements))
                    {
                        MessageBox.Show("Invaid cheat code:" + cheat_tuple);
                        continue;
                    }

                    cheat_list.Add(cheat);
                }
                else if (cheat_elements[CHEAT_CODE_TYPE] == "simple pointer")
                {

                    SimplePointerCheat cheat = new SimplePointerCheat(processManager);
                    if (!cheat.Parse(cheat_elements))
                        continue;
                    cheat_list.Add(cheat);
                }
                else
                {
                    MessageBox.Show("Invaid cheat code:" + cheat_tuple);
                    continue;
                }
            }
            return true;
        }

        public void SaveFile(string path, string prcessName, ProcessManager processManager)
        {
            GameInfo gameInfo = new GameInfo();
            string save_buf = CONSTANT.MAJOR_VERSION + "."
                + CONSTANT.SECONDARY_VERSION
                + "|" + prcessName
                + "|ID:" + gameInfo.GameID
                + "|VER:" + gameInfo.Version
                + "|FM:" + Util.Version
                + "\n";

            for (int i = 0; i < cheat_list.Count; ++i)
            {
                save_buf += cheat_list[i].ToString();
            }

            StreamWriter myStream = new StreamWriter(path);
            myStream.Write(save_buf);
            myStream.Close();
        }

        public Cheat this[int index]
        {
            get
            {
                return cheat_list[index];
            }
            set
            {
                cheat_list[index] = value;
            }
        }

        public void Clear()
        {
            cheat_list.Clear();
        }

        public int Count { get { return cheat_list.Count; } }
    }
}
