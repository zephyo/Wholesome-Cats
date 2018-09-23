using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class ExplorePlayer : MonoBehaviour
{
    public ExplorePlayer enemyPlayer;
    public ExploreCat[] allCats;
    public List<ExploreCat> aliveCats;

    public Queue<TextMeshPro> damageTexts = new Queue<TextMeshPro>();
    [HideInInspector]
    public bool enemy = false;
    public bool noMagic = false;
    private CameraShake shake;
    public Action<ExplorePlayer> GameOver;
    public CameraShake getCameraShake()
    {
        if (shake == null)
        {
            if (enemyPlayer.shake != null)
            {
                shake = enemyPlayer.shake;
            }
            else
            {
                shake = Camera.main.gameObject.GetComponent<CameraShake>();
            }
        }
        return shake;
    }

    public void init(PlayerData ownerPlayerData, ExplorePlayer enemyPlayer, GameObject catGameObjectPrefab, float CatScale, Vector2 pos, Vector3 rot, Action<ExplorePlayer> GameOver)
    {
        this.GameOver = GameOver;
        this.enemyPlayer = enemyPlayer;
        aliveCats = new List<ExploreCat>();
        allCats = new ExploreCat[ownerPlayerData.team.Count];
        HealthBar hBar = enemy ? transform.Find("enemy").GetComponent<HealthBar>() : transform.Find("player").GetComponent<HealthBar>();
        Vector2[] offsets = DataUtils.getOffsets(ownerPlayerData.team.Count);
        for (int i = 0; i < ownerPlayerData.team.Count; i++)
        {
            aliveCats.Add(addCat(ownerPlayerData.team[i], i, catGameObjectPrefab, enemy ? new Vector2(-offsets[i].x, offsets[i].y) : offsets[i], pos, rot, CatScale, giveHealthBar(hBar, i, ownerPlayerData.team.Count)));
            allCats[i] = aliveCats[i];
        }
    }

    public Vector2 getLosePos(ExploreCat cat)
    {
        int i = 0;
        for (; i < allCats.Length; i++)
        {
            if (allCats[i] == cat)
            {
                break;
            }
        }
        if (enemy)
        {
            Vector2 offset = DataUtils.getLoseOffsets(allCats.Length)[i];
            return ExploreController.enemyPos + new Vector2(-offset.x, offset.y);
        }
        else
        {
            return ExploreController.userPos + DataUtils.getLoseOffsets(allCats.Length)[i];
        }

    }

    public ExploreCat addCat(Cat cat, int i, GameObject catPrefab, Vector2 offset, Vector2 pos, Vector3 rot, float scale, HealthBar hb)
    {
        GameObject catGameObject = Instantiate(catPrefab);
        ExploreCat exploreCat = catGameObject.AddComponent<ExploreCat>();

        exploreCat.transform.position = pos;
        exploreCat.transform.position += new Vector3(offset.x, offset.y, 0);
        exploreCat.transform.eulerAngles = rot;
        exploreCat.transform.localScale = new Vector3(scale, scale, scale);

        exploreCat.init(cat, this, i, hb);
        return exploreCat;
    }

    private HealthBar giveHealthBar(HealthBar hBar, int index, int maxCats)
    {
        RectTransform rt;

        //0
        if (index == 0)
        {
            if (maxCats != 1)
            { //top right
                rt = (RectTransform)hBar.transform;
                rt.anchoredPosition = new Vector2(enemy ? -rt.sizeDelta.x : rt.sizeDelta.x, 0);
            }
            return hBar;
        }
        rt = ((RectTransform)GameObject.Instantiate(hBar.gameObject, hBar.transform.parent, false).transform);
        rt.SetSiblingIndex(0);
        float X = enemy ? -rt.sizeDelta.x : rt.sizeDelta.x;
        //1
        if (index == 1)
        {
            if (maxCats == 2 || maxCats == 3)
            {
                //top left
                rt.anchoredPosition = Vector2.zero;
            }
            else if (maxCats == 4)
            {
                //bottom right
                rt.anchoredPosition = new Vector2(X, -rt.sizeDelta.y);
            }
        }
        //2
        if (index == 2)
        {
            if (maxCats == 3)
            {
                //bottom left
                rt.anchoredPosition = new Vector2(0, -rt.sizeDelta.y);
            }
            else if (maxCats == 4)
            {
                //top left
                rt.anchoredPosition = Vector2.zero;
            }
        }
        //3
        if (index == 3)
        {
            //bottom left
            rt.anchoredPosition = new Vector2(0, -rt.sizeDelta.y);
        }
        return rt.GetComponent<HealthBar>();
    }
    public void setAsEnemy(ExplorePlayer userPlayer)
    {
        enemy = true;
        noMagic = userPlayer.aliveCats.Any(x => x.cat.catType == CatType.doot);
    }

    public int getLayer(bool enemy)
    {
        if (enemy)
        {
            return LayerMask.NameToLayer("Enemy");
        }
        else
        {
            return LayerMask.NameToLayer("CatIgnore");
        }
    }

    // public float getSummedHealthPercentage()
    // {
    //     float summedHealth = 0;
    //     float summedMaxHealth = 0;
    //     foreach (ExploreCat cat in this.cats)
    //     {
    //         summedHealth += cat.currentHealth;
    //         summedMaxHealth += cat.realizedStats.maxHealth;
    //     }
    //     if (summedMaxHealth == 0)
    //     {
    //         return 0;
    //     }
    //     return summedHealth / summedMaxHealth;
    // }

    public TextMeshPro getDamageText(int attackDamage)
    {
        if (damageTexts.Count != 0)
        {
            TextMeshPro text = damageTexts.Dequeue();
            text.fontSize = Mathf.Lerp(14f, 19f, attackDamage / 140);
            text.gameObject.SetActive(true);
            return text;
        }
        GameObject GO = new GameObject("text");
        TextMeshPro damageText = GO.AddComponent<TextMeshPro>();
        damageText.sortingLayerID = SortingLayer.NameToID("FX");
        damageText.color = enemy ? new Color32(101, 194, 247, 255) : new Color32(255, 125, 145, 255);
        damageText.fontStyle = FontStyles.Bold & FontStyles.Italic;
        damageText.fontSize = Mathf.Lerp(14f, 19f, attackDamage / 140);
        damageText.alignment = TextAlignmentOptions.Center;
        damageText.sortingLayerID = SortingLayer.NameToID("FX");
        damageText.fontMaterial = Resources.Load<Material>("Fonts & Materials/cutefont_whiteoutline");
        return damageText;
    }

}
