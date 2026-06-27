using System.Collections.Generic;

[System.Serializable]
public class JSONChoice
{
    public string choiceText;
    public string nextNodeID;
    public string eventTriggerName;
}

[System.Serializable]
public class JSONDialogueNode
{
    public string nodeID;
    public string speakerName;
    public string dialogueText;
    public List<JSONChoice> choices;
}

[System.Serializable]
public class JSONDialogueData
{
    public string startNodeID;
    public List<JSONDialogueNode> nodes;
}