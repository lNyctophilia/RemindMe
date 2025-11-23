using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class ReminderData
{
    // Her hatırlatıcının kendine ait değişmez kimliği
    public string uid; 
    public string title;
    public string content;
    public string startDateTime; // Format: "dd.MM.yyyy HH:mm"
    public int dayInterval;
    public int notificationId; // Android bildirim ID'si

    // Constructor: Yeni oluşturulurken otomatik UID verir
    public ReminderData()
    {
        uid = System.Guid.NewGuid().ToString();
    }

    public DateTime GetTargetDate()
    {
        string[] formats = { "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH.mm" };
        if (DateTime.TryParseExact(startDateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
        {
            return parsed;
        }
        // Hata durumunda şimdiki zamanı döndür ki oyun çökmesin, loga yazsın
        Debug.LogWarning($"Tarih parse edilemedi: {startDateTime}, varsayılan atanıyor.");
        return DateTime.Now;
    }
}