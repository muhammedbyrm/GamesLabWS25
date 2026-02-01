using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CompanionMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float waitTime = 3f;
    
    private NavMeshAgent agent;
    private Animator childAnim; 
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        childAnim = GetComponentInChildren<Animator>();

        // Optimized settings for smooth humanoid movement
        agent.acceleration = 30f;      // Smooth startup
        agent.angularSpeed = 400f;     // Natural turning radius
        agent.stoppingDistance = 0.5f; // Leeway to prevent jitter at target
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        if (waypoints.Length > 0)
        {
            SetNextDestination();
        }
    }

    void Update()
    {
        // 1. Pause Logic
        if (Time.timeScale == 0) 
        { 
            agent.isStopped = true; 
            return; 
        }

        // 2. Arrival Check
        // !agent.pathPending ensures we don't skip logic while the NavMesh calculates
        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndLoopRoutine());
        }

        // 3. Smooth Animation Transition
        if (childAnim != null)
        {
            float targetSpeed = agent.velocity.magnitude;
            float currentSpeed = childAnim.GetFloat("Speed");
            
            // This prevents "teleporting" animations by smoothing the float value
            childAnim.SetFloat("Speed", Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f));
        }
    }

    IEnumerator WaitAndLoopRoutine()
    {
        isWaiting = true;
        
        // Stop the physical movement
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(waitTime);

        // 4. Sequential Looping Logic
        // This moves from 0 to 1 to 2... then back to 0
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        
        SetNextDestination();
        
        agent.isStopped = false;
        isWaiting = false;
    }

    void SetNextDestination()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}