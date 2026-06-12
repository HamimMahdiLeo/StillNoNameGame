using UnityEngine;
using UnityEngine.AI;

public class FokyuController : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitAtPoint = 1.5f;

    [Header("Chase")]
    public Transform player;
    public float chaseSpeed = 4f;
    public float detectionRange = 8f;
    public float loseRange = 12f;

    [Header("State")]
    public bool canChase = false; // enabled later by GameManager

    [Header("Animation")]
    private Animator anim;
    private NavMeshAgent agent;
    private int currentPoint = 0;
    private bool isChasing = false;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        agent.speed = patrolSpeed;
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (!agent.enabled) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (canChase)
        {
            if (!isChasing && distToPlayer < detectionRange)
                StartChase();
            else if (isChasing && distToPlayer > loseRange)
                StopChase();
        }
        else if (isChasing)
        {
            StopChase();
        }

        if (isChasing)
            Chase();
        else
            Patrol();

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void EnableChase()
    {
        canChase = true;
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void StartChase()
    {
        isChasing = true;
        isWaiting = false;
    }

    void StopChase()
    {
        isChasing = false;
        agent.speed = patrolSpeed;
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPoint].position);
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPoint].position);
            }
            return;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }
}