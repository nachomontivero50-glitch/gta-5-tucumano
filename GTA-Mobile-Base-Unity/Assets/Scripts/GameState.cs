using UnityEngine;
using System.Collections.Generic;
public static class Game {
    public static Transform Player; public static PlayerVehicleInteractor Inter; public static Camera Cam;
    public static Vector3 PlayerPos { get { return Player.position; } }
    public static float PlayerSpeed() { if (Inter != null && Inter.InCar) return Inter.Current.GetComponent<Rigidbody>().velocity.magnitude; return 0f; }
}
public static class GameState {
    public static int Money = 5000; public static float Health = 100f; public static int Wanted; public static float LastHit; static float lastWanted = -99f;
    public static System.Action<string> OnMsg;
    public static void Load() { Money = PlayerPrefs.GetInt("money", 5000); }
    public static void Add(int n) { Money = Mathf.Max(0, Money + n); PlayerPrefs.SetInt("money", Money); }
    public static void Msg(string s) { if (OnMsg != null) OnMsg(s); }
    public static void Damage(float d) { Health = Mathf.Max(0f, Health - d); LastHit = Time.time; }
    public static void AddWanted(string why) { if (Time.time - lastWanted < 8f) return; lastWanted = Time.time; if (Wanted < 3) { Wanted++; Msg("¡La policía te busca! " + why); } }
    public static string Pesos(int n) { return "$ " + n.ToString("N0", System.Globalization.CultureInfo.InvariantCulture).Replace(',', '.'); }
}
public static class MatCache {
    static Dictionary<Color, Material> d = new Dictionary<Color, Material>();
    public static Material Get(Color c) { Material m; if (!d.TryGetValue(c, out m) || m == null) { m = Palette.Make(c); d[c] = m; } return m; }
}
public static class Sfx {
    const int R = 22050; static AudioClip engine, horn, crash, coin; static AudioSource src;
    static AudioClip Make(string n, float secs, System.Func<float, float> f) {
        int len = (int)(R * secs); var d = new float[len];
        for (int i = 0; i < len; i++) d[i] = Mathf.Clamp(f(i / (float)R), -1f, 1f);
        var c = AudioClip.Create(n, len, 1, R, false); c.SetData(d, 0); return c;
    }
    public static AudioClip Engine { get { if (engine == null) engine = Make("engine", 1f, t => { float p = t * 55f; return 0.25f * (2f * (p - Mathf.Floor(p)) - 1f) + 0.2f * Mathf.Sin(2f * Mathf.PI * 110f * t) + 0.1f * Mathf.Sin(2f * Mathf.PI * 165f * t); }); return engine; } }
    public static AudioClip Horn { get { if (horn == null) horn = Make("horn", 0.5f, t => 0.25f * (Mathf.Sin(2f * Mathf.PI * 370f * t) + Mathf.Sin(2f * Mathf.PI * 466f * t)) * Mathf.Min(1f, (0.5f - t) * 20f)); return horn; } }
    public static AudioClip Crash { get { if (crash == null) crash = Make("crash", 0.35f, t => (Random.value * 2f - 1f) * Mathf.Exp(-t * 9f)); return crash; } }
    public static AudioClip Coin { get { if (coin == null) coin = Make("coin", 0.4f, t => 0.4f * Mathf.Sin(2f * Mathf.PI * (t < 0.1f ? 880f : 1320f) * t) * Mathf.Exp(-t * 7f)); return coin; } }
    public static void Play(AudioClip c, float v) { if (src == null) { var g = new GameObject("Sfx"); src = g.AddComponent<AudioSource>(); } src.PlayOneShot(c, v); }
}
