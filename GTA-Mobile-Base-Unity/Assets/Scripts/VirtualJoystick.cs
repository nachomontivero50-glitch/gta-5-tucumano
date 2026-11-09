using UnityEngine;
using UnityEngine.EventSystems;
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public RectTransform handle; RectTransform rt;
    void Awake() { rt = (RectTransform)transform; }
    public void OnPointerDown(PointerEventData e) { OnDrag(e); }
    public void OnDrag(PointerEventData e) {
        Vector2 p; RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out p);
        float r = rt.sizeDelta.x * 0.5f;
        Vector2 v = Vector2.ClampMagnitude(p / r, 1f);
        InputHub.Joystick = v; handle.anchoredPosition = v * r * 0.6f;
    }
    public void OnPointerUp(PointerEventData e) { InputHub.Joystick = Vector2.zero; handle.anchoredPosition = Vector2.zero; }
}
