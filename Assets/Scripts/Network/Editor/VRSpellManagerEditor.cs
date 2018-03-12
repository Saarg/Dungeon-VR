using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRSpellManager))]
public class VRSpellManagerEditor : Editor {

	GameObject spell;
	Vector3 velocity;

	public override void OnInspectorGUI()
    {
        VRSpellManager myScript = (VRSpellManager)target;

		if (spell == null && myScript.spells.Count > 0) {
			spell = myScript.spells[0];
		}
		
		for(int i = 0; i < myScript.spells.Count; i++) {
			myScript.spells[i] = EditorGUILayout.ObjectField(myScript.spells[i].name, myScript.spells[i], typeof(GameObject), true) as GameObject;
		}
		GameObject newSpell = EditorGUILayout.ObjectField(null, typeof(GameObject), true) as GameObject;
		if (newSpell != null) {
			myScript.spells.Add(newSpell);
		}

		if (EditorApplication.isPlaying && myScript.isServer) {
			List<string> names = new List<string>();
			foreach (GameObject s in myScript.spells) { names.Add(s.name); }

			spell = myScript.spells[EditorGUILayout.Popup("Spell: ", myScript.spells.IndexOf(spell), names.ToArray(), EditorStyles.popup)];

			myScript.transform.position = EditorGUILayout.Vector3Field("Spawn point: ", myScript.transform.position);
			velocity = EditorGUILayout.Vector3Field("Velocity: ", velocity);

			if(GUILayout.Button("Spawn"))
			{
				GameObject go = Instantiate(spell, myScript.transform.position, Quaternion.identity);
				go.GetComponent<Rigidbody>().velocity = velocity;
				VRSpellManager.ThrowSpell(go);

				myScript.transform.position = Vector3.zero;
			}
		}
    }
}
