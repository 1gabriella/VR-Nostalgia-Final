using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HFChatResponder : MonoBehaviour
{
    [Tooltip("e.g. facebook/blenderbot-400M-distill")]
    public string modelId = "facebook/blenderbot-400M-distill";

    [Tooltip("Your Hugging Face inference token")]
    public string apiKey  = "hf_YOUR_TOKEN_HERE";

    /// <summary>
    /// Send a single line to HF, invoke onReply when we get the bot’s reply.
    /// </summary>
    public void SendUserLine(string userLine, Action<string> onReply)
    {
        StartCoroutine(PostToHF(userLine, onReply));
    }

    private IEnumerator PostToHF(string input, Action<string> onReply)
    {
        // Build JSON: { "inputs": "<your text>" }
        var payload = "{\"inputs\":\"" +
                      UnityWebRequest.EscapeURL(input) +
                      "\"}";
        
        using var www = new UnityWebRequest(
            $"https://api-inference.huggingface.co/models/{modelId}", "POST");
        www.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type",  "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[HF] Error {www.responseCode}: {www.error}");
            yield break;
        }

        // HF returns JSON array of { generated_text }
        var raw = www.downloadHandler.text;
        HFArray arr = JsonUtility.FromJson<HFArray>("{\"arr\":" + raw + "}");
        if (arr?.arr == null || arr.arr.Length == 0)
        {
            Debug.LogWarning("[HF] empty reply");
            yield break;
        }

        onReply?.Invoke(arr.arr[0].generated_text.Trim());
    }

    [Serializable]
    private class HFArray { public HFItem[] arr; }
    [Serializable]
    private class HFItem  { public string generated_text; }
}


