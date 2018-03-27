using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSpell : MonoBehaviour {

    protected List<Living> insideAreaLivings;

    [Range(0,10)]
    public float LifeTime = 5f;
    private float remainingLifetime;

    public float tickTime = 1f;
    public bool tickOnFixedUpdate = false;

    //protected List<Player>

	// Use this for initialization
	protected virtual void Start () {
        insideAreaLivings = new List<Living>();
        remainingLifetime = LifeTime;
        StartCoroutine("AffectCoroutine");
    }

    // Update is called once per frame
    protected virtual void Update () {
        remainingLifetime -= Time.deltaTime;
        if(remainingLifetime <= 0)
        {
            StopCoroutine("AffectCoroutine");
            foreach(Living liv in insideAreaLivings)
            {
                StopAffect(liv);
            }
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        Living living = collider.GetComponent<Living>();
        if (living != null)
        {
            insideAreaLivings.Add(living);
            StartAffect(living);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        Living living = collider.GetComponent<Living>();
        if (living != null)
        {
            insideAreaLivings.Remove(living);
            StopAffect(living);
        }
    }

    IEnumerator AffectCoroutine()
    {
        while (true)
        {
            //Affect all players in the zone
            Affect();

            if (tickOnFixedUpdate)
                yield return new WaitForFixedUpdate();
            else                
                yield return new WaitForSeconds(tickTime);
        }
    }

    protected abstract void StartAffect(Living living);
    protected abstract void Affect();
    protected abstract void StopAffect(Living living);

}
