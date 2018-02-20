using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
	[SerializeField] private float minValue = 0;
	[SerializeField] private float maxValue;
	[SerializeField] private float startValue = 0;
	[SerializeField] private float currentValue;

	[SerializeField] private bool complete = false;

	[SerializeField] private Slider slider;


	public float MinValue {
		get { return minValue; }
		set { minValue = value;	slider.minValue = value; }
	}

	public float MaxValue {
		get { return maxValue; }
		set { maxValue = value;	slider.maxValue = value; }
	}

	public float StartValue {
		get { return startValue; }
		set { startValue = value; }
	}

	public float CurrentValue {
		get { return currentValue; }
		set { currentValue = value;	slider.value = value; }
	}

	public bool Complete {
		get { return complete; }
		set { complete = value; }
	}

	void Start () {
		slider.minValue = minValue;
		slider.maxValue = maxValue;
		slider.value = startValue;
		slider.gameObject.SetActive (false);
	}

	void Update () {}

	public void Progress(float step){
		this.gameObject.SetActive (true);
		currentValue += step;
		slider.value = currentValue;

		if (currentValue > maxValue) {
			complete = true;
			this.gameObject.SetActive (false);
		}
	}
}