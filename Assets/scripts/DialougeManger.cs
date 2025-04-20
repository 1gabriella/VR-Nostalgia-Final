using System;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class DialogueManager : MonoBehaviour
{
    [Header("XR Keyboard")]
    [Tooltip("Drag in the XRGrabInteractable on your Spatial Keyboard prefab.")]
    [SerializeField] XRGrabInteractable keyboardGrabInteractable;

    [Tooltip("Drag in the XRKeyboardDisplay on your Spatial Keyboard prefab.")]
    [SerializeField] XRKeyboardDisplay keyboardDisplay;

    [Header("AI Chat (Hugging Face)")]
    [SerializeField] HFChatResponder hfResponder;

    [Header("Emotion Detection")]
    [SerializeField] EmotionDetectionAPI emotionDetector;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] TextMeshProUGUI dialogueText;

    private void OnEnable()
    {
        if (keyboardDisplay != null)
            keyboardDisplay.onTextSubmitted.AddListener(OnKeyboardTextSubmitted);
    }

    private void OnDisable()
    {
        if (keyboardDisplay != null)
            keyboardDisplay.onTextSubmitted.RemoveListener(OnKeyboardTextSubmitted);
    }

    private void Start()
    {
        // Initial prompt
        speakerText.text  = "Friend";
        dialogueText.text = "hii—don't you miss the Bieber swag era? :P";

        // Ensure the keyboard can be grabbed/used
        if (keyboardGrabInteractable != null)
            keyboardGrabInteractable.enabled = true;
    }

    private void OnKeyboardTextSubmitted(string submittedText)
    {
        var userLine = submittedText?.Trim();
        if (string.IsNullOrEmpty(userLine))
            return;

        // 1) Echo the user
        speakerText.text  = "You";
        dialogueText.text = userLine;

        // 2) Clear the input field
        if (keyboardDisplay?.inputField != null)
            keyboardDisplay.inputField.text = "";

        // 3) Disable the keyboard until AI replies
        if (keyboardGrabInteractable != null)
            keyboardGrabInteractable.enabled = false;

        // 4) (Optional) Emotion detection
        emotionDetector?.DetectEmotion(
            userLine,
            emo => Debug.Log("[Emotion] " + emo),
            err => Debug.LogError("[Emotion] " + err)
        );

        // 5) Show “typing…” indicator
        speakerText.text  = "Friend";
        dialogueText.text = "…typing…";

        // 6) Send to HF with two‑arg overload and catch exceptions
        if (hfResponder != null)
        {
            try
            {
                hfResponder.SendUserLine(
                    userLine,
                    friendReply =>
                    {
                        speakerText.text  = "Friend";
                        dialogueText.text = friendReply;
                        if (keyboardGrabInteractable != null)
                            keyboardGrabInteractable.enabled = true;
                    }
                );
            }
            catch (Exception e)
            {
                speakerText.text  = "System";
                dialogueText.text = $"[AI error] {e.Message}";
                if (keyboardGrabInteractable != null)
                    keyboardGrabInteractable.enabled = true;
            }
        }
        else
        {
            // No responder attached
            speakerText.text  = "System";
            dialogueText.text = "[No HF responder attached]";
            if (keyboardGrabInteractable != null)
                keyboardGrabInteractable.enabled = true;
        }
    }
}


