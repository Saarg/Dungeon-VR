using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using System.Collections;

public class BaseAI : NetworkBehaviour {

    NavMeshAgent agent;
    [SerializeField]
    Weapon weapon;
    [SerializeField]
    ShootingController shootingController;
    [SerializeField]
    bool isShooter = false;
    [SerializeField]
    GameObject weaponObj;

	[SerializeField] protected Animator animator;
	[SerializeField] protected float DEATH_ANIM_DELAY = 6f;

    const string PATH_NODE_TAG = "PathNode";
    const string PLAYER_TAG = "Player";

    Vector3 currentDestination = Vector3.positiveInfinity;
    PathNode currentNodeDestination = null;
    GameObject target = null;
    Vector3 targetDestination = Vector3.positiveInfinity;

    float interruptDelay = 2f;
	[SerializeField] float attackDelay = 1f;
	[SerializeField] float shootingDelay = 3f;
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
        weaponObj.SetActive(isShooter);
        agent = gameObject.GetComponent<NavMeshAgent>();
        lastDetectTarget = Time.time;
        gameObject.GetComponent<Living>().OnDeath += OnDeath;
    }

    public void SetShooter(bool val)
    {
        CmdSetShooter(val);
    }

    [Command]
    void CmdSetShooter(bool value)
    {
        if (isServer)
            RpcSetShooter(value, netId);
    }

    [ClientRpc]
    void RpcSetShooter(bool value, NetworkInstanceId id)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        BaseAI ai = obj.GetComponent<BaseAI>();
        ai.weaponObj.SetActive(value);
        isShooter = value;
    }

    // Update is called once per frame
    void Update() {

        if (!isServer)
            return;

        if (interrupt)
            return;

        if (target == null)
            UpdateMovement();
        else if (!isShooter)
            UpdateAttack();
        else
            UpdateShoot();

		if (target != null)
			transform.LookAt (target.transform);

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
        else
        {
            OnDeath();
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
			animator.SetBool ("moving", true);
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
			animator.SetBool ("moving", true);
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
			animator.SetTrigger ("attack");
			animator.SetBool ("moving", false);
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
		animator.SetBool ("moving", false);
		animator.SetTrigger ("attack");

        yield return new WaitForSecondsRealtime(shootingDelay);
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            Vector3 offset = target.GetComponent<Rigidbody>().velocity.normalized * distance * shootingOffsetMultiplier;
            shootingController.AiFire(target.transform.position + offset); 
        }
        
        attacking = false;
        shootingCoroutine = null;
        agent.isStopped = false;
    }

    public void InterruptAction()
    {
        EndCoroutine(interruptCoroutine);

        agent.isStopped = true;
        interrupt = true;
        
        EndCoroutine(attackingCoroutine);
        EndCoroutine(shootingCoroutine);
		animator.SetBool ("moving", false);
        
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
		StartCoroutine ("Death");
    }

	IEnumerator Death(){
		animator.SetBool ("IsDead", true);
		yield return new WaitForSecondsRealtime (DEATH_ANIM_DELAY); //time of the death animation
		CmdOnDeath(netId);
	}

    [Command]
    void CmdOnDeath(NetworkInstanceId id)
    {
        RpcOnDeath(id);
    }

    [ClientRpc]
    void RpcOnDeath(NetworkInstanceId id)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        BaseAI ai = obj.GetComponent<BaseAI>();
        ai.EndCoroutine(ai.attackingCoroutine);
        ai.EndCoroutine(ai.shootingCoroutine);
        ai.EndCoroutine(ai.interruptCoroutine);
        obj.GetComponent<Living>().OnDeath -= OnDeath;
        Destroy(obj);
    }

    void EndCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

	public Animator getAnimator(){
		return animator;
	}
}
