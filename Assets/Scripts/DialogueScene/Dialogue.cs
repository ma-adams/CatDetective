using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    [TextArea(3, 10)]
    public string[] sentences;

    // Quest actions 
    public string startQuestId; // starts a quest
    public string completeQuestId; // completes a quest
    public string requiredItem; // dialogue only plays if player has this item
    public string fallbackDialogue; //shown if requiredItem condition isn't met
}
