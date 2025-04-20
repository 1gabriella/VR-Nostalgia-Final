using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using HuggingFace.API;  // for AutomaticSpeechRecognition

/// <summary>
/// SpeechRecognitionTest manages a simple flow:
/// • Record via the mic when you click “Start”
/// • Stop & encode to WAV when you click “Stop”
/// • Send to Hugging Face ASR, then run emotion detection
/// • Show status in a TextMeshProUGUI
/// </summary>
public class SpeechRecognitionTest : MonoBehaviour
{
    [Header("UI Buttons")]
    [Tooltip("Click to begin recording from the mic")]
    [SerializeField] private Button startButton;

    [Tooltip("Click to end recording and process audio")]
    [SerializeField] private Button stopButton;

    [Header("UI Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    // Internal recording state
    private AudioClip clip;     // Raw mic data
    private byte[]   wavData;   // WAV-encoded bytes
    private bool     recording; // Are we currently recording?

    // Emotion detection helper
    private EmotionDetectionAPI emotionAPI;

    private void Start()
    {
        // 1) Ensure we have an EmotionDetectionAPI on this GameObject
        emotionAPI = gameObject.AddComponent<EmotionDetectionAPI>();

        // 2) Wire up UI buttons
        startButton.onClick.AddListener(OnStartClicked);
        stopButton.onClick  .AddListener(OnStopClicked);

        // 3) Initialize UI state
        startButton.interactable = true;
        stopButton.interactable  = false;
        UpdateStatus("Ready to record.", Color.white);
    }

    /// <summary>
    /// User clicked “Start” → begin mic capture
    /// </summary>
    private void OnStartClicked()
    {
        // Guard against double‑click
        if (recording) return;

        // Start the microphone (10 s max, 44.1 kHz)
        clip      = Microphone.Start(null, false, 10, 44100);
        recording = true;

        // Toggle buttons
        startButton.interactable = false;
        stopButton.interactable  = true;

        UpdateStatus("Recording speech…", Color.white);
    }

    /// <summary>
    /// User clicked “Stop” → end capture, encode, send, then reset UI
    /// </summary>
    private void OnStopClicked()
    {
        if (!recording) return;

        // 1) Stop mic & grab samples
        int pos = Microphone.GetPosition(null);
        Microphone.End(null);
        recording = false;

        float[] samples = new float[pos * clip.channels];
        clip.GetData(samples, 0);

        // 2) Encode to WAV
        wavData = EncodeAsWAV(samples, clip.frequency, clip.channels);

        // 3) Toggle buttons back
        startButton.interactable = true;
        stopButton.interactable  = false;

        // 4) Kick off the ASR → emotion pipeline
        StartCoroutine(ProcessRecording());
    }

    /// <summary>
    /// Coroutine: call ASR, then emotion detection, updating UI at each step.
    /// </summary>
    private IEnumerator ProcessRecording()
    {
        UpdateStatus("Processing speech…", Color.yellow);

        // Wrap UnityWebRequest in IEnumerator for ASR:
        bool asrDone = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(
            wavData,
            transcription =>
            {
                UpdateStatus("Transcription: " + transcription, Color.white);

                // Now emotion detection:
                emotionAPI.DetectEmotion(
                    transcription,
                    emot => UpdateStatus("Emotion: " + emot, Color.green),
                    err  => UpdateStatus("Emotion error: " + err, Color.red)
                );

                asrDone = true;
            },
            err =>
            {
                UpdateStatus("ASR Error: " + err, Color.red);
                asrDone = true;
            }
        );

        // Wait until the ASR callback sets asrDone = true
        while (!asrDone)
            yield return null;
    }

    /// <summary>
    /// Update the on‑screen status text.
    /// </summary>
    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text  = message;
            statusText.color = color;
        }
        else
        {
            Debug.LogWarning("No statusText assigned!");
        }
    }

    /// <summary>
    /// Helper: encode float samples into a RIFF/PCM16 WAV byte array.
    /// </summary>
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var ms     = new MemoryStream(44 + samples.Length * 2))
        using (var writer = new BinaryWriter(ms))
        {
            // RIFF header
            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + samples.Length * 2);
            writer.Write("WAVE".ToCharArray());

            // fmt subchunk
            writer.Write("fmt ".ToCharArray());
            writer.Write(16);
            writer.Write((ushort)1);            // PCM
            writer.Write((ushort)channels);
            writer.Write(frequency);
            writer.Write(frequency * channels * 2);
            writer.Write((ushort)(channels * 2));
            writer.Write((ushort)16);

            // data subchunk
            writer.Write("data".ToCharArray());
            writer.Write(samples.Length * 2);

            // PCM16 samples
            foreach (var s in samples)
                writer.Write((short)(s * short.MaxValue));

            return ms.ToArray();
        }
    }
}

