using UnityEngine;
public class DayNightCycle : MonoBehaviour {
    public static float Hour = 15f, Night; public float dayLengthSeconds = 720f;
    Light sun; Camera cam;
    static readonly Color Day = new Color(.62f, .78f, .92f), Dusk = new Color(.95f, .58f, .38f), NightC = new Color(.02f, .03f, .09f);
    void Start() { foreach (var l in FindObjectsOfType<Light>()) if (l.type == LightType.Directional) { sun = l; break; } cam = Camera.main; }
    void Update() {
        if (sun == null) return;
        Hour = (Hour + 24f * Time.deltaTime / dayLengthSeconds) % 24f;
        float ang = Hour / 24f * 360f - 90f, h = Mathf.Sin(ang * Mathf.Deg2Rad);
        float dayF = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-0.1f, 0.35f, h)), duskF = Mathf.Clamp01(1f - Mathf.Abs(h) / 0.25f);
        Night = 1f - dayF;
        Color sky = Color.Lerp(Color.Lerp(NightC, Day, dayF), Dusk, duskF * 0.7f);
        sun.transform.rotation = Quaternion.Euler(ang, -30f, 0f);
        sun.intensity = Mathf.Lerp(0.12f, 1.1f, dayF); sun.color = Color.Lerp(new Color(.5f, .6f, 1f), new Color(1f, .96f, .88f), dayF);
        sun.shadows = dayF > 0.2f ? LightShadows.Hard : LightShadows.None;
        RenderSettings.ambientLight = Color.Lerp(new Color(.05f, .06f, .12f), new Color(.55f, .58f, .65f), dayF);
        RenderSettings.fogColor = sky; if (cam != null) cam.backgroundColor = sky;
    }
}
