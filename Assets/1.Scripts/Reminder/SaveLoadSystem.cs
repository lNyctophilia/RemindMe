using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    private static readonly string FilePath =
        Path.Combine(Application.persistentDataPath, "reminders.json");

    public static void Save(ReminderManager.ReminderList data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
#if UNITY_EDITOR
        Debug.Log($"[SaveLoad] Saved: {FilePath}");
#endif
    }

    public static ReminderManager.ReminderList Load()
    {
        if (!File.Exists(FilePath))
            return new ReminderManager.ReminderList();

        var json = File.ReadAllText(FilePath);
        var loaded = JsonUtility.FromJson<ReminderManager.ReminderList>(json);
        return loaded ?? new ReminderManager.ReminderList();
    }
}