
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MainMenuCat))]
public class MainMenuCatEditor : Editor
{
    public CatType type = CatType.brown;
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        MainMenuCat cat = (MainMenuCat)target;
        type = (CatType)EditorGUILayout.EnumPopup(type);
        if (GUILayout.Button("SET CAT"))
        {
            Cat newCat = new Cat(type);
            newCat.SetCat(cat.transform.GetChild(0));
        }

    }
}