using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSpell : MonoBehaviour {

    [Range(0,10)]
    public float LifeTime = 5f;
    private float remainingLifetime;

    //protected List<Player>

	// Use this for initialization
	protected virtual void Start () {
        remainingLifetime = LifeTime;
        StartCoroutine("AffectCoroutine");
    }

    // Update is called once per frame
    protected virtual void Update () {
        remainingLifetime -= Time.deltaTime;
        if(remainingLifetime <= 0)
        {
            StopCoroutine("AffectCoroutine");
            Destroy(this.gameObject);
        }
    }

    IEnumerator AffectCoroutine()
    {
        while (true)
        {
            //Affect all players in the zone
            Affect();
            yield return new WaitForSeconds(1);
        }
    }

    protected abstract void Affect();

}
