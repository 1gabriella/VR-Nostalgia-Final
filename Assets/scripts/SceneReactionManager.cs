using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneReactionManager : MonoBehaviour
{
    public static SceneReactionManager Instance;
    
    [Header("Scene Elements")]
    public Light sceneLight;
    public Image nostalgiaOverlay;
    public AudioSource backgroundMusic;

    [Header("Music Clips")]
    public AudioClip lowNostalgiaMusic;
    public AudioClip highNostalgiaMusic;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateScene(int nostalgiaLevel)
    {
        if (nostalgiaLevel >= 50)
        {
            sceneLight.color = Color.yellow; // Warm lighting for nostalgia
            nostalgiaOverlay.color = new Color(1, 1, 1, 0.2f); // Subtle vintage effect
            backgroundMusic.clip = highNostalgiaMusic;
        }
        else
        {
            sceneLight.color = Color.white;
            nostalgiaOverlay.color = new Color(1, 1, 1, 0f);
            backgroundMusic.clip = lowNostalgiaMusic;
        }

        backgroundMusic.Play();
    }
}

