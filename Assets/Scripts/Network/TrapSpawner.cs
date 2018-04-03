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

	[SerializeField]
	List<GameObject> spawnableTraps = new List<GameObject>();
	GameObject GetTrap(string name) {
		if (name.Contains("(Clone)"))
			name = name.Substring(0, name.Length - 7);

		return spawnableTraps.Find(index => index.name.Equals(name));
	}

	[SerializeField]
	List<GameObject> spawnedTraps = new List<GameObject>();
	
	[SyncVar] public bool spawnForClients;

    [SerializeField]
    GameObject weaponPrefab;

    [SerializeField]
    List<WeaponSpec> weaponSpecs = new List<WeaponSpec>();

    [SerializeField]
    [Range(0.0f,1.0f)]
    float dropChance;

    void Awake()
	{
		if (NetworkManager.singleton is CustomNetworkManager) {
			(NetworkManager.singleton as CustomNetworkManager).playerConnectDelegate += AddClient;
		} else if (NetworkManager.singleton is LobbyManager) {
			(NetworkManager.singleton as LobbyManager).playerConnectDelegate += AddClient;
		}
	}

	public override void OnStartClient() {
		if (!isServer) {
			foreach (GameObject s in spawnedTraps) {
				Destroy(s);
			}

			spawnedTraps.Clear();
		}
	}

	public override void OnStartServer()
	{
		if (GameManager.instance != null)
			GameManager.instance.onStartGame += StartSpawningForClients;

		singleton = this;
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

	void RespawnForClient(NetworkConnection conn) {
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

    DungeonTrap Spawn(TrapSpawn t) {
		Debug.Log(t.path);
		GameObject go = GetTrap(t.path);

		if (go != null) {
			go = Instantiate(go, transform);

			Debug.Log(go.name);
			go.transform.position = t.position;
			go.transform.rotation = t.rotation;

			go.GetComponent<DungeonTrap>().isActive = spawnForClients;

			spawnedTraps.Add(go);

			return go.GetComponent<DungeonTrap>();
		} else {
			Debug.LogWarning(t.path + " not found");
			return null;
		}
	}

	public DungeonTrap AddTrap(TrapSpawn trap) {
		if (isServer) {
			return Spawn(trap);
		}
        else
        {
            return null;
        }
	}

	public void DestroyTrap(GameObject t) {
		if (!isServer)
			return;

		RpcDestroyTrap(spawnedTraps.FindIndex(x => x == t));
	}

	[ClientRpc]
	public void RpcDestroyTrap(int i) {
		GameObject go = spawnedTraps[i];

		spawnedTraps.RemoveAt(i);

		Destroy(go);
	}

	public void DamageTrap(GameObject t, float damage) {
		if (!isServer)
			return;
		
		RpcDamageTrap(spawnedTraps.FindIndex(x => x == t), damage);
	}

	[ClientRpc]
	public void RpcDamageTrap(int i, float damage) {
		GameObject go = spawnedTraps[i];

		go.GetComponent<DungeonTrap>().TakeDamage(damage);
	}

	public void AddClient(NetworkConnection conn) {
		StartCoroutine(WaitForConnectionIsReady(conn, () => {
			if (spawnForClients)
				RespawnForClient(conn);
			else
				TargetClearTrapsForClient(conn);
		}));
	}

	public void StartSpawningForClients() {
		spawnForClients = true;

		// ACTIVATE ALL TRAPS
		foreach (GameObject trap in spawnedTraps) {
			trap.GetComponent<DungeonTrap>().isActive = spawnForClients;
		}

		Respawn();
	}

	IEnumerator WaitForConnectionIsReady(NetworkConnection conn, Action cb) {
		while(!conn.isReady) {
			yield return new WaitForEndOfFrame();
		}
		cb();
	}

    public void SpawnWeapon(Vector3 position)
    {
        if (isServer)
            if (UnityEngine.Random.Range(0f, 1f) < dropChance)
                CmdSpawnWeapon(position);

    }

    [Command]
    void CmdSpawnWeapon(Vector3 position)
    {
        int specId = UnityEngine.Random.Range(0, weaponSpecs.Count);
        WeaponSpec spec = weaponSpecs[specId];
        GameObject weaponObj = Instantiate(weaponPrefab, position, Quaternion.identity);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        weapon.SetSpec(spec);
        NetworkServer.Spawn(weaponObj);
        RpcSpawnWeapon(weaponObj.GetComponent<NetworkIdentity>().netId,specId);
    }

    [ClientRpc]
    void RpcSpawnWeapon(NetworkInstanceId id, int specId)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        Weapon weapon = obj.GetComponent<Weapon>();
        weapon.SetSpec(weaponSpecs[specId]);
    }
}
