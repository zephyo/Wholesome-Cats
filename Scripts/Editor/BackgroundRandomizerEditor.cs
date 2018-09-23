
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(BackgroundRandomizer))]
public class BackgroundRandomizerEditor : Editor
{

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        BackgroundRandomizer BG = (BackgroundRandomizer)target;
        if (GUILayout.Button("RANDOMIZE BACKGROUND/TILE"))
        {
		    BG.RandomizeBackground();
        }

    }
}