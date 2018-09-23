using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MainMenuCat : ExploreCatHolder
{
    float waitWalkTime;
    bool walking = false;
    bool LeftDirection = false;
    float currTime = 0;
    public void init(Cat cat, Vector3 pos, string catLayer)
    {
        transform.position = pos;
        setLayer(catLayer);
        this.cat = cat;
        cat.SetCat(transform.GetChild(0));
        if (UnityEngine.Random.value > 0.5f)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
        waitWalkTime = UnityEngine.Random.Range(0f, 4f);

    }
    public void InteractCat(Animator a)
    {
        if (walking)
        {
            walking = false;
            DisableWalking();
        }
        switch (cat.getCatAsset().attackType)
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
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (walking)
        {
            if (transform.eulerAngles == Vector3.zero)
            {
                leftDirection();
            }
            else
            {
                rightDirection();
            }
        }
    }
    private void setLayer(string layerName)
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingLayerName = layerName;
        }
    }
    void DisableWalking()
    {
        getRigidBody2D().velocity = Vector2.zero;
        transform.GetChild(0).GetComponent<Animator>().SetBool("walk", walking);
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > waitWalkTime)
        {
            currTime = 0;
            waitWalkTime = UnityEngine.Random.Range(1f, 4f);
            walking = !walking;
            if (!walking)
            {
                DisableWalking();
            }
            else
            {
                LeftDirection = (UnityEngine.Random.value > 0.5f) ? true : false;
                if (LeftDirection)
                {
                    leftDirection();
                }
                else
                {
                    rightDirection();
                }
                transform.GetChild(0).GetComponent<Animator>().SetBool("walk", walking);
            }
        }
    }
    void leftDirection()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
        getRigidBody2D().velocity = Vector2.left;
    }
    void rightDirection()
    {
        transform.eulerAngles = Vector3.zero;
        getRigidBody2D().velocity = Vector2.right;
    }
    void OnDisable()
    {
        walking = false;
        getRigidBody2D().velocity = Vector2.zero;
        transform.GetChild(0).GetComponent<Animator>().SetBool("walk", walking);
    }
}
