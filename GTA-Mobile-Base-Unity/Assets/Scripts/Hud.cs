using UnityEngine;
using UnityEngine.UI;
public class Hud : MonoBehaviour {
    public MissionManager mm; Text money, wanted, clock, mission, toast; RectTransform hp, mDot; RectTransform[] lines = new RectTransform[10], pDots = new RectTransform[3];
    float toastT, upd; const float Range = 140f;
    void OnEnable() { GameState.OnMsg += Toast; } void OnDisable() { GameState.OnMsg -= Toast; }
    void Toast(string s) { if (toast == null) return; toast.text = s; toastT = 4f; }
    Text Lbl(Transform p, string n, Vector2 a, Vector2 pos, Vector2 size, int fs, TextAnchor al, Color c) {
        var t = MobileUI.Lbl(p, n, a, pos, size); t.fontSize = fs; t.alignment = al; t.color = c; t.raycastTarget = false; t.gameObject.AddComponent<Outline>(); return t;
    }
    RectTransform Solid(Transform p, string n, Vector2 a, Vector2 pos, Vector2 size, Color c, bool circle) {
        var r = MobileUI.Img(p, n, a, pos, size, c); if (!circle) r.GetComponent<Image>().sprite = null; r.GetComponent<Image>().raycastTarget = false; return r;
    }
    void Start() {
        var root = MobileUI.Root; Vector2 TR = new Vector2(1, 1), TC = new Vector2(.5f, 1), C = new Vector2(.5f, .5f);
        money = Lbl(root, "Money", TR, new Vector2(-190, -40), new Vector2(300, 40), 30, TextAnchor.MiddleRight, new Color(.6f, 1f, .6f));
        var hpBg = Solid(root, "HpBg", TR, new Vector2(-190, -78), new Vector2(300, 16), new Color(0, 0, 0, .5f), false);
        hp = Solid(hpBg, "Hp", new Vector2(0, .5f), Vector2.zero, new Vector2(300, 16), new Color(.9f, .2f, .2f, .9f), false); hp.pivot = new Vector2(0, .5f);
        wanted = Lbl(root, "Wanted", TR, new Vector2(-190, -110), new Vector2(300, 32), 24, TextAnchor.MiddleRight, new Color(.5f, .7f, 1f));
        clock = Lbl(root, "Clock", TR, new Vector2(-190, -142), new Vector2(300, 32), 22, TextAnchor.MiddleRight, Color.white);
        MobileUI.Btn(root, "II", TR, new Vector2(-60, -215), 80, PauseMenu.Toggle);
        mission = Lbl(root, "Mission", TC, new Vector2(0, -50), new Vector2(640, 80), 22, TextAnchor.MiddleCenter, Color.white);
        toast = Lbl(root, "Toast", TC, new Vector2(0, -140), new Vector2(900, 50), 30, TextAnchor.MiddleCenter, new Color(1f, .9f, .3f));
        var radar = Solid(root, "Radar", new Vector2(0, 1), new Vector2(140, -200), new Vector2(200, 200), new Color(0, 0, 0, .5f), true); radar.gameObject.AddComponent<Mask>();
        for (int i = 0; i < 10; i++) lines[i] = Solid(radar, "L", C, Vector2.zero, new Vector2(700, 10), new Color(1, 1, 1, .22f), false);
        Solid(radar, "Yo", C, Vector2.zero, new Vector2(16, 16), new Color(.3f, 1f, .4f), true);
        mDot = Solid(radar, "M", C, Vector2.zero, new Vector2(18, 18), new Color(1f, .85f, .1f), true);
        for (int i = 0; i < 3; i++) pDots[i] = Solid(radar, "P", C, Vector2.zero, new Vector2(14, 14), new Color(.2f, .4f, 1f), true);
    }
    void Dot(RectTransform r, bool on, Vector3 w, Vector3 pp, Vector3 cr, Vector3 cf, float sc) {
        r.gameObject.SetActive(on); if (!on) return;
        Vector3 d = w - pp; Vector2 v = new Vector2(Vector3.Dot(d, cr), Vector3.Dot(d, cf)) * sc; if (v.magnitude > 88f) v = v.normalized * 88f; r.anchoredPosition = v;
    }
    void Line(RectTransform r, Vector3 d, Vector3 cr, Vector3 cf, float sc, float ang) { r.anchoredPosition = new Vector2(Vector3.Dot(d, cr), Vector3.Dot(d, cf)) * sc; r.localEulerAngles = new Vector3(0, 0, ang * Mathf.Rad2Deg); }
    void LateUpdate() {
        if (toast == null || Game.Cam == null) return;
        toastT -= Time.unscaledDeltaTime; toast.color = new Color(1f, .9f, .3f, Mathf.Clamp01(toastT));
        upd -= Time.unscaledDeltaTime;
        if (upd <= 0f) {
            upd = 0.25f; money.text = GameState.Pesos(GameState.Money); hp.sizeDelta = new Vector2(300f * GameState.Health / 100f, 16f);
            wanted.text = GameState.Wanted > 0 ? "BUSCADO " + new string('*', GameState.Wanted) : "";
            int h = (int)DayNightCycle.Hour, m = (int)((DayNightCycle.Hour - h) * 60f); clock.text = "Hora " + h.ToString("00") + ":" + m.ToString("00");
            mission.text = mm != null ? mm.HudText : "";
        }
        Vector3 pp = Game.PlayerPos, cf = Game.Cam.transform.forward; cf.y = 0; if (cf.sqrMagnitude < .001f) cf = Vector3.forward; cf.Normalize();
        Vector3 cr = new Vector3(cf.z, 0, -cf.x); float sc = 100f / Range, S = CityStreamer.S;
        for (int k = 0; k < 5; k++) {
            float X = (Mathf.Floor(pp.x / S) + k - 2) * S, Z = (Mathf.Floor(pp.z / S) + k - 2) * S;
            Line(lines[k], new Vector3(X - pp.x, 0, 0), cr, cf, sc, Mathf.Atan2(cf.z, cr.z));
            Line(lines[5 + k], new Vector3(0, 0, Z - pp.z), cr, cf, sc, Mathf.Atan2(cf.x, cr.x));
        }
        Dot(mDot, mm != null && mm.HasTarget, mm != null ? mm.TargetPos : pp, pp, cr, cf, sc);
        for (int i = 0; i < 3; i++) { bool on = i < PoliceManager.Cars.Count && PoliceManager.Cars[i].gameObject.activeSelf; Dot(pDots[i], on, on ? PoliceManager.Cars[i].transform.position : pp, pp, cr, cf, sc); }
    }
}
