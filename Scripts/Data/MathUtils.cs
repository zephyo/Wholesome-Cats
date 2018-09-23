using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MathUtils
{
    public static ushort getSellingPrice(ushort origPrice)
    {
        return (ushort)(origPrice / 6);
    }
    //return a fair amount of silver for winning a random encounter; dependent on how far you've gone in Explore.
    //can find Explore level data in playerdata.WorldLevels
    //e.g. min silver given for only unlocking House (Lvl 0); max silver given for unlocking all levels in all worlds.
    public static uint FairSilver()
    {
        //[5...?]
        int progress = progressThroughWorld();
        Debug.Log("getting silver with progress:" + progress);
        return (uint)((Mathf.Pow(progress + 1, 2) - progress) / 9) + 5;
    }
    // returns an exponentially decreased reward, using the range [0, initialRewardAmt].
    // if dailyPlays is 1, return initialRewardAmt; 
    //else, have exponentially decreasing rewards based on how many times the level was played that day.
    public static uint AdjustedSilver(uint reward, uint dailyPlays, float rating)
    {
        // uint fairsilver = FairSilver();
        reward += FairSilver();
        float range1 = dailyPlays * reward * 0.35f;
        uint range = (uint)((float)reward * 0.05f) + 5;
        reward = (uint)(Mathf.Clamp(UnityEngine.Random.Range(reward - range1, reward - range1 + range), 0, Mathf.Infinity) * (rating / (float)LevelButton.MAX_STARS));
        Debug.Log("initial reward - " + reward + " at daily plays - " + dailyPlays + " with range1: " + range1 + "and range2: " + range);
        return reward;
    }

    //returns level for enemy cats based on how far you are in Explore
    //perhaps randomize a little by picking level from a range of levels?
    public static uint FairEnemyCatLevel(float totalLevels, float progressThroughWorld, float multiplier = 1)
    {
        float t = progressThroughWorld / totalLevels;
        Debug.Log("current progress - " + t + " current level - " + Mathf.Lerp(1, 85, t * t));
        Debug.Log("future progress - " + (t + t) + " future level - " + Mathf.Lerp(1, 85, (t + t) * (t + t)));
        // Debug.Log("Fair Cat level: progress ratio: " + (t * t));
        return (uint)(Mathf.Lerp(1, 84, t * t) * multiplier);
    }
    public static int progressThroughWorld()
    {
        int progressThroughWorld = 0;
        foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
        {
            int progress = GameControl.control.getWorldLevel(world);
            progressThroughWorld += (progress >= 0 ? progress : 0);
        }
        return progressThroughWorld;
    }

    public static uint getCatLevelCost(uint level)
    {
        // This function is floor(((x+1)^2 - x) / 10) + 10
        // http://www.wolframalpha.com/input/?i=table+floor(((x%2B1)%5E2+-+x)%2F10)+%2B+10+with+x+from+1+to+99
        // costs for levels 2, 3, 4, 5... 97, 98, 99
        // [10, 11, 12, 13... 960, 980, 1000]
        // The total sum for all levels 1 to 99 is 34300
        // http://www.wolframalpha.com/input/?i=sum(floor(((x%2B1)%5E2+-+x)%2F10)+%2B+10+with+x+from+1+to+99)
        return (uint)((Mathf.Pow(level + 1, 2) - level) / 10 + 9);
    }
}
