using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; 

[DisallowMultipleComponent]
public class Outline : MonoBehaviour {
    
    // --- Public Properties (Color and Width) ---
    
    // We will simplify to always use the OutlineVisible mode for the effect.
    public Color OutlineColor {
        get { return outlineColor; }
        set {
            outlineColor = value;
            needsUpdate = true;
        }
    }

    public float OutlineWidth {
        get { return outlineWidth; }
        set {
            outlineWidth = value;
            needsUpdate = true;
        }
    }

    public TextMeshPro interactionText; // Keep for optional text display

    [SerializeField]
    private Color outlineColor = Color.white;
    
    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 2f;
  
    [SerializeField] 
    public string interactMessage = "Press E to Interact";
    public float textVerticalOffset = 1f;
    private bool time_machine = false;
    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private bool needsUpdate;
    
    
    public bool enableByDistance = true;
    public float maxVisibleDistance = 5f;
    private Camera mainCamera;
    private bool isVisible = false;


    void Awake() {
        
        mainCamera = Camera.main;

        SetupInteractionText();
        
        if (interactionText != null) {
            interactionText.gameObject.SetActive(false);
        }

        // Cache renderers (Needed to apply/remove materials)
        renderers = GetComponentsInChildren<Renderer>();

        // Instantiate outline materials (Needed for the effect)
        // NOTE: The Materials/OutlineMask and Materials/OutlineFill files must exist in a Resources folder
        outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";
        
        if (interactionText != null) {
            interactionText.gameObject.SetActive(false);
        }

        // Force material update with current settings
        needsUpdate = true;
    }
    
    private void SetupInteractionText()
    {
        // Create the child GameObject
        GameObject textObject = new GameObject("Interaction Text");
        textObject.transform.SetParent(this.transform);

        // Position it above the object based on the Inspector setting
        textObject.transform.localPosition = new Vector3(0f, textVerticalOffset, 0f);
        
        // Add TextMeshPro
        interactionText = textObject.AddComponent<TextMeshPro>();
        
        // Configure text properties
        interactionText.text = interactMessage;
        interactionText.alignment = TextAlignmentOptions.Center;
        interactionText.fontSize = 0.5f; 
        interactionText.rectTransform.sizeDelta = new Vector2(3f, 0.5f);

        // Hide the text initially
        interactionText.gameObject.SetActive(false);
    }

    void OnEnable() {
        // Start with outline hidden, the Update() loop will make it visible if conditions are met.
        SetOutlineVisibility(false); 
    }

    void Update() {
        
        if (interactionText != null && mainCamera != null)
        {
            // Only rotate if the text is currently visible to save performance
            if (interactionText.gameObject.activeSelf)
            {
                interactionText.transform.rotation = mainCamera.transform.rotation;
            }
        }
        
        
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        bool shouldShowOutline = false;

        Vector3 toObject = transform.position - mainCamera.transform.position;
        float distance = toObject.magnitude;

        // Distance and Line-of-Sight Check
        if (enableByDistance && distance <= maxVisibleDistance) {
            Ray ray = new Ray(mainCamera.transform.position, toObject.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, distance)) {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) {
                    shouldShowOutline = true;
                    // this if statement is entered iff the object this script is attached
                    // to is the time machine
                    if (time_machine)
                    {
                        
                    }
                }
            }
        }
        
        
        // Update outline visibility if the state changed
        if (shouldShowOutline != isVisible) {
            isVisible = shouldShowOutline;
            SetOutlineVisibility(isVisible);

            if (interactionText != null) {
                interactionText.gameObject.SetActive(isVisible);
            }
        }

        // Update material properties (color/width) only when needed
        if (needsUpdate) {
            needsUpdate = false;
            UpdateMaterialProperties();
        }
    }

    void OnDisable() {
        // Clean up when disabled
        SetOutlineVisibility(false);
    }
    
    void OnDestroy() {
        if (interactionText != null)
        {
            Destroy(interactionText.gameObject);
        }
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }


    // Applies or removes the outline materials
    private void SetOutlineVisibility(bool visible) {
        foreach (var renderer in renderers) {
            var materials = renderer.sharedMaterials.ToList();

            if (visible) {
                // Add the two outline materials if they aren't already there
                if (!materials.Contains(outlineMaskMaterial)) materials.Add(outlineMaskMaterial);
                if (!materials.Contains(outlineFillMaterial)) materials.Add(outlineFillMaterial);
            } else {
                // Remove the two outline materials
                materials.Remove(outlineMaskMaterial);
                materials.Remove(outlineFillMaterial);
            }

            renderer.materials = materials.ToArray();
        }
    }


    // Sets the color and width on the material shaders
    void UpdateMaterialProperties() {

        outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
        
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        
        // Outline Mask (Always render to stencil buffer)
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always); 
        
        // Outline Fill (Only render where the mask passed, and where the depth is LessEqual)
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual); 
    }
}