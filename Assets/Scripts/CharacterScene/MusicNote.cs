using UnityEngine;
using UnityEngine.Pool;

public class MusicNote : MonoBehaviour
{
    private float speed = 1f;
    private float fadeSpeed = 0.5f;
    private SpriteRenderer sr;
    private float horizontalDrift;

    public IObjectPool<GameObject> pool;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        // Random horizontal drift each time a note spawns
        horizontalDrift = Random.Range(-0.3f, 0.3f);
    }

    void Update()
    {
        transform.position += new Vector3(horizontalDrift, speed, 0f) * Time.deltaTime;

        Color c = sr.color;
        c.a -= fadeSpeed * Time.deltaTime;
        sr.color = c;

        if (c.a <= 0f)
        {
            pool.Release(gameObject);
        }
    }
}