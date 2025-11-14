using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("EmptyCanvas")]
    [SerializeField] private GameObject emptyCanvas;

    [Header("References")]
    public Transform contentParent;
    public GameObject reminderPrefab;

    [Header("Screen")]
    public GameObject AddScreen;
    public Button CancelButton;
    public Button ChangeButton;
    public TMP_InputField c_titleInput;
    public TMP_InputField c_contentInput;
    public TMP_InputField c_startDateInput;   // "dd.MM.yyyy HH:mm"
    public TMP_InputField c_daysIntervalInput;

    [Header("Input Fields")]
    public TMP_InputField titleInput;
    public TMP_InputField contentInput;
    public TMP_InputField startDateInput;   // "dd.MM.yyyy HH:mm"
    public TMP_InputField daysIntervalInput;

    private void Start()
    {
        if (ReminderManager.Instance != null)
            RefreshList();

        CancelButton.onClick.AddListener(() => CloseChangeScreen());
    }

    public void RefreshList()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        foreach (var reminder in ReminderManager.Instance.reminderList.reminders)
        {
            GameObject obj = Instantiate(reminderPrefab, contentParent);
            obj.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = reminder.title;
            obj.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = reminder.content;
            obj.transform.Find("StartDate").GetComponent<TextMeshProUGUI>().text =
                reminder.GetTargetDate().ToString("dd.MM.yyyy HH:mm") + " ->";
            obj.transform.Find("DayInterval").GetComponent<TextMeshProUGUI>().text =
                reminder.dayInterval > 0 ? $"{reminder.dayInterval} gün aralık" : "Tek seferlik";

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeButton.onClick.RemoveAllListeners();
                ChangeButton.onClick.AddListener(() => Change(reminder));
                OpenChangeScreen(reminder);
            });

            Button deleteBtn = obj.transform.Find("TrashButton").GetComponent<Button>();

            // Closure problemi çözümü
            var r = reminder;
            deleteBtn.onClick.AddListener(() =>
                Warning.Instance.SetWarningScreen(true, () =>
                {
                    ReminderManager.Instance.RemoveReminder(r);
                    RefreshList();
                })
            );
        }

        if (ReminderManager.Instance.reminderList.reminders.Count == 0)
            emptyCanvas.SetActive(true);
        else
            emptyCanvas.SetActive(false);
    }

    public void OpenChangeScreen(ReminderData oldReminder)
    {
        c_titleInput.text = oldReminder.title;
        c_contentInput.text = oldReminder.content;
        c_startDateInput.text = oldReminder.GetTargetDate().ToString("dd.MM.yyyy HH:mm");
        c_daysIntervalInput.text = oldReminder.dayInterval.ToString();

        AddScreen.SetActive(true);
        CancelButton.gameObject.SetActive(true);
    }
    public void Change(ReminderData oldReminder)
    {
        OnConfirmChange();
        ReminderManager.Instance.RemoveReminder(oldReminder);
        RefreshList();
        CloseChangeScreen();
    }
    public void CloseChangeScreen()
    {
        AddScreen.SetActive(false);
        CancelButton.gameObject.SetActive(false);
    }

    public void OnConfirmChange()
    {
        try
        {
            ReminderData data = new ReminderData()
            {
                title = c_titleInput.text,
                content = c_contentInput.text,
                startDateTime = c_startDateInput.text.Trim(),
                dayInterval = string.IsNullOrWhiteSpace(c_daysIntervalInput.text) ? 0 : int.Parse(c_daysIntervalInput.text.Trim())
            };

            DateTime target = data.GetTargetDate();
            Debug.Log($"✅ Tarih: {target}");

            ReminderManager.Instance.AddReminder(data);
            RefreshList();
        }
        catch (FormatException ex)
        {
            Debug.LogWarning($"⚠️ Tarih formatı hatalı: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Beklenmeyen hata: {ex}");
        }
    }

    public void OnConfirmAdd()
    {
        try
        {
            ReminderData data = new ReminderData()
            {
                title = titleInput.text,
                content = contentInput.text,
                startDateTime = startDateInput.text.Trim(),
                dayInterval = string.IsNullOrWhiteSpace(daysIntervalInput.text) ? 0 : int.Parse(daysIntervalInput.text.Trim())
            };

            DateTime target = data.GetTargetDate();
            Debug.Log($"✅ Tarih: {target}");

            ReminderManager.Instance.AddReminder(data);
            RefreshList();
        }
        catch (FormatException ex)
        {
            Debug.LogWarning($"⚠️ Tarih formatı hatalı: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Beklenmeyen hata: {ex}");
        }
    }

    public void OnBackupPressed()
    {
        SaveLoadSystem.Backup(ReminderManager.Instance.reminderList);
    }
}