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
    Transform weaponPosition;
    [SerializeField]
    GameObject weaponObj;
    [SerializeField]
    WeaponSpec weaponSpec;

    [SerializeField] protected Animator animator;
    [SerializeField] protected NetworkAnimator netAnimator;
    [SerializeField] protected float DEATH_ANIM_DELAY = 6f;

    const string PATH_NODE_TAG = "PathNode";
    const string PLAYER_TAG = "Player";

    Vector3 currentDestination = Vector3.positiveInfinity;
    PathNode currentNodeDestination = null;
    GameObject target = null;
    Vector3 targetDestination = Vector3.positiveInfinity;

    float interruptDelay = .2f;
    [SerializeField] float attackDelay = 1f;
    [SerializeField] float shootingDelay = 3f;
    float lastAttack = 0f;

    float detectTargetDelay = 0.5f;
    float lastDetectTarget = 0f;
    [SerializeField]
    float shootingOffsetMultiplier = .2f;

    float playerDetectionRange = 20f;
    float nodeDetectionRange = 100f;
    float pickNextNodeRange = 1f;
    float meleeAttackRange = .1f;
    [SerializeField]
    float nearPositionMultiplier = 2f;
    [SerializeField]
    float farPositionMultiplier = 10f;

    bool attacking = false;
    bool interrupt = false;

    Coroutine attackingCoroutine = null;
    Coroutine shootingCoroutine = null;
    Coroutine interruptCoroutine = null;
    Coroutine checkNavMeshCoroutine = null;

    Rigidbody rb;

    // Use this for initialization
    void Start() {
        if (weaponObj == null) {
            weaponObj = Instantiate(weaponSpec.WeaponPrefab, weaponPosition);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            weaponObj.transform.localScale = Vector3.one;

            weapon = weaponObj.GetComponent<Weapon>();
            weapon.spec = weaponSpec;
            shootingController.weapon = weapon;
        }

        agent = GetComponent<NavMeshAgent>();
        lastDetectTarget = Time.time;
        GetComponent<Living>().OnDeath += OnDeath;
        CmdSetBool("moving", true);

        rb = GetComponent<Rigidbody>();
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

        if (gameObject.GetComponent<Living>().dead)
            return;

        if (interrupt)
            return;

        if (target != null && target.GetComponent<Living>().dead) {
            target = null;
        }

        if (target == null)
            UpdateMovement();
        else if (!isShooter)
            UpdateAttack();
        else
            UpdateShoot();

        if (target != null)
            transform.LookAt(target.transform);

        if (Time.time - lastDetectTarget > detectTargetDelay)
        {
            DetectPlayer();
            lastDetectTarget = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (agent.enabled) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void UpdateMovement()
    {
        if (currentNodeDestination == null)
            PickNode();
        else if ((transform.position - currentDestination).magnitude < pickNextNodeRange)
            PickNextNode();
        else if (agent.isOnNavMesh && currentDestination != null)
        {
            agent.SetDestination(currentDestination);
        }
        else
        {
            if (checkNavMeshCoroutine == null && animator.GetBool("IsGrabbed") == false)
            {
                checkNavMeshCoroutine = StartCoroutine("CheckForNavMesh");
            }
        }
    }

    //If the monster doesn't find is navmesh within 1s, he die.
    public IEnumerator CheckForNavMesh()
    {
        float currentTime = 0;
        while (currentTime < 1f)
        {
            if (agent.isOnNavMesh)
            {
                Debug.Log("NavMesh found");
                yield break;
            }
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        Debug.Log("NavMesh not found");
        StartCoroutine("Death");
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
            if (agent.enabled)
            {
                agent.SetDestination(currentDestination);
            }
        }
    }

    void PickNextNode()
    {
        PathNode nextNode = currentNodeDestination.GetNextPathNodes();
        if (nextNode != null)
        {
            currentNodeDestination = currentNodeDestination.GetNextPathNodes();
            currentDestination = currentNodeDestination.GetOffsetPosition();
            if (agent.enabled)
            {
                agent.SetDestination(currentDestination);
            }
        }
        else
        {
            Living living = gameObject.GetComponent<Living>();
            if (living != null)
                living.TakeDamage(int.MaxValue, Bullet.DamageTypeEnum.physical);
        }
    }

    void PickDestinationOffset()
    {
        currentDestination = currentNodeDestination.GetOffsetPosition();
        if (agent.enabled)
        {
            agent.SetDestination(currentDestination);
        }
    }

    void DetectPlayer()
    {
        if (target != null && target.GetComponent<Living>().dead)
            target = null;

        var hits = Physics.OverlapSphere(transform.position, playerDetectionRange);
        GameObject targetFound = null;
        bool sameTarget = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject.tag == PLAYER_TAG)
            {
                Living living = hit.gameObject.GetComponent<Living>();
                if (living == null)
                    continue;

                if (living.dead)
                    continue;

                InvisibilitySpell invisibility = hit.gameObject.GetComponentInChildren<InvisibilitySpell>();
                if (invisibility != null)
                    if (invisibility.IsEffectActive)
                        continue;

                if (hit.gameObject == target)
                {
                    if (targetFound != null)
                    {
                        float distanceHit = (hit.gameObject.transform.position - transform.position).magnitude;
                        float distanceClosest = (targetFound.transform.position - transform.position).magnitude;
                        if (distanceClosest * 2 > distanceHit)
                        {
                            sameTarget = true;
                            targetFound = hit.gameObject;
                        }
                    }
                    else
                    {
                        targetFound = hit.gameObject;
                        sameTarget = true;
                    }
                }
                else
                {
                    float distanceHit = (hit.gameObject.transform.position - transform.position).magnitude;
                    if (targetFound)
                    {
                        float distanceTarget = (targetFound.transform.position - transform.position).magnitude;
                        if (distanceTarget < distanceHit)
                            continue;

                        if (target != null)
                        {
                            float distanceCurrent = (target.transform.position - transform.position).magnitude;
                            if (distanceHit * 2 < distanceCurrent)
                            {
                                sameTarget = false;
                                targetFound = hit.gameObject;
                            }
                        }
                        else
                        {
                            sameTarget = false;
                            targetFound = hit.gameObject;
                        }
                    }
                    else
                    {
                        if (target != null)
                        {
                            float distanceCurrent = (target.transform.position - transform.position).magnitude;
                            if (distanceHit * 2 < distanceCurrent)
                            {
                                targetFound = hit.gameObject;
                                sameTarget = false;
                            }
                        }
                        else
                        {
                            sameTarget = false;
                            targetFound = hit.gameObject;
                        }
                    } 
                }
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
            if (LineOfSightToTarget(target))
            {
                Vector3 destination = target.transform.position + (transform.position - target.transform.position).normalized * farPositionMultiplier;
                agent.SetDestination(destination);
                targetDestination = destination;
            }
            else
            {
                Vector3 lineToTarget = (transform.position - target.transform.position).normalized;
                Vector3 offsetPosition = Vector3.Cross(lineToTarget, Vector3.up);
                Vector3 destination = target.transform.position + offsetPosition * farPositionMultiplier;
                if (agent.enabled)
                {
                    agent.SetDestination(destination);
                }
                targetDestination = destination;
            }
        }
    }

    void MoveNearPlayer()
    {

        //FindClosestEdge
        //SamplePathPosition
        if (target != null)
        {
            Vector3 destination = target.transform.position + (transform.position - target.transform.position).normalized * nearPositionMultiplier;
            if (agent.enabled)
            {
                agent.SetDestination(destination);
            }
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
            if (agent.enabled)
            {
                agent.isStopped = true;
            }

            CmdSetTrigger("attack", true);
            CmdSetBool("moving", false);
        }
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSecondsRealtime(attackDelay);
        attacking = false;
        attackingCoroutine = null;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }

    void ShootPlayer()
    {
        if (attacking)
            return;

        if (!LineOfSightToTarget(target))
            return;

        if (weapon != null && shootingController != null)
        {
            attacking = true;
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
            CmdSetBool("moving", false);
            CmdSetTrigger("attack", true);
        }
    }

    bool LineOfSightToTarget(GameObject target)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, target.transform.position - transform.position, (transform.position - target.transform.position).magnitude);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.tag == "Dungeon")
                return false;
        }

        return true;
    }

    IEnumerator ShootingDelay(Vector3 position)
    {
        CmdSetBool("moving", false);
        CmdSetTrigger("attack", true);

        yield return new WaitForSecondsRealtime(shootingDelay);
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            Vector3 offset = Vector3.zero;
            if (target.GetComponent<Rigidbody>().velocity.magnitude > 1)
                offset = target.GetComponent<Rigidbody>().velocity * distance * shootingOffsetMultiplier;
            shootingController.AiFire(target.transform.position + offset);
        }

        attacking = false;
        shootingCoroutine = null;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }

    public void InterruptAction()
    {
        EndCoroutine(interruptCoroutine);
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
        interrupt = true;

        EndCoroutine(attackingCoroutine);
        EndCoroutine(shootingCoroutine);
        CmdSetBool("moving", false);
        CmdSetTrigger("attack", false);       
        interruptCoroutine = StartCoroutine(InterruptTimer());
    }

    IEnumerator InterruptTimer()
    {
        attacking = false;
        yield return new WaitForSecondsRealtime(interruptDelay);
        interrupt = false;
        interruptCoroutine = null;
        agent.isStopped = false;
        CmdSetTrigger("hit", false);
        CmdSetBool("moving", true);
    }

    void OnDeath()
    {
        EndCoroutine(interruptCoroutine);
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
        CmdSetBool("moving", false);
        gameObject.GetComponent<Living>().OnDeath -= OnDeath;
        if (isServer)
        {
            RaycastHit hit;
            Vector3 pos = transform.position;
            if(Physics.Raycast(transform.position, Vector3.down * 5f, out hit, 5f, 1 << 8))
                pos = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
            TrapSpawner.singleton.SpawnWeapon(pos);
        }
        StartCoroutine("Death");
    }

    IEnumerator Death() {
        CmdSetBool("IsDead", true);
        yield return new WaitForSecondsRealtime(DEATH_ANIM_DELAY); //time of the death animation
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

    public Animator getAnimator() {
        return animator;
    }

    public void AttackAnimationStarted() {
		weaponObj.GetComponent<MeleeWeaponHitPoint>().AttackAnimationStarted();
	}
	public void AttackAnimationEndedDamage() {
		weaponObj.GetComponent<MeleeWeaponHitPoint>().AttackAnimationEndedDamage();
	}

    public void AttackAnimationEnded()
    {
        attacking = false;
        attackingCoroutine = null;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        CmdSetBool("moving", true);
        CmdSetTrigger("attack", false);
    }

    public void Shoot()
    {
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            Vector3 offset = Vector3.zero;
            if (target.GetComponent<Rigidbody>().velocity.magnitude > 1)
                offset = target.GetComponent<Rigidbody>().velocity * distance * shootingOffsetMultiplier;
            shootingController.AiFire(target.transform.position + offset);
        }
    }

    public void ShootingAnimationEnded()
    {
        attacking = false;
        shootingCoroutine = null;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        CmdSetBool("moving", true);
        CmdSetTrigger("attack", false);
    }

    public void SetIsGrabbed(bool state)
    {
        CmdSetBool("IsGrabbed", state);
    }

    [Command]
    void CmdSetTrigger(string name, bool value)
    {
        if (value)
            netAnimator.SetTrigger(name);
        else
            netAnimator.animator.ResetTrigger(name);

        RpcSetTrigger(name, value, netId);
    }

    [ClientRpc]
    void RpcSetTrigger(string name, bool value, NetworkInstanceId id)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        BaseAI ai = obj.GetComponent<BaseAI>();
        if (value)
            ai.netAnimator.SetTrigger(name);
        else
            ai.netAnimator.animator.ResetTrigger(name);
    }

    [Command]
    void CmdSetBool(string name, bool value)
    {
        netAnimator.animator.SetBool(name, value);
        RpcSetBool(name, value, netId);
    }

    [ClientRpc]
    void RpcSetBool(string name, bool value, NetworkInstanceId id)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        BaseAI ai = obj.GetComponent<BaseAI>();
        ai.netAnimator.animator.SetBool(name, value);
    }
}
