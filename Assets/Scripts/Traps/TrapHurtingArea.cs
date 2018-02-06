using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 	Handle the area where players can be hurt
/// </summary>
public class TrapHurtingArea : MonoBehaviour {

	public DungeonTrap parent;

	void OnTriggerEnter(Collider _col){ parent.OnHurtingAreaTriggerEnter (_col);}
}
