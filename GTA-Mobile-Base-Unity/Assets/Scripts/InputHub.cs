using UnityEngine;
public static class InputHub {
    public static Vector2 Joystick;
    static bool jump, interact;
    public static Vector2 Move { get {
        Vector2 v = Joystick;
        if (v.sqrMagnitude < 0.01f) v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        return Vector2.ClampMagnitude(v, 1f); } }
    public static void PressJump() { jump = true; }
    public static void PressInteract() { interact = true; }
    public static bool ConsumeJump() { bool r = jump || Input.GetKeyDown(KeyCode.Space); jump = false; return r; }
    public static bool ConsumeInteract() { bool r = interact || Input.GetKeyDown(KeyCode.E); interact = false; return r; }
    public static void Clear() { jump = false; interact = false; }
}
