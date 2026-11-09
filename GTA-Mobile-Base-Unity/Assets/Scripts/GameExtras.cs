using UnityEngine;
// Se crea solo si hay un GameBootstrap en la escena: no hace falta tocar nada.
[DefaultExecutionOrder(50)]
public class GameExtras : MonoBehaviour {
    PlayerVehicleInteractor inter; CharacterController cc; CityStreamer city;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto() { if (FindObjectOfType<GameBootstrap>() != null && FindObjectOfType<GameExtras>() == null) new GameObject("Extras").AddComponent<GameExtras>(); }
    void Start() {
        var pc = FindObjectOfType<PlayerController>(); Game.Player = pc.transform; inter = pc.GetComponent<PlayerVehicleInteractor>(); Game.Inter = inter;
        Game.Cam = Camera.main; cc = pc.GetComponent<CharacterController>(); city = FindObjectOfType<CityStreamer>();
        GameState.Load(); GameState.Health = 100f; GameState.Wanted = 0;
        var taxi = FindObjectOfType<SimpleCar>(); if (taxi != null) taxi.gameObject.AddComponent<CarFx>();
        gameObject.AddComponent<DayNightCycle>(); gameObject.AddComponent<TucumanWorld>(); gameObject.AddComponent<StreetName>(); gameObject.AddComponent<TrafficManager>();
        gameObject.AddComponent<PedestrianManager>(); gameObject.AddComponent<Collectibles>(); gameObject.AddComponent<PoliceManager>();
        var mm = gameObject.AddComponent<MissionManager>(); gameObject.AddComponent<Hud>().mm = mm; gameObject.AddComponent<PauseMenu>();
        MobileUI.Btn(MobileUI.Root, "Bocina", new Vector2(1, 0), new Vector2(-310, 235), 100, () => { if (CarFx.Player != null && inter.InCar) CarFx.Player.Horn(); });
        GameState.Msg("Bienvenido a San Miguel de Tucumán. Acercate al taxi y tocá Subir/Bajar.");
    }
    void Update() {
        if (GameState.Health <= 0f) Hospital();
        else if (GameState.Health < 100f && Time.time - GameState.LastHit > 6f) GameState.Health = Mathf.Min(100f, GameState.Health + 4f * Time.deltaTime);
    }
    void Hospital() {
        inter.ForceExit(); cc.enabled = false; Game.Player.position = new Vector3(3, 1.2f, 10); city.Prewarm(); cc.enabled = true;
        int fee = GameState.Money / 10; GameState.Add(-fee); GameState.Health = 100f; GameState.Wanted = 0; GameState.Msg("Te llevaron al hospital. Te cobraron " + GameState.Pesos(fee));
    }
    void OnApplicationPause(bool p) { if (p) PlayerPrefs.Save(); }
    void OnDestroy() { PlayerPrefs.Save(); }
}
