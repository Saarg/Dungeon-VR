using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 	Handle the area in which the trap can be damaged	
/// </summary>
public class TrapDamageArea : MonoBehaviour {
	public DungeonTrap parent;

	void OnTriggerEnter(Collider _col){
        if(parent != null)
        {
            parent.OnDamageAreaTriggerEnter(_col);
        }        
    }
}
