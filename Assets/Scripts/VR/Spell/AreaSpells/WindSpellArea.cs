using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WindSpellArea : AreaSpell {

    [Range(500, 2000)]
    public int force = 1000;

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
        NavMeshAgent nma = living.GetComponent<NavMeshAgent>();
        if (nma != null) {
            nma.enabled = false;
        }
    }

    protected override void Affect()
    {
        foreach(Living living in insideAreaLivings)
        {
            if (living.hasAuthority) {
                Vector3 dir = (living.transform.position - this.transform.position);
                dir.Normalize();

                Vector3 pullingForce = force * dir;
                pullingForce.y = 0;
                living.GetComponent<Rigidbody>().AddForce(living.transform.InverseTransformDirection(pullingForce));
            }
        }
    }

    protected override void StopAffect(Living living)
    {
        NavMeshAgent nma = living.GetComponent<NavMeshAgent>();
        if (nma != null) {
            nma.enabled = true;
        }
    }
}
