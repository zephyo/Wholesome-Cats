
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ExploreCat))]
public class ExploreCatEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        ExploreCat cat = (ExploreCat)target;

        // type = (CatType)EditorGUILayout.EnumPopup(type);
        if (GUILayout.Button("LOVE PARTICLE"))
        {
            cat.loveParticles();
        }
        if (GUILayout.Button("MAGIC PARTICLE"))
        {
            cat.Magic();
        }
        if (GUILayout.Button("FADE"))
        {
            cat.FadeAndAvoidHit();
        }
        if (GUILayout.Button("FLY"))
        {
            cat.FlyAndAvoidHit();
        }
        if (GUILayout.Button("SET TEXTURE"))
        {
            cat.sideEffect = new SideEffect(Fx.Scared, SecondaryType.Arrow);
            cat.setEffect();
        }


    }
}