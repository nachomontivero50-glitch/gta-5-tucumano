using UnityEngine;
public static class Palette {
    public static Material[] Mats; public static Mesh Cube, Cylinder; static Material baseMat;
    public static void Init() {
        if (Mats != null && Mats[0] != null) return;
        var c = GameObject.CreatePrimitive(PrimitiveType.Cube); Cube = c.GetComponent<MeshFilter>().sharedMesh; baseMat = c.GetComponent<Renderer>().sharedMaterial; Object.Destroy(c);
        var y = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Cylinder = y.GetComponent<MeshFilter>().sharedMesh; Object.Destroy(y);
        Color[] cols = { new Color(.18f,.18f,.2f), new Color(.55f,.55f,.55f), Color.white, new Color(.3f,.6f,.3f),
            new Color(.46f,.67f,.86f), new Color(.93f,.92f,.88f), new Color(.95f,.8f,.3f), new Color(.5f,.55f,.62f), new Color(.75f,.45f,.35f),
            new Color(.35f,.22f,.12f), new Color(.15f,.45f,.2f), new Color(.95f,.95f,.95f), new Color(.15f,.35f,.75f), new Color(.25f,.55f,.8f), new Color(.9f,.4f,.6f) };
        Mats = new Material[cols.Length];
        for (int i = 0; i < cols.Length; i++) Mats[i] = Make(cols[i]);
    }
    public static Material Make(Color col) {
        var m = new Material(baseMat); m.enableInstancing = true;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
        if (m.HasProperty("_Color")) m.SetColor("_Color", col);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.1f);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", 0.1f);
        return m;
    }
}
