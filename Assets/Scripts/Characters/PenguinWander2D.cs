using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PenguinWander2D : MonoBehaviour
{
    [Header("Nav")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform centrePoint;
    [SerializeField] private float range = 6f;

    [Header("Pause")]
    [SerializeField] private float pauseMin = 1f;
    [SerializeField] private float pauseMax = 5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Sampling")]
    [SerializeField] private int sampleAttempts = 10;
    [SerializeField] private float sampleMaxDistance = 2f;
    [SerializeField] private float arriveTolerance = 0.1f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        //Stop NavMeshAgent from rotating the transform (prevents paper flipping)
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        //Drive Animator Speed for Idle/Walk
        if (!animator || !agent) return;

        Vector3 v = agent.velocity;

        if (Mathf.Abs(v.x) > 0.02f)

            if (spriteRenderer)
                spriteRenderer.flipX = v.x < 0f;

        float speed = new Vector2(v.x, v.z).magnitude;

        //Clamp tiny velocities so animation doesn't flicker
        if (speed < 0.05f) speed = 0f;

        animator.SetFloat(SpeedHash, speed);
    }

    private IEnumerator WanderRoutine()
    {
        //Fallback if centrePoint isn't assigned
        Transform center = centrePoint ? centrePoint : transform;

        //Make sure agent starts on the NavMesh
        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit startHit, 5f, NavMesh.AllAreas))
            agent.Warp(startHit.position);

        while (true)
        {
            //Pick a destination
            if (TryGetRandomPoint(center.position, range, out Vector3 dest))
            {
                agent.isStopped = false;
                agent.SetDestination(dest);
            }
            else
            {
                //If we can't find a point, wait a frame and try again
                yield return null;
                continue;
            }

            //Wait for path calculation
            while (agent.pathPending)
                yield return null;

            //Wait until arrived (or no valid path)
            while (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete &&
                   agent.remainingDistance > agent.stoppingDistance + arriveTolerance)
            {
                yield return null;
            }

            //Pause before choosing next point
            agent.isStopped = true;
            agent.ResetPath();

            float pause = Random.Range(pauseMin, pauseMax);
            yield return new WaitForSeconds(pause);
        }
    }

    private bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < sampleAttempts; i++)
        {
            //Use XZ circle for top-down wandering (don't randomize Y)
            Vector2 rnd = Random.insideUnitCircle * radius;
            Vector3 candidate = new Vector3(center.x + rnd.x, center.y, center.z + rnd.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleMaxDistance, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}