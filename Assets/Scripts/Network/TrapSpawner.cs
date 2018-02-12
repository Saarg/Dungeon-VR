using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class TrapSpawn {
	public string path;
	public Vector3 position;
	public Vector3 rotation;
}

public class TrapSpawner : NetworkBehaviour {

	public static TrapSpawner singleton;

	public List<TrapSpawn> traps;
	List<GameObject> spawnedTraps = new List<GameObject>();

	public bool spawnForClients;

	public override void OnStartServer()
	{
		singleton = this;

		foreach (TrapSpawn t in traps) {
			Spawn(t);
		}

		NetworkServer.Spawn(gameObject);
	}

	void Update () {
		if (isServer && spawnForClients) {
			foreach (TrapSpawn t in traps) {
				RpcSpawnForclients(t);
			}

			traps.Clear();
		}
	}

	void Respawn() {
		RpcClearTraps();

		foreach(GameObject s in spawnedTraps) {
			TrapSpawn t = new TrapSpawn();

			t.path = s.name.Substring(0, s.name.Length - 7);
			t.position = s.transform.position;
			t.rotation = s.transform.rotation.eulerAngles;

			RpcSpawnForclients(t);
		}
	}

	[ClientRpc]
	void RpcClearTraps() {
		if (isServer)
			return;

		foreach (GameObject s in spawnedTraps) {
			Destroy(s);
		}

		spawnedTraps.Clear();
	}

	[ClientRpc]
	public void RpcSpawnForclients(TrapSpawn trap) {
		if (isServer)
			return;

		Spawn(trap);
	}

	void Spawn(TrapSpawn t) {
		GameObject go = Resources.Load(t.path) as GameObject;

		go = Instantiate(go, t.position, Quaternion.Euler(t.rotation));

		go.GetComponent<DungeonTrap>().isActive = true;

		spawnedTraps.Add(go);
	}

	public void AddTrap(TrapSpawn trap) {
		if (isServer) {
			if (spawnForClients) {
				RpcSpawnForclients(trap);
			} else {
				traps.Add(trap);
			}
		}
	}

	public void DestroyTrap(GameObject t) {
		RpcDestroyTrap(spawnedTraps.FindIndex(x => x == t));
	}

	[ClientRpc]
	public void RpcDestroyTrap(int i) {
		GameObject go = spawnedTraps[i];

		spawnedTraps.RemoveAt(i);

		Destroy(go);
	}

	public void DamageTrap(GameObject t, float damage) {
		RpcDamageTrap(spawnedTraps.FindIndex(x => x == t), damage);
	}

	[ClientRpc]
	public void RpcDamageTrap(int i, float damage) {
		GameObject go = spawnedTraps[i];

		go.GetComponent<DungeonTrap>().Damage(damage);
	}

	public void AddClient(NetworkConnection conn) {
		StartCoroutine(WaitForConnectionIsReady(conn, () => {
			Respawn();
		}));
	}

	IEnumerator WaitForConnectionIsReady(NetworkConnection conn, Action cb) {
		while(!conn.isReady) {
			yield return new WaitForEndOfFrame();
		}
		cb();
	}
}
