using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CombatantAI : MonoBehaviour
{
    [SerializeField] float fieldOfView;
    [SerializeField] float viewDistance;
    [SerializeField] LayerMask obstacleLayerMask;
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask innerObstacleLayerMask;

    [SerializeField] float moveSpeed;

    [Range(0, 1)]
    [SerializeField] float suspicionLevel;
    public bool isAlreadySuspecting;
    public float suspectedDistance;

    public Transform protagonist;
    CharacterController characterController;
    Animator animator;
    NavMeshAgent agent;

    RaycastHit hitInfo;
    float patrolOffset = 1f;

    public enum State { Patrol, RangedToPatrol, Suspicion, Meele, Ranged };
    public State currentState;

    public List<Vector3> patrolPathway;
    public Vector3 returnPatrolPos;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        patrolPathway = new List<Vector3>();

        currentState = State.Patrol;
    }

    private void FixedUpdate()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.Patrol:
                agent.isStopped = true;
                Patrol();
                break;
            case State.Suspicion:
                agent.isStopped = false;
                agent.SetDestination(protagonist.position);
                break;
            case State.Ranged:
                agent.isStopped = true;
                break;
            case State.RangedToPatrol:
                agent.SetDestination(returnPatrolPos);
                Debug.Log(agent.velocity.magnitude);
                if (transform.position == returnPatrolPos)
                {
                    agent.isStopped = true;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    currentState = State.Patrol;
                }
                break;
        }
    }

    private void Update()
    {

    }

    private void Patrol()
    {
        Debug.DrawRay(transform.position + transform.up, -transform.right * patrolOffset, Color.black);
        Debug.DrawRay(transform.position + transform.up, Quaternion.Euler(0, 45, 0) * -transform.right * patrolOffset * 2, Color.black);
        Debug.DrawRay(transform.position + transform.up - transform.forward / 2, -transform.right * patrolOffset, Color.black);
        Debug.DrawRay(transform.position + transform.up - transform.right, -transform.up * patrolOffset * 4, Color.black);


        Debug.DrawRay(transform.position + transform.forward / 2 + transform.up, -transform.up * patrolOffset * 4, Color.black);
        Debug.DrawRay(transform.position + transform.up, transform.forward * patrolOffset, Color.black);

        bool obstacleFront = Physics.Raycast(transform.position + transform.up, transform.forward, patrolOffset, obstacleLayerMask);
        bool hasGround = Physics.Raycast(transform.position + transform.forward / 2 + transform.up, -transform.up, patrolOffset * 4, platformLayerMask);
        bool noLeftObstacle = !Physics.Raycast(transform.position + transform.up, -transform.right, patrolOffset, obstacleLayerMask) && !Physics.Raycast(transform.position + transform.up, Quaternion.Euler(0, 45, 0) * -transform.right, patrolOffset * 2, obstacleLayerMask) && !Physics.Raycast(transform.position + transform.up - transform.forward / 2, -transform.right, patrolOffset, obstacleLayerMask) && Physics.Raycast(transform.position - transform.right + transform.up, -transform.up, patrolOffset * 4, platformLayerMask);

        if (noLeftObstacle)
        {
            transform.rotation *= Quaternion.Euler(0, -90, 0);
            if (!patrolPathway.Contains(transform.position))
                patrolPathway.Add(transform.position);
        }
        else if (hasGround && !obstacleFront)
        {
            Move();
        }
        else
        {
            transform.rotation *= Quaternion.Euler(0, 90, 0);
            if (!patrolPathway.Contains(transform.position))
                patrolPathway.Add(transform.position);
        }
    }


    private void DetectPlayer()
    {
        Debug.DrawRay(transform.position, Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, protagonist.transform.position - transform.position, Color.red);

        if (Vector3.Distance(transform.position, protagonist.transform.position) <= viewDistance && IsPlayerInViewRadius)
        {

            if (!Physics.Raycast(transform.position, protagonist.transform.position - transform.position, out hitInfo, (protagonist.transform.position - transform.position).magnitude, innerObstacleLayerMask))
            {
                if (!isAlreadySuspecting)
                {
                    StartCoroutine(IncreaseSuspicion());
                }

                // Debug.Log("Detected");
            }
            else
            {
                // Debug.Log("Behind covers");
                if (isAlreadySuspecting)
                {
                    StartCoroutine(DecreaseSuspicion());
                }
            }
        }
        else
        {
            if (isAlreadySuspecting)
            {
                StartCoroutine(DecreaseSuspicion());
            }
            // Debug.Log("Out of sight");
        }
    }

    private void Move()
    {
        characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    IEnumerator IncreaseSuspicion()
    {
        isAlreadySuspecting = true;
        suspectedDistance = (protagonist.transform.position - transform.position).magnitude;
        currentState = State.Suspicion;

        while (suspicionLevel < 1)
        {
            suspicionLevel += 1 / (suspectedDistance * 5);
            animator.SetFloat("suspicionLevel", suspicionLevel);
            yield return new WaitForSeconds(0.1f);

            if (!isAlreadySuspecting)
                break;
        }

        if (suspicionLevel >= 1)
        {
            currentState = State.Ranged;
        }
    }

    IEnumerator DecreaseSuspicion()
    {
        isAlreadySuspecting = false;
        currentState = State.Suspicion;

        while (suspicionLevel > 0)
        {
            suspicionLevel -= suspectedDistance / 100;
            animator.SetFloat("suspicionLevel", suspicionLevel);
            yield return new WaitForSeconds(0.1f);

            if (isAlreadySuspecting)
                break;
        }

        if (suspicionLevel <= 0)
        {
            returnPatrolPos = patrolPathway[patrolPathway.Count - 1];
            foreach (Vector3 waypoint in patrolPathway)
            {
                if (Vector3.Distance(waypoint, transform.position) < Vector3.Distance(returnPatrolPos, transform.position))
                {
                    returnPatrolPos = waypoint;
                }
            }

            agent.speed = 1;
            currentState = State.RangedToPatrol;
        }
    }

    bool IsPlayerInViewRadius => Vector3.Angle(transform.forward, (protagonist.transform.position - transform.position).normalized) < fieldOfView / 2;
}
