using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string displayName;
    [TextArea(2, 5)] public string activeDescription;
    [TextArea(2, 5)] public string completedDescription;
}