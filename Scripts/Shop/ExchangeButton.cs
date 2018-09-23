using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeButton : MonoBehaviour
{

    public uint silver;
    public uint gold;
    private void Awake()
    {
        SetButton();
    }
    public void SetButton()
    {
       
            GameControl.GetTextBox(gameObject, "price").text = CatIAP.silverStr + silver.ToString();
			GameControl.GetTextBox(gameObject, "amount").text = gold.ToString()+"x";
  
    }
}
