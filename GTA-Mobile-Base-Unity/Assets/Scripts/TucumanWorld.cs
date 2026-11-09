using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Lugares reales de San Miguel de Tucumán (estilizados). Cada uno ocupa una manzana de la grilla.
public static class TucumanData {
    public static readonly string[] Names = { "la Plaza Independencia", "la Casa de Gobierno", "la Catedral", "la Basílica de San Francisco", "la Casa Histórica", "el Parque 9 de Julio" };
    public static readonly Vector3[] Spots = {
        new Vector3(3.5f, .1f, -3.5f), new Vector3(163.5f, .1f, -3.5f), new Vector3(3.5f, .1f, 156.5f),
        new Vector3(-76.5f, .1f, -3.5f), new Vector3(3.5f, .1f, -83.5f), new Vector3(-76.5f, .1f, -83.5f) };
    public static Vector3 Spot(Vector3 from, float min, float max, out string name, Vector3 fallback) {
        for (int k = 0; k < 12; k++) { int i = Random.Range(0, Spots.Length); float d = Vector3.Distance(Spots[i], from); if (d >= min && d <= max) { name = Names[i]; return Spots[i]; } }
        name = "destino"; return fallback;
    }
}
public class TucumanWorld : MonoBehaviour {
    Transform tr; Material mat;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Hooks() { CityStreamer.Reserved = IsReserved; CityStreamer.Decorate = Decorate; }
    static bool IsReserved(int x, int z) { return (x == 0 && z == 0) || (x == 1 && z == 0) || (x == 0 && z == 1) || (x == -1 && z == 0) || (x == 0 && z == -1) || (x == -1 && z == -1); }
    static void Decorate(int cx, int cz, System.Action<int, Vector3, Vector3> box, System.Action<Vector3, Vector3> solid) {
        var rnd = new System.Random(cx * 7919 + cz * 104729 + 5);
        // B(material, x, yBase, z, tamX, tamY, tamZ, conColision)
        System.Action<int, float, float, float, float, float, float, bool> B = (m, x, y0, z, sx, sy, sz, col) => {
            var p = new Vector3(x, y0 + sy / 2f, z); var s = new Vector3(sx, sy, sz); box(m, p, s); if (col) solid(p, s);
        };
        System.Action<float, float, bool> Tree = (x, z, pink) => { B(9, x, .25f, z, .5f, 3f, .5f, true); B(pink ? 14 : 10, x, 3.25f, z, 3.6f, 2.6f, 3.6f, false); };
        if (cx == 0 && cz == 0) { // Plaza Independencia + estatua de la Libertad
            B(3, 40, .25f, 40, 66, .05f, 66, false); B(1, 40, .3f, 40, 2.6f, .05f, 66, false); B(1, 40, .3f, 40, 66, .05f, 2.6f, false);
            B(5, 40, .35f, 40, 6, 2.5f, 6, true); B(11, 40, 2.85f, 40, 1.4f, 6f, 1.4f, true); B(11, 40, 8.85f, 40, 1.8f, 3f, 1.8f, false); B(11, 40, 10.5f, 40, 4.5f, .5f, .5f, false);
            foreach (float x in new[] { 18f, 30f, 50f, 62f }) foreach (float z in new[] { 18f, 30f, 50f, 62f }) Tree(x, z, rnd.NextDouble() < .3);
        } else if (cx == 1 && cz == 0) { // Casa de Gobierno con columnas dóricas
            B(5, 36, .25f, 40, 20, 16, 56, true);
            for (int k = 0; k < 12; k++) B(11, 24.6f, .25f, 17 + k * 4.2f, .9f, 12.5f, .9f, true);
            B(11, 24, 12.75f, 40, 3.2f, 1.5f, 52, false); B(11, 24, 14.25f, 40, 3.2f, 1.2f, 22, false); B(11, 24, 15.45f, 40, 3.2f, 1.2f, 10, false);
            B(1, 22.5f, .25f, 40, 3, .5f, 52, true); B(7, 36, 16.25f, 40, 21, 1.2f, 57, false);
            Tree(12, 14, true); Tree(12, 66, false); Tree(60, 12, false); Tree(60, 68, true);
        } else if (cx == 0 && cz == 1) { // Catedral Nuestra Señora de la Encarnación
            B(5, 40, .25f, 46, 24, 14, 44, true); B(5, 29, .25f, 27.5f, 6, 26, 6, true); B(5, 51, .25f, 27.5f, 6, 26, 6, true);
            B(11, 40, .25f, 24.5f, 10, 10, 3, false); B(7, 40, 14.25f, 46, 25, 1.5f, 45, false); B(7, 40, 15.75f, 46, 14, 3, 20, false);
            B(11, 29, 26.25f, 27.5f, 6.4f, 1.5f, 6.4f, false); B(11, 51, 26.25f, 27.5f, 6.4f, 1.5f, 6.4f, false);
            Tree(14, 20, false); Tree(66, 20, true); Tree(14, 70, true); Tree(66, 70, false);
        } else if (cx == -1 && cz == 0) { // Basílica de San Francisco
            B(6, 40, .25f, 40, 18, 13, 38, true); B(6, 40, .25f, 18, 7, 24, 7, true); B(7, 40, 13.25f, 40, 19, 1.2f, 39, false);
            Tree(14, 14, false); Tree(66, 66, true); Tree(14, 66, false); Tree(66, 14, true);
        } else if (cx == 0 && cz == -1) { // Casa Histórica de la Independencia (blanca, puerta azul)
            B(3, 40, .25f, 40, 66, .05f, 66, false);
            B(11, 40, .3f, 65, 34, 5, 10, true); B(8, 40, 5.3f, 65, 36, 1.2f, 12, false); B(12, 40, .3f, 70.15f, 2.2f, 3.4f, .4f, false);
            foreach (float x in new[] { 33f, 37f, 43f, 47f }) B(11, x, .3f, 72, .5f, 3.5f, .5f, false);
            B(11, 40, 3.8f, 72, 16, .4f, 3, false);
            Tree(14, 20, true); Tree(66, 20, false); Tree(20, 40, true); Tree(60, 40, false); Tree(40, 25, true); Tree(14, 50, false); Tree(66, 50, true);
        } else if (cx == -1 && cz == -1) { // Parque 9 de Julio con laguna
            B(3, 40, .25f, 40, 66, .05f, 66, false); B(13, 40, .3f, 40, 26, .05f, 20, false);
            for (int k = 0; k < 22; k++) { float x = 12 + (float)rnd.NextDouble() * 56, z = 12 + (float)rnd.NextDouble() * 56; if (Mathf.Abs(x - 40) < 15 && Mathf.Abs(z - 40) < 12) continue; Tree(x, z, rnd.NextDouble() < .35); }
        } else { // arbolado de vereda: lapachos (rosas), tipas y naranjos
            for (int k = 0; k < 4; k++) {
                float t = 14 + k * 16 + (float)rnd.NextDouble() * 4;
                if (rnd.NextDouble() < .8) Tree(7.6f, t, rnd.NextDouble() < .25); if (rnd.NextDouble() < .8) Tree(t, 7.6f, rnd.NextDouble() < .25);
                if (rnd.NextDouble() < .8) Tree(72.4f, t, rnd.NextDouble() < .25); if (rnd.NextDouble() < .8) Tree(t, 72.4f, rnd.NextDouble() < .25);
            }
        }
    }
    // ---- Cerro San Javier y Cristo Bendicente de fondo (al oeste, como en la realidad) ----
    const int N = 28;
    static int Layer(List<Vector3> v, List<Color> c, List<int> t, float D, float hmax, Color lo, Color hi, float phase) {
        int o = v.Count;
        for (int k = 0; k <= N; k++) {
            float th = Mathf.Lerp(-80f, 80f, k / (float)N) * Mathf.Deg2Rad, u = 1f - Mathf.Abs(th) / (80f * Mathf.Deg2Rad);
            float h = hmax * (0.35f + 0.65f * Mathf.Pow(u, 1.2f)) + 4f * Mathf.Sin(k * 1.7f + phase * 3f);
            Vector3 d = new Vector3(-Mathf.Cos(th), 0, Mathf.Sin(th));
            v.Add(d * D + Vector3.down * 12f); c.Add(lo); v.Add(d * D + Vector3.up * h); c.Add(hi);
        }
        for (int k = 0; k < N; k++) { int a = o + k * 2; t.Add(a); t.Add(a + 1); t.Add(a + 2); t.Add(a + 1); t.Add(a + 3); t.Add(a + 2); }
        return o;
    }
    static void Quad(List<Vector3> v, List<Color> c, List<int> t, Vector3 ctr, float w, float h, Color col) {
        int o = v.Count; Vector3 tg = Vector3.forward * w / 2f, up = Vector3.up * h / 2f;
        v.Add(ctr - tg - up); v.Add(ctr - tg + up); v.Add(ctr + tg - up); v.Add(ctr + tg + up);
        for (int i = 0; i < 4; i++) c.Add(col);
        t.Add(o); t.Add(o + 1); t.Add(o + 2); t.Add(o + 1); t.Add(o + 3); t.Add(o + 2);
    }
    void Start() {
        var v = new List<Vector3>(); var c = new List<Color>(); var t = new List<int>();
        Layer(v, c, t, 185f, 62f, new Color(.35f, .5f, .62f), new Color(.5f, .65f, .75f), 1f);
        int o = Layer(v, c, t, 160f, 42f, new Color(.14f, .3f, .17f), new Color(.3f, .5f, .28f), 0f);
        Vector3 peak = v[o + (N / 2) * 2 + 1] + Vector3.right * 2f;
        Quad(v, c, t, peak + Vector3.up * 5f, 2.4f, 10f, Color.white); Quad(v, c, t, peak + Vector3.up * 8f, 9f, 1.6f, Color.white); // Cristo Bendicente
        var mesh = new Mesh(); mesh.SetVertices(v); mesh.SetColors(c); mesh.SetTriangles(t, 0); mesh.bounds = new Bounds(Vector3.zero, new Vector3(1200, 400, 1200));
        var go = new GameObject("CerroSanJavier"); go.AddComponent<MeshFilter>().sharedMesh = mesh; var mr = go.AddComponent<MeshRenderer>();
        mat = new Material(Graphic.defaultGraphicMaterial); mat.renderQueue = 1000; mr.sharedMaterial = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; mr.receiveShadows = false; tr = go.transform;
    }
    void LateUpdate() {
        if (tr == null || Game.Cam == null) return;
        Vector3 p = Game.Cam.transform.position; tr.position = new Vector3(p.x, 0, p.z);
        mat.color = Color.Lerp(Color.white, new Color(.12f, .15f, .3f), DayNightCycle.Night);
    }
}
