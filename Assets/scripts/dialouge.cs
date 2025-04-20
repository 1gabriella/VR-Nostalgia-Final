using UnityEngine;
using TMPro;
using UnityEngine.UI;  // for using UI elements 
using System.Collections;

/// <summary>
/// NPCChatWithChatbot handles a short  dialogue sequence using TextMeshPro,

/// </summary>
public class NPCChatWithChatbotTMP : MonoBehaviour {
    // Indicates whether the NPC dialogue has already started 
    private bool dialogueStarted = false;

    // UI panel that shows the  dialogue lines
    public GameObject dialoguePanel;
    // Text component to display each line of dialogue
    public TMP_Text dialogueText;
    // Array of dialogue lines - these are added via the inspector 
    public string[] dialogueLines;

    // UI panel for the chatbot 
    public GameObject chatbotPanel;
    // Input field for the user to put in text 
    public TMP_InputField chatbotInputField;
    // Text component to show  responses - next node 
    public TMP_Text chatbotResponseText;

    // Button that players click to submit chat messages
    public Button sendButton;

    // Flag to indicate that dialogue finished 
    private bool dialogueFinished = false;

    void Start() {
        // Hide both UI panels at the start so they only appear when triggered
        dialoguePanel.SetActive(false);
        chatbotPanel.SetActive(false);

        // a second button  callback (for the keyboard) 
        if (sendButton != null) {
            sendButton.onClick.AddListener(SubmitChatMessage);
        }
    }

    // Called when another collider enters this object's trigger zone
    void OnTriggerEnter(Collider other) {
        // Only react if the player walks into the trigger area
        if (other.CompareTag("Player")) {
            Debug.Log("Player entered trigger range.");
            // Start the dialogue 
            if (!dialogueStarted) {
                dialogueStarted = true;
                StartCoroutine(PlayDialogue());
            }
        }
    }

  

    /// <summary>
    /// Plays through the dialogue lines one by one, with delay for typewriter effect
    /// </summary>
    IEnumerator PlayDialogue() {
        dialoguePanel.SetActive(true);  // Show the dialogue UI
        for (int i = 0; i < dialogueLines.Length; i++) {
            dialogueText.text = dialogueLines[i];
            Debug.Log("Dialogue: " + dialogueText.text);
            // delay - 
            yield return new WaitForSeconds(2f);
        }
        // After the last line, hide the dialogue and mark it as done
        dialoguePanel.SetActive(false);
        dialogueFinished = true;
        OpenChatbot();  //  chatbot interaction
    }

    /// <summary>
    /// Activates the chatbot panel and unlocks the cursor for typing.
    /// </summary>
    void OpenChatbot() {
        chatbotPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;   // mouse movement
        Cursor.visible = true;                    // Show the cursor
        chatbotInputField.ActivateInputField();   // Focus typing cursor
        Debug.Log("Chatbot panel activated and input field focused");
    }

    /// <summary>
    /// Called by the send button to process the player's chat message.
    /// </summary>
    public void SubmitChatMessage() {
        string userMessage = chatbotInputField.text;
        // Ignore empty messages to prevent spam
        if (string.IsNullOrEmpty(userMessage))
            return;

        // Display a simple response based on keywords
        chatbotResponseText.text = GenerateChatResponse(userMessage);
        chatbotInputField.text = string.Empty;      // Clear input field
        chatbotInputField.ActivateInputField();     // Refocus for next message
        Debug.Log("Chat message submitted: " + userMessage);
    }

    /// <summary>
    /// Generates a basic chatbot reply using keyword matching.
    /// This is replaced by text in the inspector - but fallback 
    /// </summary>
    string GenerateChatResponse(string input) {
        input = input.ToLower();
        if (input.Contains("what"))
            return "See what's behind the door <3";
        else if (input.Contains("ok"))
            return "Bye!";
        else
            return "Umm...";  // Fallback response for unrecognized input
    }
}


