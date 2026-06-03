using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuizQuestion
{
    [TextArea] public string question;
    public string[] answers;
    public int correctAnswerIndex;
    [TextArea] public string wrongAnswerText;
}

// Can be triggered by an NPC via DialogueTrigger.quizToTriggerOnEnd,
// or by the player pressing E near an interactable (via InteractionManager).
public class BasementQuiz : MonoBehaviour
{
    [SerializeField] private string questId = "solve_the_basement";
    [SerializeField] private string[] requiredCompletedQuests;

    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("Quiz UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;

    [Header("Wrong Answer Feedback")]
    [SerializeField] private GameObject wrongAnswerPanel;
    [SerializeField] private TMP_Text wrongAnswerFeedbackText;

    [Header("Completion Screen")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TMP_Text completionMessageText;
    [SerializeField] [TextArea] private string completionMessage = "Good job! Thanks for playing!";
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] [TextArea(5, 20)] private string credits;

    private int currentQuestionIndex = 0;

    void Start()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (wrongAnswerPanel != null) wrongAnswerPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
    }

    // Called by InteractionManager when the player presses E — requires quest to be active.
    public bool TryTriggerPuzzle()
    {
        if (!CanActivate()) return false;
        OpenQuiz();
        return true;
    }

    // Called directly by DialogueTrigger when a dialogue ends — no quest gate needed.
    public void OpenQuiz()
    {
        currentQuestionIndex = 0;
        ShowCurrentQuestion();
    }

    bool CanActivate()
    {
        if (MainManager.mainManager == null) return false;
        if (!MainManager.mainManager.quests.Contains(questId)) return false;
        return MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests);
    }

    void ShowCurrentQuestion()
    {
        if (quizPanel == null || questions == null || currentQuestionIndex >= questions.Length) return;

        quizPanel.SetActive(true);
        QuizQuestion q = questions[currentQuestionIndex];

        if (questionText != null)
            questionText.text = q.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = q.answers != null && i < q.answers.Length;
            answerButtons[i].gameObject.SetActive(hasAnswer);

            if (hasAnswer && answerTexts != null && i < answerTexts.Length)
                answerTexts[i].text = q.answers[i];

            int captured = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(captured));
        }
    }

    void OnAnswerSelected(int index)
    {
        QuizQuestion q = questions[currentQuestionIndex];

        if (index == q.correctAnswerIndex)
        {
            DismissWrongAnswer();
            currentQuestionIndex++;

            if (currentQuestionIndex >= questions.Length)
            {
                quizPanel.SetActive(false);
                CompleteQuiz();
            }
            else
            {
                ShowCurrentQuestion();
            }
        }
        else
        {
            if (wrongAnswerPanel != null)
            {
                wrongAnswerPanel.SetActive(true);
                if (wrongAnswerFeedbackText != null)
                    wrongAnswerFeedbackText.text = q.wrongAnswerText;
            }
        }
    }

    public void DismissWrongAnswer()
    {
        if (wrongAnswerPanel != null) wrongAnswerPanel.SetActive(false);
    }

    void CompleteQuiz()
    {
        if (MainManager.mainManager != null)
        {
            MainManager.mainManager.quests.Remove(questId);
            if (!MainManager.mainManager.completedQuests.Contains(questId))
                MainManager.mainManager.completedQuests.Add(questId);
            MainManager.mainManager.onQuestsChanged?.Invoke();
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);

            if (completionMessageText != null)
                completionMessageText.text = completionMessage;

            if (creditsText != null)
                creditsText.text = credits;
        }
    }
}
