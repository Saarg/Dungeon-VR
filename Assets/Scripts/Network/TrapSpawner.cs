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
	
	[SyncVar] public bool spawnForClients;
	
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

		foreach (TrapSpawn t in traps) {
			Spawn(t);
		}
	}

	void Update () {
		if (isServer && spawnForClients) {
			foreach (TrapSpawn t in traps) {
				RpcSpawnForClients(t);
			}

			traps.Clear();
		}
        
        //To do Move to an EnemiesSpawner
        UpdateMonster();
	}

    ////////////////////////////////////////
    //To do Move to an EnemiesSpawner
    [SerializeField]
    bool spawnEnemies;
    [SerializeField]
    float delay = 10f;
    [SerializeField]
    int enemiesPerWave;
    float lastUpdate = 0f;
    [SerializeField]
    GameObject enemyToSpawn;
    [SerializeField]
    Transform spawnPoint;
    void UpdateMonster()
    {
        if (!spawnEnemies)
            return;

        if (isServer)
        { 
            if (Time.time - lastUpdate > delay)
            {
                for(int i = 0; i < enemiesPerWave;i++)
                    CmdSpawnMonster();
                lastUpdate = Time.time;
            }
        }
    }

    [Command]
    void CmdSpawnMonster()
    {
        GameObject enemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        BaseAI ai = enemy.GetComponent<BaseAI>();
        NetworkServer.Spawn(enemy);
        ai.SetShooter(UnityEngine.Random.Range(0, 2) == 0);
    }
    /////////////////////////////////////

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
		GameObject go = Resources.Load(t.path) as GameObject;

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
}
