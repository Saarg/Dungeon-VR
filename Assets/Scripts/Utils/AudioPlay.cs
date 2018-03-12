using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlay : MonoBehaviour {

	AudioSource source;

	void Start () {
		source = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		if (source != null)
			source.Play();
	}
}
