using UnityEngine;

public class EnemyDamageTest : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            var comp = col.gameObject.GetComponent<Living>();
            if (comp != null)
                comp.TakeDamage(18, Bullet.DamageTypeEnum.fire );
        }
    }
}
