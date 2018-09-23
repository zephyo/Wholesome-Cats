using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreditsCat : ExploreCatHolder
{

    public void init()
    {
        Transform body = transform.GetChild(0);
        transform.localScale = Vector3.one * 0.8f;
        body.GetComponent<RandomCatNoises>().enableNoises = false;
        body.GetComponent<Animator>().SetBool("walk", true);
        this.cat = new Cat(CatType.sleepy);
        cat.SetCat(transform.GetChild(0));
    }

    public IEnumerator walkAcrossScreen()
    {
        Debug.Log("walk across screen!");
        Vector3 targetPos = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.one).x + 1.5f, 2.15f, 0);
        Vector3 orig = transform.position;
        Debug.Log("target pos - " + targetPos);
        // 1.2f = speed
        float time = 15f;
        float curT = 0;
        while (curT < time)
        {
            curT += Time.deltaTime;
            transform.position = Vector2.Lerp(orig, targetPos, curT / time);
            yield return null;
        }
        Debug.Log("done walking!" + targetPos);
        yield return null;
        Destroy(gameObject);
    }
}
