using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable), typeof(AudioSource))]
public class PlaySoundOnHover : MonoBehaviour
{
    private XRSimpleInteractable m_Interactable;
    private AudioSource m_AudioSource;

    void Awake()
    {
        // Cache references
        m_Interactable = GetComponent<XRSimpleInteractable>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // Subscribe to hover events
        m_Interactable.hoverEntered.AddListener(OnHoverEntered);
        m_Interactable.hoverExited.AddListener(OnHoverExited);
    }

    void OnDisable()
    {
        // Unsubscribe from hover events
        m_Interactable.hoverEntered.RemoveListener(OnHoverEntered);
        m_Interactable.hoverExited.RemoveListener(OnHoverExited);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        // Play sound when user hovers on the button
        if (m_AudioSource != null && !m_AudioSource.isPlaying)
        {
            m_AudioSource.Play();
        }
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        // Stop or fade out sound, or do nothing
        if (m_AudioSource != null && m_AudioSource.isPlaying)
        {
            // Option 1: Stop immediately
            m_AudioSource.Stop();

            // Option 2: Or fade out, if you have a fade logic
            // StartCoroutine(FadeOutSound(m_AudioSource, fadeDuration));
        }
    }
}
