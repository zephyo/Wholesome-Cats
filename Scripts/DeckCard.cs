using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class DeckCard : MonoBehaviour
{

    public Cat cat;

    public void setName()
    {
        GameControl.GetTextBox(transform, "name").SetText(cat.Name);
    }

    public void setLvl()
    {
        transform.Find("lvlBG").GetChild(0).GetComponent<TextMeshProUGUI>().text = cat.catLvl.level.ToString();
    }

    public void setDeckCard(Cat cat, TextMeshProUGUI rarity)
    {
        this.cat = cat;
        setName();
        transform.GetChild(0).GetComponent<Image>().sprite = cat.getCatAsset().head;
        rarity.SetText(GameControl.GenerateStarText(cat.getCatAsset().rarity, false));
        setLvl();
    }


    public void setPurchaseDeckCard(Cat cat, TextMeshProUGUI rarity)
    {
        this.cat = cat;
        transform.GetChild(0).GetComponent<Image>().sprite = cat.getCatAsset().head;
        rarity.SetText(GameControl.GenerateStarText(cat.getCatAsset().rarity, false));
        setLvl();
        TextMeshProUGUI name = GameControl.GetTextBox(transform, "name");
        name.fontSizeMax = 105;
        name.text = "<font=\"pixelfont\" material=\"pixelfont_outline\">"+CatIAP.goldStr+cat.getCatAsset().price.ToString();
        name.color= new Color32(255,81,93,255);
        name.rectTransform.offsetMax = new Vector2(-70, name.rectTransform.offsetMax.y);
    }

    public void setListener(UnityAction UA)
    {
        Button b = transform.GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(()=>{
            UA();
            GameControl.control.getSoundManager().playButton();
        });
    }
}
