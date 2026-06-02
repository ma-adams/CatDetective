using UnityEngine;

// Plays a random clip from an array on a random interval. Generic ambience helper —
// good for water drops, distant creaks, footsteps, anything that should feel irregular.
// Attach to a GameObject with an AudioSource (auto-creates one if missing).
public class RandomOneShot : MonoBehaviour
{
    [Header("Clips")]
    [Tooltip("Pool of clips. A random one is picked per trigger.")]
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Random volume variance: actual volume = volume * Random(1-jitter, 1).")]
    [Range(0f, 1f)] public float volumeJitter = 0.15f;

    [Header("Timing (seconds between plays)")]
    public float minInterval = 4f;
    public float maxInterval = 12f;

    [Header("Start")]
    [Tooltip("Wait at least this long after Start before the first play, so the scene doesn't fire immediately.")]
    public float initialDelay = 1f;

    private AudioSource _source;
    private float _nextPlayTime;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null)
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.loop = false;
            _source.spatialBlend = 0f;
        }
        ScheduleNext(initialDelay);
    }

    void Update()
    {
        if (clips == null || clips.Length == 0) return;
        if (Time.time < _nextPlayTime) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null)
        {
            float v = volume * Random.Range(1f - volumeJitter, 1f);
            _source.PlayOneShot(clip, v);
        }
        ScheduleNext(Random.Range(minInterval, maxInterval));
    }

    private void ScheduleNext(float wait)
    {
        _nextPlayTime = Time.time + wait;
    }
}
