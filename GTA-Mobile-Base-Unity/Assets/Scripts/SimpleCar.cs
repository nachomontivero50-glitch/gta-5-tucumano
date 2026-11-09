using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class SimpleCar : MonoBehaviour {
    public float maxSpeed = 30f, accel = 16f, brake = 28f, steer = 95f, grip = 5f; public bool driven;
    Rigidbody rb;
    void Awake() { rb = GetComponent<Rigidbody>(); }
    void FixedUpdate() {
        if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, 0.9f)) return;
        float th = driven ? InputHub.Move.y : 0f, st = driven ? InputHub.Move.x : 0f;
        Vector3 v = rb.velocity; float fs = Vector3.Dot(v, transform.forward);
        if (Mathf.Abs(th) > 0.05f) { bool br = th * fs < 0; if (br || Mathf.Abs(fs) < maxSpeed) rb.AddForce(transform.forward * th * (br ? brake : accel), ForceMode.Acceleration); }
        else rb.AddForce(-transform.forward * fs * 0.4f, ForceMode.Acceleration);
        rb.AddForce(-transform.right * Vector3.Dot(v, transform.right) * grip, ForceMode.Acceleration);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, st * steer * Mathf.Clamp(fs / 6f, -1f, 1f) * Time.fixedDeltaTime, 0));
    }
}
