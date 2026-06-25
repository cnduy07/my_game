using UnityEngine;
using UnityEngine.EventSystems;

public class UiInteractMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    public float hoverScale = 1.04f;
    public float pressedScale = 0.97f;
    public float selectedScale = 1.04f;
    public float pulseAmplitude = 0.018f;
    public float speed = 18f;

    bool hovered;
    bool pressed;
    bool selected;
    RectTransform rect;
    Vector3 baseScale;

    void Awake()
    {
        rect = transform as RectTransform;
        baseScale = transform.localScale;
    }

    void OnEnable()
    {
        if (rect == null)
            rect = transform as RectTransform;
        baseScale = transform.localScale;
    }

    void OnDisable()
    {
        hovered = false;
        pressed = false;
        selected = false;
        transform.localScale = baseScale;
    }

    void Update()
    {
        float target = 1f;
        if (selected)
            target = selectedScale + Mathf.Sin(Time.unscaledTime * 7.5f) * pulseAmplitude;
        if (hovered)
            target = Mathf.Max(target, hoverScale);
        if (pressed)
            target = pressedScale;

        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * target, Time.unscaledDeltaTime * speed);
    }

    public void SetSelected(bool value)
    {
        selected = value;
    }

    public void OnPointerEnter(PointerEventData eventData) => hovered = true;
    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        pressed = false;
    }

    public void OnPointerDown(PointerEventData eventData) => pressed = true;
    public void OnPointerUp(PointerEventData eventData) => pressed = false;
    public void OnSelect(BaseEventData eventData) => hovered = true;
    public void OnDeselect(BaseEventData eventData)
    {
        hovered = false;
        pressed = false;
    }
}
