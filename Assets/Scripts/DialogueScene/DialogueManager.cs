using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    
    public Animator animator;

    private Queue<string> sentences;

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

        Debug.Log("Dialogue has " + dialogue.sentences.Length + " sentences.");
        foreach (string sentence in dialogue.sentences)
        {
            Debug.Log("Adding sentence: " + sentence);
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        Debug.Log("Sentences remaining: " + sentences.Count);
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        Debug.Log("Displaying sentence: " + sentence);
        Debug.Log("Sentences left after dequeue: " + sentences.Count);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    void EndDialogue()
    {        Debug.Log("End of conversation.");
        dialogueText.text = "Meow";
        animator.SetBool("isOpen", false);
    }
}
