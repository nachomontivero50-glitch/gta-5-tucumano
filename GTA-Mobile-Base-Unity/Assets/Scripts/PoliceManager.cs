using System.Collections.Generic;
using UnityEngine;
public class PoliceManager : MonoBehaviour {
    public static List<TrafficCar> Cars = new List<TrafficCar>(); float chk, bustT, calmT;
    void Start() { Cars.Clear(); for (int i = 0; i < 3; i++) Cars.Add(TrafficManager.I.Build(false, true, Color.white)); }
    void Spawn(TrafficCar c) { Vector3 p; int d; TrafficManager.LanePoint(Game.PlayerPos, 70f, 110f, out p, out d); c.Place(p, d); }
    void Update() {
        chk -= Time.deltaTime; if (chk > 0f) return; const float dt = 0.2f; chk = dt; int w = GameState.Wanted;
        for (int i = 0; i < Cars.Count; i++) {
            var c = Cars[i];
            if (i < w) { if (!c.gameObject.activeSelf || Vector3.Distance(c.transform.position, Game.PlayerPos) > 260f) Spawn(c); }
            else if (c.gameObject.activeSelf) c.gameObject.SetActive(false);
        }
        if (w == 0) { bustT = 0f; calmT = 0f; return; }
        float near = 9999f; foreach (var c in Cars) if (c.gameObject.activeSelf) near = Mathf.Min(near, Vector3.Distance(c.transform.position, Game.PlayerPos));
        if (near < 8f && Game.PlayerSpeed() < 6f) { bustT += dt; if (bustT > 2.5f) Busted(); } else bustT = 0f;
        if (near > 80f) { calmT += dt; if (calmT > 15f) { GameState.Wanted--; calmT = 0f; GameState.Msg(GameState.Wanted > 0 ? "Se alejaron un poco..." : "Perdiste a la policía."); } } else calmT = 0f;
    }
    void Busted() { int fine = GameState.Money / 6; GameState.Add(-fine); GameState.Wanted = 0; bustT = 0f; GameState.Msg("¡Te agarró la policía! Multa " + GameState.Pesos(fine)); }
}
