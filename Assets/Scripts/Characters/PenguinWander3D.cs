using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PenguinWander3D : MonoBehaviour
{
    [Header("Navigation")]
    private NavMeshAgent agent;
    [SerializeField] float range = 6f; //radius of wander area
    [SerializeField] Transform centrePoint; //centre of the area the agent moves around in, set it to the Penguin to wander randomly

    [Header("Pause")]
    [SerializeField] float pauseMin = 1f;
    [SerializeField] float pauseMax = 5f;

    //these values are based on the purchased penguin assets
    [Header("Animation")]
    private Animator animator;
    private string animationParamName = "animation";
    private int idleValue = 3;
    private int moveValue = 1;

    private int animationParamHash;

    //logic use Random.insudeUnityCircle in goal Locations,
    //to grab random point in that location
    // then, play matching animation for each goal zone
    //(for this world, snowball, clap, just do it, sn

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        //don't flip the sprite here (no sprites)
        agent.updateRotation = true;

        //grab animation param "animation" and set value to int
        animationParamHash = Animator.StringToHash(animationParamName);
    }

    private void OnEnable()
    {
        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        UpdateMoveIdleAnimation();
    }

    private IEnumerator WanderRoutine()
    {
        Transform center = centrePoint ? centrePoint : transform;

        //start on Navmesh
        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit startHit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(startHit.position);
        }

        while (true)
        {
            //pick a destination
            if (RandomPoint(center.position, range, out Vector3 point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.isStopped = false;
                agent.SetDestination(point);
            }

            //wait for path calc
            while (agent.pathPending)
                yield return null;

            //wait until arrived
            while (agent.hasPath && agent.remainingDistance > agent.stoppingDistance + 0.1f)
                yield return null;

            //pause
            agent.isStopped = true;
            agent.ResetPath();

            float pause = Random.Range(pauseMin, pauseMax);
            yield return new WaitForSeconds(pause);
        }
    }

    private void UpdateMoveIdleAnimation()
    {
        if (!animator || !agent)
        {
            Debug.Log("Missing animator or agent component.");
            return;
        }

        float speed = agent.velocity.magnitude;

        //are we moving?
        bool isMoving = speed > 0.05f;

        //set the animation by name based on isMoving (set to moveValue if true)
        animator.SetInteger(animationParamHash, isMoving ? moveValue : idleValue);
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        //wander in a circular range
        Vector2 rnd = Random.insideUnitCircle * range;
        //randomize x & z
        Vector3 randomPoint = new Vector3(center.x + rnd.x, center.y, center.z + rnd.y);

        //find a point in the NavMesh and return it
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}