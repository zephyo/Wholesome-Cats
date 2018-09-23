using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainUI : MonoBehaviour
{

    public AudioMixer AM;
    public Deck deck;
    public GameObject mainButtons;
    [Header("Scene UI")]

    public DetailCard detailCard;
    public Shop gacha;

    public void GoToMap(Image explore)
    {
        explore.raycastTarget = false;
        GameControl.control.getSoundManager().playButton();
        GameControl.control.getSoundManager().playExploreMusic(() =>
        {
            SceneManager.LoadScene("Map", LoadSceneMode.Single);
            GameControl.control.getBackButton().gameObject.SetActive(true);
        });
    }

    #region FIREBASE ------ 
    public GameObject loginScreen;
    public void LoginListener()
    {
        //instantiate login
        Instantiate(loginScreen, GameControl.control.transform, false);
    }
    #endregion
    private void Start()
    {
        //set sound
        if (PlayerPrefs.GetInt(AboutText.MusicKey) == 1)
        {
            AM.SetFloat(AboutText.MusicKey + "Vol", -80);
        }
        else
        {
            AM.SetFloat(AboutText.MusicKey + "Vol", 0);
        }
        if (PlayerPrefs.GetInt(AboutText.SfxKey) == 1)
        {
            AM.SetFloat(AboutText.SfxKey + "Vol", -80);
        }
        else
        {
            AM.SetFloat(AboutText.SfxKey + "Vol", 0);
        }
        //set null bc no need to use after
        AM = null;
        GameControl.control.GetComponent<Canvas>().enabled = true;
    }
    public void ChangeUIFromHome()
    {
        buttonClick();
        GameControl.control.getBackButton().gameObject.SetActive(true);
    }

    public void buttonClick()
    {
        GameControl.control.getSoundManager().playButton();
    }
    public void buttonError()
    {
        GameControl.control.getSoundManager().playNotif();
    }
    public void buttonExplore()
    {
        GameControl.control.getSoundManager().playExploreButton();
    }

    public void SetPurchaseDeck()
    {
        deck.SetPurchaseDeck(gacha);
    }


    /** FOR DECK BUTTON USE ONLY */
    public void DeckButton()
    {
        deck.SetDeck(null, true);
        Button detailCardExit = detailCard.transform.Find("exit").GetComponent<Button>();
        detailCardExit.onClick.AddListener(() =>
        {
            deck.transform.parent.gameObject.SetActive(true);
        });
        GameControl.control.getBackButton().onClick.AddListener(() =>
        {

            detailCardExit.onClick.RemoveAllListeners();
            GameControl.control.getBackButton().onClick.RemoveAllListeners();
        });
    }

    public void UpdateDetailCard(DeckCard deckCard)
    {
        detailCard.setDetailCard(deckCard);
    }

}
