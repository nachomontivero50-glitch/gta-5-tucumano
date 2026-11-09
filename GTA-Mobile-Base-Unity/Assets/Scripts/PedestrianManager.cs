using UnityEngine;
public class PedestrianAgent : MonoBehaviour {
    int bx, bz, corner, step = 1; float downT; Vector3 target; const float M = 7.75f;
    public static Vector3 Corner(int bx, int bz, int c) {
        float S = CityStreamer.S; return new Vector3(bx * S + ((c == 1 || c == 2) ? S - M : M), 1.1f, bz * S + ((c == 2 || c == 3) ? S - M : M));
    }
    public void Place(int x, int z) {
        bx = x; bz = z; corner = Random.Range(0, 4); step = Random.value < .5f ? 1 : 3;
        Vector3 a = Corner(bx, bz, corner); corner = (corner + step) % 4; target = Corner(bx, bz, corner);
        transform.position = Vector3.Lerp(a, target, Random.value); downT = 0f;
    }
    void Update() {
        if (downT > 0f) { downT -= Time.deltaTime; if (downT <= 0f) { var p = transform.position; p.y = 1.1f; transform.position = p; transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); } return; }
        Vector3 d = target - transform.position; d.y = 0f;
        if (d.magnitude < 0.3f) { corner = (corner + step) % 4; target = Corner(bx, bz, corner); return; }
        Vector3 dir = d.normalized; transform.position += dir * 1.4f * Time.deltaTime; transform.rotation = Quaternion.LookRotation(dir);
    }
    void OnCollisionEnter(Collision c) {
        if (downT > 0f) return; var car = c.collider.GetComponentInParent<SimpleCar>();
        if (car != null && c.relativeVelocity.magnitude > 4f) {
            downT = 4f; var p = transform.position; p.y = 0.5f; transform.position = p; transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0);
            if (car.driven) GameState.AddWanted("Atropellaste a un peatón.");
        }
    }
}
public class PedestrianManager : MonoBehaviour {
    PedestrianAgent[] peds; int idx;
    void Start() {
        int n = SystemInfo.systemMemorySize < 4000 ? 8 : 14; peds = new PedestrianAgent[n];
        Color[] shirts = { new Color(.8f, .2f, .2f), new Color(.2f, .4f, .8f), new Color(.95f, .85f, .3f), new Color(.3f, .7f, .4f), new Color(.9f, .9f, .9f), new Color(.46f, .67f, .86f) };
        for (int i = 0; i < n; i++) {
            var g = GameObject.CreatePrimitive(PrimitiveType.Capsule); g.name = "Peaton"; g.transform.localScale = new Vector3(.55f, .85f, .55f);
            g.GetComponent<Renderer>().sharedMaterial = MatCache.Get(shirts[Random.Range(0, shirts.Length)]);
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere); DestroyImmediate(head.GetComponent<Collider>()); head.transform.SetParent(g.transform, false);
            head.transform.localPosition = new Vector3(0, .78f, 0); head.transform.localScale = new Vector3(.7f, .45f, .7f); head.GetComponent<Renderer>().sharedMaterial = MatCache.Get(new Color(.85f, .65f, .5f));
            var rb = g.AddComponent<Rigidbody>(); rb.isKinematic = true;
            peds[i] = g.AddComponent<PedestrianAgent>(); Spawn(peds[i]);
        }
    }
    void Spawn(PedestrianAgent p) { Vector3 pp = Game.PlayerPos; float S = CityStreamer.S; p.Place(Mathf.FloorToInt(pp.x / S) + Random.Range(-2, 3), Mathf.FloorToInt(pp.z / S) + Random.Range(-2, 3)); }
    void Update() { idx = (idx + 1) % peds.Length; if ((peds[idx].transform.position - Game.PlayerPos).sqrMagnitude > 200f * 200f) Spawn(peds[idx]); }
}
