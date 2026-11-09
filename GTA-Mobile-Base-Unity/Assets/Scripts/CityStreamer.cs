using System.Collections.Generic;
using UnityEngine;
public class CityStreamer : MonoBehaviour {
    public Transform target; public const float S = 80f;
    public static System.Func<int, int, bool> Reserved;
    public static System.Action<int, int, System.Action<int, Vector3, Vector3>, System.Action<Vector3, Vector3>> Decorate;
    Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    int lastRadius = -1;
    public void Prewarm() {
        int cx = Mathf.FloorToInt(target.position.x / S), cz = Mathf.FloorToInt(target.position.z / S);
        for (int x = -1; x <= 1; x++) for (int z = -1; z <= 1; z++) { var k = new Vector2Int(cx + x, cz + z); if (!chunks.ContainsKey(k)) chunks[k] = Build(k.x, k.y); }
    }
    void Update() {
        if (target == null) return;
        int r = PerformanceManager.ViewRadius;
        if (r != lastRadius) {
            lastRadius = r; float d = r * S;
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear; RenderSettings.fogStartDistance = d * 0.6f; RenderSettings.fogEndDistance = d;
            if (Camera.main) Camera.main.farClipPlane = d + 40f;
        }
        int cx = Mathf.FloorToInt(target.position.x / S), cz = Mathf.FloorToInt(target.position.z / S);
        var rm = new List<Vector2Int>();
        foreach (var k in chunks.Keys) if (Mathf.Abs(k.x - cx) > r + 1 || Mathf.Abs(k.y - cz) > r + 1) rm.Add(k);
        foreach (var k in rm) { Unload(chunks[k]); chunks.Remove(k); }
        for (int d = 0; d <= r; d++) for (int dx = -d; dx <= d; dx++) for (int dz = -d; dz <= d; dz++) {
            if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dz)) != d) continue;
            var k = new Vector2Int(cx + dx, cz + dz);
            if (chunks.ContainsKey(k)) continue;
            chunks[k] = Build(k.x, k.y); return; // 1 chunk por frame: sin tirones
        }
    }
    void Unload(GameObject go) { foreach (var mf in go.GetComponentsInChildren<MeshFilter>()) Destroy(mf.sharedMesh); Destroy(go); }
    GameObject Build(int cx, int cz) {
        var root = new GameObject("Chunk " + cx + "," + cz); root.isStatic = true; root.transform.position = new Vector3(cx * S, 0, cz * S);
        var rnd = new System.Random(cx * 73856093 ^ cz * 19349663 ^ 1337);
        var lists = new List<CombineInstance>[Palette.Mats.Length];
        for (int i = 0; i < lists.Length; i++) lists[i] = new List<CombineInstance>();
        var col = new GameObject("Col"); col.transform.SetParent(root.transform, false);
        System.Action<int, Vector3, Vector3> box = (m, c, s) => lists[m].Add(new CombineInstance { mesh = Palette.Cube, transform = Matrix4x4.TRS(c, Quaternion.identity, s) });
        System.Action<Vector3, Vector3> solid = (c, s) => { var b = col.AddComponent<BoxCollider>(); b.center = c; b.size = s; };
        var gc = new Vector3(S / 2, -0.5f, S / 2); box(0, gc, new Vector3(S, 1, S)); solid(gc, new Vector3(S, 1, S));
        var sc = new Vector3(S / 2, 0.125f, S / 2); box(1, sc, new Vector3(66, 0.25f, 66)); solid(sc, new Vector3(66, 0.25f, 66));
        for (float t = 12; t <= S - 12; t += 6) { box(2, new Vector3(0, 0.01f, t), new Vector3(0.3f, 0.02f, 3)); box(2, new Vector3(t, 0.01f, 0), new Vector3(3, 0.02f, 0.3f)); }
        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) {
            if (Reserved != null && Reserved(cx, cz)) continue;
            float lx = 18 + i * 22, lz = 18 + j * 22;
            if (rnd.NextDouble() < 0.15) { box(3, new Vector3(lx, 0.275f, lz), new Vector3(18, 0.05f, 18)); continue; }
            float w = 13 + (float)rnd.NextDouble() * 6, d = 13 + (float)rnd.NextDouble() * 6, h = 8 + (float)(System.Math.Pow(rnd.NextDouble(), 2) * 55);
            var c = new Vector3(lx, 0.25f + h / 2, lz); var s = new Vector3(w, h, d);
            box(4 + rnd.Next(5), c, s); solid(c, s);
        }
        if (Decorate != null) Decorate(cx, cz, box, solid);
        for (int m = 0; m < lists.Length; m++) {
            if (lists[m].Count == 0) continue;
            var mesh = new Mesh(); mesh.CombineMeshes(lists[m].ToArray(), true, true); mesh.UploadMeshData(true);
            var go = new GameObject("M" + m); go.isStatic = true; go.transform.SetParent(root.transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh; var mr = go.AddComponent<MeshRenderer>(); mr.sharedMaterial = Palette.Mats[m];
            mr.shadowCastingMode = m >= 4 ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        return root;
    }
}
