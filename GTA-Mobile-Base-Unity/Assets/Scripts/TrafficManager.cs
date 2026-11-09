using System.Collections.Generic;
using UnityEngine;
public class TrafficManager : MonoBehaviour {
    public static TrafficManager I; List<TrafficCar> cars = new List<TrafficCar>(); int idx;
    void Awake() { I = this; }
    void Start() {
        int n = SystemInfo.systemMemorySize < 4000 ? 8 : 14;
        Color[] cols = { new Color(.8f, .15f, .15f), new Color(.2f, .35f, .8f), new Color(.9f, .9f, .9f), new Color(.2f, .2f, .22f), new Color(.2f, .6f, .3f), new Color(.46f, .67f, .86f) };
        for (int i = 0; i < n; i++) {
            bool bus = i % 5 == 4;
            var tc = Build(bus, false, cols[Random.Range(0, cols.Length)]);
            tc.cruise = bus ? Random.Range(8f, 10f) : Random.Range(10f, 15f);
            cars.Add(tc); Respawn(tc);
        }
    }
    void Respawn(TrafficCar c) { Vector3 p; int d; LanePoint(Game.PlayerPos, 70f, 190f, out p, out d); c.Place(p, d); }
    void Update() {
        if (cars.Count == 0) return; idx = (idx + 1) % cars.Count; var c = cars[idx];
        if ((c.transform.position - Game.PlayerPos).sqrMagnitude > 230f * 230f) Respawn(c);
    }
    public static void LanePoint(Vector3 c, float min, float max, out Vector3 pos, out int dir) {
        float S = CityStreamer.S; dir = Random.Range(0, 4);
        int i = Mathf.RoundToInt(Vector3.Dot(c, TrafficCar.R(dir)) / S) + Random.Range(-1, 2);
        float al = Vector3.Dot(c, TrafficCar.F[dir]) + (Random.value < .5f ? -1f : 1f) * Random.Range(min, max);
        pos = TrafficCar.F[dir] * al + TrafficCar.R(dir) * (i * S + TrafficCar.Lane); pos.y = 0.05f;
    }
    public static Renderer Part(Transform par, PrimitiveType t, Vector3 pos, Vector3 scale, Vector3 rot, Color c) {
        var g = GameObject.CreatePrimitive(t); DestroyImmediate(g.GetComponent<Collider>()); g.transform.SetParent(par, false);
        g.transform.localPosition = pos; g.transform.localScale = scale; g.transform.localEulerAngles = rot;
        var r = g.GetComponent<Renderer>(); r.sharedMaterial = MatCache.Get(c); return r;
    }
    public TrafficCar Build(bool bus, bool police, Color body) {
        var go = new GameObject(police ? "Policia" : bus ? "Colectivo" : "Auto"); go.SetActive(false); var tr = go.transform;
        var rb = go.AddComponent<Rigidbody>(); rb.isKinematic = true; rb.interpolation = RigidbodyInterpolation.Interpolate;
        var bc = go.AddComponent<BoxCollider>(); var tc = go.AddComponent<TrafficCar>(); Color dark = new Color(.1f, .1f, .12f);
        if (bus) {
            bc.center = new Vector3(0, 1.5f, 0); bc.size = new Vector3(2.5f, 3f, 10f); tc.halfLen = 5f;
            Part(tr, PrimitiveType.Cube, new Vector3(0, 1.5f, 0), new Vector3(2.5f, 2.4f, 10f), Vector3.zero, body);
            Part(tr, PrimitiveType.Cube, new Vector3(0, 2.75f, 0), new Vector3(2.55f, .25f, 10f), Vector3.zero, Color.white);
            Part(tr, PrimitiveType.Cube, new Vector3(0, 1.9f, 0), new Vector3(2.56f, .8f, 8.6f), Vector3.zero, dark);
            foreach (float x in new[] { -1.2f, 1.2f }) foreach (float z in new[] { -3f, 3f }) Part(tr, PrimitiveType.Cylinder, new Vector3(x, .5f, z), new Vector3(1f, .15f, 1f), new Vector3(0, 0, 90), dark);
        } else {
            bc.center = new Vector3(0, 0.6f, 0); bc.size = new Vector3(2f, 1.2f, 4.2f); tc.halfLen = 2.1f;
            Part(tr, PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(2f, .7f, 4.2f), Vector3.zero, body);
            Part(tr, PrimitiveType.Cube, new Vector3(0, 1.15f, -.2f), new Vector3(1.7f, .6f, 2.2f), Vector3.zero, police ? Color.white : body * 0.8f);
            foreach (float x in new[] { -1f, 1f }) foreach (float z in new[] { -1.3f, 1.3f }) Part(tr, PrimitiveType.Cylinder, new Vector3(x, .35f, z), new Vector3(.7f, .12f, .7f), new Vector3(0, 0, 90), dark);
        }
        if (police) {
            tc.chase = true; tc.cruise = 21f;
            Part(tr, PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(2.02f, .25f, 4.22f), Vector3.zero, new Color(.1f, .25f, .7f));
            tc.sirenR = Part(tr, PrimitiveType.Cube, new Vector3(-.4f, 1.5f, -.2f), new Vector3(.6f, .15f, .4f), Vector3.zero, Color.red);
            tc.sirenB = Part(tr, PrimitiveType.Cube, new Vector3(.4f, 1.5f, -.2f), new Vector3(.6f, .15f, .4f), Vector3.zero, new Color(.1f, .3f, 1f));
        }
        return tc;
    }
}
