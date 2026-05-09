using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class JournalButtonBehavior : MonoBehaviour {
    [SerializeField] private GameObject questPage;
    [SerializeField] private Text questTextBox;
    [SerializeField] private GameObject notification;
    [SerializeField] private string[] noQuestsText;
    private bool openBook;

    void Start()
    {
        if (MainManager.mainManager != null)
        {
            MainManager.mainManager.onQuestsChanged += ShowNotification;
        }
    }

    void ShowNotification()
    {
        if (notification != null)
        {
            notification.SetActive(true);
        }
    }

    public void OpenQuestBook()
    {
        openBook = !openBook;
        CreatePage();
        WriteQuests();
    }

    private void CreatePage()
    {
        if (questPage != null && notification != null)
        {
            if (openBook)
            {
                questPage.SetActive(true);
                notification.SetActive(false);
            }
            else
            {
                questPage.SetActive(false);
            }
        }
    }

    private void WriteQuests()
    {
        if (questTextBox == null) return;
        if (MainManager.mainManager == null)
        {
            questTextBox.text = "No journal data found.";
            return;
        }
        if (MainManager.mainManager.quests.Count == 0)
        {
            if (noQuestsText != null && noQuestsText.Length > 0)
            {
                questTextBox.text = noQuestsText[Random.Range(0, noQuestsText.Length)];
            }
            else
            {
                StringBuilder stringBuilder = new();
                foreach (string quest in MainManager.mainManager.quests)
                {
                    stringBuilder.AppendLine("•" + quest);
                }
                questTextBox.text = stringBuilder.ToString();
            }
            questTextBox.rectTransform.sizeDelta = new Vector2(questTextBox.rectTransform.sizeDelta.x, questTextBox.preferredHeight);
        }
    }
}