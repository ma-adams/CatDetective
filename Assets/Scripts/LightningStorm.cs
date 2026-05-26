using System.Collections;
using UnityEngine;

// Periodically triggers a lightning flash paired with delayed thunder, like a distant storm.
// Attach to an empty GameObject in the scene, then assign a flash Light and one or more
// thunder clips in the Inspector.
public class LightningStorm : MonoBehaviour
{
    [Header("Lightning")]
    [Tooltip("Light flashed during a strike. A dedicated light aimed at the room works best.")]
    public Light flashLight;
    [Tooltip("Peak intensity the light reaches during a flash.")]
    public float flashIntensity = 4f;

    [Header("Thunder")]
    [Tooltip("AudioSource used to play thunder. 2D, Loop off, Play On Awake off.")]
    public AudioSource thunderSource;
    [Tooltip("One or more thunder clips. A random one is picked per strike.")]
    public AudioClip[] thunderClips;
    [Tooltip("Delay (seconds) between the flash and the thunder. Bigger = more distant storm.")]
    public Vector2 thunderDelayRange = new Vector2(1f, 4f);

    [Header("Timing")]
    [Tooltip("Random wait (seconds) between strikes.")]
    public Vector2 strikeIntervalRange = new Vector2(10f, 30f);

    void Start()
    {
        if (flashLight != null)
            flashLight.enabled = false;

        StartCoroutine(StormLoop());
    }

    IEnumerator StormLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(strikeIntervalRange.x, strikeIntervalRange.y));
            yield return StartCoroutine(Strike());
        }
    }

    IEnumerator Strike()
    {
        // Lightning: a quick burst of 2-3 flickers rather than one clean flash.
        if (flashLight != null)
        {
            int flickers = Random.Range(2, 4);
            for (int i = 0; i < flickers; i++)
            {
                flashLight.intensity = flashIntensity * Random.Range(0.6f, 1f);
                flashLight.enabled = true;
                yield return new WaitForSeconds(Random.Range(0.04f, 0.10f));
                flashLight.enabled = false;
                yield return new WaitForSeconds(Random.Range(0.04f, 0.12f));
            }
        }

        // Thunder lags the flash — the delay length is what sells the storm's distance.
        yield return new WaitForSeconds(Random.Range(thunderDelayRange.x, thunderDelayRange.y));

        if (thunderSource != null && thunderClips != null && thunderClips.Length > 0)
            thunderSource.PlayOneShot(thunderClips[Random.Range(0, thunderClips.Length)]);
    }
}
