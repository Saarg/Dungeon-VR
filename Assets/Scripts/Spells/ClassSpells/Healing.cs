using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour {

    [SerializeField] float radius;
    [SerializeField] float factorHeal;
    [SerializeField] float duration;

    void Start () {
        InvokeRepeating("Heal", 0.0f,1.0f);
        StartCoroutine("HealingPeriode");
    }

    void Heal()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius);

        foreach (Collider col in hitColliders)
        {
            if(col.tag == "Player")
            {
                col.GetComponent<Living>().Heal((int)(col.GetComponent<Living>().maxLife / factorHeal));
            }
        }
    }

    void EndEffects()
    {
        Destroy(gameObject);
    }

    IEnumerator HealingPeriode()
    {
        yield return new WaitForSeconds(duration);
        EndEffects();
    }
}
