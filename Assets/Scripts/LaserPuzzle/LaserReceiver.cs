using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public bool isHit { get; private set; }
    public Color activeColor = Color.green;
    private Color inactiveColor;
    private MeshRenderer meshRenderer;
    private float lastHitTime;
    public float hitTimeout = 0.1f;

    void Start()
    {
        meshRenderer = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        inactiveColor = meshRenderer.material.color;
    }

    void Update()
    {
        if (Time.time - lastHitTime > hitTimeout)
        {
            isHit = false;
            meshRenderer.material.color = inactiveColor;
        }
    }

    public void NotifyHit()
    {
        isHit = true;
        lastHitTime = Time.time;
        meshRenderer.material.color = activeColor;
    }
}