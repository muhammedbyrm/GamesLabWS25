using UnityEngine;
using System.Collections;

public class CompanionEndScene : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform moveTarget;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private float slowdownDistance = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float walkAnimationDelay = 0.5f;

    [Header("Dialogue")]
    [SerializeField] private float speakDuration = 3f;
    [SerializeField] private float stopToTalkDelay = 0.5f;

    [SerializeField] private GameObject dialogueText;
    [SerializeField] private float textShowDelay = 0.5f;

    private bool isSequenceActive = false;

    private void Awake()
    {
        if (dialogueText != null)
            dialogueText.SetActive(false);
    }

    public void StartFinalSequence()
    {
        if (!isSequenceActive)
        {
            StartCoroutine(FinalSequence());
        }
    }

    private IEnumerator FinalSequence()
    {
        isSequenceActive = true;

        animator.SetBool("IsWalking", true);
        yield return new WaitForSeconds(walkAnimationDelay);

        while (Vector3.Distance(transform.position, moveTarget.position) > stopDistance)
        {
            Vector3 targetPos = new Vector3(
                moveTarget.position.x,
                transform.position.y,
                moveTarget.position.z
            );

            float distance = Vector3.Distance(transform.position, targetPos);

            float speedMultiplier = 1f;
            if (distance < slowdownDistance)
            {
                speedMultiplier = Mathf.Clamp01(distance / slowdownDistance);
                speedMultiplier = Mathf.Max(speedMultiplier, 0.3f);
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * speedMultiplier * Time.deltaTime
            );

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            yield return null;
        }

        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(stopToTalkDelay);

        animator.SetTrigger("Talk");

        yield return new WaitForSeconds(textShowDelay);

        if (dialogueText != null)
            dialogueText.SetActive(true);

        yield return new WaitForSeconds(speakDuration);

        GameManager.Instance.EndGame();
        isSequenceActive = false;
    }
}
