using UnityEngine;
public class Collectibles : MonoBehaviour {
    const int N = 16; Transform[] items = new Transform[N];
    void Start() {
        var mat = MatCache.Get(new Color(.3f, .85f, .4f));
        for (int i = 0; i < N; i++) {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube); DestroyImmediate(g.GetComponent<Collider>());
            g.name = "Billete"; g.transform.localScale = new Vector3(.7f, .06f, .35f); g.GetComponent<Renderer>().sharedMaterial = mat; items[i] = g.transform; Respawn(i);
        }
    }
    void Respawn(int i) {
        Vector3 pp = Game.PlayerPos; float S = CityStreamer.S; int bx = Mathf.FloorToInt(pp.x / S) + Random.Range(-2, 3), bz = Mathf.FloorToInt(pp.z / S) + Random.Range(-2, 3), c = Random.Range(0, 4);
        Vector3 p = Vector3.Lerp(PedestrianAgent.Corner(bx, bz, c), PedestrianAgent.Corner(bx, bz, (c + 1) % 4), Random.value); p.y = 0.9f; items[i].position = p;
    }
    void Update() {
        Vector3 pp = Game.PlayerPos;
        for (int i = 0; i < N; i++) {
            var t = items[i]; t.Rotate(0, 120f * Time.deltaTime, 0);
            Vector3 d = t.position - pp; float flat = new Vector2(d.x, d.z).magnitude;
            if (flat < 2.2f && Mathf.Abs(d.y) < 3f) { int r = Random.Range(2, 6) * 100; GameState.Add(r); Sfx.Play(Sfx.Coin, .6f); GameState.Msg("+" + GameState.Pesos(r)); Respawn(i); }
            else if (flat > 230f) Respawn(i);
        }
    }
}
