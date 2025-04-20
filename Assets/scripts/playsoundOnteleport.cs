using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(TeleportationAnchor), typeof(AudioSource))]
public class PlaySoundOnTeleport : MonoBehaviour
{
    private TeleportationAnchor teleportAnchor;
    private AudioSource audioSource;

    private void Awake()
    {
        teleportAnchor = GetComponent<TeleportationAnchor>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Subscribe to the 'teleporting' event
        teleportAnchor.teleporting.AddListener(OnTeleport);
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled or destroyed
        teleportAnchor.teleporting.RemoveListener(OnTeleport);
    }

    private void OnTeleport(TeleportingEventArgs args)
    {
        // This method is invoked automatically when the user teleports to this anchor
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
