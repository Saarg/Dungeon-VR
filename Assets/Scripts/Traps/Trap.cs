using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Trap : MonoBehaviour {

    public struct TrapEventArgs
    {

    }

    public delegate void TrapEventHandler(TrapEventArgs e);

    public event TrapEventHandler TrapRemoved;

    public TrapType trapType = TrapType.Ground;

    public abstract void StartTrap();

    public abstract void StopTrap();

    public void DestroyTrap()
    {
        if (TrapRemoved != null)
            TrapRemoved(new TrapEventArgs());

        if (TrapSpawner.singleton != null)
            TrapSpawner.singleton.DestroyTrap(this.gameObject);

        StartCoroutine("DestroyAfterThrowing");
    }

    public IEnumerator DestroyAfterThrowing()
    {
        yield return new WaitForSeconds(3);
        TrapSpawner.singleton.DestroyTrap(this.gameObject);
    }
}
