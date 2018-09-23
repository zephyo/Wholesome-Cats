using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

/*

All user-editable fields must call copyCatIntoTeam before setting.
 */

[Serializable]
[DataContract]
public class Cat : Collectable
{
    [SerializeField]
    private CatType privateType;

    public CatType catType
    {
        get
        {
            return privateType;
        }
        set
        {
            if (value != privateType)
            {
                catAsset = null;
            }
            if (catAsset == null)
            {
                catAsset = Resources.Load<CatAsset>("CatAssets/" + value.ToString());
            }

            privateType = value;
        }
    }

    [System.NonSerialized]
    private CatAsset catAsset;


    public CatAsset getCatAsset()
    {
        if (catAsset == null)
        {
            catAsset = Resources.Load<CatAsset>("CatAssets/" + catType.ToString());
        }
        return catAsset;
    }
    [SerializeField]
    private string privateName = "";
    public string Name
    {
        //have the ability to rename cats
        get
        {
            if (String.IsNullOrEmpty(privateName))
            {
                privateName = getCatAsset().defaultName;
            }
            return privateName;
        }
        set
        {
            copyCatIntoTeam();
            privateName = value;
            GameControl.control.SavePlayerData();
        }
    }
    [SerializeField]
    private CatLevel lvl = new CatLevel();
    public CatLevel catLvl
    {
        //have the ability to rename cats
        get
        {
            return lvl;
        }
        set
        {
            copyCatIntoTeam();
            lvl = value;
            GameControl.control.SavePlayerData();
        }
    }
    private void copyCatIntoTeam()
    {
        for (int i = 0; i < GameControl.control.playerData.team.Count; i++)
        {
            if (this == GameControl.control.playerData.team[i])
            {
                Debug.Log("found cat!");
                GameControl.control.playerData.team[i] = this;
            }
        }
    }

    // Default = brown
    public Cat() : this(CatType.brown) { }


    public Cat(CatType type) : base()
    {
        this.privateType = type;
    }

    public override bool Equals(object cat)
    {
        return this == cat as Cat;
    }

    public static bool operator ==(Cat a, Cat b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        if (ReferenceEquals(a, null))
        {
            return object.ReferenceEquals(b, null);
        }
        if (ReferenceEquals(b, null))
        {
            return object.ReferenceEquals(a, null);
        }
        return a.GUID == b.GUID && a.Name == b.Name;
    }

    public static bool operator !=(Cat a, Cat b)
    {
        if (ReferenceEquals(a, b))
        {
            return false;
        }
        if (ReferenceEquals(a, null))
        {
            return !object.ReferenceEquals(b, null);
        }
        if (ReferenceEquals(b, null))
        {
            return !object.ReferenceEquals(a, null);
        }
        return a.GUID != b.GUID || a.Name != b.Name;
    }
    public void SetCat(Transform body)
    {
        body.Find("tail").GetComponent<SpriteRenderer>().sprite = getCatAsset().tail;
        body.Find("head").GetComponent<SpriteRenderer>().sprite = getCatAsset().head;
        body.GetComponent<SpriteRenderer>().sprite = getCatAsset().body;
        for (int i = 2; i < body.childCount; i++)
        {
            body.GetChild(i).GetComponent<SpriteRenderer>().sprite = getCatAsset().foot;
        }
    }

    public CatDynamicStatsRealized getRealizedStats()
    {
        return getCatAsset().dynamicStats.getRealizedStatsFromLevel(catLvl);
    }

    public CatDynamicStatsRealized getTemporaryBoost(uint increment)
    {
        return getCatAsset().dynamicStats.getRealizedStatsFromLevel(new CatLevel(catLvl.level + increment));
    }
    public float getSpeedBoost(uint increment)
    {

        return getCatAsset().dynamicStats.getRealizedStatsFromLevel(new CatLevel(catLvl.level + increment)).speed;
    }
}

