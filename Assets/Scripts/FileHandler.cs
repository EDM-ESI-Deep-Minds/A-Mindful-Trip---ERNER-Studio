using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        string path = GetPath(filename);
        Debug.Log("Saving to: " + path);
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(path, content);
    
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        string path = GetPath(filename);
        Debug.Log("Saving to: " + path);
        string content = JsonUtility.ToJson(toSave);
        WriteFile(path, content);
    }

    public static List<T> ReadListFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }
        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }

    public static T ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename)); // Ensure GetPath is used
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }
        return JsonUtility.FromJson<T>(content);
    }


    public static string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }


    private static void WriteFile(string path, string content)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing to file {path}: {e.Message}");
        }
    }

    private static string ReadFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
            else
            {
                Debug.LogWarning($"File not found: {path}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading file {path}: {e.Message}");
        }
        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}