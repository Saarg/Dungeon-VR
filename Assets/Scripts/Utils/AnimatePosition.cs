using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>  
/// 	This class is used to animate GameObjects
///		The objects can be animated as loops or as single time animations
/// </summary>
/// <remarks>
/// 	Used by traps
/// </remarks>
public class AnimatePosition : MonoBehaviour {

	public AnimationCurve posXCurve = AnimationCurve.Constant(0, 1, 0);
	public AnimationCurve posYCurve = AnimationCurve.Constant(0, 1, 0);
	public AnimationCurve posZCurve = AnimationCurve.Constant(0, 1, 0);

	public AnimationCurve rotXCurve = AnimationCurve.Constant(0, 1, 0);
	public AnimationCurve rotYCurve = AnimationCurve.Constant(0, 1, 0);
	public AnimationCurve rotZCurve = AnimationCurve.Constant(0, 1, 0);

	public bool running = false;
	public float animLength = 1f;
	public float refreshTime = 0.01f;

	public bool loop = false;
	public float offset = 0;
	public bool requestedStop = false;
	public bool runOnStart = false;
	public bool runOnEnable = false;

	public bool enableOnStart = true;
	public bool disableOnStop = false;	

	Vector3 startPos;
	Vector3 startRot;

	/// <summary>  
	///		Start the animation if runOnStart is true
	/// </summary> 
	void Start() {
		if (runOnStart) {
			StartAnimation();
		}
	}

	/// <summary>  
	///		OnEnable the animation if runOnEnable is true
	/// </summary> 
	void OnEnable() {
		if (runOnEnable) {
			StartAnimation();
		}
	}

	/// <summary>
	/// 	This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable()
	{
		if (running) {
			transform.localPosition = startPos;
			transform.localRotation = Quaternion.Euler(startRot);
		}

		running = false;
	}

	/// <summary>  
	/// 	Run the animation if not already running
	/// </summary> 
	public void StartAnimation () {
		if (enableOnStart)
			gameObject.SetActive(true);

		if (!running)
			StartCoroutine(RunAnimation());
	}

	/// <summary>  
	/// 	Stop the animation if it's running in a loop
	/// </summary> 
	public void StopAnimation () {
		requestedStop = true;
	}

	/// <summary>  
	/// 	Coroutine running the animation
	/// </summary> 
	IEnumerator RunAnimation () {
		startPos = transform.localPosition;
		startRot = transform.localRotation.eulerAngles;

		float startTime = Time.realtimeSinceStartup + offset;
		running = true;

		while ((Time.realtimeSinceStartup - startTime) / animLength < 1f || (loop && !requestedStop)) {
			float curTime = ((Time.realtimeSinceStartup - startTime) / animLength) % 1f;

			transform.localPosition = startPos + new Vector3(posXCurve.Evaluate(curTime), posYCurve.Evaluate(curTime), posZCurve.Evaluate(curTime));
			transform.localRotation = Quaternion.Euler(startRot + new Vector3(rotXCurve.Evaluate(curTime), rotYCurve.Evaluate(curTime), rotZCurve.Evaluate(curTime)));

			yield return new WaitForSeconds(refreshTime);
		}

		running = false;
		requestedStop = false;

		if (disableOnStop)
			gameObject.SetActive(false);
		
		transform.localPosition = startPos;
		transform.localRotation = Quaternion.Euler(startRot);
	}
}
