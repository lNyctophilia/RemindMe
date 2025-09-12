using System;
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

    public static void Backup(ReminderManager.ReminderList data)
    {
        try
        {
            // Cihazın güvenli klasör yolu
            string backupFolder = Path.Combine(Application.persistentDataPath, "Backups");
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            // Dosya adı: timestamp ile benzersiz
            string backupFile = Path.Combine(backupFolder,
                $"reminders_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            // JSON olarak kaydet
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(backupFile, json);

    #if UNITY_EDITOR
            Debug.Log($"[Backup] Yedek alındı: {backupFile}");
    #endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Backup] Yedekleme başarısız: {ex}");
        }
    }
}