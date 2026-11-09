using UnityEngine;
using UnityEngine.UI;
// Calles reales de Tucumán asignadas de forma estilizada a la grilla.
public class StreetName : MonoBehaviour {
    Text t; float tm;
    static readonly string[] NS = { "25 de Mayo", "Laprida", "Maipú", "Congreso", "Muñecas", "Mendoza", "Rivadavia", "Junín" };
    static readonly string[] EW = { "24 de Septiembre", "San Martín", "Córdoba", "Santiago del Estero", "Buenos Aires", "Corrientes", "Las Piedras", "Crisóstomo Álvarez" };
    static int Mod(int a, int n) { return ((a % n) + n) % n; }
    void Start() {
        t = MobileUI.Lbl(MobileUI.Root, "Calle", new Vector2(0, 1), new Vector2(140, -335), new Vector2(280, 56));
        t.fontSize = 20; t.alignment = TextAnchor.MiddleCenter; t.color = new Color(1f, 1f, .85f); t.raycastTarget = false; t.gameObject.AddComponent<Outline>();
    }
    void Update() {
        tm -= Time.deltaTime; if (tm > 0f) return; tm = 0.3f;
        Vector3 p = Game.PlayerPos; float S = CityStreamer.S; int i = Mathf.RoundToInt(p.x / S), j = Mathf.RoundToInt(p.z / S);
        bool ns = Mathf.Abs(p.x - i * S) < Mathf.Abs(p.z - j * S);
        t.text = (ns ? NS[Mod(i, NS.Length)] : EW[Mod(j, EW.Length)]) + "\nSan Miguel de Tucumán";
    }
}
