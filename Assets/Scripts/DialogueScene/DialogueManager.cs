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
    private bool isOpen = false;
    private bool justOpened = false;
    public static bool IsOpen => _isOpen;
    private static bool _isOpen = false;
    private string currentSentence = "";
    private DialogueTrigger currentTrigger;
    private Dialogue currentDialogue;

    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        if (justOpened) { justOpened = false; return; }
        if (isOpen && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            OnContinueButtonPressed();
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger trigger = null)
    {
        currentTrigger = trigger;
        currentDialogue = dialogue;
        isOpen = true;
        _isOpen = true;
        justOpened = true;
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

    public void ForceClose()
    {
        if (!isOpen) return;
        StopAllCoroutines();
        isTyping = false;
        currentTrigger = null;
        currentDialogue = null;
        isOpen = false;
        _isOpen = false;
        animator.SetBool("isOpen", false);
    }

    void EndDialogue()
    {
    isOpen = false;
    _isOpen = false;
    animator.SetBool("isOpen", false);

    if (currentTrigger == null || currentDialogue == null) return;
    if (MainManager.mainManager == null)
    {
        Debug.LogError("MainManager missing from scene!");
        return;
    }

    if (!string.IsNullOrEmpty(currentDialogue.startQuestId))
        MainManager.mainManager.AddQuest(currentDialogue.startQuestId);

    if (currentDialogue.startQuestIds != null)
        foreach (string id in currentDialogue.startQuestIds)
            if (!string.IsNullOrEmpty(id))
                MainManager.mainManager.AddQuest(id);

    if (!string.IsNullOrEmpty(currentDialogue.completeQuestId))
    {
        MainManager.mainManager.quests.Remove(currentDialogue.completeQuestId);
        MainManager.mainManager.CompleteQuest(currentDialogue.completeQuestId);

        // Remove single item (existing quests)
        if (!string.IsNullOrEmpty(currentTrigger.requiredItem))
            MainManager.mainManager.RemoveItem(currentTrigger.requiredItem);

        // Remove multiple items (new multi-item quests)
        if (currentTrigger.requiredItems != null)
            foreach (string item in currentTrigger.requiredItems)
                MainManager.mainManager.RemoveItem(item);
    }

    currentTrigger = null;
    currentDialogue = null;
    }
}