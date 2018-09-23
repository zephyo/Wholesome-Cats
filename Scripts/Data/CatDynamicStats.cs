using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CatDynamicStats")]
public class CatDynamicStats : ScriptableObject
{
    public uint baseMaxHealth;
    public float baseSpeed = 1f; // who knows how this translates to move units ingame maybe we'll have to tweak it
    public uint baseAttackDamage; // damage per attack
    // public float baseAttackCooldown; // seconds between attacks
    public float baseProjectileSpeed;

    public uint maxMaxHealth;
    public float maxSpeed = 1f; // who knows how this translates to move units ingame maybe we'll have to tweak it
    public uint maxAttackDamage; // damage per attack
    // public float maxAttackCooldown; // seconds between attacks
        public float maxProjectileSpeed;

    public string algorithm = "linear";

    public CatDynamicStatsRealized getRealizedStatsFromLevel(CatLevel level)
    {
        //TODO we have algorithm as linear, if it was different we would do a different algo here
        float percentageToMaxLevel = level.getPercentageToMaxLevel();
        CatDynamicStatsRealized realizedStats = new CatDynamicStatsRealized();
        realizedStats.maxHealth = (uint)Mathf.Lerp(baseMaxHealth, maxMaxHealth, percentageToMaxLevel);
        realizedStats.speed = getRealizedSpeedFromLevel(level, percentageToMaxLevel);
        realizedStats.attackDamage = (uint)Mathf.Lerp(baseAttackDamage, maxAttackDamage, percentageToMaxLevel);
        // realizedStats.attackCooldown = (uint)Mathf.Lerp(baseAttackCooldown, maxAttackCooldown, percentageToMaxLevel);
       // realizedStats.range = (uint)Mathf.Lerp(baseRange, maxRange, percentageToMaxLevel);
        realizedStats.projectileSpeed = Mathf.Lerp(baseProjectileSpeed, maxProjectileSpeed, percentageToMaxLevel);
        return realizedStats;
    }

    public float getRealizedSpeedFromLevel(CatLevel level, float percentageToMaxLevel)
    {
        return Mathf.Lerp(baseSpeed, maxSpeed, percentageToMaxLevel);
    }
    
}
