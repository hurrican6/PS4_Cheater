using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Configuration;
namespace PS4_Cheater
{
    struct CONSTANT
    {
        public const uint SAVE_FLAG_NONE = 0x0;
        public const uint SAVE_FLAG_LOCK = 0x1;
        public const uint SAVE_FLAG_MODIFED = 0x2;

        public const uint SECTION_EXECUTABLE = 0x5;

        public const uint MAJOR_VERSION = 1;
        public const uint SECONDARY_VERSION = 3;
        public const uint THIRD_VERSION = 1;
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
