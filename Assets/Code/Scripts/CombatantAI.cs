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
    public LayerMask protagonistLayerMask;

    [SerializeField] float moveSpeed;
    float suspicionMoveSpeed = 0.55f;

    [Range(0, 1)]
    [SerializeField] public float suspicionLevel;
    public bool isAlreadySuspecting;
    public float suspectedDistance;

    public Transform protagonist;
    CharacterController characterController;
    CombatantController combatantController;
    Animator animator;
    NavMeshAgent agent;

    RaycastHit hitInfo;
    float patrolOffset = 1f;

    public enum State { Patrol, Melee, RangedToPatrol, Suspicion, Ranged };
    public State currentState;

    public List<Vector3> patrolPathway;
    public Vector3 returnToPatrolPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        combatantController = GetComponent<CombatantController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        patrolPathway = new List<Vector3>();

        currentState = State.Patrol;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, protagonist.transform.position) < .75f)
        {
            currentState = State.Melee;
        }
        else
        {
            DetectPlayer();
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Suspicion:
                animator.SetBool("isMelee", false);
                agent.speed = suspicionMoveSpeed;
                agent.isStopped = false;
                agent.SetDestination(protagonist.position);
                break;
            case State.Ranged:
                agent.isStopped = true;
                animator.SetBool("isMelee", false);
                transform.LookAt(protagonist.position, Vector3.up);
                break;
            case State.RangedToPatrol:
                agent.speed = moveSpeed;
                agent.SetDestination(returnToPatrolPosition);
                if (agent.velocity.magnitude <= 0)
                {
                    agent.isStopped = true;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    currentState = State.Patrol;
                }
                break;
            case State.Melee:
                transform.LookAt(protagonist.position, Vector3.up);
                agent.SetDestination(transform.position);
                animator.SetBool("isMelee", true);
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
        Debug.DrawRay(transform.position + transform.up * 1.2f, protagonist.GetChild(2).position - transform.position, Color.red);

        if (Vector3.Distance(transform.position, protagonist.transform.position) <= viewDistance && IsPlayerInViewRadius)
        {
            if (suspicionLevel >= 1 && currentState == State.Melee)
            {
                currentState = State.Ranged;
            }

            if (!Physics.Raycast(transform.position+ transform.up * 1.2f, protagonist.GetChild(2).position - transform.position, out hitInfo, (protagonist.transform.position - transform.position).magnitude, innerObstacleLayerMask))
            {
                if (!isAlreadySuspecting)
                {
                    StartCoroutine(IncreaseSuspicion());
                }
            }
            else
            {
                if (isAlreadySuspecting)
                {
                    StartCoroutine(DecreaseSuspicion());
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, protagonist.transform.position - transform.position, out hitInfo, (protagonist.transform.position - transform.position).magnitude, innerObstacleLayerMask) && currentState == State.Ranged)
            {
                StartCoroutine(DecreaseSuspicion());
            }
            else if (!Physics.Raycast(transform.position, protagonist.transform.position - transform.position, out hitInfo, (protagonist.transform.position - transform.position).magnitude, innerObstacleLayerMask) && currentState == State.Suspicion)
            {
                StartCoroutine(IncreaseSuspicion());
            }
        }
    }

    private void Move()
    {
        characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    IEnumerator IncreaseSuspicion()
    {
        suspectedDistance = Mathf.Clamp((protagonist.transform.position - transform.position).magnitude, 2, 5);

        isAlreadySuspecting = true;
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
            suspicionLevel -= suspectedDistance / 300;
            animator.SetFloat("suspicionLevel", suspicionLevel);
            yield return new WaitForSeconds(0.1f);

            if (isAlreadySuspecting)
                break;
        }

        if (suspicionLevel <= 0)
        {
            animator.ResetTrigger("GotHit");
            returnToPatrolPosition = FindClosestPatrolPoint();
            currentState = State.RangedToPatrol;
        }
    }

    private Vector3 FindClosestPatrolPoint()
    {
        Vector3 patrolPosition = patrolPathway[patrolPathway.Count - 1];

        foreach (Vector3 waypoint in patrolPathway)
        {
            if (Vector3.Distance(waypoint, transform.position) < Vector3.Distance(patrolPosition, transform.position))
            {
                patrolPosition = waypoint;
            }
        }

        return patrolPosition;
    }

    bool IsPlayerInViewRadius => Vector3.Angle(transform.forward, (protagonist.transform.position - transform.position).normalized) < fieldOfView / 2;
}
