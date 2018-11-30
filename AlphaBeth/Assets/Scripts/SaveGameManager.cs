using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Currently a wrapper around PlayerPrefs, but as we will probably move away from PlayerPrefs in the future we can easily swap out.
public static class SaveGameManager
{
    public delegate void SaveGameDelegate(string key, object value);
    public delegate void SaveGameIntDelegate(string key, int value);
    public delegate void SaveGameFloatDelegate(string key, float value);
    public delegate void SaveGameBoolDelegate(string key, bool value);
    public delegate void SaveGameStringDelegate(string key, string value);
    public delegate void SaveGameDeletedDelegate();

    public static string SAVE_LEVEL_WIDTH = "SAVE_LEVEL_WIDTH";
    public static string SAVE_LEVEL_HEIGHT = "SAVE_LEVEL_HEIGHT";
    public static string SAVE_LEVEL_TEXTCHARACTERS = "SAVE_LEVEL_TEXTCHARACTERS";
    public static string SAVE_LEVEL_SEED = "SAVE_LEVEL_SEED";
    public static string SAVE_OPTION_NEWCHARSONMISTAKE = "SAVE_OPTION_NEWCHARSONMISTAKE";
    public static string SAVE_OPTION_FOGOFWAR = "SAVE_OPTION_FOGOFWAR";
    public static string SAVE_OPTION_NODESDISAPPEAR = "SAVE_OPTION_NODESDISAPPEAR";
    public static string SAVE_OPTION_SCREENSHAKE = "SAVE_OPTION_SCREENSHAKE";

    public static event SaveGameDelegate VariableChangedEvent;
    public static event SaveGameIntDelegate IntVariableChangedEvent;
    public static event SaveGameFloatDelegate FloatVariableChangedEvent;
    public static event SaveGameBoolDelegate BoolVariableChangedEvent;
    public static event SaveGameStringDelegate StringVariableChangedEvent;
    public static event SaveGameDeletedDelegate DeletedEvent;

    //---------------------
    // Mutators
    //---------------------
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);

        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (IntVariableChangedEvent != null)
            IntVariableChangedEvent(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);

        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (FloatVariableChangedEvent != null)
            FloatVariableChangedEvent(key, value);
    }

    public static void SetBool(string key, bool value)
    {
        int intValue = 1;
        if (value == false) { intValue = 0; }

        PlayerPrefs.SetInt(key, intValue);

        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (BoolVariableChangedEvent != null)
            BoolVariableChangedEvent(key, value);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);

        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (StringVariableChangedEvent != null)
            StringVariableChangedEvent(key, value);
    }

    public static void DeleteSavGame()
    {
        PlayerPrefs.DeleteAll();

        if (DeletedEvent != null)
            DeletedEvent();
    }

    //---------------------
    // Accessors
    //---------------------
    public static int GetInt(string key, int defaultValue = 0)
    {
        if (PlayerPrefs.HasKey(key) == false)
            Debug.Log("Save Game doesn't contain the " + key + " variable!");

        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static float GetFloat(string key, float defaultValue = 0.0f)
    {
        if (PlayerPrefs.HasKey(key) == false)
            Debug.Log("Save Game doesn't contain the " + key + " variable!");

        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        if (PlayerPrefs.HasKey(key) == false)
            Debug.Log("Save Game doesn't contain the " + key + " variable!");

        int defaultIntValue = 1;
        if (defaultValue == false) { defaultIntValue = 0; }

        //Convert
        return (PlayerPrefs.GetInt(key, defaultIntValue) != 0);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        if (PlayerPrefs.HasKey(key) == false)
            Debug.Log("Save Game doesn't contain the " + key + " variable!");

        return PlayerPrefs.GetString(key, defaultValue);
    }
}
