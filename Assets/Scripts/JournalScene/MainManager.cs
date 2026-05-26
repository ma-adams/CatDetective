using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{

    public static MainManager mainManager;

    public List<string> quests = new();
    public List<string> completedQuests = new();
    public List<string> inventory = new();

    private Dictionary<string, QuestData> questRegistry = new();

    public System.Action onQuestsChanged;
    // Fires only when a NEW quest is added (not on completion). Use this for "new quest" UI cues.
    public System.Action onQuestAdded;

    [Header("Sound")]
    [Tooltip("Played when a quest is added via AddQuest().")]
    public AudioClip questStartedSound;
    [Range(0f, 1f)] public float questStartedVolume = 1f;
    [Tooltip("Played when a quest is completed via CompleteQuest().")]
    public AudioClip questCompletedSound;
    [Range(0f, 1f)] public float questCompletedVolume = 1f;
    [Tooltip("Optional. If set, its mixer routing is copied to the playback AudioSource.")]
    public AudioSource sfxSourceTemplate;

    private AudioSource _sfx;

    //makes sure there is only one instace of main manager throughout game
    //can transition between scenes without losing data
    private void Awake()
    {
        if (mainManager != null)
        {
            Destroy(gameObject);
            return;
        }

        mainManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterQuest(QuestData data)
    {
        if (!questRegistry.ContainsKey(data.questId))
        {
            questRegistry[data.questId] = data;
        }
    }

    public QuestData GetQuestData(string questId) 
    {
        questRegistry.TryGetValue(questId, out QuestData data);
        return data;
    }    

    public void AddQuest(string questId)
    {
        if (!quests.Contains(questId))
        {
            quests.Add(questId);
            Debug.Log("AddQuest called on instance: " + GetInstanceID() + " questId: " + questId);
            PlaySfx(questStartedSound, questStartedVolume);
            onQuestAdded?.Invoke();
            onQuestsChanged?.Invoke();
        }
    }

    // Single chokepoint for marking a quest complete. Plays the SFX and notifies listeners.
    public void CompleteQuest(string questId)
    {
        if (string.IsNullOrEmpty(questId)) return;
        if (completedQuests.Contains(questId)) return;
        completedQuests.Add(questId);
        PlaySfx(questCompletedSound, questCompletedVolume);
        onQuestsChanged?.Invoke();
    }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null) return;
        if (_sfx == null)
        {
            // Root-level GameObject so the AudioSource is unaffected by scene/parent activation.
            GameObject go = new GameObject("MainManagerSFX");
            _sfx = go.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;
            _sfx.loop = false;
            _sfx.spatialBlend = 0f;
            DontDestroyOnLoad(go);
            if (sfxSourceTemplate != null && sfxSourceTemplate.outputAudioMixerGroup != null)
                _sfx.outputAudioMixerGroup = sfxSourceTemplate.outputAudioMixerGroup;
        }
        _sfx.PlayOneShot(clip, volume);
    }

    public bool HasItem(string itemId) => inventory.Contains(itemId);
    public void AddItem(string itemId) { if (!inventory.Contains(itemId)) inventory.Add(itemId); }
    public void RemoveItem(string itemId) => inventory.Remove(itemId);

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public bool AllQuestsComplete(string[] questIds)
    {
        foreach (string id in questIds)
            if (!completedQuests.Contains(id))
                return false;
        return true;
    }
    //trying to make it reload
}
