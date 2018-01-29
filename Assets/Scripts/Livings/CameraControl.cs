using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraControl : MonoBehaviour
{
    //Determines the limitations of vertical camera movement
    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 25.0f;

    public Transform character; 

    private float distance = - 5.0f; // Distance from character
    private float currentX = 0.0f; // Holds value of X mouse movement
    private float currentY = 0.0f; // Holds value of Y mouse movement
    
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
        gameObject.transform.position = character.position + Quaternion.Euler(currentY, currentX, 0) * new Vector3(0, 3, distance);
        gameObject.transform.LookAt(character.position);
    }
}
