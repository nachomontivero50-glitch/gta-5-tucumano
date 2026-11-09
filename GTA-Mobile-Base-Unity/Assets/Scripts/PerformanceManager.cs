using UnityEngine;
using UnityEngine.Rendering;
public class PerformanceManager : MonoBehaviour {
    public static int ViewRadius = 3; public static float Fps = 60f;
    public int targetFps = 60;
    float renderScale = 1f, tierScale = 1f, smooth = 1f / 60f, timer, txtTimer; int good;
    void Awake() {
        Application.targetFrameRate = targetFps; QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep; Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; QualitySettings.pixelLightCount = 1;
        QualitySettings.softParticles = false; QualitySettings.realtimeReflectionProbes = false;
        int ram = SystemInfo.systemMemorySize; float shadow; int msaa;
        if (ram < 4000) { ViewRadius = 2; tierScale = 0.7f; shadow = 0; msaa = 1; }
        else if (ram < 6500) { ViewRadius = 3; tierScale = 0.85f; shadow = 40; msaa = 1; }
        else { ViewRadius = 3; tierScale = 1f; shadow = 70; msaa = 2; }
        QualitySettings.shadows = shadow > 0 ? ShadowQuality.HardOnly : ShadowQuality.Disable;
        QualitySettings.shadowDistance = shadow; QualitySettings.antiAliasing = msaa == 1 ? 0 : msaa;
        SetRP("shadowDistance", shadow); SetRP("msaaSampleCount", msaa); SetScale(tierScale);
    }
    void SetRP(string n, object v) { var rp = GraphicsSettings.currentRenderPipeline; if (rp == null) return; var p = rp.GetType().GetProperty(n); if (p != null) p.SetValue(rp, v); }
    void SetScale(float s) { renderScale = s; SetRP("renderScale", s); }
    bool shOn = true; float shDist = 60f;
    public bool Shadows { get { return shOn; } }
    public void SetFps(int f) { targetFps = f; Application.targetFrameRate = f; }
    public void AddView(int d) { ViewRadius = ViewRadius >= 4 ? 2 : ViewRadius + d; }
    public void ToggleShadows() { shOn = !shOn; QualitySettings.shadows = shOn ? ShadowQuality.HardOnly : ShadowQuality.Disable; QualitySettings.shadowDistance = shOn ? shDist : 0f; SetRP("shadowDistance", shOn ? shDist : 0f); }
    void Update() {
        float dt = Time.unscaledDeltaTime; smooth = Mathf.Lerp(smooth, dt, 0.05f); Fps = 1f / smooth;
        timer += dt; txtTimer += dt;
        if (timer >= 3f) { timer = 0; Evaluate(); }
        if (txtTimer >= 0.5f && MobileUI.Fps != null) { txtTimer = 0; MobileUI.Fps.text = "FPS " + Mathf.RoundToInt(Fps) + "  |  Escala " + renderScale.ToString("0.00"); }
    }
    void Evaluate() {
        if (Fps < targetFps * 0.8f) { good = 0; if (renderScale > 0.61f) SetScale(renderScale - 0.1f); else if (ViewRadius > 2) ViewRadius--; }
        else if (Fps > targetFps * 0.95f) { good++; if (good >= 4 && renderScale < tierScale) { SetScale(Mathf.Min(tierScale, renderScale + 0.05f)); good = 0; } }
        else good = 0;
    }
}
