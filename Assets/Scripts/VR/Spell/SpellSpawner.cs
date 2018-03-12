using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSpawner : MonoBehaviour {

    public VR_Spell spellPrefab;
    public VR_Spell CurrentSpell { get; set; }

    [Range(0,60)]
    public float RespawnTime = 10f;
    private float currentTimeBeforeRespawn = 0;
    private Transform spawnPoint;


    // Use this for initialization
    void Start () {
        spawnPoint = transform.Find("SpawnPoint");
    }
	
	// Update is called once per frame
	void Update () {
		if(CurrentSpell == null)
        {
            if(currentTimeBeforeRespawn <= 0)
            {
                SpawnSpell();
                currentTimeBeforeRespawn = RespawnTime;
            }
            else
            {
                currentTimeBeforeRespawn -= Time.deltaTime;
            }
        }
	}

    private void SpawnSpell()
    {
        CurrentSpell = Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
        CurrentSpell.SpellSpawner = this;
    }
}
