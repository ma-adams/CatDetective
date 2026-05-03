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

    void Start()
    {
        sentences = new Queue<string>();
        Debug.Log("DialogueManager initialized.");
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation...");
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
        Debug.Log("End of conversation.");
        dialogueText.text = "Meow";
        animator.SetBool("isOpen", false);
    }
}