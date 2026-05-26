using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to the laundry chute interactable alongside an Outline component.
// InteractionManager will call TryTriggerPuzzle() when the player presses E nearby.
// The quiz opens only when questId is active and all requiredCompletedQuests are done.
public class LaundryChutePuzzle : MonoBehaviour
{
    [SerializeField] private string questId = "solve_the_chute";
    [SerializeField] private string[] requiredCompletedQuests;
    [SerializeField] private string nextScene;

    [Header("Quiz UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private int correctAnswerIndex;

    [Header("Content")]
    [SerializeField] [TextArea] private string question;
    [SerializeField] private string[] answers;
    [SerializeField] [TextArea] private string wrongAnswerText;

    [Header("Wrong Answer Feedback")]
    [SerializeField] private GameObject wrongAnswerPanel;
    [SerializeField] private TMP_Text wrongAnswerFeedbackText;

    void Start()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (wrongAnswerPanel != null) wrongAnswerPanel.SetActive(false);
    }

    // Returns true if the puzzle handled the interaction (quiz opened or already open).
    public bool TryTriggerPuzzle()
    {
        if (!CanActivate()) return false;
        OpenQuiz();
        return true;
    }

    bool CanActivate()
    {
        if (MainManager.mainManager == null) return false;
        if (!MainManager.mainManager.quests.Contains(questId)) return false;
        return MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests);
    }

    void OpenQuiz()
    {
        if (quizPanel == null) return;
        quizPanel.SetActive(true);

        if (questionText != null)
            questionText.text = question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerTexts != null && i < answerTexts.Length && i < answers.Length)
                answerTexts[i].text = answers[i];

            int captured = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(captured));
        }
    }

    void OnAnswerSelected(int index)
    {
        if (index == correctAnswerIndex)
        {
            quizPanel.SetActive(false);
            CompleteAndTransition();
        }
        else
        {
            if (wrongAnswerPanel != null)
            {
                wrongAnswerPanel.SetActive(true);
                if (wrongAnswerFeedbackText != null)
                    wrongAnswerFeedbackText.text = wrongAnswerText;
            }
        }
    }

    public void DismissWrongAnswer()
    {
        if (wrongAnswerPanel != null) wrongAnswerPanel.SetActive(false);
    }

    void CompleteAndTransition()
    {
        if (MainManager.mainManager == null) return;
        MainManager.mainManager.quests.Remove(questId);
        if (!MainManager.mainManager.completedQuests.Contains(questId))
            MainManager.mainManager.completedQuests.Add(questId);
        MainManager.mainManager.onQuestsChanged?.Invoke();

        if (!string.IsNullOrEmpty(nextScene))
            MainManager.mainManager.LoadScene(nextScene);
    }
}
