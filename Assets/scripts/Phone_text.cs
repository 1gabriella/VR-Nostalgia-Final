using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class XRFlipPhoneChat : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI chatLog;

    [Header("XR Keyboard")]
    [Tooltip("Drag your XR SpatialKeyboard here and leave it active in the scene.")]
    [SerializeField] private XRKeyboard xrKeyboard;

    [Header("Hugging Face Settings")]
    [Tooltip("Try DialoGPT‑medium for a good nostalgic style.")]
    [SerializeField] private string modelId = "microsoft/DialoGPT-medium";
    [Tooltip("Your HF Inference API token.")]
    [SerializeField] private string apiKey  = "hf_YOUR_TOKEN_HERE";

    // we only keep the most recent few lines to avoid token overflow
    private List<string> history = new List<string>();

    private void OnEnable()
    {
        if (xrKeyboard != null)
            xrKeyboard.onTextSubmitted.AddListener(OnKeyboardSubmitted);
    }

    private void OnDisable()
    {
        if (xrKeyboard != null)
            xrKeyboard.onTextSubmitted.RemoveListener(OnKeyboardSubmitted);
    }

    private void Start()
    {
        // seed with a system kickoff message
        AppendLog("<i>System:</i> Let’s chat 2004‑style—flip‑phones, mix‑CDs & Bieber era! 📱💿");
    }

    private void OnKeyboardSubmitted(KeyboardTextEventArgs args)
    {
        string userLine = args.keyboardText?.Trim();
        if (string.IsNullOrEmpty(userLine))
            return;

        // 1) echo user
        AppendLog($"<b>You:</b> {userLine}");
        history.Add($"You: {userLine}");

        // 2) fire off HF request
        StartCoroutine(SendToHuggingFace());
    }

    private IEnumerator SendToHuggingFace()
    {
        // cap history to last 4 lines
        if (history.Count > 4)
            history = history.GetRange(history.Count - 4, 4);

        // build the flip‑phone prompt
        var sb = new StringBuilder();
        sb.AppendLine("You are a 2000’s‑era flip‑phone buddy. Reply in 1–2 short lines as “Friend: …”");
        sb.AppendLine("Use ‘omg’, ‘:P’, ‘<3’, refs to mix‑CDs, Razr phones, MySpace Top 8, early Bieber, etc.");
        sb.AppendLine();
        // a few handcrafted seed lines
        sb.AppendLine("Friend: omg remember making mix‑cds in my dorm? :P");
        sb.AppendLine("You: yesss—those midnight burns were 🔥");
        sb.AppendLine();
        sb.AppendLine("Friend: brb gotta charge my Razr ⚡");
        sb.AppendLine("You: lol flip‑phone life was wild <3");
        sb.AppendLine();
        // now append recent conversation
        foreach (var line in history)
            sb.AppendLine(line);
        sb.Append("Friend:");

        string prompt = sb.ToString();
        Debug.Log("[HF] Prompt:\n" + prompt);

        // escape newline/quotes for JSON
        string esc = prompt
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n");

        // build payload
        string json =
            "{" +
                "\"inputs\":\"" + esc + "\"," +
                "\"parameters\":{" +
                    "\"max_new_tokens\":100," +
                    "\"truncation\":\"only_first\"" +
                "}" +
            "}";

        Debug.Log("[HF] Payload:\n" + json);

        using var www = new UnityWebRequest(
            $"https://api-inference.huggingface.co/models/{modelId}", "POST");
        www.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type",  "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return www.SendWebRequest();

        Debug.Log($"[HF] HTTP {www.responseCode}");
        Debug.Log("[HF] Body:\n" + www.downloadHandler.text);

        if (www.result != UnityWebRequest.Result.Success)
        {
            AppendLog($"<color=red>System: Error {www.responseCode}</color>");
        }
        else
        {
            var raw = www.downloadHandler.text;
            HFArray wrapper = null;
            try
            {
                wrapper = JsonUtility.FromJson<HFArray>("{\"arr\":" + raw + "}");
            }
            catch (Exception ex)
            {
                Debug.LogError("[HF] JSON parse error: " + ex);
            }

            if (wrapper?.arr == null || wrapper.arr.Length == 0)
            {
                AppendLog("<color=red>System: No reply</color>");
            }
            else
            {
                // append each non‑empty line from the model’s output
                var reply = wrapper.arr[0].generated_text;
                foreach (var line in reply.Split(
                    new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var t = line.Trim();
                    AppendLog($"<b>Friend:</b> {t}");
                    history.Add($"Friend: {t}");
                }
            }
        }
    }

    private void AppendLog(string line)
    {
        chatLog.text += "\n" + line;
    }

    [Serializable]
    private class HFArray { public HFItem[] arr; }
    [Serializable]
    private class HFItem  { public string generated_text; }
}
