
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DetailCard))]
public class DetailCardEditor : Editor
{
   public CatType type = CatType.artist;
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        DetailCard card = (DetailCard)target;
        type = (CatType)EditorGUILayout.EnumPopup(type);
        if (GUILayout.Button("SET DETAIL CARD"))
        {
            Cat cat = new Cat(type);

            card.setDetailCard(cat);
            card.transform.Find("about").localScale = Vector3.one;
        }

    }
}