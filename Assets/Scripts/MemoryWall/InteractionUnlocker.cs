using UnityEngine;

public class InteractionUnlocker : MonoBehaviour
{
    [SerializeField] private Collider[] targetColliders;

    private void Awake()
    {
        foreach (var col in targetColliders)
            col.enabled = false;
    }

    public void EnableInteractions()
    {
        foreach (var col in targetColliders)
            col.enabled = true;
    }
}
