using UnityEngine;
using UnityEngine.UI;

// Attach to a UI GameObject (e.g. the journal's notification dot) to make it pulse
// gently while active. Because Update only runs when the GameObject is enabled, the
// pulse stops automatically the moment something calls SetActive(false).
[RequireComponent(typeof(RectTransform))]
public class UIPulse : MonoBehaviour
{
    [Header("Scale Pulse")]
    [Tooltip("Pulse speed in cycles per second.")]
    public float speed = 1.5f;
    [Tooltip("Scale at the dim end of the pulse (1 = normal size).")]
    public float minScale = 0.9f;
    [Tooltip("Scale at the bright end of the pulse.")]
    public float maxScale = 1.15f;

    [Header("Alpha Pulse (optional)")]
    [Tooltip("Also fade the Image's alpha between these values. Disable for scale-only.")]
    public bool pulseAlpha = true;
    [Range(0f, 1f)] public float minAlpha = 0.6f;
    [Range(0f, 1f)] public float maxAlpha = 1f;

    private RectTransform _rt;
    private Image _image;
    private Vector3 _baseScale;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _baseScale = _rt.localScale;
    }

    void OnEnable()
    {
        // Restart cleanly each time the dot is shown.
        _rt.localScale = _baseScale * minScale;
        if (pulseAlpha && _image != null)
        {
            Color c = _image.color;
            c.a = minAlpha;
            _image.color = c;
        }
    }

    void Update()
    {
        // PingPong returns 0..1, smoothed with SmoothStep for an eased pulse.
        float t = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time * speed, 1f));

        float s = Mathf.Lerp(minScale, maxScale, t);
        _rt.localScale = _baseScale * s;

        if (pulseAlpha && _image != null)
        {
            Color c = _image.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            _image.color = c;
        }
    }
}
