using UnityEngine;
public class CarFx : MonoBehaviour {
    public static CarFx Player; SimpleCar car; Rigidbody rb; AudioSource eng, one; Light head;
    void Start() {
        Player = this; car = GetComponent<SimpleCar>(); rb = GetComponent<Rigidbody>();
        eng = gameObject.AddComponent<AudioSource>(); eng.clip = Sfx.Engine; eng.loop = true; eng.spatialBlend = 1f; eng.minDistance = 6f; eng.maxDistance = 70f; eng.volume = 0f; eng.Play();
        one = gameObject.AddComponent<AudioSource>(); one.spatialBlend = 0.7f;
        var hl = new GameObject("Faros"); hl.transform.SetParent(transform, false); hl.transform.localPosition = new Vector3(0, 0.9f, 2f);
        head = hl.AddComponent<Light>(); head.type = LightType.Spot; head.range = 40f; head.spotAngle = 70f; head.intensity = 3f; head.shadows = LightShadows.None; head.enabled = false;
    }
    void Update() {
        eng.pitch = 0.55f + rb.velocity.magnitude / 30f * 1.8f;
        eng.volume = car.driven ? Mathf.Lerp(0.15f, 0.55f, Mathf.Abs(InputHub.Move.y)) : 0f;
        head.enabled = car.driven && DayNightCycle.Night > 0.35f;
    }
    public void Horn() { one.PlayOneShot(Sfx.Horn); }
    void OnCollisionEnter(Collision c) {
        float v = c.relativeVelocity.magnitude;
        if (v > 6f && car.driven) { GameState.Damage(v * 1.1f); one.PlayOneShot(Sfx.Crash, Mathf.Clamp01(v / 20f)); }
    }
}
