using System;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class ReminderManager : MonoBehaviour
{
    public static ReminderManager Instance;

    [Serializable]
    public class ReminderList
    {
        public List<ReminderData> reminders = new List<ReminderData>();
    }

    public ReminderList reminderList = new ReminderList();

    private void Awake()
    {
        Instance = this;
        reminderList = SaveLoadSystem.Load() ?? new ReminderList();
    }

    private void Start()
    {
        RefreshReminders();
    }

    public void RefreshReminders()
    {
        AndroidNotificationCenter.DeleteNotificationChannel(NotificationManager.ChannelId);
        NotificationManager.Instance.SetChannelRegistered = false;
        NotificationManager.EnsureChannel();

        foreach (var reminder in reminderList.reminders)
        {
            NotificationManager.Instance.ScheduleReminder(reminder);
        }

        // Burada tüm id'ler güncellenmiş oluyor → kaydet
        SaveLoadSystem.Save(reminderList);

        Debug.Log("🔄 Tüm hatırlatıcılar yenilendi.");
    }

    public void AddReminder(ReminderData data)
    {
        NotificationManager.Instance?.ScheduleReminder(data);

        reminderList.reminders.Add(data);
        SaveLoadSystem.Save(reminderList);

        Debug.Log($"📌 Hatırlatıcı eklendi: {data.title} ({data.GetTargetDate()}) " +
                  (data.dayInterval > 0 ? $"→ {data.dayInterval} günde bir tekrar" : "→ Tek seferlik"));
    }

    public void RemoveReminder(ReminderData data)
    {
        NotificationManager.Instance?.CancelReminder(data);

        reminderList.reminders.Remove(data);
        SaveLoadSystem.Save(reminderList);

        Debug.Log($"❌ Hatırlatıcı silindi: {data.title}");
    }
}