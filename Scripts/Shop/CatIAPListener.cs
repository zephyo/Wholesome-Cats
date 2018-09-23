using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
public class CatIAPListener : MonoBehaviour
{
    public void PurchaseSuccess(Product p)
    {
        uint goldAmt = CatIAP.getGoldFromProduct(p.definition.id);
        GameControl.control.IncrementGold(goldAmt);
        GameControl.control.Notify("<size=80%>Your purchase was successful!<line-height=170%></size>\n" + CatIAP.goldStr + goldAmt, GameControl.control.transform).RewardBackground();
    }

    public void PurchaseFailed(Product p, PurchaseFailureReason reason)
    {
        GameControl.control.getSoundManager().playError();
        string reasonString = "";
        switch (reason)
        {
            case PurchaseFailureReason.DuplicateTransaction:
                reasonString = "Duplicate transaction";
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                reasonString = "Existing purchase is pending";
                break;
            case PurchaseFailureReason.PaymentDeclined:
                reasonString = "Payment declined";
                break;
            case PurchaseFailureReason.ProductUnavailable:
                reasonString = "Product unavailable";
                break;
            case PurchaseFailureReason.PurchasingUnavailable:
                reasonString = "Cannot connect";
                break;
            case PurchaseFailureReason.SignatureInvalid:
                reasonString = "Signature validation failed";
                break;
            case PurchaseFailureReason.Unknown:
                reasonString = "Reason unknown";
                break;
            case PurchaseFailureReason.UserCancelled:
                reasonString = "Purchase was cancelled";
                break;
            default:
                break;
        }
        GameControl.control.Notify("Couldn't buy " + p.metadata.localizedTitle + ":\n" + reasonString + " :(" + reason.ToString(), GameControl.control.transform).RewardBackground();
        //Couldn't buy 5 coins:\nUser cancelled(‘д`)!
    }
}
