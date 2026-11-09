using UnityEngine;
// Pegalo en un GameObject vacío de una escena vacía. Arma todo solo.
public class GameBootstrap : MonoBehaviour {
    void Awake() {
        Palette.Init(); gameObject.AddComponent<PerformanceManager>();
        Color sky = new Color(.62f, .78f, .92f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = new Color(.55f, .58f, .65f); RenderSettings.fogColor = sky;
        if (FindObjectOfType<Light>() == null) { var l = new GameObject("Sol").AddComponent<Light>(); l.type = LightType.Directional; l.intensity = 1.1f; l.shadows = LightShadows.Hard; l.transform.rotation = Quaternion.Euler(50, -30, 0); }
        var p = GameObject.CreatePrimitive(PrimitiveType.Capsule); p.name = "Jugador"; DestroyImmediate(p.GetComponent<Collider>());
        p.transform.position = new Vector3(3, 1.2f, 10); p.GetComponent<Renderer>().sharedMaterial = Palette.Make(new Color(.9f, .5f, .15f));
        var cc = p.AddComponent<CharacterController>(); cc.height = 2; cc.radius = 0.5f; cc.stepOffset = 0.3f;
        p.AddComponent<PlayerController>();
        var cgo = Camera.main != null ? Camera.main.gameObject : new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cgo.tag = "MainCamera"; var cam = cgo.GetComponent<Camera>(); cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = sky; cam.fieldOfView = 65; cam.nearClipPlane = 0.3f;
        var tpc = cgo.AddComponent<ThirdPersonCamera>(); tpc.SetTarget(p.transform, false);
        var st = gameObject.AddComponent<CityStreamer>(); st.target = p.transform; st.Prewarm();
        MakeTaxi(new Vector3(0, 0.1f, 22));
        p.AddComponent<PlayerVehicleInteractor>().cam = tpc;
        MobileUI.Build();
    }
    void MakeTaxi(Vector3 pos) {
        var car = new GameObject("Taxi"); car.transform.position = pos;
        var rb = car.AddComponent<Rigidbody>(); rb.mass = 1200; rb.drag = 0.1f; rb.angularDrag = 2f; rb.centerOfMass = new Vector3(0, 0.3f, 0);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; rb.interpolation = RigidbodyInterpolation.Interpolate;
        var bc = car.AddComponent<BoxCollider>(); bc.center = new Vector3(0, 0.6f, 0); bc.size = new Vector3(2, 1.2f, 4.2f);
        car.AddComponent<SimpleCar>();
        Part(car.transform, PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(2, .7f, 4.2f), Vector3.zero, new Color(.08f, .08f, .08f)); // taxi porteño: negro
        Part(car.transform, PrimitiveType.Cube, new Vector3(0, 1.15f, -.2f), new Vector3(1.7f, .6f, 2.2f), Vector3.zero, new Color(.95f, .8f, .1f)); // techo amarillo
        foreach (float x in new[] { -1f, 1f }) foreach (float z in new[] { -1.3f, 1.3f })
            Part(car.transform, PrimitiveType.Cylinder, new Vector3(x, .35f, z), new Vector3(.7f, .12f, .7f), new Vector3(0, 0, 90), new Color(.1f, .1f, .1f));
    }
    void Part(Transform par, PrimitiveType t, Vector3 pos, Vector3 scale, Vector3 rot, Color c) {
        var g = GameObject.CreatePrimitive(t); DestroyImmediate(g.GetComponent<Collider>()); g.transform.SetParent(par, false);
        g.transform.localPosition = pos; g.transform.localScale = scale; g.transform.localEulerAngles = rot; g.GetComponent<Renderer>().sharedMaterial = Palette.Make(c);
    }
}
