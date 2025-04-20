using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EmotionDetectionAPI : MonoBehaviour
{
   /// <summary>
/// EmotionDetectionAPI connects to Hugging Face interface 
/// takes a given text - from the dialougeManger and return the dominant emotion label based on model
/// 
/// </summary>
 // URL of the  Hugging Face model fine-tuned for nostalgic emotion detection
    private string apiUrl = "https://api-inference.huggingface.co/models/1bbypluto/Nostalgic_finetuned";
 // Personal API key for authenticating requests 
    private string apiKey = " [REMOVED_HF_TOKEN]";

    /// <summary>
    /// Analyzes the provided text using Hugging Face and returns the dominant emotion.
    /// </summary>
   
    public void DetectEmotion(string text, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(text))
        {
            onError?.Invoke("Input text is empty.");
            return;
        }
        StartCoroutine(SendRequest(text, onSuccess, onError));
    }

    /// <summary>
    ///  Constructs and sends a POST request to Hugging Face for emotion detection
    /// handles success or error callbacks.
    /// </summary>
    private IEnumerator SendRequest(string text, Action<string> onSuccess, Action<string> onError)
    {
   // Create simple JSON callback { "inputs": "<text>" }
        string jsonData = "{\"inputs\":\"" + text + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
  // Attach JSON body of Post 
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
   // Set headers required by the API
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
 // Execute the request - wait for response WIFI is needed 
            yield return request.SendWebRequest();
  // guard the HTTP errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke("API Error: " + request.error);
            }
            else
            {
   // If its successful Log and store the prime emotion and phrase the JSON 
                string response = request.downloadHandler.text;
                Debug.Log("Emotion API Response: " + response);
                string dominantEmotion = ParseEmotionResponse(response);
                // Store detected emotion 
                EmotionStorage.StoreEmotion(dominantEmotion);
                onSuccess?.Invoke(dominantEmotion);
            }
        }
    }

    /// <summary>
    /// Parses the JSON response from Hugging Face and returns the label of the emotion with the highest score.
    /// </summary>
 
    private string ParseEmotionResponse(string jsonResponse)
    {
        try
        {
            // The API returns an array of objects. Wrap it in an object to use Unity's JsonUtility.
            string wrappedJson = "{\"emotions\":" + jsonResponse + "}";
            EmotionListWrapper wrapper = JsonUtility.FromJson<EmotionListWrapper>(wrappedJson);

            if (wrapper == null || wrapper.emotions == null || wrapper.emotions.Length == 0)
                return "Unknown";

            Emotion dominantEmotion = wrapper.emotions[0];
            for (int i = 1; i < wrapper.emotions.Length; i++)
            {
                if (wrapper.emotions[i].score > dominantEmotion.score)
                {
                    dominantEmotion = wrapper.emotions[i];
                }
            }
            return dominantEmotion.label;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing emotion response: " + ex.Message);
            return "Error";
        }
    }
}

[Serializable]
public class Emotion
{
    public string label;
    public float score;
}

[Serializable]
public class EmotionListWrapper
{
    public Emotion[] emotions;
}