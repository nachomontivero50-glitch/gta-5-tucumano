using UnityEngine;
public class PlayerVehicleInteractor : MonoBehaviour {
    public ThirdPersonCamera cam; SimpleCar cur; CharacterController cc; PlayerController pc; Renderer rend;
    void Awake() { cc = GetComponent<CharacterController>(); pc = GetComponent<PlayerController>(); rend = GetComponent<Renderer>(); }
    void Update() {
        if (cur == null && !InputHub.ConsumeInteract()) return;
        if (cur != null) { if (!InputHub.ConsumeInteract()) return; Exit(); return; }
        SimpleCar best = null; float bd = 5f;
        foreach (var c in FindObjectsOfType<SimpleCar>()) { float d = Vector3.Distance(c.transform.position, transform.position); if (d < bd) { bd = d; best = c; } }
        if (best != null) Enter(best);
    }
    public bool InCar { get { return cur != null; } }
    public SimpleCar Current { get { return cur; } }
    public void ForceExit() { if (cur != null) Exit(); }
    void Enter(SimpleCar c) { cc.enabled = false; pc.enabled = false; rend.enabled = false; cur = c; c.driven = true; transform.SetParent(c.transform); cam.SetTarget(c.transform, true); InputHub.Clear(); }
    void Exit() {
        transform.SetParent(null); transform.position = cur.transform.position - cur.transform.right * 2.5f + Vector3.up * 1.2f;
        cc.enabled = true; pc.enabled = true; rend.enabled = true; cur.driven = false; cur = null; cam.SetTarget(transform, false); InputHub.Clear();
    }
}
