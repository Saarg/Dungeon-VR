using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public TrapType trapType = TrapType.Ground;
    public AnimatePosition animatePosition;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartTrap()
    {
        animatePosition.StartAnimation();
    }

    public void StopTrap()
    {

    }

    public void RemoveTrap()
    {
        // Give gold back
        Destroy(gameObject);
    }
}
