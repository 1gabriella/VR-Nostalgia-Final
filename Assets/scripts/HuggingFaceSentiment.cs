using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
// Ensure Newtonsoft.Json is added to the Unity project (via package or DLL)
using Newtonsoft.Json;

/// <summary>
/// HuggingFaceSentiment provides a coroutine-based interface to
/// send text data to a remote Hugging Face sentiment analysis model
/// and receive the most dominant lanel 
/// </summary>
public class HuggingFaceSentiment : MonoBehaviour
{
    // Inference endpoint for the DistilBERT SST-2 sentiment classifier
    private const string apiURL = "https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english";

    // API key for authenticating requests (consider securing in production)
    [Tooltip("Hugging Face API token for authenticated inference requests.")]
    public string apiKey = " [REMOVED_HF_TOKEN]";

    /// <summary>
    /// Coroutine that sends a text payload to Hugging Face and
    /// invokes resultCallback with the returned sentiment label.
    /// </summary>
    /// <param name="text">The input sentence or phrase to classify.</param>
    /// <param name="resultCallback">Action to receive the label: "positive", "negative", or "error".</param>
    public IEnumerator GetSentiment(string text, Action<string> resultCallback)
    {
        // 1. Prepare the request payload by wrapping text in a serializable class
        HuggingFaceInput input = new HuggingFaceInput { inputs = text };
        string jsonData = JsonUtility.ToJson(input);  // Unity's built-in JSON serializer
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);  // Convert to bytes for HTTP body

        // 2. Configure the HTTP POST request
        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            // Attach raw JSON body
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Set HTTP headers for JSON content and authentication
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // 3. Send the request and wait asynchronously
            yield return request.SendWebRequest();

            // 4. Handle response success vs. failure
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Extract raw JSON response (nested array of label/score objects)
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Hugging Face Response: " + jsonResponse);

                // 5. Parse the JSON to determine the top label
                string sentiment = ParseSentiment(jsonResponse);
                resultCallback?.Invoke(sentiment);
            }
            else
            {
                // Log the error for debugging and propagate an "error" label
                Debug.LogError($"Hugging Face API error: {request.error}");
                resultCallback?.Invoke("error");
            }
        }
    }

    /// <summary>
    /// Parses the nested JSON array returned by Hugging Face into
    /// List<List<LabelScore>> and returns the label with highest score.
    /// Falls back to "neutral" if parsing fails or no data.
    /// </summary>
    /// <param name="json">Raw JSON text from the API.</param>
    /// <returns>Lowercase label string for the dominant sentiment.</returns>
    private string ParseSentiment(string json)
    {
        try
        {
            // Deserialize into list-of-lists of LabelScore objects
            var results = JsonConvert.DeserializeObject<List<List<LabelScore>>>(json);

            // Guard clauses: ensure we have at least one list and one score
            if (results != null && results.Count > 0 && results[0].Count > 0)
            {
                // Start by assuming the first element is best
                LabelScore best = results[0][0];
                // Iterate to find any score higher than the current best
                foreach (var score in results[0])
                {
                    if (score.score > best.score)
                        best = score;
                }
                // Return the label in lowercase for consistency
                return best.label.ToLower();
            }
        }
        catch (Exception e)
        {
            // Catch potential JSON errors and log for debugging
            Debug.LogError("Error parsing sentiment JSON: " + e.Message);
        }
        // Default fallback sentiment
        return "neutral";
    }
}

/// <summary>
/// Serializable input class for Hugging Face inference. Maps to payload { "inputs": "..." }.
/// </summary>
[Serializable]
public class HuggingFaceInput
{
    public string inputs;  // The text to classify
}

/// <summary>
/// Data structure mapping a single label and its confidence score.
/// Mirrors the JSON objects returned by the Hugging Face API.
/// </summary>
[Serializable]
public class LabelScore
{
    public string label;  // e.g., "POSITIVE"
    public float score;   // Confidence score between 0.0 and 1.0
}

