using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_UI : MonoBehaviour {

    private bool isShown = false;
    public bool IsShown {
        get
        {
            return isShown;
        }
        set
        {
            isShown = value;
            if(isShown == true)
            {
                this.GetComponent<Canvas>().enabled = true;
                headsetToTrack = GameObject.Find("Camera (eye)");
            }
            else
            {
                this.GetComponent<Canvas>().enabled = false;
            }
            
        }
    }

    private GameObject headsetToTrack;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (IsShown && headsetToTrack != null)
        {
            transform.LookAt(headsetToTrack.transform.position);
        }
	}
}
