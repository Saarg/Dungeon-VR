using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class VRSpellManager : NetworkBehaviour {
	static VRSpellManager instance;

	[Header("Used in editor")]
	[SerializeField]
	public List<GameObject> spells = new List<GameObject>();

	void Start()
	{
		instance = this;
	}

	public static void ThrowSpell(GameObject spellThrown) {
		if (!instance.isServer) {
			Debug.LogError("trying to spawn spell from non server");
			return;
		}

		spellThrown.GetComponent<Rigidbody>().useGravity = true;

		if (spellThrown.name.Contains("(Clone)"))
			spellThrown.name = spellThrown.name.Substring(0, spellThrown.name.Length - 7);

		Debug.Log(spellThrown.name);
		
		instance.RpcThrowSpell(spellThrown.name, spellThrown.transform.position, spellThrown.transform.rotation, spellThrown.GetComponent<Rigidbody>().velocity);
	}

	[ClientRpc]
	void RpcThrowSpell(string ressource, Vector3 position, Quaternion rotation, Vector3 velocity) {
		if (isServer) return;

		GameObject go = Resources.Load(ressource) as GameObject;
		go = Instantiate(go, position, rotation);

		Rigidbody rb = go.GetComponent<Rigidbody>();
		rb.useGravity = true;
		rb.velocity = velocity;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(transform.position, 1f);
	}
}
