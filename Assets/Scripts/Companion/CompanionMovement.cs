using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshPlaceholderAI : MonoBehaviour
{
    [Header("References")] public Transform player;
    private Animator anim;
    private Outline outline;

    [Header("Behavior Toggles")] public bool followPlayer = true;
    public bool followPath = true;

    [Header("Follow Player")] public float followDistance = 6f;
    public float stoppingDistance = 1.5f;

    [Header("Path Settings")] public Transform[] waypoints;
    public float waypointThreshold = 0.5f;
    public float waitTimeAtWaypoint = 2f;

    private NavMeshAgent agent;
    private int waypointIndex;
    private float waitTimer;
    private bool isWaiting;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        outline = GetComponent<Outline>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleInteraction();
        UpdateAnimation();

        // 1. HEIGHT FIX (Always keep this first or last)
        if (anim != null) anim.transform.localPosition = new Vector3(0, -0.93f, 0);

        // 2. ROTATION
        if (player != null && (PlayerInRange() || isWaiting))
        {
            SmoothRotateTowards(player.position);
        }

        // 3. WAYPOINT WAITING (Move this HIGHER in priority)
        if (isWaiting)
        {
            // Force velocity to 0 so Animator switches to Idle
            agent.velocity = Vector3.zero; 
        
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
            }
            return; // Don't let followPlayer run while we are waiting
        }

        // 4. FOLLOW PLAYER
        if (followPlayer && player != null && PlayerInRange())
        {
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(player.position);
        
            // If we reach the player, the agent naturally slows down
            return;
        }

        // 5. PATROL PATH
        if (followPath && waypoints.Length > 0)
        {
            agent.stoppingDistance = 0.1f;
            PatrolPath();
        }
    }

    void PatrolPath()
    {
        // Simplified: Just set the destination
        agent.SetDestination(waypoints[waypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= waypointThreshold)
        {
            isWaiting = true;
            waitTimer = 0f;
            agent.ResetPath(); 
        }
    }

    void UpdateAnimation()
    {
        if (anim != null)
        {
            // Tells the animator how fast we are walking
            float speed = agent.velocity.magnitude;
            Debug.Log("Agent velocity magnitude is: "+ speed);
            anim.SetFloat("Speed", speed);
        }
    }

    void HandleInteraction()
    {
        if (player == null) return;
        float interactRange = (outline != null) ? outline.maxVisibleDistance : 2f;

        if (Vector3.Distance(transform.position, player.position) <= interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interacted with companion");
            }
        }
    }

    bool PlayerInRange() => Vector3.Distance(transform.position, player.position) <= followDistance;
    

    void SmoothRotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Change '5f' to a higher number for faster turning
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

}