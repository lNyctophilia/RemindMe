using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReminderManager : MonoBehaviour
{
    public static ReminderManager Instance;

    [System.Serializable]
    public class ReminderList
    {
        public List<ReminderData> reminders = new List<ReminderData>();
    }

    public ReminderList reminderList = new ReminderList();

    private void Awake()
    {
        Instance = this;
        reminderList = SaveLoadSystem.Load();
        
        // Eğer dosya yoksa veya liste boşsa
        if (reminderList == null) reminderList = new ReminderList();
        
        // Eski sürümden gelen veri varsa ve UID yoksa onlara UID ata
        foreach(var r in reminderList.reminders)
        {
            if(string.IsNullOrEmpty(r.uid)) r.uid = System.Guid.NewGuid().ToString();
        }
    }

    private void Start()
    {
        NotificationPermission.PermissionRequest();
        // Uygulama başladığında tüm bildirimleri temizle ve tekrar kur (Senkronizasyon için en temizi)
        RescheduleAll();
    }

    // Tüm bildirimleri silip baştan kurar (Ghost bildirimleri önler)
    public void RescheduleAll()
    {
        NotificationManager.Instance.CancelAll();

        foreach (var reminder in reminderList.reminders)
        {
            int newId = NotificationManager.Instance.ScheduleNotification(reminder);
            reminder.notificationId = newId;
        }
        
        SaveLoadSystem.Save(reminderList);
        Debug.Log("🔄 Sistem Senkronize Edildi.");
    }

    public void AddReminder(ReminderData data)
    {
        // 1. Listeye Ekle
        reminderList.reminders.Add(data);
        
        // 2. Bildirimi Kur ve ID'yi Al
        int notifId = NotificationManager.Instance.ScheduleNotification(data);
        data.notificationId = notifId;

        // 3. Kaydet
        SaveLoadSystem.Save(reminderList);
        SaveLoadSystem.Backup(reminderList);
        Debug.Log($"✅ Eklendi: {data.title}");
    }

    public void DeleteReminder(ReminderData data)
    {
        // 1. Bildirimi İptal Et
        NotificationManager.Instance.CancelNotification(data.notificationId);

        // 2. Listeden Sil
        // Referans hatası olmaması için UID ile bulup siliyoruz
        var itemToRemove = reminderList.reminders.FirstOrDefault(x => x.uid == data.uid);
        if (itemToRemove != null)
        {
            reminderList.reminders.Remove(itemToRemove);
        }

        // 3. Kaydet
        SaveLoadSystem.Save(reminderList);
        SaveLoadSystem.Backup(reminderList);
        Debug.Log($"❌ Silindi: {data.title}");
    }

    public void UpdateReminder(ReminderData oldData, ReminderData newData)
    {
        // Eski bildirimi iptal et
        NotificationManager.Instance.CancelNotification(oldData.notificationId);
        
        // Listeden eskisini sil, yenisini ekle (veya veriyi güncelle)
        var existingItem = reminderList.reminders.FirstOrDefault(x => x.uid == oldData.uid);
        if(existingItem != null)
        {
            // Verileri güncelle
            existingItem.title = newData.title;
            existingItem.content = newData.content;
            existingItem.startDateTime = newData.startDateTime;
            existingItem.dayInterval = newData.dayInterval;

            // Yeni bildirim kur
            int newId = NotificationManager.Instance.ScheduleNotification(existingItem);
            existingItem.notificationId = newId;
            
            SaveLoadSystem.Save(reminderList);
            SaveLoadSystem.Backup(reminderList);
            Debug.Log($"✏️ Güncellendi: {existingItem.title}");
        }
    }
}