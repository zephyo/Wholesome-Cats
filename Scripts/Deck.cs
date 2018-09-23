using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using System.Linq;
public class Deck : MonoBehaviour
{

    private uint EightSlotsPrice;

    [Header("Prefabs")]
    public GameObject deckCardPrefab;
    private void Awake()
    {
        ChangeGrid();
        setDeckUpgrade();
    }
    public void setDeckUpgrade()
    {
        EightSlotsPrice = GameControl.control.playerData.maxCats;
        GameControl.GetTextBox(transform.Find("more"), "text").text = "<b>upgrade</b>" +
        "\n<size=80%>(" + GameControl.control.playerData.maxCats + " to " + (GameControl.control.playerData.maxCats + 8) + " slots)</size><line-height=140%>" +
        "\n" + CatIAP.goldStr + EightSlotsPrice;
    }
    private void ChangeGrid()
    {
        GridLayoutGroup group = gameObject.GetComponent<GridLayoutGroup>();
        int divisor;
        if (Camera.main.aspect >= 2.1f)
        {
            divisor = 5;
            RectTransform scrollbar = (RectTransform)transform.parent.Find("scrollbar");
            scrollbar.sizeDelta = new Vector2(100, scrollbar.sizeDelta.y);
        }
        else if (Camera.main.aspect >= 1.49f)
        {
            divisor = 4;
        }
        else
        {
            divisor = 3;
            RectTransform scrollbar = (RectTransform)transform.parent.Find("scrollbar");
            scrollbar.anchoredPosition = new Vector2(-14.6f, scrollbar.anchoredPosition.y);

        }
        float X = ((RectTransform)group.transform).rect.size.x / divisor;
        group.cellSize = new Vector2(X, X / 1.08f);
    }

    #region DECK FUNCTIONS ------
    public void EnlargenDeck()
    {
        if (GameControl.control.playerData.gold < EightSlotsPrice)
        {
            GameControl.control.NotEnoughGoldPrompt(GameControl.control.transform);
            return;
        }
        GameControl.control.getSoundManager().playExploreButton();
        GameControl.control.YesNoPrompt("Buy 8 more slots with " + CatIAP.goldStr + EightSlotsPrice.ToString() + "?",
        GameControl.control.transform, () =>
        {
            GameControl.control.playerData.maxCats = GameControl.control.playerData.maxCats + 8;

            GameControl.control.DecrementGold(EightSlotsPrice);
            
            setDeckUpgrade();
        });
    }

    //change deck appearance to show all cats we can buy
    //see: shop -> PickCat()
    public void SetPurchaseDeck(Shop gacha)
    {
        ResetDeck();
        uint optimizedLevel = MathUtils.FairEnemyCatLevel(DataUtils.getTotalLevels(), MathUtils.progressThroughWorld(), 0.55f);
        Color cold = new Color32(247, 254, 255, 255);
        for (int i = 0; i < gacha.getCatBuckets().Count; i++)
        {
            List<Cat> rarity_i = new List<Cat>();
            foreach (CatType catType in gacha.getCatBuckets()[i])
            {
                Cat cat = new Cat(catType);
                cat.catLvl = new CatLevel(optimizedLevel);
                rarity_i.Add(cat);
            }
            //sort
            rarity_i = rarity_i.OrderBy(x => x.getCatAsset().price).ToList();
            foreach (Cat cat in rarity_i)
            {
                AddToPurchaseDeck(cat, () =>
               {
                   gacha.ConfirmCatPurchase(cat);
               }, cold);
            }

        }
        addExploreText();
    }
    private void addExploreText()
    {
        if (GameControl.control.getWorldLevel(ExploreController.lastWorld) > ExploreController.lastLevel)
        {
            //already unlocked all levels
            return;
        }
        //add explore for more
        TextMeshProUGUI explore = GameObject.Instantiate(transform.GetChild(transform.childCount - 1).Find("text").gameObject,
          transform, false).GetComponent<TextMeshProUGUI>();
        explore.text =
        "<font=\"cutefont\" material=\"cutefont_whiteoutline\"><b>explore</b> to unlock more cats!";
        explore.fontSizeMax = 75;
        explore.color = new Color32(89, 98, 125, 255);
    }
    private DeckCard AddToPurchaseDeck(Cat cat, UnityAction deckListener, Color32 cold)
    {
        GameObject card = (GameObject)Instantiate(deckCardPrefab, transform);
        card.GetComponent<Image>().color = cold;
        card.transform.SetSiblingIndex(transform.childCount - 2);
        DeckCard DC = card.GetComponent<DeckCard>();
        DC.setPurchaseDeckCard(cat, GameControl.GetTextBox(card, "rarity"));
        DC.setListener(deckListener);
        return DC;
    }

    public void SetDeck(Action onComplete, bool setListener)
    {
        if (transform.childCount - 1 == GameControl.control.playerData.deck.Count)
        {
            Debug.Log("saved some CPU; didn't set deck!");
            if (setListener)
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "more")
                    {
                        continue;
                    }
                    DeckCard DC = child.GetComponent<DeckCard>();
                    DC.setListener(DeckCardFunction(DC, GameControl.control.getMainUI().detailCard));
                }
            }
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }
        GameControl.control.StartCoroutine(hardResetDeck(onComplete, setListener));
    }
    private IEnumerator hardResetDeck(Action cb, bool setListener)
    {
        ResetDeck();
        yield return null;
        foreach (Cat cat in GameControl.control.playerData.deck)
        {
            AddToDeck(cat, setListener);
        }
        if (cb != null)
        {
            cb();
        }
    }

    public DeckCard AddToDeck(Cat cat, bool setListener)
    {
        DeckCard DC = newDeckCard(cat);
        if (setListener)
        {
            DC.setListener(DeckCardFunction(DC, GameControl.control.getMainUI().detailCard));
        }
        return DC;
    }
    private UnityAction DeckCardFunction(DeckCard card, DetailCard detailCard)
    {
        return () =>
           {
               detailCard.setDetailCard(card);
               transform.parent.gameObject.SetActive(false);
           };
    }

    private DeckCard newDeckCard(Cat cat)
    {
        GameObject card = (GameObject)Instantiate(deckCardPrefab, transform);
        card.transform.SetSiblingIndex(transform.childCount - 2);
        DeckCard DC = card.GetComponent<DeckCard>();
        DC.setDeckCard(cat, GameControl.GetTextBox(card, "rarity"));
        return DC;
    }
    public void RemoveFromDeck(Cat cat)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "more")
            {
                continue;
            }
            if (child.GetComponent<DeckCard>().cat == cat)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    public void ResetDeck()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "more")
            {
                continue;
            }
            Destroy(child.gameObject);
        }
    }
    #endregion
}
