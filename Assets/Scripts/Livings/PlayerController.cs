using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]

/// <summary>  
/// 	Player controller
/// </summary>
public class PlayerController : Living
{

    Transform cam;

    private float turnSpeed = 50;

    public Transform spellOrigin;
    public GameObject ammo;

    private Animator _animator;
    private Rigidbody rigidBody;

    [SerializeField]
    [Range(1.0f, 3.0f)] 
    private float JumpFactor = 2;

    [SerializeField]
    [Range(1.0f, 3.0f)]
    private float RunFactor = 2;
    
    [Header("jump")]
    public bool isGrounded;

    /// <summary>  
    /// 	Fetch animator
    ///		Destroy camera if not localplayer
    /// </summary>
    void Start()
    {
        _animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        ApplyMoveStatus("Free");

        if (!isLocalPlayer)
        {
            Destroy(cam.gameObject);
        }
    }

    /// <summary>  
    ///     Camera follow player and player orientation depend of the camera
    /// 	Translate and rotate player
    /// 	Jump player if input
    ///		Updates animator
    ///		Take move status into consideration
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Vector3 dir = (cam.right * Input.GetAxis("Horizontal") * Time.deltaTime) + (cam.forward * Input.GetAxis("Vertical") /** Time.deltaTime*/);
            dir.y = 0;
            if (canMove)
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    rigidBody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed);
                    var v3 = Vector3.zero;
                    if (canRun)
                    {
                        v3 = transform.forward * speed;
                        v3.y = rigidBody.velocity.y;
                    }
                    else
                    {
                        v3 = transform.forward * speed/ RunFactor;
                        v3.y = rigidBody.velocity.y;
                    }
                    rigidBody.velocity = v3;
                    if (Input.GetAxis("Horizontal") != 0)
                    {
                        _animator.SetFloat("Speed", speed);
                    }

                    if (Input.GetAxis("Vertical") != 0)
                    {
                        _animator.SetFloat("Speed", speed);
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Input.GetButtonDown("Fire1"))
            {
                CmdFire();
            }

            if (Input.GetButtonDown("Jump") && isGrounded && canJump)
            {
                _animator.SetTrigger("Jump");
                if (lowJump)
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed / JumpFactor, ForceMode.Impulse);
                }
                else
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);
                }
            }

            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !Input.GetButtonDown("Jump"))
            {
                //_animator.SetTrigger("Idle");;//animation Idle
                _animator.SetFloat("Speed", 0);
            }
        }
    }

    /// <summary>  
    /// 	Use to allow player another jump after hitting the ground
    /// </summary>
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag.Equals("Ground"))
        {
            isGrounded = true;
        }
    }

    /// <summary>  
    /// 	Use to prevent player another jump in air
    /// </summary>
    void OnCollisionExit(Collision coll)
    {
        if (coll.collider.tag.Equals("Ground"))
        {
            isGrounded = false;
        }
    }

    /// <summary>  
    /// 	Instanciate and spawn bullet
    /// </summary>
    [Command]
    void CmdFire()
    {
        GameObject bullet = Instantiate(ammo, spellOrigin.position, spellOrigin.rotation);

        bullet.GetComponent<Rigidbody>().velocity = spellOrigin.forward * 10;

        NetworkServer.Spawn(bullet);
    }

    /// <summary>  
    /// 	Move status liste and way to apply them
    /// </summary>
    public void ApplyMoveStatus(string status)
    {
        switch (status)
        {
            case "Free":
                canRun = true;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case "Ralenti":
                canRun = false;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case "Casting":
                canRun = true;
                canJump = false;
                canMove = true;
                lowJump = true;
                break;
            case "Immobilisé":
                canRun = false;
                canJump = false;
                canMove = false;
                lowJump = false;
                break;
            default:
                break;
        }
    }
}
