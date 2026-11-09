using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour {
    static PauseMenu I; GameObject panel; Text fpsT, viewT, shT; PerformanceManager pm;
    public static void Toggle() { if (I != null) I.Set(!I.panel.activeSelf); }
    void Start() { I = this; pm = FindObjectOfType<PerformanceManager>(); Build(); panel.SetActive(false); }
    void OnDestroy() { Time.timeScale = 1f; AudioListener.pause = false; }
    void Set(bool on) { panel.SetActive(on); Time.timeScale = on ? 0f : 1f; AudioListener.pause = on; Refresh(); }
    void Refresh() {
        fpsT.text = "FPS objetivo: " + pm.targetFps; viewT.text = "Distancia de dibujo: " + (PerformanceManager.ViewRadius * (int)CityStreamer.S) + " m"; shT.text = "Sombras: " + (pm.Shadows ? "Sí" : "No");
    }
    void Build() {
        Vector2 C = new Vector2(.5f, .5f);
        var bg = MobileUI.Img(MobileUI.Root, "Pausa", C, Vector2.zero, new Vector2(3000, 1600), new Color(0, 0, 0, .75f)); bg.GetComponent<Image>().sprite = null; panel = bg.gameObject;
        var t = MobileUI.Lbl(bg, "T", C, new Vector2(0, 270), new Vector2(600, 60)); t.text = "PAUSA"; t.fontSize = 42; t.alignment = TextAnchor.MiddleCenter;
        fpsT = Row(bg, 150, () => { pm.SetFps(pm.targetFps == 60 ? 30 : 60); Refresh(); });
        viewT = Row(bg, 60, () => { pm.AddView(1); Refresh(); });
        shT = Row(bg, -30, () => { pm.ToggleShadows(); Refresh(); });
        Row(bg, -120, () => { GameState.Money = 5000; PlayerPrefs.SetInt("money", 5000); }).text = "Reiniciar dinero";
        Row(bg, -230, () => Set(false)).text = "Reanudar";
    }
    Text Row(RectTransform p, float y, UnityEngine.Events.UnityAction a) {
        Vector2 C = new Vector2(.5f, .5f); var r = MobileUI.Img(p, "Fila", C, new Vector2(0, y), new Vector2(520, 70), new Color(1, 1, 1, .2f)); r.GetComponent<Image>().sprite = null;
        r.gameObject.AddComponent<Button>().onClick.AddListener(a);
        var t = MobileUI.Lbl(r, "L", C, Vector2.zero, new Vector2(520, 70)); t.alignment = TextAnchor.MiddleCenter; t.raycastTarget = false; return t;
    }
}
