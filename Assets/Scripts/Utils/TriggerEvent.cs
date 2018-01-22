using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

/// <summary>  
/// 	Link UnityEvents to trigger events with tag restriction and cooldowns
/// </summary>
/// <remarks>
/// 	Used by traps
/// </remarks>
public class TriggerEvent : MonoBehaviour {

	public UnityEvent onEnter;
	public UnityEvent onStay;
	public UnityEvent onExit;

	public string detectedTag = "";
	public float coolDown = 0f;
	private float lastEnterTrigger = 0f;
	private float lastStayTrigger = 0f;
	private float lastExitTrigger = 0f;

	/// <summary>
	/// 	Initialize timers
	/// </summary>
	void Start() {
		lastEnterTrigger = -coolDown;
		lastStayTrigger = -coolDown;
		lastExitTrigger = -coolDown;
	}

	/// <summary>
	/// 	OnTriggerEnter
	/// </summary>
	void OnTriggerEnter (Collider other) {
		if (Time.realtimeSinceStartup - lastEnterTrigger < coolDown) {
			return;
		}
		lastEnterTrigger = Time.realtimeSinceStartup;

		if (detectedTag == "" || other.tag == detectedTag) {
			onEnter.Invoke();
		}
	}

	/// <summary>
	/// 	OnTriggerStay
	/// </summary>
	void OnTriggerStay (Collider other) {
		if (Time.realtimeSinceStartup - lastStayTrigger < coolDown) {
			return;
		}
		lastStayTrigger = Time.realtimeSinceStartup;

		if (detectedTag == "" || other.tag == detectedTag) {
			onStay.Invoke();
		}
	}

	/// <summary>
	/// 	OnTriggerExit
	/// </summary>
	void OnTriggerExit (Collider other) {
		if (Time.realtimeSinceStartup - lastExitTrigger < coolDown) {
			return;
		}
		lastExitTrigger = Time.realtimeSinceStartup;

		if (detectedTag == "" || other.tag == detectedTag) {
			onExit.Invoke();
		}
	}
}
