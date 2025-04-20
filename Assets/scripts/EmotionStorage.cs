using System.IO;
using UnityEngine;

/// <summary>
/// Static utility for storing detected emotion labels to disk.
/// </summary>
public static class EmotionStorage
{
  
    private static readonly string path = Path.Combine(Application.persistentDataPath, "EmotionLog.txt");

    /// <summary>
    /// Saves a  new emotion entry to the log file with a timestamp.
    /// Skips saving if the provided emotion string is null or empty.
    /// </summary>

    public static void StoreEmotion(string emotion)
    {
        // No empty entries 
        if (string.IsNullOrEmpty(emotion))
            return;

        // Format: YYYY-MM-DD HH:mm:ss - Emotion: <label>
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp} - Emotion: {emotion}\n";

        // Append the entry to the log file 
        File.AppendAllText(path, logEntry);

        // Debug output to confirm storage during development
        Debug.Log("Emotion stored: " + logEntry);
    }
}
