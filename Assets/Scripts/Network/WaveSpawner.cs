using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Wave {
	public GameObject[] enemy;

	public void SpawnWave(Vector3 spawnPoint) {
		foreach (GameObject e in enemy) {
			GameObject enemy = GameObject.Instantiate(e, spawnPoint, Quaternion.identity);
			BaseAI ai = enemy.GetComponent<BaseAI>();

			NetworkServer.Spawn(enemy);
		}
	}
}

public class WaveSpawner : NetworkBehaviour {
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

		waves[curWave++].SpawnWave(spawnPoint.position);
    }
}
