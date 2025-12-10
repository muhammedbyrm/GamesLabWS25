using UnityEngine;
using UnityEngine.UIElements;

public class MicroscopeImageSelector : MonoBehaviour
{
    public UIDocument uiDocument;

    public Texture2D[] blurryImages;
    public Texture2D[] sharpImages;

    private int currentIndex = 0;

    private VisualElement lens;
    private Label imageLabel;
    private Label zoomLabel;
    private VisualElement zoomFill;

    private float zoomLevel = 0f;
    private float zoomSpeed = 0.05f;

    private VisualElement root;
    private bool uiVisible = true;

    public GameObject canvasElement;
    public GameObject crosshair;

    void OnEnable()
    {
        StartCoroutine(InitializeAfterEnable());
    }

    private System.Collections.IEnumerator InitializeAfterEnable()
    {
        while (uiDocument == null || uiDocument.rootVisualElement == null)
            yield return null;

        root = uiDocument.rootVisualElement;

        lens = root.Q<VisualElement>("lens");
        imageLabel = root.Q<Label>("imageLabel");
        zoomLabel = root.Q<Label>("zoomLabel");
        zoomFill = root.Q<VisualElement>("zoomFill");

        if (lens == null)
            yield break;

        zoomLevel = 0f;
        LoadImage(currentIndex);
        UpdateLensAppearance();
        UpdateHUD();
    }

    void Update()
    {
        if (!uiVisible)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ToggleUI();
            return;
        }

        HandleZoom();
        HandleSwitchImages();
        UpdateHUD();

        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleUI();
    }

    void HandleSwitchImages()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentIndex++;
            if (currentIndex >= sharpImages.Length)
                currentIndex = 0;

            ResetZoom();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = sharpImages.Length - 1;

            ResetZoom();
        }
    }

    void ResetZoom()
    {
        zoomLevel = 0f;
        LoadImage(currentIndex);
        UpdateLensAppearance();
        UpdateHUD();
    }

    void LoadImage(int index)
    {
        lens.style.backgroundImage = new StyleBackground(blurryImages[index]);
    }

    void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            zoomLevel = Mathf.Clamp01(zoomLevel + scroll * zoomSpeed);
            UpdateLensAppearance();
        }
    }

    void UpdateLensAppearance()
    {
        if (zoomLevel < 0.99f)
            lens.style.backgroundImage = new StyleBackground(blurryImages[currentIndex]);
        else
            lens.style.backgroundImage = new StyleBackground(sharpImages[currentIndex]);

        lens.style.opacity = Mathf.Lerp(0.6f, 1f, zoomLevel);

        float scaleValue = Mathf.Lerp(0.7f, 1.05f, zoomLevel);
        lens.style.scale = new Scale(new Vector2(scaleValue, scaleValue));
    }

    void UpdateHUD()
    {
        if (imageLabel != null)
            imageLabel.text = $"Image {currentIndex + 1} / {sharpImages.Length}";

        if (zoomLabel != null)
            zoomLabel.text = $"Zoom {(zoomLevel * 100f):0}%";

        if (zoomFill != null)
            zoomFill.style.width = Length.Percent(zoomLevel * 100f);
    }

    void ToggleUI()
    {
        uiVisible = !uiVisible;

        if (uiDocument.rootVisualElement == null)
            return;

        uiDocument.rootVisualElement.style.display = uiVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
