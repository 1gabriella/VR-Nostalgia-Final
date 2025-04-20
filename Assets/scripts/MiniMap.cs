using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The overhead orthographic camera rendering to the minimap RenderTexture.")]
    public Camera minimapCamera;
    [Tooltip("Player transform to track on the minimap.")]
    public Transform player;
    [Tooltip("RawImage that displays the minimap RenderTexture.")]
    public RawImage mapImage;
    [Tooltip("UI Image used as the player icon/dot on the minimap.")]
    public RectTransform playerIcon;

    [Header("World Bounds")]
    [Tooltip("Size of the world area (X,Z) that the minimap covers.")]
    public Vector2 worldSize = new Vector2(100, 100);

    [Header("UI Toggle")]
    [Tooltip("UI Button that toggles the minimap on/off.")]
    public Button toggleMapButton;

    private Canvas mapCanvas;
    private bool   mapVisible = false;

    void Awake()
    {
        mapCanvas = GetComponent<Canvas>();
        mapCanvas.enabled = false;  // start hidden

        // Subscribe to the UI button's click event
        if (toggleMapButton != null)
            toggleMapButton.onClick.AddListener(ToggleMap);
        else
            Debug.LogWarning("ToggleMapButton not assigned!");
    }

    void OnDestroy()
    {
        // Clean up listener
        if (toggleMapButton != null)
            toggleMapButton.onClick.RemoveListener(ToggleMap);
    }

    void Update()
    {
        if (!mapVisible) return;

        // Map the player's world position into [0,1] UV space
        float normX = (player.position.x / worldSize.x) + 0.5f;
        float normY = (player.position.z / worldSize.y) + 0.5f;

        normX = Mathf.Clamp01(normX);
        normY = Mathf.Clamp01(normY);

        // Place the icon within the mapImage's rect
        RectTransform rt = mapImage.rectTransform;
        Vector2 localPos = new Vector2(
            (normX - 0.5f) * rt.sizeDelta.x,
            (normY - 0.5f) * rt.sizeDelta.y
        );
        playerIcon.anchoredPosition = localPos;
    }

    /// <summary>
    /// Show or hide the minimap Canvas when the button is clicked.
    /// </summary>
    public void ToggleMap()
    {
        mapVisible = !mapVisible;
        mapCanvas.enabled = mapVisible;
    }
}

