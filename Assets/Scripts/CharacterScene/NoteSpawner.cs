using UnityEngine;
using UnityEngine.Pool;

public class NoteSpawner : MonoBehaviour
{
    public float spawnInterval = 0.5f;

    private GameObject notePrefab;
    private ObjectPool<GameObject> notePool;
    private float timer;

    void Start()
    {
        notePrefab = Resources.Load<GameObject>("Music Note");

        notePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(notePrefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj)
        );
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnNote();
        }
    }

    void SpawnNote()
    {
        GameObject note = notePool.Get();
        Vector3 spawnPos = transform.position;
        spawnPos.y += 1.0f;
        spawnPos.z = transform.position.z;
        note.transform.position = spawnPos;
        note.GetComponent<MusicNote>().pool = notePool;
    }
}