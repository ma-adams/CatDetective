using UnityEngine;

// Subtle flicker for a Light — wobbles intensity using Perlin noise (natural-feeling,
// not stuttery random), with optional rare full blink-offs for an eerier effect.
// Attach to a GameObject with a Light, or assign one in the Inspector.
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker")]
    [Tooltip("Speed of the wobble. Higher = faster flicker.")]
    public float speed = 3f;
    [Tooltip("Lowest fraction of base intensity during wobble (e.g. 0.7 = 70% of base).")]
    [Range(0f, 1f)] public float minIntensityFraction = 0.7f;
    [Tooltip("Highest fraction of base intensity during wobble.")]
    [Range(0f, 2f)] public float maxIntensityFraction = 1.1f;

    [Header("Blink-Off (optional)")]
    [Tooltip("Average seconds between rare full blink-offs. Set very high to effectively disable.")]
    public float blinkOffEverySeconds = 12f;
    [Tooltip("How long the light stays off during a blink.")]
    public Vector2 blinkOffDurationRange = new Vector2(0.04f, 0.12f);

    private Light _light;
    private float _baseIntensity;
    private float _noiseOffset;
    private float _nextBlinkOffTime;

    void Awake()
    {
        _light = GetComponent<Light>();
        _baseIntensity = _light.intensity;
        // Random offset per-instance so multiple flickering lights don't move in lockstep.
        _noiseOffset = Random.Range(0f, 1000f);
        ScheduleNextBlinkOff();
    }

    void Update()
    {
        if (Time.time >= _nextBlinkOffTime)
        {
            StartCoroutine(BlinkOff());
            ScheduleNextBlinkOff();
        }

        // PerlinNoise returns 0..1; smooth, organic variation rather than jittery random.
        float n = Mathf.PerlinNoise(Time.time * speed + _noiseOffset, 0f);
        float frac = Mathf.Lerp(minIntensityFraction, maxIntensityFraction, n);
        _light.intensity = _baseIntensity * frac;
    }

    private void ScheduleNextBlinkOff()
    {
        // Exponential-ish spacing: average around blinkOffEverySeconds with some variance.
        _nextBlinkOffTime = Time.time + Random.Range(blinkOffEverySeconds * 0.5f, blinkOffEverySeconds * 1.5f);
    }

    private System.Collections.IEnumerator BlinkOff()
    {
        _light.enabled = false;
        yield return new WaitForSeconds(Random.Range(blinkOffDurationRange.x, blinkOffDurationRange.y));
        _light.enabled = true;
    }
}
