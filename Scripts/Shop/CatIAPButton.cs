using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
public class CatIAPButton : MonoBehaviour
{

    public string productID;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI priceText;
    private Product product;
    public Product getProduct()
    {
        if (product == null)
        {
            product = CodelessIAPStoreListener.Instance.GetProduct(productID);
        }
        return product;
    }
    private void Awake()
    {
        if (titleText != null)
        {
            titleText.text = getProduct().metadata.localizedTitle;
        }
        if (priceText != null)
        {
            priceText.text = getProduct().metadata.localizedPriceString;
        }
    }
}
