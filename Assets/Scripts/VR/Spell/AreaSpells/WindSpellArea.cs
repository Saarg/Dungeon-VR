using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpellArea : AreaSpell {

    [Range(0, 500)]
    public int force = 50;

    // Use this for initialization
    protected override void Start () {
        base.Start();

	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();

	}

    protected override void StartAffect(Living living)
    {

    }

    protected override void Affect()
    {
        foreach(Living living in insideAreaLivings)
        {
            Vector3 pullingForce = force * (living.transform.position - this.transform.position);
            pullingForce.y = 0;
            living.GetComponent<Rigidbody>().AddForce(pullingForce);
        }
    }

    protected override void StopAffect(Living living)
    {

    }
}
