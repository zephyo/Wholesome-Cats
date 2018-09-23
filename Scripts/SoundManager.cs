using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
public class SoundManager : MonoBehaviour
{

    public AudioClip button, exploreButton, mainMusic, error, notif;
    public AudioSource sfx;
    private AudioClip[] KittenSounds = null;
    public AudioClip[] getKittenSounds()
    {
        if (KittenSounds == null)
        {
            KittenSounds = Resources.LoadAll<AudioClip>("KittenSounds");
        }
        return KittenSounds;
    }


    public void playOneShot(AudioClip clip, float pitch)
    {
        if (sfx == null) return;
        changeSfxPitch(pitch);
        sfx.PlayOneShot(clip);
        changeSfxPitch(1);
    }

    public void changeSfxPitch(float pitch)
    {
        sfx.pitch = pitch;
    }
    void Start()
    {
        playMainMusic(null);
    }


    public void playNotif()
    {
        if (sfx == null) return;
        changeSfxPitch(UnityEngine.Random.Range(1f, 1.15f));
        sfx.PlayOneShot(notif, UnityEngine.Random.Range(0.5f, 1f));
    }
    public void playButton()
    {
        if (sfx == null) return;
        changeSfxPitch(UnityEngine.Random.Range(0.9f, 1.1f));
        sfx.PlayOneShot(button, UnityEngine.Random.Range(0.5f, 1f));
    }
    public void playSoftButton()
    {
        if (sfx == null) return;
        changeSfxPitch(UnityEngine.Random.Range(0.95f, 1.05f));
        sfx.PlayOneShot(button, UnityEngine.Random.Range(0.5f, 0.75f));
    }
    public void playExploreButton()
    {
        if (sfx == null) return;
        sfx.PlayOneShot(exploreButton);
    }
    // GameControl.control.getSoundManager().playMainMusic(() => { SceneManager.LoadScene(0); });
    public void playMainMusic(Action callback)
    {
        fadeClip(mainMusic, callback);
    }
    public void playExploreMusic(Action callback)
    {
        fadeClip(Resources.Load<AudioClip>(GameControl.control.playerData.lastWorld.ToString()), callback);
    }

    private void fadeClip(AudioClip clip, Action onComplete)
    {
        AudioSource audio = GetComponent<AudioSource>();

        float origVol = 0.75f;

        LeanTween.value(gameObject, (float val) =>
        {
            audio.volume = Mathf.Lerp(origVol, 0.2f, val);
        }, 0, 1, 0.3f).setOnComplete(() =>
        {
            audio.Stop();
            if (onComplete != null)
            {
                onComplete();
            }
            audio.clip = clip;
            audio.Play();
            LeanTween.value(gameObject, (float val) =>
                {
                    audio.volume = Mathf.Lerp(0.2f, origVol, val);
                }, 0, 1, 0.3f).setOnComplete(() =>
                {
                    audio.volume = origVol;
                });
        });
    }

    IEnumerator returnOrigPitch()
    {
        yield return new WaitForSeconds(0.2f);
        changeSfxPitch(1);
    }


    public void playError()
    {
        if (sfx == null) return;
        sfx.PlayOneShot(error, UnityEngine.Random.Range(0.5f, 1f));
    }
}
