using UnityEngine;
using UnityEngine.UI;

public class CameraSmearPlusHueEffect : MonoBehaviour
{
    [Header("Overlay Settings")]
    public Color red = new Color(1f, 0f, 0f, 1f);
    public Color green = new Color(0f, 1f, 0f, 1f);
    [Range(0f, 1f)] public float overlayOpacity = 0.5f;
    public float fadeDuration = 1f;

    [Header("External References")]
    public Transform cameraTransform; // Drag player's camera

    private RawImage vignetteImage;
    private Canvas overlayCanvas;
    private bool effectActive = false;
    private float fadeTimer = 0f;
    private bool fadingIn = true;
    private Color targetColor;

    private Material vignetteMaterial;

    void Awake()
    {
        CreateOverlayUI();
    }

    private void CreateOverlayUI()
    {
        // Create canvas
        overlayCanvas = new GameObject("SanityVignetteCanvas").AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        DontDestroyOnLoad(overlayCanvas.gameObject);

        // Create vignette image
        GameObject imageObj = new GameObject("VignetteImage");
        imageObj.transform.SetParent(overlayCanvas.transform, false);
        vignetteImage = imageObj.AddComponent<RawImage>();
        vignetteImage.rectTransform.anchorMin = Vector2.zero;
        vignetteImage.rectTransform.anchorMax = Vector2.one;
        vignetteImage.rectTransform.offsetMin = Vector2.zero;
        vignetteImage.rectTransform.offsetMax = Vector2.zero;

        // Create and assign vignette material
        vignetteMaterial = new Material(Shader.Find("Hidden/VignetteSanity"));
        vignetteImage.material = vignetteMaterial;

        overlayCanvas.enabled = false;
    }

    public void TriggerEffect()
    {
        targetColor = Random.value > 0.5f ? red : green;
        vignetteMaterial.SetColor("_VignetteColor", new Color(targetColor.r, targetColor.g, targetColor.b, 0f));

        fadeTimer = 0f;
        fadingIn = true;
        overlayCanvas.enabled = true;
        effectActive = true;
    }

    private void Update()
    {
        if (!effectActive) return;

        fadeTimer += Time.deltaTime;
        float t = fadeTimer / fadeDuration;

        float alpha = fadingIn
            ? Mathf.Lerp(0f, overlayOpacity, t)
            : Mathf.Lerp(overlayOpacity, 0f, t);

        vignetteMaterial.SetColor("_VignetteColor", new Color(targetColor.r, targetColor.g, targetColor.b, alpha));

        if (t >= 1f)
        {
            if (fadingIn)
            {
                fadeTimer = 0f;
                fadingIn = false;
            }
            else
            {
                overlayCanvas.enabled = false;
                effectActive = false;
            }
        }
    }
}
