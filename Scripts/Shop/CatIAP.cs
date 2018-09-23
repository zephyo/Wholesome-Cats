using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class CatIAP : MonoBehaviour
{
    public const string silverStr = "<sprite=1>";
    public const string goldStr = "<sprite=2>";

    #region CONFIRMATION PROMPTS ----

    public void buttonError()
    {
        GameControl.control.getSoundManager().playError();
    }
    private void ConfirmSilverForGold(uint silver, uint gold)
    {
        /*
                  Exchange <x> silver for <y> gold? 
                  Yes     No
        */

        GameControl.control.YesNoPrompt(LanguageSupport.ExchangePrompt(
            silverStr + silver.ToString(),
             goldStr + gold.ToString()), GameControl.control.transform, () =>
         {
             // on YES 
             GameControl.control.DecrementSilver(silver);
             GameControl.control.IncrementGold(gold);
         }).RewardBackground();
    }
    // private void ConfirmGoldForSilver(uint gold, uint silver)
    // {
    //     GameControl.control.YesNoPrompt(LanguageSupport.ExchangePrompt(
    //       goldStr + gold.ToString(),
    //        silverStr + silver.ToString()
    //       ), GameControl.control.transform, () =>
    //      {
    //          // on YES 
    //          GameControl.control.DecrementGold(gold);
    //          GameControl.control.IncrementSilver(silver);
    //      });
    // }

    private void ConfirmGoldPurchase(uint gold, Product product)
    {
        /*
                  Buy <x> gold for <y>?
                  Yes     No
        */

        GameControl.control.YesNoPrompt(LanguageSupport.BuyPrompt(goldStr + gold.ToString(),
        product.metadata.localizedPriceString
        ), GameControl.control.transform, () =>
         {
             // on YES 
             Debug.Log("purchasing: " + product.definition.id);

             CodelessIAPStoreListener.Instance.InitiatePurchase(product.definition.id);

         }).RewardBackground();
        //  add iap button to yes button. yes -> set gold, no -> bleh; PurchaseFailed listener
    }
    #endregion
    public static uint getGoldFromProduct(string id)
    {
        switch (id)
        {
            case "1coin":
                return 1;
            case "5coins":
                return 5;
            case "15coins":
                return 15;
            case "40coins":
                return 40;
        }
        return 0;
    }

    #region ONCLICK LISTENERS --
    public void AddGold(CatIAPButton b)
    {
        GameControl.control.getSoundManager().playExploreButton();
        uint goldAmt = getGoldFromProduct(b.productID);

        ConfirmGoldPurchase(goldAmt, b.getProduct());
    }
    public void ExchangeSilverForGold(ExchangeButton EB)
    {
        if (GameControl.control.playerData.silver < EB.silver)
        {
           GameControl.control.Notify(LanguageSupport.NotEnoughSilver(), GameControl.control.transform, null, true).RewardBackground();
            return;
        }

        GameControl.control.getSoundManager().playExploreButton();
        ConfirmSilverForGold(EB.silver, EB.gold);
    }
    // public void ExchangeGoldForSilver(ExchangeButton EB)
    // {
    //     if (GameControl.control.playerData.gold < EB.gold)
    //     {
    //         GameControl.control.NotEnoughGoldPrompt();
    //         return;
    //     }
    //     ConfirmGoldForSilver(EB.gold, EB.silver);
    // }

    public void W2EButton()
    {
        GameControl.control.getSoundManager().playNotif();
        Instantiate(Resources.Load<GameObject>("miscPrefabs/W2EPopup"), transform.root, false);
    }
    #endregion


}
