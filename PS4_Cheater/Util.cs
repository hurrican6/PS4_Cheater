using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Configuration;
namespace PS4_Cheater
{
    class CONSTANT
    {
        public const uint SAVE_FLAG_NONE = 0x0;
        public const uint SAVE_FLAG_LOCK = 0x1;
        public const uint SAVE_FLAG_MODIFED = 0x2;

        public const uint SECTION_EXECUTABLE = 0x5;

        public const uint MAJOR_VERSION = 1;
        public const uint SECONDARY_VERSION = 3;
        public const uint THIRD_VERSION = 3;

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

    class Util
    {

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
