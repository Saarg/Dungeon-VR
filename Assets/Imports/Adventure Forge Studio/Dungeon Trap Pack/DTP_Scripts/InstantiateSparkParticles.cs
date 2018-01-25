using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSparkParticles : MonoBehaviour {

	public GameObject SparkParticle;
	public Transform Sawblade;
	GameObject SparkParticlesLocal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InstantiateParticles()
	{
		SparkParticlesLocal = Instantiate(SparkParticle, new Vector3(Sawblade.position.x, this.transform.position.y, Sawblade.position.z), Quaternion.identity, Sawblade);
	}

	public void DestroyParticles()
	{
		Destroy(SparkParticlesLocal);
	}
}
