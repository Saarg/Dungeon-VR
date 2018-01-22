using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DoorTrigerHingesData {
	public HingeJoint joint;
	public float openAngle = 90f;	
}

/// <summary>  
/// 	Open/Close HingeJoints OnTriggerEnter
/// </summary>
public class DoorTriger : MonoBehaviour {

	public DoorTrigerHingesData[] doors;

	public string detectedTag = "";
	public float coolDown = 0f;
	private float lastEnterTrigger = 0f;

	/// <summary>  
	/// 	Init cooldown timers
	/// </summary>
	void Start() {
		lastEnterTrigger = -coolDown;
	}

	/// <summary>  
	/// 	OnTriggerEnter
	/// </summary>
	void OnTriggerEnter(Collider col) {
		if (Time.realtimeSinceStartup - lastEnterTrigger < coolDown) {
			return;
		}
		lastEnterTrigger = Time.realtimeSinceStartup;

		if (detectedTag == "" || col.tag == detectedTag) {
			foreach (DoorTrigerHingesData door in doors)
			{
				JointSpring s = door.joint.spring;

				s.targetPosition = door.openAngle;

				door.joint.spring = s;
			}
		}
	}
}
