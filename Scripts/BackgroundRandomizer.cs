using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BackgroundRandomizer : MonoBehaviour
{

    void Start()
    {
        RandomizeBackground();
    }

    public void RandomizeBackground()
    {
        Sprite[] tiles = Resources.LoadAll<Sprite>("tiles");
        GetComponent<Image>().sprite = tiles[Random.Range(0, tiles.Length)];
    }

}
