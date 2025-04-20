using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to singular objects which represent a single part of dialogue in the conversation tree  
/// Allows for both linear and branching dialogue via choices.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Tooltip("The name of the character speaking.")]
    public string speaker;  // the character in the scene who will be speaking 

    [TextArea(3, 10)]
    
    public string dialogueText;  // The text content shown to the player

   
    public bool requiresTypedResponse;  // Enables free-text input instead of choices


    public string expectedResponseKeyword;  // Keyword - if the user has responded with any of those words 

  
    public List<DialogueChoice> choices;  // Branches for player to select


    public DialogueNode nextNode;  // Fallback node 
}

/// <summary>
///  Single dialogue option the player can choose leads to another Dialogue Node 
/// </summary>
[System.Serializable]
public class DialogueChoice
{
   
    public string choiceText;  

    
    public DialogueNode nextNode;  // Pointer to the DialogueNode to load next
}

