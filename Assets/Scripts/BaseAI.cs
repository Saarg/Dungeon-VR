using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BaseAI : MonoBehaviour {

    [SerializeField]
    NavMeshAgent agent;

    [SerializeField]
    Weapon weapon;

    [SerializeField]
    ShootingController shootingController;

    [SerializeField]
    bool isShooter = false;

    const string PATH_NODE_TAG = "PathNode";
    const string PLAYER_TAG = "Player";

    Vector3 currentDestination = Vector3.positiveInfinity;
    PathNode currentNodeDestination = null;
    GameObject target = null;
    Vector3 targetDestination = Vector3.positiveInfinity;

    float interruptDelay = 2f;
    float attackDelay = 1f;
    float shootingDelay = 3f;
    float lastAttack = 0f;

    float detectTargetDelay = 0.5f;
    float lastDetectTarget = 0f;
    float shootingOffsetMultiplier = .2f;

    float playerDetectionRange = 20f;
    float nodeDetectionRange = 100f;
    float pickNextNodeRange = 1f;
    float meleeAttackRange = .1f;
    float nearPositionMultiplier = 3f;
    float farPositionMultiplier = 10f;

    bool attacking = false;
    bool interrupt = false;

    Coroutine attackingCoroutine = null;
    Coroutine shootingCoroutine = null;
    Coroutine interruptCoroutine = null;

    // Use this for initialization
    void Start() {
        lastDetectTarget = Time.time;
        gameObject.GetComponent<Living>().OnDeath += OnDeath;
    }

    // Update is called once per frame
    void Update() {

        if (interrupt)
            return;

        if (target == null)
            UpdateMovement();
        else if (!isShooter)
            UpdateAttack();
        else
            UpdateShoot();

        if (Time.time - lastDetectTarget > detectTargetDelay)
        {
            DetectPlayer();
            lastDetectTarget = Time.time;
        }
    }

    void UpdateMovement()
    {
        if (currentNodeDestination == null)
            PickNode();
        else if ((transform.position - currentDestination).magnitude < pickNextNodeRange)
            PickNextNode();
        else
            agent.SetDestination(currentDestination);
    }

    void UpdateAttack()
    {
        if (attacking)
            return;

        MoveNearPlayer();
        MeleeAttack();
    }

    void UpdateShoot()
    {
        if (attacking)
            return;

        StayAwayFromPlayer();
        ShootPlayer();
    }

    void PickNode()
    {
        PathNode closestNode = null;
        PathNode secondClosestNode = null;

        var hits = Physics.OverlapSphere(transform.position, nodeDetectionRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject.tag != PATH_NODE_TAG)
                continue;

            if (closestNode == null)
                closestNode = hit.gameObject.GetComponent<PathNode>();
            else if (secondClosestNode == null)
            {
                PathNode node = hit.gameObject.GetComponent<PathNode>();
                if ((closestNode.gameObject.transform.position - transform.position).sqrMagnitude > (node.gameObject.transform.position - transform.position).sqrMagnitude)
                {
                    secondClosestNode = closestNode;
                    closestNode = node;
                }
                else
                    secondClosestNode = node;
            }
            else
            {
                PathNode node = hit.gameObject.GetComponent<PathNode>();
                if ((secondClosestNode.gameObject.transform.position - transform.position).sqrMagnitude < (node.gameObject.transform.position - transform.position).sqrMagnitude)
                    continue;
                else if ((closestNode.gameObject.transform.position - transform.position).sqrMagnitude > (node.gameObject.transform.position - transform.position).sqrMagnitude)
                {
                    secondClosestNode = closestNode;
                    closestNode = node;
                }
                else
                    secondClosestNode = node;
            }
        }

        if (closestNode != null && secondClosestNode == null)
            currentNodeDestination = closestNode;
        else if (closestNode != null)
        {
            if (closestNode.ContainsNode(secondClosestNode))
                currentNodeDestination = secondClosestNode;
            else
                currentNodeDestination = closestNode;
        }

        if (currentNodeDestination != null)
        {
            currentDestination = currentNodeDestination.GetOffsetPosition();
            agent.SetDestination(currentDestination);
        }
    }

    void PickNextNode()
    {
        PathNode nextNode = currentNodeDestination.GetNextPathNodes();
        if (nextNode != null)
        {
            currentNodeDestination = currentNodeDestination.GetNextPathNodes();
            currentDestination = currentNodeDestination.GetOffsetPosition();
            agent.SetDestination(currentDestination);
        }
    }

    void PickDestinationOffset()
    {
        currentDestination = currentNodeDestination.GetOffsetPosition();
        agent.SetDestination(currentDestination);
    }

    void DetectPlayer()
    {
        var hits = Physics.OverlapSphere(transform.position, playerDetectionRange);
        GameObject targetFound = null;
        bool sameTarget = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject.tag == PLAYER_TAG)
            {
                if (hit.gameObject == target)
                    sameTarget = true;

                targetFound = hit.gameObject;
            }
        }

        if (targetFound == null)
        {
            target = null;
        }
        else if (targetFound)
        {
            if (sameTarget)
                return;

            currentDestination = Vector3.positiveInfinity;
            currentNodeDestination = null;
            target = targetFound;
        }
    }

    void StayAwayFromPlayer()
    {
        if (target != null)
        {
            Vector3 destination = target.transform.position + (transform.position - target.transform.position).normalized * farPositionMultiplier;
            agent.SetDestination(destination);
            targetDestination = destination;
        }
    }

    void MoveNearPlayer()
    {
        //FindClosestEdge
        //SamplePathPosition
        if (target != null)
        {
            Vector3 destination = target.transform.position + (transform.position - target.transform.position).normalized * nearPositionMultiplier;
            agent.SetDestination(destination);
            targetDestination = destination;
        }
    }

    void MeleeAttack()
    {
        if (attacking)
            return;

        if ((transform.position - targetDestination).sqrMagnitude < meleeAttackRange)
        {
            attacking = true;
            agent.isStopped = true;
            attackingCoroutine = StartCoroutine(AttackTimer());
        }
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSecondsRealtime(attackDelay);
        attacking = false;
        attackingCoroutine = null;
        agent.isStopped = false;
    }

    void ShootPlayer()
    {
        if (attacking)
            return;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, target.transform.position - transform.position, (transform.position - target.transform.position).magnitude);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.tag == "Dungeon")
                return;
        }

        if (weapon != null && shootingController != null)
        {
            attacking = true;
            shootingCoroutine = StartCoroutine(ShootingDelay(target.transform.position));
        }
    }

    IEnumerator ShootingDelay(Vector3 position)
    {
        yield return new WaitForSecondsRealtime(shootingDelay);
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            Vector3 offset = target.GetComponent<Rigidbody>().velocity.normalized * distance * shootingOffsetMultiplier;
            shootingController.AiFire(target.transform.position + offset); 
        }
        attacking = false;
        shootingCoroutine = null;
    }

    public void InterruptAction()
    {
        EndCoroutine(interruptCoroutine);

        agent.isStopped = true;
        interrupt = true;
        
        EndCoroutine(attackingCoroutine);
        EndCoroutine(shootingCoroutine);
        
        interruptCoroutine = StartCoroutine(InterruptTimer());
    }

    IEnumerator InterruptTimer()
    {
        attacking = false;
        yield return new WaitForSecondsRealtime(interruptDelay);
        interrupt = false;
        interruptCoroutine = null;
        agent.isStopped = false;
    }

    void OnDeath()
    {
        EndCoroutine(attackingCoroutine);
        EndCoroutine(shootingCoroutine);
        EndCoroutine(interruptCoroutine);

        gameObject.GetComponent<Living>().OnDeath -= OnDeath;
        //to do Add death animation than destroy gameobject
    }

    void EndCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
