using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>  
/// 	Camera control
/// 	Distance from the followed player and angle of vision
/// </summary>
public class CameraControl : MonoBehaviour
{
    //Determines the limitations of vertical camera movement
    private const float Y_ANGLE_MIN = -30.0f;
    private const float Y_ANGLE_MAX = 30.0f;

    public Transform character; //character followed by the camera

    public Vector3 distance = new Vector3(0, 2, -4); // Distance from character
    private float currentX = 0.0f; // Holds value of X mouse movement
    private float currentY = 0.0f; // Holds value of Y mouse movement

    public Vector3 viewOffset;
    
    void Start(){

    }

    void Update()
    {
         currentX += Input.GetAxis("Mouse X");
         currentY += Input.GetAxis("Mouse Y");
         currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    void LateUpdate()
    {                
        gameObject.transform.position = character.position + Quaternion.Euler(currentY, currentX, 0) * distance;
        gameObject.transform.LookAt(character.position + character.InverseTransformDirection(viewOffset));
    }
}
