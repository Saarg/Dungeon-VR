using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Wave {
	public GameObject[] enemy;

	public IEnumerator SpawnWave(Vector3 spawnPoint, int iteration){
        for (int i = 0; i < iteration; i++)
        {
            foreach (GameObject e in enemy) {
                GameObject enemy = GameObject.Instantiate(e, spawnPoint, Quaternion.identity);
                BaseAI ai = enemy.GetComponent<BaseAI>();

                NetworkServer.Spawn(enemy);
                yield return new WaitForSeconds(0.1f);
            }
        }
	}
}

public class WaveSpawner : NetworkBehaviour {
    [SerializeField]
    int IterationSpawn;
	[SerializeField]
    bool spawnEnemies;
    [SerializeField]
    float delay = 10f;
    float lastUpdate = 0f;
    [SerializeField]
    Wave[] waves;
    public Wave GetWave(int i) { return waves[i]; }
    public int GetWaveLenth() { return waves.Length; }
    [SerializeField]
    Transform spawnPoint;

	int curWave = 0;

    public override void OnStartServer()
	{
		if (GameManager.instance != null)
			GameManager.instance.onStartGame += StartSpawningForClients;
    }

	void Update () {
		UpdateMonster();
	}
    void UpdateMonster()
    {
        if (!spawnEnemies)
            return;

        if (isServer)
        { 
            if (Time.time - lastUpdate > delay || curWave == 0)
            {
                CmdSpawnWave();
                lastUpdate = Time.time;
            }
        }
    }
	
    [Command]
    void CmdSpawnWave()
    {
		if (curWave >= waves.Length)
			return;

		StartCoroutine(waves[curWave++].SpawnWave(spawnPoint.position, IterationSpawn));
    }

    public void StartSpawningForClients() {
        spawnEnemies = true;
    }
}
