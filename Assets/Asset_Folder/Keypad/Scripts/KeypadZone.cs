using UnityEngine;

public class KeypadZone : MonoBehaviour
{
    public FirstPersonControllerInteractable player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.IsInteractingWithKeypad = true;
            Debug.Log("Enabled");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.IsInteractingWithKeypad  = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
