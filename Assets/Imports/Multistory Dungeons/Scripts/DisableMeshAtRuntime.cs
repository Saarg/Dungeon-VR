using UnityEngine;
using System.Collections;

namespace commanastationwww.multistorydungeons{

public class DisableMeshAtRuntime : MonoBehaviour {

	// Use this for initialization
	void Start () {
	GetComponent<Renderer>().enabled = false;
	}
	
}
}
