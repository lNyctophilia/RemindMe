using UnityEngine;
using TMPro;

public class Testing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIManager uiManager;

    [Header("Input Fields")]
    public TMP_InputField titleInput;
    public TMP_InputField contentInput;
    public TMP_InputField startDateInput;
    public TMP_InputField daysIntervalInput;

    private void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.P))
            {
                titleInput.text = "Test Title";
                contentInput.text = "Test Content";
                startDateInput.text = "03.09.2025 06:00";
                daysIntervalInput.text = "1";

                uiManager.OnConfirmAdd();

                titleInput.text = "";
                contentInput.text = "";
                startDateInput.text = "";
                daysIntervalInput.text = "";
            }
        #endif
    }
}
