using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CatLevel
{

    public const uint startingLevel = 1;
    public const uint maxLevel = 99;
    public uint level = startingLevel;

    public CatLevel() : this(startingLevel) { }
    public CatLevel(uint level)
    {
        this.level = (uint)Mathf.Clamp(level, startingLevel, maxLevel);
    }

    public float getPercentageToMaxLevel()
    {
        return ((float)(level - startingLevel)) / (maxLevel - startingLevel);
    }

    public bool canLevelUp()
    {
        if (level >= maxLevel)
        {
            return false;
        }
        return GameControl.control != null && GameControl.control.playerData.silver >= getNextLevelCost();
    }

    public bool levelUp(Cat cat)
    {
        if (canLevelUp())
        {
            GameControl.control.DecrementSilver(getNextLevelCost());
            cat.catLvl = new CatLevel(level + 1);
            return true;
        }
        return false;
    }

    public uint getNextLevelCost()
    {
        return MathUtils.getCatLevelCost(level + 1);
    }


    public override string ToString()
    {
        return level.ToString();
    }

}
