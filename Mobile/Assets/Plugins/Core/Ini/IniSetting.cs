using System;
using System.IO;
using UnityEngine;

public static class IniSetting
{
    private static string mINIFileName = Application.dataPath + "/../config.ini";

    public static void SetIniFilePath(string path)
    {
        mINIFileName = path;
    }
    
    public static int GetInt(string Key, string sectionName, int defaultValue = 0)
    {
        int value = defaultValue;
        Action<INIParser> action =
            (ini) =>
            {
                if (!ini.IsKeyExists(sectionName, Key))
                    ini.WriteValue(sectionName, Key, defaultValue);
                value = ini.ReadValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
        //Debug.LogWarning(Key + "  " + value);
        return value;
    }

    public static bool GetBool(string Key, string sectionName, bool defaultValue = false)
    {
        bool value = defaultValue;

        Action<INIParser> action =
            (ini) =>
            {
                if (!ini.IsKeyExists(sectionName, Key))
                    ini.WriteValue(sectionName, Key, defaultValue);
                value = ini.ReadValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
        //Debug.LogWarning(Key + "  " + value);
        return value;
    }


    public static string GetString(string Key, string sectionName, string defaultValue = "")
    {
        string value = defaultValue;

        Action<INIParser> action =
            (ini) =>
            {
                if (!ini.IsKeyExists(sectionName, Key))
                    ini.WriteValue(sectionName, Key, defaultValue);
                value = ini.ReadValue(sectionName, Key, defaultValue);
            };

        OpenAndReadINI(mINIFileName, action);
        return value;
    }

    public static float GetFloat(string Key, string sectionName, float defaultValue = 0)
    {
        float value = defaultValue;

        Action<INIParser> action =
            (ini) =>
            {
                if (!ini.IsKeyExists(sectionName, Key))
                    ini.WriteValue(sectionName, Key, defaultValue);
                value = (float)ini.ReadValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
        //Debug.LogWarning(Key + "  " + value);
        return value;

    }

    public static void WriteString(string Key, string sectionName, string defaultValue = "")
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.WriteValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
    }

    public static void WriteBool(string Key, string sectionName, bool defaultValue = false)
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.WriteValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
    }
    public static void WriteFloat(string Key, string sectionName, float defaultValue = 0)
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.WriteValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
    }

    public static void WriteInt(string Key, string sectionName, int defaultValue = 0)
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.WriteValue(sectionName, Key, defaultValue);
            };
        OpenAndReadINI(mINIFileName, action);
    }

    public static void DeleteSection(string section)
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.SectionDelete(section);
            };
        OpenAndReadINI(mINIFileName, action);
    }


    public static void DeleteAllSections()
    {
        Action<INIParser> action =
            (ini) =>
            {
                ini.DeleteAllSection();
            };
        OpenAndReadINI(mINIFileName, action);
    }

    static void OpenAndReadINI(string path, Action<INIParser> actionRead)
    {
        try
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                Debug.LogWarning("Create Success:" + path);
            }
            //else
            {
                INIParser iniParser = new INIParser();
                iniParser.Open(path);

                actionRead(iniParser);

                iniParser.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("INIFile Not Found Or Create:\r\n" + path + "\r\n" + e);
        }
    }
}