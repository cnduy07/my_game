using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PixelButtonPressOffset : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    readonly Vector2 pressedOffset = new Vector2(0f, -2f);
    RectTransform rect;
    Vector2 basePosition;
    bool pressed;

    void Awake()
    {
        rect = (RectTransform)transform;
        basePosition = rect.anchoredPosition;
    }

    void OnEnable()
    {
        if (rect == null) rect = (RectTransform)transform;
        basePosition = rect.anchoredPosition;
        pressed = false;
    }

    void OnDisable()
    {
        ResetPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        if (!pressed)
        {
            basePosition = rect.anchoredPosition;
            pressed = true;
        }

        rect.anchoredPosition = basePosition + pressedOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetPosition();
    }

    bool IsInteractable()
    {
        Button button = GetComponent<Button>();
        return button == null || button.interactable;
    }

    void ResetPosition()
    {
        if (!pressed || rect == null) return;
        rect.anchoredPosition = basePosition;
        pressed = false;
    }
}
