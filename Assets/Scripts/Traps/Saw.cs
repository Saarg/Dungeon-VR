using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour {

	public SawbladeTrap parentTrap;

	void OnTriggerEnter(Collider col){ parentTrap.OnSawTriggerEnter (col); }
}
