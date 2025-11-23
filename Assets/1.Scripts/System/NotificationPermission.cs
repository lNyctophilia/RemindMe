using UnityEngine;
using UnityEngine.Android; // Permission için gerekli

public static class NotificationPermission
{
    public static void PermissionRequest()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 1. Bildirim İzni (Android 13+)
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        // 2. Tam Zamanlı Alarm İzni (Android 12+)
        // Bu izin genelde otomatik verilir ama kontrol etmekte fayda var.
        if (!Permission.HasUserAuthorizedPermission("android.permission.SCHEDULE_EXACT_ALARM"))
        {
             // Genelde sistem otomatik verir, vermezse kullanıcıyı ayarlara yönlendirmek gerekir.
             // Şimdilik sadece manifest'te olması çoğu durumda yeterlidir.
        }
#endif
    }
}