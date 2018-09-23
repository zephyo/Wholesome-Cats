using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSetUp : MonoBehaviour
{

    public MainMenuCat MainMenuCat;

    void Start()
    {
        if (GameControl.control.playerData == null || GameControl.control.playerData.deck.Count == 0)
        {
            //FTUE or sold all cats
            // MainMenuCat.gameObject.SetActive(false);
            return;
        }
        List<Cat> cats = GameControl.control.playerData.deck;
        uint catLayer = 0;
        MainMenuCat.gameObject.SetActive(true);
        MainMenuCat.init(cats[0], new Vector3(UnityEngine.Random.Range(-7f, 7f), MainMenuCat.transform.position.y, 0), "Cat" + catLayer++);
        float step = 1;
        if (cats.Count > 5)
        {
            step = cats.Count / 5 - (cats.Count / 15);
        }
        for (float i = 1; i < cats.Count; i += step)
        {
            Cat cat = cats[(int)i];

            if (UnityEngine.Random.value > 0.5f)
            {
                MainMenuCat MMCat = GameObject.Instantiate(MainMenuCat.gameObject, transform, false).GetComponent<MainMenuCat>();
                MMCat.init(cat, new Vector3(UnityEngine.Random.Range(-7f, 7f), MainMenuCat.transform.position.y, 0), "Cat" + catLayer);

                catLayer += 1;
                if (catLayer > 9)
                {
                    return;
                }

            }
        }
    }

}
