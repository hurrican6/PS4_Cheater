using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Configuration;
using librpc;
using System.Collections.Generic;

namespace PS4_Cheater
{

    class GameInfo
    {
        const string GAME_INFO_4_05_PROCESS_NAME = "SceCdlgApp";
        const string GAME_INFO_4_05_SECTION_NAME = "libSceCdlgUtilServer.sprx";
        const int GAME_INFO_4_05_SECTION_PROT = 3;
        const int GAME_INFO_4_05_ID_OFFSET = 0XA0;
        const int GAME_INFO_4_05_VERSION_OFFSET = 0XC8;

        const string GAME_INFO_4_55_PROCESS_NAME = "SceCdlgApp";
        const string GAME_INFO_4_55_SECTION_NAME = "libSceCdlgUtilServer.sprx";
        const int GAME_INFO_4_55_SECTION_PROT = 3;
        const int GAME_INFO_4_55_ID_OFFSET = 0XA0;
        const int GAME_INFO_4_55_VERSION_OFFSET = 0XC8;

		const string GAME_INFO_5_05_PROCESS_NAME = "SceCdlgApp";
        const string GAME_INFO_5_05_SECTION_NAME = "libSceCdlgUtilServer.sprx";
        const int GAME_INFO_5_05_SECTION_PROT = 3;
        const int GAME_INFO_5_05_ID_OFFSET = 0XA0;
        const int GAME_INFO_5_05_VERSION_OFFSET = 0XC8;
		
        public string GameID = "";
        public string Version = "";

        public GameInfo()
        {
            string process_name = "";
            string section_name = "";
            ulong id_offset = 0;
            ulong version_offset = 0;
            int section_prot = 0;

            switch (Util.Version)
            {
                case 405:
                    process_name = GAME_INFO_4_05_PROCESS_NAME;
                    section_name = GAME_INFO_4_05_SECTION_NAME;
                    id_offset = GAME_INFO_4_05_ID_OFFSET;
                    version_offset = GAME_INFO_4_05_VERSION_OFFSET;
                    section_prot = GAME_INFO_4_05_SECTION_PROT;
                    break;
                case 455:
                    process_name = GAME_INFO_4_55_PROCESS_NAME;
                    section_name = GAME_INFO_4_55_SECTION_NAME;
                    id_offset = GAME_INFO_4_55_ID_OFFSET;
                    version_offset = GAME_INFO_4_55_VERSION_OFFSET;
                    section_prot = GAME_INFO_4_55_SECTION_PROT;
                    break;
				case 505:
                    process_name = GAME_INFO_5_05_PROCESS_NAME;
                    section_name = GAME_INFO_5_05_SECTION_NAME;
                    id_offset = GAME_INFO_5_05_ID_OFFSET;
                    version_offset = GAME_INFO_5_05_VERSION_OFFSET;
                    section_prot = GAME_INFO_5_05_SECTION_PROT;
                    break;
                default:
                    break;
            }

            try
            {
                ProcessManager processManager = new ProcessManager();
                ProcessInfo processInfo = processManager.GetProcessInfo(process_name);

                MemoryHelper memoryHelper = new MemoryHelper(false, processInfo.pid);
                MappedSectionList mappedSectionList = processManager.MappedSectionList;
                mappedSectionList.InitMemorySectionList(processInfo);
                List<MappedSection> sectionList = mappedSectionList.GetMappedSectionList(section_name, section_prot);

                if (sectionList.Count != 1)
                    return;

                GameID = System.Text.Encoding.Default.GetString(memoryHelper.ReadMemory(sectionList[0].Start + id_offset, 16));
                GameID = GameID.Trim(new char[] { '\0' });
                Version = System.Text.Encoding.Default.GetString(memoryHelper.ReadMemory(sectionList[0].Start + version_offset, 16));
                Version = Version.Trim(new char[] { '\0' });
            }
            catch
            {

            }
        }
    }

    class CONSTANT
    {
        public const uint SAVE_FLAG_NONE = 0x0;
        public const uint SAVE_FLAG_LOCK = 0x1;
        public const uint SAVE_FLAG_MODIFED = 0x2;

        public const uint SECTION_EXECUTABLE = 0x5;

        public const uint MAJOR_VERSION = 1;
        public const uint SECONDARY_VERSION = 4;
        public const uint THIRD_VERSION = 5;

        public const string EXACT_VALUE = "Exact Value";
        public const string FUZZY_VALUE = "Fuzzy Value";
        public const string INCREASED_VALUE = "Increased Value";
        public const string INCREASED_VALUE_BY = "Increased Value By";
        public const string DECREASED_VALUE = "Decreased Value";
        public const string DECREASED_VALUE_BY = "Decreased Value By";
        public const string BIGGER_THAN = "Bigger Than";
        public const string SMALLER_THAN = "Smaller Than";
        public const string CHANGED_VALUE = "Changed Value";
        public const string UNCHANGED_VALUE = "Unchanged Value";
        public const string BETWEEN_VALUE = "Between Value";
        public const string UNKNOWN_INITIAL_VALUE = "Unknown Initial Value";

        public const string BYTE_TYPE = "byte";
        public const string BYTE_1_TYPE = "1 byte";
        public const string BYTE_2_TYPE = "2 bytes";
        public const string BYTE_4_TYPE = "4 bytes";
        public const string BYTE_8_TYPE = "8 bytes";
        public const string BYTE_FLOAT_TYPE = "float";
        public const string BYTE_DOUBLE_TYPE = "double";
        public const string BYTE_STRING_TYPE = "string";
        public const string BYTE_HEX_TYPE = "hex";
        public const string BYTE_POINTER = "pointer";

        public const string STOP = "Stop";
        public const string FIRST_SCAN = "First Scan";
        public const string NEW_SCAN = "New Scan";
        public const string NEXT_SCAN = "Next Scan";
        public const string REFRESH = "Refresh";

        public static string[] SEARCH_VALUE_TYPE = new string[]
        {
             CONSTANT.BYTE_TYPE,
             CONSTANT.BYTE_2_TYPE,
             CONSTANT.BYTE_4_TYPE,
             CONSTANT.BYTE_8_TYPE,
             CONSTANT.BYTE_FLOAT_TYPE,
             CONSTANT.BYTE_DOUBLE_TYPE,
             CONSTANT.BYTE_STRING_TYPE,
             CONSTANT.BYTE_HEX_TYPE,
        };

        public const int MAX_PEEK_QUEUE = 4;
        public const int PEEK_BUFFER_LENGTH = 32 * 1024 * 1024;
    }

    public class Util
    {
        public static int DefaultProcessID = 0;
        public static int SceProcessID = 0;
        public static int Version = 0;
    }
       
    class Config
    {
        public static string fileName = System.IO.Path.GetFileName(Application.ExecutablePath);
        public static bool addSetting(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(fileName);
                config.AppSettings.Settings.Add(key, value);
                config.Save();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public static string getSetting(string key)
        {
            try
            {
                Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(fileName);
                string value = config.AppSettings.Settings[key].Value;
                return value;
            }
            catch
            {

            }
            return "";
        }
        public static bool updateSeeting(string key, string newValue)
        {
            try
            {
                Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(fileName);
                string value = config.AppSettings.Settings[key].Value = newValue;
                config.Save();
                return true;
            }
            catch
            {
                addSetting(key, newValue);
            }
            return false;
        }
    }
}
