using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class MixingBowl : MonoBehaviour
{
    [Header("Mixing Bowl Settings")]
    [Tooltip("Prefab to instantiate after mixing is complete.")]
    public GameObject jellyPrefab;

    // Socket that will accept our ingredients
    private XRSocketInteractor socketInteractor;

    // Track which ingredients have been placed
    private bool hasGelatine = false;
    private bool hasFruit   = false;

    private void Awake()
    {
        // Grab the XRSocketInteractor on this GameObject
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        // Listen for any interactable being dropped into the socket
        socketInteractor.selectEntered.AddListener(OnIngredientPlaced);
        socketInteractor.selectExited .AddListener(OnIngredientRemoved);
    }

    private void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(OnIngredientPlaced);
        socketInteractor.selectExited .RemoveListener(OnIngredientRemoved);
    }

    /// <summary>
    /// Called when an XRGrabInteractable is placed into the socket.
    /// </summary>
    private void OnIngredientPlaced(SelectEnterEventArgs args)
    {
        GameObject placed = args.interactableObject.transform.gameObject;
        if (placed.CompareTag("Gelatine") && !hasGelatine)
        {
            hasGelatine = true;
            Debug.Log("Gelatine placed in bowl.");
        }
        else if (placed.CompareTag("Fruit") && !hasFruit)
        {
            hasFruit = true;
            Debug.Log("Fruit placed in bowl.");
        }

        // When both are in, start mixing
        if (hasGelatine && hasFruit)
            StartCoroutine(MixIngredients());
    }

    /// <summary>
    /// Called when an ingredient is removed from the socket.
    /// Resets the corresponding flag so you can re‑mix later.
    /// </summary>
    private void OnIngredientRemoved(SelectExitEventArgs args)
    {
        GameObject removed = args.interactableObject.transform.gameObject;
        if (removed.CompareTag("Gelatine"))
        {
            hasGelatine = false;
            Debug.Log("Gelatine removed from bowl.");
        }
        else if (removed.CompareTag("Fruit"))
        {
            hasFruit = false;
            Debug.Log("Fruit removed from bowl.");
        }
    }

    private IEnumerator MixIngredients()
    {
        Debug.Log("Mixing ingredients...");
        yield return new WaitForSeconds(2f);

        // Spawn the jelly at the bowl’s position
        Instantiate(jellyPrefab, transform.position, Quaternion.identity);
        Debug.Log("Jelly created!");

        // Reset for next mix
        hasGelatine = false;
        hasFruit   = false;
    }
}
