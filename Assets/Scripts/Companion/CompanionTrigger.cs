using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private CompanionEndScene companion;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour playerController; 
    [SerializeField] private GameObject[] objectsToDisable; 

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

   
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            companion.StartFinalSequence();
        }
    }
}