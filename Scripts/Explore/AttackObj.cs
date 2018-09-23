using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackObj : MonoBehaviour
{
    [HideInInspector]
    public ExploreCat owner;
    [HideInInspector]
    public GameObject particles;
    protected bool callOnHit = true;

    protected float damage;
    public void setDamage(float d){
        damage = d;
    }

    public void setOnHit(bool val)
    {
        Debug.Log("set " + owner.cat.Name + " setOnHit as" + val);
        callOnHit = val;
    }

    protected void onHit()
    {
        Debug.Log("onHit: callOnHit:" + callOnHit);
        if (callOnHit && owner.onHit != null)
        {
            owner.onHit(AttackType.Ranged, owner);
        }
    }

    protected void particleEffect()
    {
        if (particles != null)
        {
            //  particles.gameObject.SetActive(false);
            particles.transform.position = transform.position;
            particles.gameObject.SetActive(true);
            return;
        }

        string folder = "Particles/";
        string particleName = "";
        switch (owner.cat.getCatAsset().secondaryType)
        {
            //distracted
            case SecondaryType.Bubbles:
            case SecondaryType.Yarn:
            case SecondaryType.Fish:
            case SecondaryType.Mouse:
            case SecondaryType.Butterfly:
                particleName = "hypnotized";
                break;
            //scared
            case SecondaryType.Skull:
            case SecondaryType.Bat:
                particleName = "bats";
                break;
            //confused/distracted
            case SecondaryType.Music:
            case SecondaryType.Gem:
            case SecondaryType.Nebula:
                particleName = "confused";
                break;

            //in love
            case SecondaryType.Rose:
            case SecondaryType.Arrow:
            case SecondaryType.Heart:
                particleName = "love";
                break;

            case SecondaryType.Star:
            case SecondaryType.Snowflake:
                particleName = "sparks";
                break;

            case SecondaryType.Rainbow:
            case SecondaryType.Sprinkles:
            case SecondaryType.Candy:
                particleName = "circles";
                break;
            case SecondaryType.Flower:
            case SecondaryType.Leaf:
            case SecondaryType.Berry:
            case SecondaryType.Paw:
            case SecondaryType.Lick:
            case SecondaryType.Bite:
                particleName = "puffs";
                break;
            //no effect
            default:
                return;
        }
        GameObject g = Resources.Load<GameObject>(folder + particleName);
        particles = Instantiate(g, transform.position, g.transform.rotation);
        particles.gameObject.name = particleName;
    }
}
