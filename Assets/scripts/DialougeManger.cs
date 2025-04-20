using System;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class DialogueManager : MonoBehaviour
{
    [Header("XR Keyboard")]
    [Tooltip("Drag in your XRKeyboard here.")]
    [SerializeField] private XRKeyboard xrKeyboard;

    [Header("AI Chat (Hugging Face)")]
    [Tooltip("Component that actually calls HF and returns you a string.")]
    [SerializeField] private HFChatResponder hfResponder;

    [Header("Emotion Detection")]
    [Tooltip("Component that calls your text→emotion API.")]
    [SerializeField] private EmotionDetectionAPI emotionDetector;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void OnEnable()
    {
        if (xrKeyboard != null)
            xrKeyboard.onTextSubmitted.AddListener(OnKeyboardTextSubmitted);
    }

    private void OnDisable()
    {
        if (xrKeyboard != null)
            xrKeyboard.onTextSubmitted.RemoveListener(OnKeyboardTextSubmitted);
    }

    private void Start()
    {
        // Kick things off with a friendly opener
        speakerText.text  = "Friend";
        dialogueText.text = "hii—don't you miss the Bieber swag era? :P";

        // Make sure keyboard is enabled
        if (xrKeyboard != null)
            xrKeyboard.interactable = true;
    }

    private void OnKeyboardTextSubmitted(KeyboardTextEventArgs args)
    {
        string userLine = args.keyboardText?.Trim();
        if (string.IsNullOrEmpty(userLine))
            return;

        // 1) Echo the user line:
        speakerText.text  = "You";
        dialogueText.text = userLine;

        // 2) Clear & disable the keyboard until AI replies:
        xrKeyboard.text         = "";      // reset the field
        xrKeyboard.interactable = false;

        // 3) Fire off emotion detection in parallel (optional):
        if (emotionDetector != null)
        {
            emotionDetector.DetectEmotion(
                userLine,
                onSuccess: emo => Debug.Log("[Emotion] " + emo),
                onError:   err => Debug.LogError("[Emotion] " + err)
            );
        }

        // 4) Show a “typing” indicator while HF works:
        speakerText.text  = "Friend";
        dialogueText.text = "…typing…";

        // 5) Send to HF, and when it replies, show it & re‑enable keyboard:
        if (hfResponder != null)
        {
            hfResponder.SendUserLine(
                userLine,
                onSuccess: friendReply =>
                {
                    speakerText.text  = "Friend";
                    dialogueText.text = friendReply;
                    xrKeyboard.interactable = true;
                },
                onError: hfError =>
                {
                    speakerText.text  = "System";
                    dialogueText.text = $"[AI error] {hfError}";
                    xrKeyboard.interactable = true;
                }
            );
        }
        else
        {
            // Fallback if no HF component attached:
            speakerText.text  = "System";
            dialogueText.text = "[No HF responder attached]";
            xrKeyboard.interactable = true;
        }
    }
}

