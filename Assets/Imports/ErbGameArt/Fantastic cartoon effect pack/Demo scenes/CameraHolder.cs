using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    //camera holder
    public Transform Holder;
    public float currDistance = 5.0f;
    public float xRotate = 250.0f;
    public float yRotate = 120.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float prevDistance;
    private float x = 0.0f;
    private float y = 0.0f;
    //GUI
    float windowDpi;
    private GUIStyle screenGUI = new GUIStyle();
    public GameObject[] Prefabs;
    private int Prefab;
    private GameObject Instance;

    void Start ()
    {
        if (Screen.dpi < 1) windowDpi = 1;
        if (Screen.dpi < 200) windowDpi = 1;
        else windowDpi = Screen.dpi / 200f;
        screenGUI.fontSize = (int)(15f * windowDpi);
        screenGUI.normal.textColor = new Color(0.5f, 0f, 0f);
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        Counter(0);
    }
	
    private void OnGUI()
    {
        if (GUI.Button(new Rect(5 * windowDpi, 5 * windowDpi, 110 * windowDpi, 30 * windowDpi), "Previous effect"))
        {
            Counter(-1);
        }
        if (GUI.Button(new Rect(120 * windowDpi, 5 * windowDpi, 110 * windowDpi, 30 * windowDpi), "Next effect"))
        {
            Counter(+1);
        }
    }

    void Counter(int count)
    {
        Prefab += count;
        if (Prefab > Prefabs.Length - 1)
        {
            Prefab = 0;
        }
        else if (Prefab < 0)
        {
            Prefab = Prefabs.Length - 1;
        }
        if (Instance != null) Destroy(Instance);
        {
            Instance = Instantiate(Prefabs[Prefab]);
        }
    }

    void LateUpdate ()
    {
        if (currDistance < 2)
        {
            currDistance = 2;
        }
        currDistance -= Input.GetAxis("Mouse ScrollWheel") * 2;
        if (Holder && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            var pos = Input.mousePosition;
            float dpiScale = 1;
            if (Screen.dpi < 1) dpiScale = 1;
            if (Screen.dpi < 200) dpiScale = 1;
            else dpiScale = Screen.dpi / 200f;  
            if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            x += (float)(Input.GetAxis("Mouse X") * xRotate * 0.02);
            y -= (float)(Input.GetAxis("Mouse Y") * yRotate * 0.02);
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * new Vector3(0, 0, -currDistance) + Holder.position;
            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (prevDistance != currDistance)
        {
            prevDistance = currDistance;
            var rot = Quaternion.Euler(y, x, 0);
            var po = rot * new Vector3(0, 0, -currDistance) + Holder.position;
            transform.rotation = rot;
            transform.position = po;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
