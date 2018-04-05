using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSpawner : MonoBehaviour {

    public VR_Spell spellPrefab;
    public VR_Spell CurrentSpell { get; set; }

    [Range(0,60)]
    public float respawnTime = 10f;
    private float currentTimeBeforeRespawn = 0;
    private Transform spawnPoint;

    public Image progressBar;

    public static List<SpellSpawner> spawners = new List<SpellSpawner>();

    // Use this for initialization
    void Start () {
        spawners.Add(this);

        spawnPoint = transform.Find("SpawnPoint");
    }
	
	// Update is called once per frame
	void Update () {
		if(CurrentSpell == null)
        {
            if(currentTimeBeforeRespawn <= 0)
            {
                SpawnSpell();
                currentTimeBeforeRespawn = respawnTime;                
            }
            else
            {
                currentTimeBeforeRespawn -= Time.deltaTime;
                if (currentTimeBeforeRespawn <= 0)
                {
                    progressBar.fillAmount = 1;
                }
                else
                {
                    progressBar.fillAmount = 1 - this.currentTimeBeforeRespawn / this.respawnTime;
                }
            }
        }
	}

    private void SpawnSpell()
    {
        CurrentSpell = Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
        CurrentSpell.SpellSpawner = this;
    }

    public void SmallCooldown() {
        Destroy(CurrentSpell.gameObject);
        currentTimeBeforeRespawn = respawnTime/2f;
    }
}
