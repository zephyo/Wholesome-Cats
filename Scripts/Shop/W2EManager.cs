#if UNITY_IOS || UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public enum W2EState
{
    StillLeft,
    Finished,
    NoAds,
    // MiscError,
    Skipped,

}

public class W2EManager : MonoBehaviour
{
    public const ushort maxVideos = 25;

    public TextMeshProUGUI header;
    public TextMeshProUGUI vidsLeft;
    public Image coin;
    private const string placementId = "rewardedVideo";
    private void Start()
    {
        if (GameControl.control.playerData.videosLeft == 0)
        {
            setUI(W2EState.Finished);
        }
        else
        {
            setUI(W2EState.StillLeft);

        }
    }
    public void buttonError()
    {
        GameControl.control.getSoundManager().playButton();
    }
    private void setUI(W2EState state)
    {
        ushort v = GameControl.control.playerData.videosLeft;
        if (state == W2EState.StillLeft)
        {
            //set header, vidsLeft, coin
            header.text = "♡ watch videos ♡";
            vidsLeft.text = v + (v > 1 ? " videos" : " video") + " left before daily coin!";

            coin.fillAmount = (float)(maxVideos - v) / (float)maxVideos;
        }
        else if (state == W2EState.Finished)
        {
            header.text = "You got a coin! ^o^";
            vidsLeft.text = "Please rest and\ncome again tomorrow~";
            transform.Find("def").Find("watch").GetComponent<Button>().interactable = false;
            coin.fillAmount = 1;
        }
        else if (state == W2EState.Skipped)
        {
            header.text = "Uh oh :<";
            vidsLeft.text = "The video was skipped. There's still " + v + (v > 1 ? " videos" : " video") + " left before coin.";
        }
        else if (state == W2EState.NoAds)
        {
            header.text = "Oh no :<";
            vidsLeft.text = "Can't find videos;\nplease come back later!";
        }
    }
    public void WatchButton()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;
        vidsLeft.text = "Loading..";
        Advertisement.Show("rewardedVideo", options);
    }
    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Video completed - Offer a reward to the player");
            // Reward your player here.
            if (GameControl.control.playerData.videosLeft > 0)
            {
                GameControl.control.playerData.videosLeft = (ushort)(GameControl.control.playerData.videosLeft - 1);
                if (GameControl.control.playerData.videosLeft == 0)
                {
                    GameControl.control.getSoundManager().playExploreButton();
                    GameControl.control.IncrementGold(1);
                    setUI(W2EState.Finished);
                }
                else
                {
                    setUI(W2EState.StillLeft);
                    GameControl.control.SavePlayerData();
                }
            }
            else
            {
                setUI(W2EState.Finished);
            }
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");
            setUI(W2EState.Skipped);

        }
        else if (result == ShowResult.Failed)
        {
            setUI(W2EState.NoAds);

        }
    }
}
#endif
