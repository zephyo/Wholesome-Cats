using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Vector2Ser
{
    public float x, y;
    public Vector2Ser(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class PlayerData
{
    public uint silver; // Common in game currency
    public uint gold; // IAP, enought to buy 3 cats, so with the starting blue eyes and tabby you can have 5 and change their positions

    public const uint defaultSilver = 10;
    public const uint defaultGold = 3;

    // TODO: multiple teams
    public List<Cat> team = new List<Cat>();
    public List<Cat> deck = new List<Cat>();

    //Dictionary holds string (world names) which say how many ints (levels of that world) are locked
    public WorldIntDictionary WorldLocks = new WorldIntDictionary();

    //the world that the player is currently in, so that when you close and reopen the game, the player still remains in that world
    public WorldType currentWorld;
    //mapCat's last position and last world; if not zero, then must've been interrupted by random event
    public Vector2Ser lastPos;
    public WorldType lastWorld;
    //max number of cats we can have. If max is hit, must buy more storage with coins.
    public uint maxCats;
    //videos left before getting a coin
    public ushort videosLeft;
    public string lastDay;

    //Firebase account info
    public string refreshToken;
    public string uid;
    public string email;
    public bool loggedIn;
    public PlayerData()
    {
        maxCats = 8;
        silver = 0;
        gold = 0;
#if UNITY_IOS || UNITY_ANDROID
        videosLeft = W2EManager.maxVideos;
#endif
    }
    public void defPlayerData()
    {
        lastDay = DateTime.Today.ToString("yyyy-MM-dd");
        currentWorld = WorldType.house;
        WorldLocks = new WorldIntDictionary();
        int maxHouseLevels = DataUtils.loadLevelAsset(WorldType.house).stageNames.Length;
        WorldLocks[currentWorld] = new WorldLevel(0, -1, new uint[maxHouseLevels], new ushort[maxHouseLevels]);
        lastPos = new Vector2Ser(0, 0);
    }


    public void EndTutorialData()
    {
        if (deck.Count == 0)
        {
            GameControl.control.getMainUI().gacha.YieldCat();
        }
        if (team.Count == 0)
        {
            team.Add(deck[0]);
        }
        deck[0].catLvl = new CatLevel(6);
        silver = defaultSilver;
        gold = defaultGold;
        GameControl.control.SavePlayerData();
    }

    public bool ListEquals<T>(IList<T> a, IList<T> b)
    {
        if (a == null || b == null)
            return (a == null && b == null);

        if (a.Count != b.Count)
            return false;

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        for (int i = 0; i < a.Count; i++)
        {
            if (!comparer.Equals(a[i], b[i]))
                return false;
        }

        return true;
    }

    public bool DicEquals(WorldIntDictionary x, WorldIntDictionary y)
    {
        // early-exit checks
        if (null == y)
            return null == x;
        if (null == x)
            return false;
        if (x.Count != y.Count)
            return false;

        // check keys are the same
        foreach (WorldType k in x.Keys)
        {
            if (!y.ContainsKey(k))
            {
                return false;
            }
        }
        // check values are the same
        foreach (WorldType k in x.Keys)
        {
            if (!x[k].Equals(y[k]))
            {
                return false;
            }
        }

        return true;
    }

    public bool Equals(PlayerData obj)
    {
        Debug.Log("testing for equals");
        return obj != null &&
        this.silver == obj.silver &&
        this.gold == obj.gold &&
        this.maxCats == obj.maxCats &&
        ListEquals<Cat>(this.team, obj.team) &&
        ListEquals<Cat>(this.deck, obj.deck) &&
        DicEquals(this.WorldLocks, obj.WorldLocks);
        // Or whatever you think qualifies as the objects being equal.
    }


    // public uint getCatTypeCount(List<Cat> cats, CatType type)
    // {
    //     uint typeCount = 0;
    //     foreach (Cat cat in cats)
    //     {
    //         if (cat.catType == type)
    //         {
    //             typeCount++;
    //         }
    //     }
    //     return typeCount;
    // }

    // public uint getCatTypeCount(List<Cat> cats, CatType type)
    // {
    //     uint typeCount = 0;
    //     foreach (Cat cat in cats)
    //     {
    //         if (cat.catType == type)
    //         {
    //             typeCount++;
    //         }
    //     }
    //     return typeCount;
    // }
}
