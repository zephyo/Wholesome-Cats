using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Melee : AttackObj
{
    private SpriteRenderer sprite;
    public SpriteRenderer getSprite()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        return sprite;
    }

    public float timeToLive = 1; //time to live in seconds
    private float timeAlive = 0;
    private Vector3 RotateTo;
    public void Init(ExploreCat owner)
    {
        this.timeToLive = 0.75f;
        this.owner = owner;
        RotateTo = new Vector3(0, owner.transform.eulerAngles.y, 30);
        transform.eulerAngles = owner.transform.eulerAngles;
    }

    public void Target(ExploreCat EC)
    {
        EC.hitByObj(damage, this);

        if (damage > 0)
        {
            timeAlive = 0;
            particleEffect();
        }
        else
        {
            timeAlive = timeToLive;
            getSprite().color = Color.clear;
        }

        onHit();
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        if (timeAlive < timeToLive)
        {
            Color color = Color.white;
            getSprite().color = color;
            yield return new WaitForSeconds(0.75f);
            Vector3 orig = transform.eulerAngles;

            while (timeAlive < timeToLive)
            {
                timeAlive += Time.deltaTime;
                color.a = (timeToLive - timeAlive) / timeToLive;
                getSprite().color = color;
                transform.eulerAngles = Vector3.Lerp(RotateTo, orig, color.a);
                yield return null;
            }

            transform.eulerAngles = orig;
        }
        gameObject.SetActive(false);
        owner.attackObjs.Enqueue(this);
    }
}
