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
        reminderList = SaveLoadSystem.Load();
    }

    private void Start()
    {
        RefreshReminders();
    }

    public void RefreshReminders()
    {
        // Önce tüm bildirimleri iptal et
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        // Sonra mevcut reminder listesinden tekrar ekle
        foreach (var reminder in reminderList.reminders)
        {
            NotificationManager.Instance.ScheduleReminder(reminder);
        }

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