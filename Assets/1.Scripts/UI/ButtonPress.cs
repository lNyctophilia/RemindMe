using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] private float _clickIncreaseRate = 1.4f;
    [SerializeField] private float _hoverIncreaseRate = 1.05f;
    [SerializeField] private float _duration = 0.1f;
    private Vector3 buttonScale;
    

    private void Start()
    {
        buttonScale = gameObject.transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData) => Pressed();
    public void OnPointerUp(PointerEventData eventData) => UnPressed();

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, buttonScale * _hoverIncreaseRate, _duration).setIgnoreTimeScale(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, buttonScale, _duration).setIgnoreTimeScale(true);
    }

    void Pressed()
    {
        LeanTween.scale(gameObject, buttonScale / _clickIncreaseRate, _duration / 2).setIgnoreTimeScale(true);
    }
    void UnPressed() => LeanTween.scale(gameObject, buttonScale, _duration / 2).setIgnoreTimeScale(true);
}

