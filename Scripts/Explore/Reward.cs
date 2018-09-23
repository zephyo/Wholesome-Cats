using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Reward : MonoBehaviour
{
    public GameObject deckCard; //has a setcardsprites
    public bool Initiated = false;
    public void init()
    {
        Initiated = true;
    }

    public void initsilver(uint silver)
    {
        init();
        GetComponent<TextMeshProUGUI>().text = CatIAP.silverStr + silver.ToString() + "\nsilver";
        GameControl.control.IncrementSilver(silver);
    }
    public void initgold(uint gold)
    {
        init();
        GetComponent<TextMeshProUGUI>().text = CatIAP.goldStr + gold.ToString() + "\ngold";
        GameControl.control.IncrementGold(gold);
    }
    public void initcat(CatType catType)
    {
        Cat cat = new Cat(catType);
        GetComponent<TextMeshProUGUI>().text = "";
        Debug.Log("init cat reward!");
        GameControl.control.checkDeckAvailability(transform.root,
        ((bool deckAvailable) =>
        {
            if (deckAvailable)
            {
                init();
                cat.catLvl = new CatLevel(MathUtils.FairEnemyCatLevel(DataUtils.getTotalLevels(), MathUtils.progressThroughWorld(), UnityEngine.Random.Range(0.5f, 0.7f)));
                GameControl.control.AddToDeck(cat);
                DeckCard card = GameObject.Instantiate(deckCard, transform, false).GetComponent<DeckCard>();
                card.setDeckCard(cat, GameControl.GetTextBox(card.gameObject, "rarity"));
                card.transform.localScale = Vector3.one * 0.55f;
                deckCard.gameObject.SetActive(true);
                GameControl.control.SavePlayerData();
            }
        }), cat.Name + " wants to join! ");
    }
}
