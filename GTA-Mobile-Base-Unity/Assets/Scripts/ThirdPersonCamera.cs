using UnityEngine;
using UnityEngine.EventSystems;
public class ThirdPersonCamera : MonoBehaviour {
    public Transform target; float dist = 5f, height = 0.7f, yaw, pitch = 20f; bool auto; int lookId = -1; bool looking;
    public void SetTarget(Transform t, bool car) { target = t; auto = car; dist = car ? 8f : 5f; height = car ? 1.8f : 0.7f; yaw = t.eulerAngles.y; }
    void Update() {
        looking = false;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(1)) { yaw += Input.GetAxis("Mouse X") * 3f; pitch -= Input.GetAxis("Mouse Y") * 3f; looking = true; }
#endif
        foreach (var t in Input.touches) {
            if (t.phase == TouchPhase.Began && t.position.x > Screen.width * 0.4f && !EventSystem.current.IsPointerOverGameObject(t.fingerId)) lookId = t.fingerId;
            if (t.fingerId != lookId) continue;
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) { lookId = -1; continue; }
            yaw += t.deltaPosition.x * 0.12f; pitch -= t.deltaPosition.y * 0.12f; looking = true;
        }
        pitch = Mathf.Clamp(pitch, -10f, 60f);
        if (auto && !looking && target != null) { var rb = target.GetComponent<Rigidbody>(); if (rb != null && rb.velocity.magnitude > 2f) yaw = Mathf.LerpAngle(yaw, target.eulerAngles.y, 2f * Time.deltaTime); }
    }
    void LateUpdate() {
        if (target == null) return;
        Vector3 pivot = target.position + Vector3.up * height; Vector3 dir = Quaternion.Euler(pitch, yaw, 0) * Vector3.back; float d = dist; RaycastHit hit;
        if (Physics.SphereCast(pivot, 0.25f, dir, out hit, dist, ~0, QueryTriggerInteraction.Ignore)) d = Mathf.Max(0.6f, hit.distance);
        transform.position = Vector3.Lerp(transform.position, pivot + dir * d, 1f - Mathf.Exp(-14f * Time.deltaTime));
        transform.rotation = Quaternion.LookRotation(pivot - transform.position);
    }
}
