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

    //public Camera cam;
    Transform cam;

    private float turnSpeed = 50;

    public Transform spellOrigin;
    public GameObject ammo;

    private Animator _animator;
    private Rigidbody rigidBody;

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

        if (!isLocalPlayer)
        {
            Destroy(cam.gameObject);
        }
    }

    /// <summary>  
    /// 	Translate and rotate player
    ///		Updates animator
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            /*float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
			float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

			transform.Rotate(0, x, 0);
			transform.Translate(0, 0, z);

			if (Input.GetButtonDown("Fire1")) {
				CmdFire();
			}

			if (Input.GetButtonDown("Jump") && canJump) {
				_animator.SetTrigger("Jump");
			}

			_animator.SetFloat("Speed", z);*/

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Vector3 dir = (cam.right * Input.GetAxis("Horizontal") * Time.deltaTime) + (cam.forward * Input.GetAxis("Vertical") * Time.deltaTime);
            dir.y = 0;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                rigidBody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                rigidBody.velocity = transform.forward * speed;
                //_animator.SetTrigger("Running");;//Animation de course
                if(Input.GetAxis("Horizontal") != 0)
                {
                    _animator.SetFloat("Speed", /*Input.GetAxis("Horizontal")* */Time.deltaTime * speed);
                }

                if (Input.GetAxis("Vertical") != 0)
                {
                    _animator.SetFloat("Speed",/*Input.GetAxis("Vertical")* */Time.deltaTime * speed);
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Input.GetButtonDown("Fire1"))
            {
                CmdFire();
            }

            if (Input.GetButtonDown("Jump") && canJump && isGrounded)
            {
                _animator.SetTrigger("Jump");
            }

            if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            {
                //_animator.SetTrigger("Idle");;//animation Idle
                _animator.SetFloat("Speed", 0);
            }
        }
    }
 
    
    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag.Equals("Ground"))
        {
            isGrounded = true;
            Debug.Log(coll.collider.tag);
        }
    }

    /*void OnCollisionStay(Collision coll)
    {
        if (coll.collider.tag == "Ground")
        {
            isGrounded = true;
        }
    }*/

    void OnCollisionExit(Collision coll)
    {
        Debug.Log("sortir collision");
        if (isGrounded && coll.collider.tag.Equals("Ground"))
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

    
}