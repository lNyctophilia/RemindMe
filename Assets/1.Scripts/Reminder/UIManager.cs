using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject emptyCanvas;
    public Transform contentParent;
    public GameObject reminderPrefab;

    [Header("Add/Edit Screen")]
    public GameObject AddScreen; // Hem ekleme hem düzenleme için ortak panel kullanıyorsan
    public TMP_InputField inputTitle;
    public TMP_InputField inputContent;
    public TMP_InputField inputDate;
    public TMP_InputField inputInterval;
    
    public Button saveButton;
    public Button closeButton;

    private ReminderData _currentEditingData = null; // Şu an düzenlenmekte olan veri

    private void Start()
    {
        RefreshUI();
        
        inputTitle.onSelect.RemoveAllListeners();
        inputContent.onSelect.RemoveAllListeners();
        inputDate.onSelect.RemoveAllListeners();
        inputInterval.onSelect.RemoveAllListeners();

        // Önce üzerindeki (Inspector dahil) tüm olayları temizle
        closeButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();

        inputTitle.onSelect.AddListener(_ => MoveCaretToEnd(inputTitle));
        inputContent.onSelect.AddListener(_ => MoveCaretToEnd(inputContent));
        inputDate.onSelect.AddListener(_ => MoveCaretToEnd(inputDate));
        inputInterval.onSelect.AddListener(_ => MoveCaretToEnd(inputInterval));

        // Sonra temiz temiz yenilerini ekle
        closeButton.onClick.AddListener(CloseScreen);
        saveButton.onClick.AddListener(OnSavePressed);
    }

    public void RefreshUI()
    {
        // Mevcut listeyi temizle
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        var list = ReminderManager.Instance.reminderList.reminders;

        if (list.Count == 0)
        {
            emptyCanvas.SetActive(true);
            return;
        }
        
        emptyCanvas.SetActive(false);

        foreach (var rem in list)
        {
            GameObject obj = Instantiate(reminderPrefab, contentParent);

            obj.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = rem.title;
            obj.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = rem.content;
            obj.transform.Find("StartDate").GetComponent<TextMeshProUGUI>().text = rem.startDateTime;

            string statusText = GetRemainingTimeText(rem);
            string intervalText = rem.dayInterval > 0 ? $"{rem.dayInterval} günde bir" : "Tek seferlik";
            string finalString = $"{intervalText} <color=#137FEC>{statusText}</color>";
            obj.transform.Find("DayInterval").GetComponent<TextMeshProUGUI>().text = finalString;

            // Silme Butonu
            Button deleteBtn = obj.transform.Find("TrashButton").GetComponent<Button>();
            deleteBtn.onClick.AddListener(() => 
            {
                Warning.Instance.SetWarningScreen(true, () => 
                {
                    ReminderManager.Instance.DeleteReminder(rem);
                    Warning.Instance.CloseWarningScreen();
                    RefreshUI();
                });
            });

            // Tıklayınca Düzenleme
            obj.GetComponent<Button>().onClick.AddListener(() => OpenEditScreen(rem));
        }
    }

    private string GetRemainingTimeText(ReminderData data)
    {
        DateTime targetTime = data.GetTargetDate();
        DateTime now = DateTime.Now;

        // Eğer tarih geçmişse ve tekrar döngüsü varsa, bir sonraki tarihi bul
        if (targetTime <= now && data.dayInterval > 0)
        {
            TimeSpan diff = now - targetTime;
            int intervalsPassed = (int)(diff.TotalDays / data.dayInterval) + 1;
            targetTime = targetTime.AddDays(intervalsPassed * data.dayInterval);
        }

        // Gün farkını hesapla (Sadece tarih bazlı, saati önemsemeden)
        TimeSpan timeDifference = targetTime.Date - now.Date;
        int daysLeft = timeDifference.Days;

        if (targetTime <= now && data.dayInterval == 0)
        {
             return "(Süresi Doldu)";
        }

        if (daysLeft == 0) return "(Bugün)";
        if (daysLeft == 1) return "(Yarın)";
        
        return $"({daysLeft} gün kaldı)";
    }

    // --- Ekleme ve Düzenleme Ekranı Yönetimi ---

    public void OpenAddScreen()
    {
        _currentEditingData = null; // Yeni kayıt
        
        ClearInputs();

        closeButton.gameObject.SetActive(true);
        AddScreen.SetActive(true);
    }

    public void OpenEditScreen(ReminderData data)
    {
        _currentEditingData = data; // Düzenleme modu
        inputTitle.text = data.title;
        inputContent.text = data.content;
        inputDate.text = data.startDateTime;
        inputInterval.text = data.dayInterval.ToString();

        closeButton.gameObject.SetActive(true);
        AddScreen.SetActive(true);
    }

    private void MoveCaretToEnd(TMP_InputField input)
    {
        input.caretPosition = input.text.Length;
        input.selectionAnchorPosition = input.text.Length;
        input.selectionFocusPosition = input.text.Length;
    }

    public void CloseScreen()
    {
        closeButton.gameObject.SetActive(false);
        AddScreen.SetActive(false);
        ClearInputs();
    }

    private void ClearInputs()
    {
        inputTitle.text = "";
        inputContent.text = "";
        inputDate.text = "";
        inputInterval.text = "";
    }

    private void OnSavePressed()
    {
        // Basit Validasyon
        if (string.IsNullOrWhiteSpace(inputTitle.text) || string.IsNullOrWhiteSpace(inputDate.text))
        {
            Debug.LogWarning("Başlık ve Tarih boş olamaz!");
            return;
        }

        int interval = 0;
        int.TryParse(inputInterval.text, out interval);

        // Geçici yeni veri objesi oluştur
        ReminderData newData = new ReminderData()
        {
            title = inputTitle.text,
            content = inputContent.text,
            startDateTime = inputDate.text,
            dayInterval = interval
        };

        if (_currentEditingData == null)
        {
            // --- YENİ KAYIT ---
            ReminderManager.Instance.AddReminder(newData);
        }
        else
        {
            // --- DÜZENLEME ---
            // Mevcut düzenlediğimiz verinin UID'sini koru
            newData.uid = _currentEditingData.uid; 
            ReminderManager.Instance.UpdateReminder(_currentEditingData, newData);
        }

        CloseScreen();
        RefreshUI();
    }
}