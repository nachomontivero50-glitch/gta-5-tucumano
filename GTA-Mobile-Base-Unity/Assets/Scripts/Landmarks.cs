using System.Collections.Generic;
using UnityEngine;
public class Landmarks : MonoBehaviour {
    void Start() {
        var go = new GameObject("Obelisco"); go.isStatic = true;
        var v = new List<Vector3>(); var t = new List<int>();
        Frustum(v, t, 0f, 60f, 2.2f, 1.0f); Frustum(v, t, 60f, 67f, 1.0f, 0.02f);
        var m = new Mesh(); m.SetVertices(v); m.SetTriangles(t, 0); m.RecalculateNormals(); m.RecalculateBounds();
        go.AddComponent<MeshFilter>().sharedMesh = m; go.AddComponent<MeshRenderer>().sharedMaterial = Palette.Mats[5];
        var bc = go.AddComponent<BoxCollider>(); bc.center = new Vector3(0, 33.5f, 0); bc.size = new Vector3(4.4f, 67f, 4.4f);
    }
    static void Frustum(List<Vector3> v, List<int> t, float y0, float y1, float b, float tp) {
        Vector3[] lo = { new Vector3(-b, y0, -b), new Vector3(b, y0, -b), new Vector3(b, y0, b), new Vector3(-b, y0, b) };
        Vector3[] hi = { new Vector3(-tp, y1, -tp), new Vector3(tp, y1, -tp), new Vector3(tp, y1, tp), new Vector3(-tp, y1, tp) };
        for (int i = 0; i < 4; i++) {
            int j = (i + 1) % 4; Vector3 o = (lo[i] + lo[j]) * 0.5f; o.y = 0;
            Tri(v, t, lo[i], lo[j], hi[j], o); Tri(v, t, lo[i], hi[j], hi[i], o);
        }
    }
    static void Tri(List<Vector3> v, List<int> t, Vector3 a, Vector3 b, Vector3 c, Vector3 outward) {
        int o = v.Count; v.Add(a); v.Add(b); v.Add(c);
        if (Vector3.Dot(Vector3.Cross(b - a, c - a), outward) > 0) { t.Add(o); t.Add(o + 1); t.Add(o + 2); } else { t.Add(o); t.Add(o + 2); t.Add(o + 1); }
    }
}
