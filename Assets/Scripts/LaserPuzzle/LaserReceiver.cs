using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public bool isHit { get; private set; }
    public Color activeColor = Color.green;
    private Color inactiveColor;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        inactiveColor = meshRenderer.material.color;
    }

    void Update()
    {
        // Reset every frame; if the laser hits it, it will set to true again
        isHit = false;
        meshRenderer.material.color = inactiveColor;
    }

    public void NotifyHit()
    {
        isHit = true;
        meshRenderer.material.color = activeColor;
    }
}