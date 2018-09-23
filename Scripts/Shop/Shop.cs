using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing;
public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    private DeckCard lastDC;
    [HideInInspector]
    private List<List<CatType>> catBuckets = null;
    public List<List<CatType>> getCatBuckets()
    {
        if (catBuckets == null)
        {
            catBuckets = DataUtils.getCatBuckets();
        }
        return catBuckets;
    }
    private void OnEnable()
    {
        ShowShop();
        GameControl.control.getBackButton().onClick.AddListener(() =>
   {
       HideShop();

       GameControl.control.getBackButton().onClick.RemoveAllListeners();
   });
    }

    #region SHOP HELPERS------
    private void ShowShop()
    {
        if (shopPanel.activeSelf)
        {
            return;
        }
        shopPanel.transform.localScale = Vector3.zero;
        shopPanel.transform.eulerAngles = new Vector3(0, 180, -90);

        shopPanel.SetActive(true);

        LeanTween.rotateZ(shopPanel, 0, 0.35f).setEaseInQuad();
        LeanTween.scale(shopPanel, Vector3.one, 0.35f).setEaseInQuad();
    }
    private void HideShop()
    {
        if (!shopPanel.activeSelf)
        {
            return;
        }
        LeanTween.rotateZ(shopPanel, -90, 0.35f).setEaseOutCubic();
        LeanTween.scale(shopPanel, Vector3.zero, 0.35f).setEaseOutCubic().setOnComplete(() =>
        {
            shopPanel.SetActive(false);
        });
    }
    private void buyCat(Cat cat)
    {
        if (cat.getCatAsset().price <= GameControl.control.playerData.gold)
        {
            GameControl.control.AddToDeck(cat, false);
            GameControl.control.DecrementGold(cat.getCatAsset().price);
            GameControl.control.Notify("You've collected " + cat.Name + "!", GameControl.control.transform).RewardBackground();
        }
    }
    public void ConfirmCatPurchase(Cat cat)
    {
        /*
            Buy <catType> for <X> gold?
            Yes     No
         */

        if (cat.getCatAsset().price > GameControl.control.playerData.gold)
        {
            GameControl.control.NotEnoughGoldPrompt(GameControl.control.transform);
            return;
        }
        GameControl.control.YesNoPrompt(LanguageSupport.BuyPrompt(cat.Name,
        CatIAP.goldStr + cat.getCatAsset().price.ToString()), GameControl.control.transform,
        () =>
        {
            GameControl.control.checkDeckAvailability(GameControl.control.transform,
                    (bool deckAvailable) =>
          {
              if (deckAvailable)
              {
                  buyCat(cat);
              }
          });

        }).RewardBackground();
    }

    #endregion
    #region ONCLICK LISTENERS --

    public void PickCat()
    {
        GameControl.control.getSoundManager().playButton();
        GameControl.control.getMainUI().SetPurchaseDeck();
        GameControl.control.getMainUI().deck.transform.parent.gameObject.SetActive(false);
        HideShop();
        gameObject.SetActive(false);
        GameControl.control.getMainUI().deck.transform.parent.gameObject.SetActive(true);
        GameControl.control.getBackButton().onClick.AddListener(() =>
        {
            GameControl.control.getMainUI().deck.ResetDeck();
            GameControl.control.getBackButton().onClick.RemoveAllListeners();
        });
    }

    public void RandomCat()
    {
        HideShop();
        CheckOperationWithBalance();
    }

    public void BuyCurrency()
    {
        GameControl.control.getSoundManager().playExploreButton();
        GameControl.control.CoinShopNoReturn();
    }

    #endregion


    #region RANDOM CAT -----------

    private void CheckOperationWithBalance()
    {
        if (GameControl.control.playerData.gold <= 0)
        {
            GameControl.control.NotEnoughGoldPrompt(GameControl.control.transform, ShowShop);
            GameControl.control.getSoundManager().playError();
            return;

        }
        GameControl.control.checkDeckAvailability(GameControl.control.transform,
         (bool deckAvailable) =>
              {
                  if (deckAvailable)
                  {
                      if (GameControl.control.playerData.gold <= 0)
                      {
                          GameControl.control.getSoundManager().playError();
                          return;

                      }
                      StartGachaAnimation();
                      YieldCat();
                      GameControl.control.DecrementGold(1);
                      GameControl.control.getSoundManager().playButton();
                  }
                  else
                  {
                      GameControl.control.getSoundManager().playError();
                  }
              }
        );
    }
    private void StartGachaAnimation()
    {
        GetComponent<Animator>().SetTrigger("coinDrop");
    }
    // animator event
    public void DisplayCard()
    {
        if (lastDC == null)
        {
            Debug.LogError("this should not even be called now");
            return;
        }

        GameControl.control.getMainUI().UpdateDetailCard(lastDC);
        lastDC = null;
        Button exit = GameControl.control.getMainUI().detailCard.transform.Find("exit").GetComponent<Button>();
        exit.onClick.AddListener(() =>
        {
            ShowShop();
            exit.onClick.RemoveAllListeners();
        });
    }

    // Naive rarity tiers
    // Attempt to roll for a uniform cat in a weighted bucket
    private CatType Yield()
    {
        List<List<CatType>> buckets = getCatBuckets();
        float p = UnityEngine.Random.Range(0.0f, 1.0f);
        int bucket = 0;
        float[] distribution = { 0.55f, 0.3f, 0.1f, 0.05f };
        for (int i = 0; i < buckets.Count; i++)
        {
            float bVal = distribution[i];
            bucket = i;
            if (p <= bVal)
            {
                break;
            }
            else
            {
                p -= bVal;
            }
        }
        int y = UnityEngine.Random.Range(0, buckets[bucket].Count);

        Debug.Log("bucket - " + bucket);
        Debug.Log("y = " + y);
        return buckets[bucket][y];
    }
    #endregion

    public void YieldCat()
    {
        // assumes player has enough gold
        CatType result = Yield();
        Cat newCat = new Cat(result);
        newCat.catLvl = new CatLevel(MathUtils.FairEnemyCatLevel(DataUtils.getTotalLevels(), MathUtils.progressThroughWorld(), UnityEngine.Random.Range(0.4f, 0.6f)));
        lastDC = GameControl.control.AddToDeck(newCat);
    }
}

