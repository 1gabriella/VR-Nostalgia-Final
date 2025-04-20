using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class DoorSceneSwitch : MonoBehaviour
{
    [Tooltip("Scene index to load")]
    public int sceneNumber;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        // Subscribe to the select event (triggered when the door is “pressed” or selected)
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
