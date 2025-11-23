using UnityEngine;
using Unity.Notifications.Android;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public const string ChannelId = "reminder_channel_v2"; // Kanal ID'sini değiştirdim ki eski buglı kanal temizlensin

    private void Awake()
    {
        Instance = this;
        InitializeChannel();
    }

    private void InitializeChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = ChannelId,
            Name = "Hatırlatıcılar",
            Description = "Planlanmış hatırlatıcı bildirimleri",
            Importance = Importance.High,
            CanShowBadge = true,
            EnableLights = true,
            EnableVibration = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public int ScheduleNotification(ReminderData data)
    {
        DateTime fireTime = data.GetTargetDate();

        // Geçmiş zaman kontrolü ve döngü hesaplaması
        if (fireTime <= DateTime.Now)
        {
            if (data.dayInterval > 0)
            {
                // Geçmişte kalmışsa, gelecekteki bir sonraki periyoda atla
                TimeSpan diff = DateTime.Now - fireTime;
                int intervalsPassed = (int)(diff.TotalDays / data.dayInterval) + 1;
                fireTime = fireTime.AddDays(intervalsPassed * data.dayInterval);
            }
            else
            {
                // Tek seferlik ve zamanı geçmişse bildirim atma (-1 dön)
                return -1;
            }
        }

        var notification = new AndroidNotification
        {
            Title = data.title,
            Text = data.content,
            FireTime = fireTime,
            SmallIcon = "icon_1", // Unity ayarlarında bu ikonların (small/large) eklendiğinden emin ol
            LargeIcon = "icon_0"
        };

        if (data.dayInterval > 0)
        {
            notification.RepeatInterval = TimeSpan.FromDays(data.dayInterval);
        }

        // Yeni bir bildirim gönder ve ID'sini döndür
        return AndroidNotificationCenter.SendNotification(notification, ChannelId);
    }

    public void CancelNotification(int notificationId)
    {
        if (notificationId == 0) return;
        
        // SADECE Gelecek planını iptal et, ekrana düşmüşse elleme kalsın
        AndroidNotificationCenter.CancelScheduledNotification(notificationId);
        
        // BU SATIRI SİLİYORUZ (veya yorum satırı yap):
        // AndroidNotificationCenter.CancelDisplayedNotification(notificationId); 
        
        Debug.Log($"[Notification] Plan İptal Edildi ID: {notificationId}");
    }

    public void CancelAll()
    {
        // SADECE Planları iptal et
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        // BU SATIRI DA SİLİYORUZ:
        // AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }
}