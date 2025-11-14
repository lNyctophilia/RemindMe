using UnityEngine;
using Unity.Notifications.Android;
using System;

public class NotificationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite _smallIcon;
    [SerializeField] private Sprite _largeIcon;

    public const string ChannelId = "reminder_channel";
    public static NotificationManager Instance;
    private static bool channelRegistered = false;

    public bool SetChannelRegistered { get => channelRegistered; set => channelRegistered = value; }

    private void Awake()
    {
        Instance = this;
        EnsureChannel();
    }
    private void Start()
    {
        NotificationPermission.PermissionRequest();
    }
    public static void EnsureChannel()
    {
        if (channelRegistered) return;

        var channel = new AndroidNotificationChannel
        {
            Id = ChannelId,
            Name = "Reminders",
            Description = "Hatırlatıcı Bildirimleri",
            Importance = Importance.High
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        channelRegistered = true;
    }

    public void ScheduleReminder(ReminderData data)
    {
        EnsureChannel();

        DateTime fireTime = data.GetTargetDate();

        // Geçmiş tarih kontrolü
        if (fireTime <= DateTime.Now)
        {
            if (data.dayInterval > 0)
            {
                // Repeat varsa gelecekteki ilk FireTime’a planla
                int daysPassed = (int)Math.Floor((DateTime.Now - fireTime).TotalDays);
                int increments = ((daysPassed / data.dayInterval) + 1) * data.dayInterval;
                fireTime = fireTime.AddDays(increments);
            }
            else
            {
                // Tek seferlik geçmiş bildirimi atma
                return;
            }
        }

        var n = new AndroidNotification
        {
            Title = data.title,
            Text = data.content,
            FireTime = fireTime,
            SmallIcon = "icon_1",
            LargeIcon = "icon_0"
        };

        if (data.dayInterval > 0)
            n.RepeatInterval = TimeSpan.FromDays(data.dayInterval);

        int id = AndroidNotificationCenter.SendNotification(n, ChannelId);
        data.notificationId = id;

#if UNITY_EDITOR
        Debug.Log($"[NotificationManager] Planlandı: '{data.title}' @ {fireTime} (repeat={data.dayInterval}) id={id}");
#endif
    }

    public void CancelReminder(ReminderData data)
    {
        AndroidNotificationCenter.CancelScheduledNotification(data.notificationId);
        AndroidNotificationCenter.CancelDisplayedNotification(data.notificationId);
        AndroidNotificationCenter.CancelNotification(data.notificationId);

        #if UNITY_EDITOR
            Debug.Log($"[NotificationManager] İptal edildi: id={data.notificationId} '{data.title}'");
        #endif

        ReminderManager.Instance.RefreshReminders();
    }
}