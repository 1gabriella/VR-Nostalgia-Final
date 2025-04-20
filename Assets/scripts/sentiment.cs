using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class SimpleSentimentAnalysis : MonoBehaviour
{
    [Tooltip("Reference to the XRKeyboard component.")]
    public XRKeyboard xrKeyboard;

    // Define a few positive and negative keywords.
    private readonly string[] positiveWords = { "hi", "hello", "great", "awesome", "fantastic", "good" };
    private readonly string[] negativeWords = { "bad", "terrible", "horrible", "awful", "sad", "hate" };

    void OnEnable()
    {
        if (xrKeyboard == null)
        {
            Debug.LogError("XRKeyboard reference is not set on SimpleSentimentAnalysis.");
            return;
        }
        // Subscribe to the text submitted event.
        xrKeyboard.onTextSubmitted.AddListener(HandleTextSubmitted);
    }

    void OnDisable()
    {
        if (xrKeyboard != null)
        {
            xrKeyboard.onTextSubmitted.RemoveListener(HandleTextSubmitted);
        }
    }

    private void HandleTextSubmitted(KeyboardTextEventArgs args)
    {
        string submittedText = args.keyboardText;
        string sentiment = AnalyzeSentiment(submittedText);
        Debug.Log("SimpleSentimentAnalysis: Detected sentiment is " + sentiment);
        // Optionally, you can add more responses or logic based on the sentiment result here.
    }

    // A simple keyword-based sentiment analysis method.
    private string AnalyzeSentiment(string inputText)
    {
        int score = 0;
        string lowerText = inputText.ToLower();

        foreach (string word in positiveWords)
        {
            if (lowerText.Contains(word))
            {
                score++;
            }
        }
        foreach (string word in negativeWords)
        {
            if (lowerText.Contains(word))
            {
                score--;
            }
        }

        if (score > 0)
            return "Positive";
        else if (score < 0)
            return "Negative";
        else
            return "Neutral";
    }
}
