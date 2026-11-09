using UnityEngine;
public class TrafficCar : MonoBehaviour {
    public static readonly Vector3[] F = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    public static Vector3 R(int d) { return new Vector3(F[d].z, 0f, -F[d].x); }
    public const float Lane = 3.5f;
    public int dir; public float cruise = 11f, halfLen = 2.1f; public bool chase; public Renderer sirenR, sirenB;
    float speed, turnAt, checkT, blockedT, ignoreT; int turn, nextDir; bool blocked; Vector3 pos; Quaternion rot; Rigidbody rb;
    public void Place(Vector3 p, int d) {
        gameObject.SetActive(true); rb = GetComponent<Rigidbody>();
        dir = d; pos = p; rot = Quaternion.LookRotation(F[d]); transform.SetPositionAndRotation(p, rot); rb.position = p; rb.rotation = rot;
        speed = cruise * 0.5f; blockedT = 0f; Plan();
    }
    void Plan() {
        float S = CityStreamer.S, a = Vector3.Dot(pos, F[dir]), N = (Mathf.Floor(a / S) + 1f) * S;
        if (N - a < 7f) N += S;
        Vector3 C = pos + F[dir] * (N - a) - R(dir) * Lane;
        int r = (dir + 1) % 4, l = (dir + 3) % 4; turn = 0;
        if (chase) {
            Vector3 to = Game.PlayerPos - C; float best = Vector3.Dot(F[dir], to) + 20f;
            float sr = Vector3.Dot(F[r], to), sl = Vector3.Dot(F[l], to);
            if (sr > best) { best = sr; turn = 1; }
            if (sl > best) turn = 2;
        } else { float q = Random.value; turn = q < 0.6f ? 0 : (q < 0.8f ? 1 : 2); }
        nextDir = turn == 0 ? dir : (turn == 1 ? r : l);
        Vector3 W = C + R(nextDir) * Lane;
        turnAt = turn == 0 ? N + 8f : Vector3.Dot(W, F[dir]);
    }
    void FixedUpdate() {
        float dt = Time.fixedDeltaTime; checkT -= dt;
        if (checkT <= 0f) {
            checkT = 0.1f; RaycastHit h;
            blocked = ignoreT <= 0f && Physics.Raycast(pos + Vector3.up * 0.8f + F[dir] * (halfLen + 0.3f), F[dir], out h, 3f + speed * 0.8f, ~0, QueryTriggerInteraction.Ignore);
        }
        if (ignoreT > 0f) ignoreT -= dt;
        if (blocked) { blockedT += dt; if (blockedT > 5f) { ignoreT = 3f; blockedT = 0f; } } else blockedT = 0f;
        speed = Mathf.MoveTowards(speed, blocked ? 0f : cruise, (blocked ? 30f : 7f) * dt);
        pos += F[dir] * speed * dt;
        if (Vector3.Dot(pos, F[dir]) >= turnAt) { if (turn != 0) dir = nextDir; Plan(); }
        rot = Quaternion.RotateTowards(rot, Quaternion.LookRotation(F[dir]), 220f * dt);
        rb.MovePosition(pos); rb.MoveRotation(rot);
        if (sirenR != null) { bool a = ((int)(Time.time * 4f)) % 2 == 0; sirenR.enabled = a; sirenB.enabled = !a; }
    }
    void OnCollisionEnter(Collision c) {
        var car = c.collider.GetComponentInParent<SimpleCar>();
        if (car != null && car.driven && !chase && c.relativeVelocity.magnitude > 7f) GameState.AddWanted("Chocaste un auto.");
    }
}
