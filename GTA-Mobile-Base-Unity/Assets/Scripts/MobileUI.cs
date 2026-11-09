using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public static class MobileUI {
    public static Transform Root; public static Text Fps; static Font font; static Sprite circle;
    public static void Build() {
        if (Object.FindObjectOfType<EventSystem>() == null) { var es = new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>(); }
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); circle = MakeCircle();
        var cg = new GameObject("UI"); var cv = cg.AddComponent<Canvas>(); cv.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = cg.AddComponent<CanvasScaler>(); sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; sc.referenceResolution = new Vector2(1280, 720); sc.matchWidthOrHeight = 0.5f;
        cg.AddComponent<GraphicRaycaster>(); Root = cg.transform;
        var bg = Img(cg.transform, "Joystick", new Vector2(0, 0), new Vector2(200, 200), new Vector2(240, 240), new Color(1, 1, 1, 0.25f));
        var h = Img(bg, "Handle", new Vector2(.5f, .5f), Vector2.zero, new Vector2(100, 100), new Color(1, 1, 1, 0.6f));
        bg.gameObject.AddComponent<VirtualJoystick>().handle = h;
        Btn(cg.transform, "Saltar", new Vector2(1, 0), new Vector2(-140, 170), 130, InputHub.PressJump);
        Btn(cg.transform, "Subir/Bajar", new Vector2(1, 0), new Vector2(-300, 100), 110, InputHub.PressInteract);
        Fps = Lbl(cg.transform, "Fps", new Vector2(0, 1), new Vector2(170, -40), new Vector2(300, 40)); Fps.alignment = TextAnchor.MiddleLeft;
    }
    public static void Btn(Transform p, string label, Vector2 a, Vector2 pos, float size, UnityEngine.Events.UnityAction act) {
        var r = Img(p, label, a, pos, new Vector2(size, size), new Color(1, 1, 1, 0.35f));
        r.gameObject.AddComponent<Button>().onClick.AddListener(act);
        var t = Lbl(r, "T", new Vector2(.5f, .5f), Vector2.zero, new Vector2(size, size)); t.text = label; t.fontSize = 22; t.alignment = TextAnchor.MiddleCenter; t.raycastTarget = false;
    }
    public static RectTransform Img(Transform p, string n, Vector2 a, Vector2 pos, Vector2 size, Color c) {
        var go = new GameObject(n, typeof(RectTransform), typeof(Image)); go.transform.SetParent(p, false);
        var r = (RectTransform)go.transform; r.anchorMin = a; r.anchorMax = a; r.pivot = new Vector2(.5f, .5f); r.anchoredPosition = pos; r.sizeDelta = size;
        var i = go.GetComponent<Image>(); i.sprite = circle; i.color = c; return r;
    }
    public static Text Lbl(Transform p, string n, Vector2 a, Vector2 pos, Vector2 size) {
        var go = new GameObject(n, typeof(RectTransform), typeof(Text)); go.transform.SetParent(p, false);
        var r = (RectTransform)go.transform; r.anchorMin = a; r.anchorMax = a; r.pivot = new Vector2(.5f, .5f); r.anchoredPosition = pos; r.sizeDelta = size;
        var t = go.GetComponent<Text>(); t.font = font; t.fontSize = 26; t.color = Color.white; return t;
    }
    static Sprite MakeCircle() {
        int n = 64; var tex = new Texture2D(n, n, TextureFormat.RGBA32, false); var px = new Color32[n * n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++) { float dx = x - n / 2f + .5f, dy = y - n / 2f + .5f; px[y * n + x] = dx * dx + dy * dy <= (n / 2f) * (n / 2f) ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0); }
        tex.SetPixels32(px); tex.Apply(); return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(.5f, .5f));
    }
}
