using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class JournalManager : MonoBehaviour
{
    [Header("Left Page - Quest Buttons")]
    public GameObject buttonContainer;      // parent of your 8 button slots
    public GameObject[] questButtons;           // drag all 8 buttons here
    public TMP_Text[] questButtonLabels;        // drag each button's Text here

    [Header("Right Page - Quest Detail")]
    public Text detailText;                 // the "New Text" on the right page

    [Header("Main Mystery")]
    public string mainMysteryTitle = "The Mystery";
    [TextArea(2, 5)]
    public string mainMysteryDescription = "Something strange happened tonight...";

    [Header("Journal Panel")]
    public GameObject journalPanel;
    public GameObject notification;

    private bool isOpen = false;
    private string selectedQuestId = null;

    void Start()
    {
        journalPanel.SetActive(false);
        if (notification != null) notification.SetActive(false);

        if (MainManager.mainManager != null)
        {
            MainManager.mainManager.onQuestsChanged += OnQuestsChanged;
            MainManager.mainManager.onQuestAdded += OnQuestAdded;
        }

        // Wire up button clicks via Button component
        for (int i = 0; i < questButtons.Length; i++)
        {
            int index = i;
            Button btn = questButtons[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnQuestButtonClicked(index));
            else
                Debug.LogWarning("No Button component on quest button: " + i);

            // Strip the button background so the entry reads as a plain text line
            Image bg = questButtons[i].GetComponent<Image>();
            if (bg != null) bg.enabled = false;
        }

        // Configure labels: wrap long text, auto-shrink if needed, ellipsis as last resort
        foreach (TMP_Text label in questButtonLabels)
        {
            if (label == null) continue;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.enableAutoSizing = true;
            label.fontSizeMin = 14f;
            label.fontSizeMax = 36f;
            label.color = Color.black;
        }

        ConfigureDetailText();
        RefreshButtons();
        ShowMainMystery();
    }

    private void ConfigureDetailText()
    {
        if (detailText == null) return;

        // Let the text grow as tall as it needs
        detailText.horizontalOverflow = HorizontalWrapMode.Wrap;
        detailText.verticalOverflow = VerticalWrapMode.Overflow;

        // ScrollRect needs content height > viewport height to scroll.
        // ContentSizeFitter on the content makes it grow with the text.
        ContentSizeFitter fitter = detailText.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = detailText.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Content anchors must be top-aligned so it expands downward as text grows.
        RectTransform rt = detailText.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(1f, 1f);
    }

    [Header("Sizing")]
    [Tooltip("Fraction of the screen height the journal should occupy when opened.")]
    [Range(0.5f, 1f)]
    public float screenFillFraction = 0.95f;

    [Header("Sound")]
    [Tooltip("AudioSource used to play the open/close SFX. 2D, Loop off, Play On Awake off.")]
    public AudioSource sfxSource;
    public AudioClip openSound;
    [Range(0f, 1f)] public float openVolume = 1f;
    public AudioClip closeSound;
    [Range(0f, 1f)] public float closeVolume = 1f;

    public void ToggleJournal()
    {
        bool shouldOpen = !journalPanel.activeSelf;

        if (shouldOpen)
        {
            // Activate the panel first so an AudioSource living inside it is enabled before we play.
            journalPanel.SetActive(true);
            PlaySfx(openSound, openVolume);
            isOpen = true;
            FitToScreen();
            RefreshButtons();      // always refresh on open
            ShowMainMystery();     // always reset to main page on open
        }
        else
        {
            // Play SFX before deactivating so the AudioSource isn't disabled mid-clip.
            PlaySfx(closeSound, closeVolume);
            journalPanel.SetActive(false);
            // Clear the "new quest" dot now that the player has closed the journal.
            if (notification != null) notification.SetActive(false);
            isOpen = false;
        }
    }

    // SFX always plays from this GameObject (which stays active), not from one that might be
    // inside journalPanel and get disabled mid-clip. Mixer routing is inherited from sfxSource
    // so the user's Audio Mixer setup is preserved.
    private AudioSource _sfx;

    private void EnsureSfx()
    {
        if (_sfx != null) return;
        // Create a dedicated GameObject at scene root (parent = null) so this AudioSource
        // is independent of journalPanel's active state. If the JournalManager lives
        // inside journalPanel, an AudioSource on it would be killed mid-clip when the
        // panel deactivates. A root-level GameObject can't be.
        GameObject go = new GameObject("JournalSFX");
        _sfx = go.AddComponent<AudioSource>();
        _sfx.playOnAwake = false;
        _sfx.loop = false;
        _sfx.spatialBlend = 0f;
        if (sfxSource != null && sfxSource.outputAudioMixerGroup != null)
            _sfx.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
    }

    private void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        EnsureSfx();
        _sfx.PlayOneShot(clip, volume);
    }

    private void OnQuestsChanged()
    {
        // Refresh only — notification is handled by OnQuestAdded so it doesn't relight
        // when a quest is completed.
        Debug.Log("OnQuestsChanged fired, panel active: " + journalPanel.activeSelf);
        if (journalPanel.activeSelf)
        {
            RefreshButtons();
            if (selectedQuestId == null)
                ShowMainMystery();
            else
                ShowQuestDetail(selectedQuestId);
        }
    }

    private void OnQuestAdded()
    {
        // Light the notification dot only when a brand-new quest is added.
        // It's cleared again when the player closes the journal.
        if (notification != null) notification.SetActive(true);
    }

    private void RefreshButtons()
    {
        Debug.Log("RefreshButtons reading from instance: " + MainManager.mainManager.GetInstanceID());
        if (MainManager.mainManager == null) return;

        questButtons[0].SetActive(true);
        questButtonLabels[0].text = mainMysteryTitle;

        List<string> allQuests = new();
        foreach (string q in MainManager.mainManager.completedQuests)
            if (!allQuests.Contains(q)) allQuests.Add(q);
        foreach (string q in MainManager.mainManager.quests)
            if (!allQuests.Contains(q)) allQuests.Add(q);

        int slots = questButtons.Length - 1; // slot 0 is Main Mystery
        while (allQuests.Count > slots)
        {
            // drop the oldest completed quest first
            string removed = null;
            foreach (string q in allQuests)
            {
                if (MainManager.mainManager.completedQuests.Contains(q))
                {
                    removed = q;
                    break;
                }
            }
            if (removed != null) allQuests.Remove(removed);
            else break; // only active quests left, can't trim further
        }

        Debug.Log("RefreshButtons: total quests = " + allQuests.Count);
        for (int i = 0; i < allQuests.Count; i++)
            Debug.Log("Quest " + i + ": " + allQuests[i]);

        for (int i = 1; i < questButtons.Length; i++)
        {
            int questIndex = i - 1;
            if (questIndex < allQuests.Count)
            {
                string questId = allQuests[questIndex];
                QuestData data = MainManager.mainManager.GetQuestData(questId);
                Debug.Log("Button " + i + " questId: " + questId + " data: " + (data != null ? data.displayName : "NULL"));
                questButtons[i].SetActive(true);
                questButtonLabels[i].text = data != null ? data.displayName : questId;
            }
            else
            {
                questButtons[i].SetActive(false);
            }
        }
    }

    private void OnQuestButtonClicked(int index)
    {
        Debug.Log("Button clicked: " + index);
        if (index == 0)
        {
            selectedQuestId = null;
            ShowMainMystery();
            return;
        }

        List<string> allQuests = new();
        foreach (string q in MainManager.mainManager.completedQuests)
            if (!allQuests.Contains(q)) allQuests.Add(q);
        foreach (string q in MainManager.mainManager.quests)
            if (!allQuests.Contains(q)) allQuests.Add(q);

        int slots = questButtons.Length - 1;
        while (allQuests.Count > slots)
        {
            string removed = null;
            foreach (string q in allQuests)
            {
                if (MainManager.mainManager.completedQuests.Contains(q))
                {
                    removed = q;
                    break;
                }
            }
            if (removed != null) allQuests.Remove(removed);
            else break;
        }

        int questIndex = index - 1;
        Debug.Log("Quest index: " + questIndex + " total: " + allQuests.Count);
        if (questIndex < allQuests.Count)
        {
            selectedQuestId = allQuests[questIndex];
            Debug.Log("Showing detail for: " + selectedQuestId);
            ShowQuestDetail(selectedQuestId);
        }
    }

    private void FitToScreen()
    {
        RectTransform panelRt = journalPanel.GetComponent<RectTransform>();
        if (panelRt == null) return;

        Canvas canvas = journalPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        RectTransform canvasRt = canvas.GetComponent<RectTransform>();
        if (canvasRt == null) return;

        Vector2 panelSize = panelRt.rect.size;
        Vector2 canvasSize = canvasRt.rect.size;
        if (panelSize.x <= 0 || panelSize.y <= 0) return;

        float scaleX = (canvasSize.x * screenFillFraction) / panelSize.x;
        float scaleY = (canvasSize.y * screenFillFraction) / panelSize.y;
        float scale = Mathf.Min(scaleX, scaleY, 1f);
        panelRt.localScale = new Vector3(scale, scale, 1f);
    }

    private void ShowMainMystery()
    {
        // Build main mystery page from all discovered clues
        System.Text.StringBuilder sb = new();
        sb.AppendLine("<b>" + mainMysteryTitle + "</b>");
        sb.AppendLine();
        sb.AppendLine(mainMysteryDescription);
        sb.AppendLine();

        if (MainManager.mainManager.completedQuests.Count > 0)
        {
            sb.AppendLine("<b>Discovered:</b>");
            foreach (string qId in MainManager.mainManager.completedQuests)
            {
                QuestData data = MainManager.mainManager.GetQuestData(qId);
                if (data != null)
                    sb.AppendLine("✓ " + data.displayName);
            }
        }

        if (MainManager.mainManager.quests.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("<b>Active leads:</b>");
            foreach (string qId in MainManager.mainManager.quests)
            {
                QuestData data = MainManager.mainManager.GetQuestData(qId);
                if (data != null)
                    sb.AppendLine("→ " + data.displayName);
            }
        }

        detailText.text = sb.ToString();
    }

    private void ShowQuestDetail(string questId)
    {
        QuestData data = MainManager.mainManager.GetQuestData(questId);
        if (data == null) { detailText.text = questId; return; }

        bool complete = MainManager.mainManager.completedQuests.Contains(questId);

        System.Text.StringBuilder sb = new();
        sb.AppendLine("<b>" + data.displayName + "</b>");
        sb.AppendLine();
        sb.AppendLine(complete ? data.completedDescription : data.activeDescription);
        if (complete) sb.AppendLine("\n<color=green>✓ Completed</color>");

        detailText.text = sb.ToString();
        Debug.Log("DetailText set to: " + detailText.text);
        Debug.Log("DetailText object active: " + detailText.gameObject.activeInHierarchy);
    }
}
