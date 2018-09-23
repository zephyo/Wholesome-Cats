using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class About : MonoBehaviour
{
    const string ABOUT_PREFAB_NAME = "miscPrefabs/About";
    public void InstantiateAbout()
    {
        GameObject GO = GameObject.Instantiate(Resources.Load<GameObject>(ABOUT_PREFAB_NAME), transform.root, false);
        setUpUI(GO.GetComponent<Image>());
    }
    void setUpUI(Image aboutBG)
    {
        if (UnityEngine.Random.value > 0.7f)
        {
            Sprite[] tiles = Resources.LoadAll<Sprite>("tiles");
            aboutBG.sprite = tiles[UnityEngine.Random.Range(0, tiles.Length)];
            //avoid transparent tiles
            while (aboutBG.sprite.name == "11" || aboutBG.sprite.name == "bearbunny"
            || aboutBG.sprite.name == "feather" || aboutBG.sprite.name == "leaf"
            || aboutBG.sprite.name == "leaf2"
            || aboutBG.sprite.name == "star4")
            {
                aboutBG.sprite = tiles[UnityEngine.Random.Range(0, tiles.Length)];
            }
        }
        else
        {
            //because of none asset
            aboutBG.sprite = Resources.Load<CatAsset>("CatAssets/" + ((CatType)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(CatType)).Length - 1))).ToString()).bg;

        }
    }
}
