using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatAsset : ScriptableObject
{
    //STRING STATS ------------------------------------------

    public string defaultName;
    public string[] Abouts;
    public float pitch = 1;

    [Header("BATTLE")]
    public CatDynamicStats dynamicStats;
    public AttackType attackType;
    public SecondaryType secondaryType;
    public FactionType faction;
    public ActionType action;
    public ushort rarity;

    //SPRITES ------------------------------------------
    [Header("SPRITES")]
    public Sprite head;
    public Sprite meow, body, foot, tail;
    public Sprite bg, bullet;

    //SHOP INFO ------------------------------------------
    [Header("SHOP")]
    //from 3 to 10
    public ushort price;


}
