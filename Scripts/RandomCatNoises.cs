using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCatNoises : MonoBehaviour
{
    public CatAsset catAsset;
    public bool enableNoises = true;
    private AudioSource source = null;
    private AudioSource getSource()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
            getSource().pitch = getCatAsset().pitch;
        }
        return source;
    }
    private AudioClip[] kittenSoundsRef = null;
    private AudioClip[] getKittenSounds()
    {
        if (kittenSoundsRef == null)
        {
            kittenSoundsRef = GameControl.control.getSoundManager().getKittenSounds();
        }
        return kittenSoundsRef;
    }
    CatAsset getCatAsset()
    {
        if (catAsset == null)
        {
            catAsset = transform.parent.
            GetComponent<ExploreCatHolder>().cat.getCatAsset();
            getSource().pitch = getCatAsset().pitch;
        }
        return catAsset;
    }

    public void playMeow()
    {
        getSource().PlayOneShot(getKittenSounds()[Random.Range(0, getKittenSounds().Length)]);
        StartCoroutine(yelpFace());
    }

    public void CatNoise()
    {
        if (!enableNoises) return;
        if (Random.value > 0.99f)
        {
            playMeow();
        }
    }


    public void LikelyCatNoise()
    {
        if (!enableNoises) return;
       
        if (Random.value > 0.4f)
        {
            playMeow();
        }
    }

    IEnumerator yelpFace()
    {
        SpriteRenderer head = transform.Find("head").GetComponent<SpriteRenderer>();
        head.sprite = getCatAsset().meow;
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        if (isActiveAndEnabled)
        {
            head.sprite = getCatAsset().head;
        }


    }
}
