using UnityEngine;

public static class MobileUtilities
{
    // Standart Android Input Çubuğu genelde 50-60dp civarındadır. 
    // Garanti olsun diye 60dp alıyoruz.
    private const int AndroidInputBarHeightDP = 60;

    // Klavye yüksekliği (piksel)
    // withInputBox: Eğer "Hide Mobile Input" false ise (beyaz kutu varsa) true gönderin.
    public static int GetKeyboardHeightPixels(bool withInputBox = false)
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                var window   = activity.Call<AndroidJavaObject>("getWindow");
                var decor    = window.Call<AndroidJavaObject>("getDecorView");

                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    // 1. Görünür alan (Klavye hariç alan)
                    decor.Call("getWindowVisibleDisplayFrame", rect);
                    int visibleHeight = rect.Call<int>("height");

                    // 2. Gerçek ekran yüksekliği ve Yoğunluk (Density)
                    var dm = new AndroidJavaObject("android.util.DisplayMetrics");
                    var display = activity.Call<AndroidJavaObject>("getWindowManager")
                                          .Call<AndroidJavaObject>("getDefaultDisplay");
                    display.Call("getRealMetrics", dm);
                    
                    int realHeight = dm.Get<int>("heightPixels");
                    float density = dm.Get<float>("density"); // Ekran yoğunluğu (1.0, 2.0, 3.0 vs)

                    // 3. Saf Klavye Yüksekliği
                    int keyboardHeight = Mathf.Max(0, realHeight - visibleHeight);

                    // 4. Beyaz Kutu Hesabı
                    // Eğer klavye açıksa VE beyaz kutu modundaysak ekle
                    if (keyboardHeight > 0 && withInputBox)
                    {
                        // DP'yi Piksele çevir: Pixel = DP * Density
                        int extraBoxHeight = Mathf.RoundToInt(AndroidInputBarHeightDP * density);
                        keyboardHeight += extraBoxHeight;
                    }

                    return keyboardHeight;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("GetKeyboardHeightPixels failed: " + e.Message);
            return 0;
        }
    #elif UNITY_IOS && !UNITY_EDITOR
        // iOS'te TouchScreenKeyboard.area genelde accessory bar'ı da kapsar
        return (int)TouchScreenKeyboard.area.height;
    #else
        return 0; // Editörde 0 döner
    #endif
    }

    // UI birimi (RectTransform/Canvas ölçeğine çevrilmiş)
    public static int GetKeyboardHeightUI(RectTransform anyOnCanvas, bool withInputBox = false)
    {
        // Piksel cinsinden yüksekliği al (input box parametresini geçir)
        int px = GetKeyboardHeightPixels(withInputBox);

        var canvas = anyOnCanvas ? anyOnCanvas.GetComponentInParent<Canvas>() : null;
        float scale = (canvas != null) ? canvas.scaleFactor : 1f;

        // Canvas birimine çevir
        return Mathf.RoundToInt(px / Mathf.Max(0.0001f, scale));
    }
}