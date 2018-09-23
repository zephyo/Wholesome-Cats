using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class DetailCard : MonoBehaviour
{
    public Transform deckCat;
    private DeckCard deckCard;
    public Transform bullets;
    bool leveledUp = false;

    private Button levelUp;
    private Button getLevelUp()
    {
        if (levelUp == null)
        {
            levelUp = transform.Find("lvlUp").gameObject.GetComponent<Button>();
        }
        return levelUp;
    }
    private TextMeshProUGUI stats;
    private TextMeshProUGUI getStats()
    {
        if (stats == null)
        {
            stats = GameControl.GetTextBox(transform.Find("statgrid"), "stats");
        }
        return stats;
    }
    private TextMeshProUGUI lvl;
    private TextMeshProUGUI getLvl()
    {
        if (lvl == null)
        {
            lvl = GameControl.GetTextBox(transform, "lvl");
        }
        return lvl;
    }

    //  const int multiplier = 2;
    public void InteractCat(Animator a)
    {
        RandomCatNoises rc = a.GetComponent<RandomCatNoises>();
        if (rc.enableNoises)
        {
            return;
        }
        rc.catAsset = deckCard.cat.getCatAsset();
        rc.enableNoises = true;
        switch (deckCard.cat.getCatAsset().attackType)
        {
            case AttackType.Melee:
                a.SetTrigger("swipe");
                break;
            case AttackType.Ranged:
                a.SetTrigger("throw");
                break;
            default:
                break;
        }
        StartCoroutine(TurnOffMeow(a, rc));
    }
    private IEnumerator TurnOffMeow(Animator a, RandomCatNoises rc)
    {
        yield return new WaitForSeconds(0.9f);
        rc.enableNoises = false;
    }
    public void changeName(string name)
    {
        deckCard.cat.Name = name;
        deckCard.setName();
    }
    public void sell()
    {
        ushort price = MathUtils.getSellingPrice(deckCard.cat.getCatAsset().price);
        string prompt;
        string soldStr;
        if (price == 0)
        {
            soldStr = "set free!";
            prompt = "Set " + deckCard.cat.Name + " out into the wild?";
        }
        else
        {
            soldStr = "sold!";
            prompt = "Sell " + deckCard.cat.Name + " for " + CatIAP.goldStr + price.ToString() + "?";
        }
        GameControl.control.YesNoPrompt(prompt, GameControl.control.transform, () =>
           {
               GameControl.control.RemoveFromDeck(deckCard.cat);
               //sold animation -big red SOLD slams down on card
               TextMeshProUGUI text = GameObject.Instantiate(getLvl().gameObject, GameControl.control.transform, false).GetComponent<TextMeshProUGUI>();
               text.gameObject.name = "sold";
               Button exit = transform.Find("exit").GetComponent<Button>();
               exit.onClick.AddListener(DestroyText);
               GameControl.control.getBackButton().onClick.AddListener(DestroyText);
               text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
               text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
               text.rectTransform.sizeDelta = new Vector2(1617.6f, 1043.1f);
               text.rectTransform.anchoredPosition = new Vector2(-86.5f, 0);
               text.transform.eulerAngles = new Vector3(0, 0, 13);
               text.color = new Color32(252, 91, 110, 255);
               text.alignment = TextAlignmentOptions.Center;
               text.text = soldStr;
               text.fontSize = 200;
               text.raycastTarget = true;
               text.transform.localScale = Vector3.one * 3;
               LeanTween.scale(text.gameObject, Vector3.one, 0.5f).setEaseInCubic();

               if (price > 0)
               {
                   GameControl.control.IncrementGold(price);
               }
               else
               {
                   GameControl.control.SavePlayerData();
               }

           }, null, "yes", true);
    }
    private void DestroyText()
    {
        Destroy(GameControl.control.transform.Find("sold").gameObject);
        GameControl.control.getBackButton().onClick.RemoveListener(DestroyText);
        transform.Find("exit").GetComponent<Button>().onClick.RemoveListener(DestroyText);
    }


    public void setDetailCard(DeckCard deckCard)
    {
        this.deckCard = deckCard;
        leveledUp = false;
        setUI(deckCard.cat);
        setInfo(deckCard.cat);
        setStats(deckCard.cat);
        gameObject.SetActive(true);
    }
#if UNITY_EDITOR
    public void setDetailCard(Cat cat)
    {
        setUI(cat);
        setInfo(cat);
        setStats(cat);
    }
#endif
    private void setInfo(Cat cat)
    {
        transform.Find("name").GetComponent<TMP_InputField>().text = cat.Name;
        setAbout(cat);
        ushort price = MathUtils.getSellingPrice(cat.getCatAsset().price);
        if (price == 0)
        {
            GameControl.GetTextBox(transform.Find("sell"), "text").SetText("Set\nfree");
        }
        else
        {
            GameControl.GetTextBox(transform.Find("sell"), "text").SetText("Sell\n" + CatIAP.goldStr + price.ToString());
        }
    }

    private void setStats(Cat cat)
    {
        updateCatLevelUI(cat);
        GameControl.GetTextBox(transform, "rarity").SetText(GameControl.GenerateStarText(cat.getCatAsset().rarity, true));
        CatDynamicStatsRealized realizedStats = cat.getRealizedStats();
        Debug.Log("projectile speed: " + realizedStats.projectileSpeed + " speed: " + realizedStats.speed);
        getStats().text = realizedStats.maxHealth.ToString() + "\n" +
        realizedStats.attackDamage.ToString() + "\n" +
        realizedStats.projectileSpeed.ToString("0.0") + "\n" +
        realizedStats.speed.ToString("0.0") + "\n" +
        cat.getCatAsset().secondaryType.ToString() + "\n" +
        cat.getCatAsset().action.ToString();
    }

    private void updateCatLevelUI(Cat cat)
    {
        getLvl().SetText(
            "Lv." + cat.catLvl.ToString());


        GameControl.GetTextBox(getLevelUp().transform, "text").SetText("level up" +
        "\n<sprite=1>" + cat.catLvl.getNextLevelCost().ToString());
        if (cat.catLvl.canLevelUp())
        {
            getLevelUp().interactable = true;
            getLevelUp().onClick.RemoveAllListeners();
            getLevelUp().onClick.AddListener(() =>
            {
                if (!leveledUp)
                {
                    GameControl.control.YesNoPrompt("Level up " + cat.Name + " for " + CatIAP.silverStr + cat.catLvl.getNextLevelCost().ToString() + "?", GameControl.control.transform, () =>
                    {
                        Notification n = GameControl.control.Notify(DataUtils.randomLvlUp(cat.catType), GameControl.control.transform);
                        n.setCatUI(cat.getCatAsset().meow);
                        n.RewardBackground();

                        leveledUp = true;
                        cat.catLvl.levelUp(cat);
                        setStats(cat);
                        deckCard.setLvl();
                    }, null, "yes", true);
                }
                else
                {
                    GameControl.control.getSoundManager().playExploreButton();
                    cat.catLvl.levelUp(cat);
                    setStats(cat);
                    deckCard.setLvl();
                }
            });
        }
        else
        {
            levelUp.interactable = false;
            if (cat.catLvl.level == CatLevel.maxLevel)
            {
                GameControl.GetTextBox(getLevelUp().transform, "text").SetText("level" +
       "\nat max");
            }
        }
    }

    private void setAbout(Cat cat)
    {
        Transform about = transform.Find("about");
        about.transform.localScale = Vector3.zero;
        GameControl.GetTextBox(about, "text").SetText(DataUtils.adjustCosmeticString(
            cat.getCatAsset().Abouts[UnityEngine.Random.Range(0, cat.getCatAsset().Abouts.Length)], cat.catType));
        LeanTween.scale(about.gameObject, Vector3.one, 0.3f).setEaseInQuart();
    }

    private void setUI(Cat cat)
    {
        deckCat.gameObject.SetActive(true);
        cat.SetCat(deckCat.GetChild(0));
        transform.Find("faction").GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/" + cat.getCatAsset().faction.ToString());

        GetComponent<Image>().sprite = cat.getCatAsset().bg;

        foreach (Transform child in bullets)
        {
            if (child.name != "b")
            {
                continue;
            }
            child.GetComponent<Image>().sprite = cat.getCatAsset().bullet;
        }
    }
}
