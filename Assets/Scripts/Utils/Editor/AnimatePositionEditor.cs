using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimatePosition))]

/// <summary>  
/// 	AnimatePositionEditor adds a button to start animation from editor
/// </summary>
public class AnimatePositionEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AnimatePosition myScript = (AnimatePosition)target;
        if(EditorApplication.isPlaying && GUILayout.Button("Start"))
        {
            myScript.StartAnimation();
        }
    }
}
