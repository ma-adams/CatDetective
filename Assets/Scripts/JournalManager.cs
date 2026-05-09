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
            MainManager.mainManager.onQuestsChanged += OnQuestsChanged;

        // Wire up button clicks via Button component
        for (int i = 0; i < questButtons.Length; i++)
        {
            int index = i;
            Button btn = questButtons[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnQuestButtonClicked(index));
            else
                Debug.LogWarning("No Button component on quest button: " + i);
        }

        RefreshButtons();
        ShowMainMystery();
    }

    public void ToggleJournal()
    {
        isOpen = !isOpen;
        journalPanel.SetActive(isOpen);
        if (isOpen)
        {
            if (notification != null) notification.SetActive(false);
            RefreshButtons();
            // Default to main mystery on open
            ShowMainMystery();
        }
    }

    private void OnQuestsChanged()
    {
        if (notification != null) notification.SetActive(true);
        if (isOpen)
        {
            RefreshButtons();
            // Refresh whichever page is currently showing
            if (selectedQuestId == null)
                ShowMainMystery();
            else
                ShowQuestDetail(selectedQuestId);
        }
    }

    private void RefreshButtons()
    {
        if (MainManager.mainManager == null) return;

        questButtons[0].SetActive(true);
        questButtonLabels[0].text = mainMysteryTitle;

        // Build a combined list with no duplicates, completed quests first
        List<string> allQuests = new();
        foreach (string q in MainManager.mainManager.completedQuests)
            if (!allQuests.Contains(q)) allQuests.Add(q);
        foreach (string q in MainManager.mainManager.quests)
            if (!allQuests.Contains(q)) allQuests.Add(q);

        for (int i = 1; i < questButtons.Length; i++)
        {
            int questIndex = i - 1;
            if (questIndex < allQuests.Count)
            {
                string questId = allQuests[questIndex];
                QuestData data = MainManager.mainManager.GetQuestData(questId);
                questButtons[i].SetActive(true);
                // Fall back to questId if data not registered yet
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
        if (index == 0)
        {
            ShowMainMystery();
            return;
        }

        List<string> allQuests = new();
        allQuests.AddRange(MainManager.mainManager.quests);
        allQuests.AddRange(MainManager.mainManager.completedQuests);

        int questIndex = index - 1;
        if (questIndex < allQuests.Count)
        {
            selectedQuestId = allQuests[questIndex];
            ShowQuestDetail(selectedQuestId);
        }
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
    }
}