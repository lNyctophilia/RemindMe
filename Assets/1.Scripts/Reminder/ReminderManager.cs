using System;
using System.Collections.Generic;
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
        NotificationManager manager = NotificationManager.Instance;
        if (manager == null) return;

        foreach (var reminder in reminderList.reminders)
        {
            DateTime first = reminder.GetTargetDate();

            // Geçmiş tarih kontrolü
            if (first < DateTime.Now)
            {
                if (reminder.dayInterval > 0)
                {
                    // Tekrarlayan için future FireTime hesapla
                    int daysPassed = (int)Math.Floor((DateTime.Now - first).TotalDays);
                    int increments = ((daysPassed / reminder.dayInterval) + 1) * reminder.dayInterval;
                    DateTime nextFireTime = first.AddDays(increments);

                    var tempReminder = new ReminderData()
                    {
                        title = reminder.title,
                        content = reminder.content,
                        startDateTime = nextFireTime.ToString("dd.MM.yyyy HH:mm"),
                        dayInterval = reminder.dayInterval
                    };

                    manager.ScheduleReminder(tempReminder);
                    reminder.notificationId = tempReminder.notificationId;
                }
                // Tek seferlik geçmiş reminder → atlanır
            }
            /*
            else
            {
                // Gelecek tarih → normal planla
                manager.ScheduleReminder(reminder);
            }
            */
        }
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