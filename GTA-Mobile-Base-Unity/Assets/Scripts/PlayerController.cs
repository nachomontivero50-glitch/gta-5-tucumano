using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    public float walk = 4.5f, run = 7.5f, jumpH = 1.2f, gravity = -20f;
    CharacterController cc; float vy;
    void Awake() { cc = GetComponent<CharacterController>(); }
    void Update() {
        var cam = Camera.main.transform; Vector2 m = InputHub.Move;
        Vector3 f = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized, r = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 dir = f * m.y + r * m.x; Vector3 mv = dir * (m.magnitude > 0.85f ? run : walk);
        if (cc.isGrounded && vy < 0) vy = -2f;
        if (InputHub.ConsumeJump() && cc.isGrounded) vy = Mathf.Sqrt(jumpH * -2f * gravity);
        vy += gravity * Time.deltaTime; mv.y = vy; cc.Move(mv * Time.deltaTime);
        if (dir.sqrMagnitude > 0.01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }
}
