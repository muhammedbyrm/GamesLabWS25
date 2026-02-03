// This scripts unlock puzzle colliders
using UnityEngine;

public class InteractionUnlocker : MonoBehaviour
{
    public static InteractionUnlocker Instance { get; private set; }

    [SerializeField] private Collider[] targetColliders;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DisableInteractions();
    }

    public void EnableInteractions()
    {
        foreach (var col in targetColliders)
            col.enabled = true;
    }

    public void DisableInteractions()
    {
        foreach (var col in targetColliders)
            col.enabled = false;
    }
}

