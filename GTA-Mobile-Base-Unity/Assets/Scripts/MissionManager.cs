using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MissionManager : MonoBehaviour {
    public string HudText = ""; public bool HasTarget; public Vector3 TargetPos;
    List<Vector3> wp = new List<Vector3>(); List<string> txt = new List<string>(); int idx, reward; float timeLeft, cd = 10f; bool active, needCar; string title; Transform beacon;
    void Start() {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cylinder); DestroyImmediate(g.GetComponent<Collider>());
        var m = new Material(Graphic.defaultGraphicMaterial); m.color = new Color(1f, .85f, .1f, .28f);
        var r = g.GetComponent<Renderer>(); r.sharedMaterial = m; r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        g.transform.localScale = new Vector3(6f, 25f, 6f); beacon = g.transform; g.SetActive(false);
    }
    static float Man(Vector3 a, Vector3 b) { return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z); }
    Vector3 Pt(Vector3 from, float min, float max) {
        float S = CityStreamer.S;
        for (int k = 0; k < 30; k++) {
            float ang = Random.value * 6.2832f, d = Random.Range(min, max);
            Vector3 p = from + new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * d; int i = Mathf.RoundToInt(p.x / S), j = Mathf.RoundToInt(p.z / S);
            if (i == 0 && j == 0) continue;
            Vector3 q = new Vector3(i * S + 3.5f, 0.1f, j * S - 3.5f); float dd = Vector3.Distance(q, from);
            if (dd >= min * 0.7f && dd <= max * 1.3f) return q;
        }
        return new Vector3(S * 3 + 3.5f, 0.1f, S * 3 - 3.5f);
    }
    void StartMission() {
        wp.Clear(); txt.Clear(); idx = 0; Vector3 pp = Game.PlayerPos; int type = Random.Range(0, 3); float len;
        if (type == 0) {
            title = "TAXI: pasajero"; needCar = true; var a = Pt(pp, 80, 200); string bn; var b = TucumanData.Spot(a, 90f, 700f, out bn, Pt(a, 200, 450)); wp.Add(a); wp.Add(b);
            txt.Add("Andá a buscar al pasajero"); txt.Add("Llevalo a " + bn); len = Man(pp, a) + Man(a, b); timeLeft = 25f + len / 9f; reward = 700 + (int)(len * 3f);
        } else if (type == 1) {
            title = "DELIVERY: empanadas tucumanas"; needCar = false; var a = Pt(pp, 60, 140); string bn; var b = TucumanData.Spot(a, 90f, 350f, out bn, Pt(a, 140, 300)); wp.Add(a); wp.Add(b);
            txt.Add("Retirá las empanadas en la empanadería"); txt.Add("Entregalas en " + bn); len = Man(pp, a) + Man(a, b); timeLeft = 30f + len / 4.2f; reward = 500 + (int)(len * 3f);
        } else {
            title = "CARRERA callejera"; needCar = true; Vector3 prev = pp; len = 0f;
            for (int i = 0; i < 5; i++) { var p = Pt(prev, 120, 260); wp.Add(p); txt.Add("Checkpoint " + (i + 1) + " de 5"); len += Man(prev, p); prev = p; }
            timeLeft = 20f + len / 11f; reward = 1500 + (int)(len * 2f);
        }
        active = true; GameState.Msg("Nueva misión: " + title);
    }
    void Finish(bool ok) {
        active = false; cd = 8f;
        if (ok) { GameState.Add(reward); Sfx.Play(Sfx.Coin, .8f); GameState.Msg("¡Misión cumplida! +" + GameState.Pesos(reward)); }
        else GameState.Msg("Se acabó el tiempo. Misión fallida.");
    }
    void Update() {
        float dt = Time.deltaTime;
        if (!active) { HasTarget = false; beacon.gameObject.SetActive(false); cd -= dt; HudText = "Próxima misión en " + Mathf.CeilToInt(Mathf.Max(0f, cd)) + " s"; if (cd <= 0f) StartMission(); return; }
        timeLeft -= dt; Vector3 t = wp[idx]; HasTarget = true; TargetPos = t;
        beacon.gameObject.SetActive(true); beacon.position = new Vector3(t.x, 25f, t.z);
        Vector3 pp = Game.PlayerPos; float d = Vector2.Distance(new Vector2(pp.x, pp.z), new Vector2(t.x, t.z));
        if (d < 6f && (!needCar || Game.Inter.InCar)) { idx++; Sfx.Play(Sfx.Coin, .6f); if (idx >= wp.Count) { Finish(true); return; } GameState.Msg(txt[idx]); }
        else if (timeLeft <= 0f) { Finish(false); return; }
        HudText = title + "\n" + txt[idx] + "  -  " + Mathf.RoundToInt(d) + " m  |  " + Mathf.CeilToInt(timeLeft) + " s" + (needCar && !Game.Inter.InCar ? "\n(necesitás un vehículo)" : "");
    }
}
