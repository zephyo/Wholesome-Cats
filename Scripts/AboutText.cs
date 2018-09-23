using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleFirebaseUnity;

public class AboutText : MonoBehaviour
{

    public AudioMixer AM;
    public const string MusicKey = "Music";
    public const string SfxKey = "Sfx";
    public void LoginListener()
    {
        //instantiate login
        GameControl.control.getSoundManager().playButton();
        GameControl.control.getMainUI().LoginListener();
    }
    public void buttonError()
    {
        GameControl.control.getSoundManager().playError();
    }

    private void Start()
    {
        Debug.Log("setting slider - " + PlayerPrefs.GetInt("speed", ExploreStory.defaultTypeSpeed) + "... default speed - " + ExploreStory.defaultTypeSpeed);
        transform.Find("slider").GetComponent<Slider>().value = PlayerPrefs.GetInt("speed", ExploreStory.defaultTypeSpeed);
        //setting music buttons
        if (PlayerPrefs.GetInt(MusicKey) == 1)
        {
            transform.Find(MusicKey).GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/" + MusicKey + "off");
        }
        if (PlayerPrefs.GetInt(SfxKey) == 1)
        {
            transform.Find(SfxKey).GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/" + SfxKey + "off");
        }
    }

    public void turnMusic(Image music)
    {
        soundHelper(MusicKey, music);
    }

    public void turnSound(Image sound)
    {
        soundHelper(SfxKey, sound);
    }

    public void clearData()
    {
        GameControl.control.YesNoPrompt("Are you sure you want to start a new game?", GameControl.control.transform, () =>
       {
           if (GameControl.control.playerData.loggedIn)
           {
               GameControl.control.getUser((FirebaseParam user) =>
               {
                   GameControl.control.getFirebase().Delete(user);
               }, false, true);
           }
           if (File.Exists(GameControl.datapath))
           {
               File.Delete(GameControl.datapath);
           }
           GameControl.control.setControlNull();
           PlayerPrefs.DeleteAll();
           SceneManager.LoadScene(0);
       }, null, "yes", true);
    }
    public void changeTypingSpeed(float speed)
    {
        PlayerPrefs.SetInt("speed", (int)speed);
    }
    private void soundHelper(string key, Image music)
    {
        GameControl.control.getSoundManager().playButton();
        float sound = 0;
        AM.GetFloat(key + "Vol", out sound);
        if (sound > -80)
        {
            PlayerPrefs.SetInt(key, 1);
            //turn off music
            AM.SetFloat(key + "Vol", -80);
            music.sprite = Resources.Load<Sprite>("miscUI/" + key + "off");
        }
        else
        {
            PlayerPrefs.SetInt(key, 0);
            //turn on music
            AM.SetFloat(key + "Vol", 0);
            music.sprite = Resources.Load<Sprite>("miscUI/" + key + "on");
        }
    }

    public void changeLanguage(int option)
    {
        // LanguageSupport.language = (Language)option;
    }
    public void Tumblr()
    {
        Application.OpenURL("https://zephyo.tumblr.com/");
    }
    public void Twitters()
    {
        Application.OpenURL("https://twitter.com/zephybite");
    }
    public void YT()
    {
        Application.OpenURL("https://youtube.com/zephybite");
    }
    public void JoakimYT()
    {
        Application.OpenURL("https://youtube.com/joakimkarud");
    }
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/zephybite/");
    }
    // public void PatrickLinkedin()
    // {
    //     Application.OpenURL("https://www.linkedin.com/in/patrick-fay-5418a822/");
    // }
}
