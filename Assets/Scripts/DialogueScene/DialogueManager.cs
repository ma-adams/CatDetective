using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public Animator animator;

    private Queue<string> sentences;
    private bool isTyping = false;
    private string currentSentence = "";
    private DialogueTrigger currentTrigger;
    private Dialogue currentDialogue;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger trigger = null)
    {
        currentTrigger = trigger;
        currentDialogue = dialogue;
        animator.SetBool("isOpen", true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void OnContinueButtonPressed()
    {
        if (isTyping)
        {
            // First click
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
        } else 
        {
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        currentSentence = sentence;
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }

    void EndDialogue() 
    {
    animator.SetBool("isOpen", false);

    if (currentTrigger == null || currentDialogue == null) return;
    if (MainManager.mainManager == null)
    {
        Debug.LogError("MainManager missing from scene!");
        return;
    }

    if (!string.IsNullOrEmpty(currentDialogue.startQuestId))
        MainManager.mainManager.AddQuest(currentDialogue.startQuestId);

    if (!string.IsNullOrEmpty(currentDialogue.completeQuestId))
    {
        MainManager.mainManager.quests.Remove(currentDialogue.completeQuestId);
        MainManager.mainManager.completedQuests.Add(currentDialogue.completeQuestId);
        if (!string.IsNullOrEmpty(currentTrigger.requiredItem))
            MainManager.mainManager.RemoveItem(currentTrigger.requiredItem);
    }

    currentTrigger = null;
    currentDialogue = null;
    }
}