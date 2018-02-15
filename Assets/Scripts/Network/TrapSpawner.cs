using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Lobby;

[Serializable]
public class TrapSpawn {
	public string path;
	public Vector3 position;
	public Quaternion rotation;
}

public class TrapSpawner : NetworkBehaviour {

	public static TrapSpawner singleton;

	public List<TrapSpawn> traps;
	[SerializeField]
	List<GameObject> spawnedTraps = new List<GameObject>();
	
	public bool spawnForClients;

	[Header("InEditorHelp")]
	public GameObject GM_UI;
	
	public override void OnStartClient() {
		if (GM_UI != null)
			GM_UI.SetActive(isServer);

		if (!isServer) {
			foreach (GameObject s in spawnedTraps) {
				Destroy(s);
			}

			spawnedTraps.Clear();
		}
	}

	public override void OnStartServer()
	{
		if (LobbyManager.instance != null)
			LobbyManager.instance.playerConnectDelegate += AddClient;

		singleton = this;

		foreach (TrapSpawn t in traps) {
			Spawn(t);
		}

		// NetworkServer.Spawn(gameObject);
	}

	void Update () {
		if (isServer && spawnForClients) {
			foreach (TrapSpawn t in traps) {
				RpcSpawnForClients(t);
			}

			traps.Clear();
		}
	}

	void Respawn() {
		if (!spawnForClients)
			return;

		RpcClearTrapsForClients();

		foreach(GameObject s in spawnedTraps) {
			TrapSpawn t = new TrapSpawn();

			t.path = s.name.Substring(0, s.name.Length - 7);
			t.position = s.transform.position;
			t.rotation = s.transform.rotation;

			RpcSpawnForClients(t);
		}
	}

	void Respawn(NetworkConnection conn) {
		if (!spawnForClients)
			return;

		TargetClearTrapsForClient(conn);

		foreach(GameObject s in spawnedTraps) {
			TrapSpawn t = new TrapSpawn();

			t.path = s.name.Substring(0, s.name.Length - 7);
			t.position = s.transform.position;
			t.rotation = s.transform.rotation;

			TargetSpawnForClient(conn, t);
		}
	}

	[ClientRpc]
	void RpcClearTrapsForClients() {
		if (isServer)
			return;

		foreach (GameObject s in spawnedTraps) {
			Destroy(s);
		}

		spawnedTraps.Clear();
	}

	[ClientRpc]
	public void RpcSpawnForClients(TrapSpawn trap) {
		if (isServer)
			return;

		Spawn(trap);
	}

	[TargetRpc]
	void TargetRespawnForClient(NetworkConnection conn) {
		if (!spawnForClients)
			return;

		TargetClearTrapsForClient(conn);

		foreach(GameObject s in spawnedTraps) {
			TrapSpawn t = new TrapSpawn();

			t.path = s.name.Substring(0, s.name.Length - 7);
			t.position = s.transform.position;
			t.rotation = s.transform.rotation;

			TargetSpawnForClient(conn, t);
		}
	}

	[TargetRpc]
	void TargetClearTrapsForClient(NetworkConnection conn) {
		if (isServer)
			return;

		foreach (GameObject s in spawnedTraps) {
			Destroy(s);
		}

		spawnedTraps.Clear();
	}

	[TargetRpc]
	public void TargetSpawnForClient(NetworkConnection conn, TrapSpawn trap) {
		if (isServer)
			return;

		Spawn(trap);
	}

	void Spawn(TrapSpawn t) {
		Debug.Log(t.path);
		GameObject go = Resources.Load(t.path) as GameObject;

		go = Instantiate(go, transform);

		go.transform.position = t.position;
		go.transform.rotation = t.rotation;

		go.GetComponent<DungeonTrap>().isActive = true;

		spawnedTraps.Add(go);
	}

	public void AddTrap(TrapSpawn trap) {
		if (isServer) {
			Spawn(trap);
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
			if (spawnForClients)
				TargetRespawnForClient(conn);
			else
				TargetClearTrapsForClient(conn);
		}));
	}

	public void StartSpawningForClients() {
		spawnForClients = true;
		Respawn();
	}

	IEnumerator WaitForConnectionIsReady(NetworkConnection conn, Action cb) {
		while(!conn.isReady) {
			yield return new WaitForEndOfFrame();
		}
		cb();
	}
}
