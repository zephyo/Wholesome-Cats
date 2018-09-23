using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Linq;
using UnityEngine.EventSystems;
public class MapManager : MonoBehaviour
{
    public MapCat MapCat;
    public WorldPin lastUsedPin;
    public GameObject Prompt;

    public GameObject levelSelect;

    public LevelButton[] buttons;
    private LevelButton selected;

    #region BUTTON LISTENERS ----------------------
    public void buttonClick()
    {
        GameControl.control.getSoundManager().playButton();
    }
    public void buttonExplore()
    {
        GameControl.control.getSoundManager().playExploreButton();
    }
    public void buttonError()
    {
        GameControl.control.getSoundManager().playError();
    }
    #endregion

    public WorldPin FindPin(WorldType world)
    {
        return transform.Find(world.ToString()).GetComponent<WorldPin>();
    }
    private void Awake()
    {
        if (GameControl.control == null)
        {
            return;
        }
        if (GameControl.control.playerData.team.Count == 0)
        {
            GameControl.control.getSoundManager().playNotif();
            GameControl.control.Notify("you need a cat on your team to explore!", GameControl.control.transform, () =>
           {
               GameControl.control.loadMain();
           }, true);
            MapCat.gameObject.SetActive(false);
            return;
        }

        lastUsedPin = FindPin(GameControl.control.playerData.currentWorld);
        MapCat.Initialise(this, lastUsedPin);
    }

    public void setSelectedButton(LevelButton button)
    {
        buttonClick();
        if (selected != null)
        {
            selected.GetComponent<Image>().color = Color.white;
        }
        selected = button;
        if (selected.Unlocked)
        {
            levelSelect.transform.GetChild(0).Find("play").GetComponent<Button>().interactable = true;
            selected.GetComponent<Image>().color = new Color32(255, 236, 186, 255);
        }
        else
        {
            levelSelect.transform.GetChild(0).Find("play").GetComponent<Button>().interactable = false;
        }
    }
    public void removeSelectedButton()
    {
        selected = null;
    }

    public void Play()
    {
        if (selected == null)
        {
            return;
        }
        if (lastUsedPin.worldType == WorldType.ocean && selected.Stage > 0)
        {
            if (!GameControl.control.playerData.team.Any(x =>
         x.getCatAsset().action == ActionType.Swim))
            {
                GameControl.control.Notify("you need a cat that can <b>Swim</b> to travel the ocean!", GameControl.control.transform, null, true);
                return;
            }
        }
        if (lastUsedPin.worldType == WorldType.sky || lastUsedPin.worldType == WorldType.space)
        {
            if (!GameControl.control.playerData.team.Any(x =>
           x.getCatAsset().action == ActionType.Magic || x.getCatAsset().action == ActionType.Fly
            ))
            {
                GameControl.control.Notify("you need a cat that can <b>Fly</b> or do <b>Magic</b> to travel the skies!", GameControl.control.transform, null, true);
                return;
            }
        }
        GameControl.control.playerData.currentWorld = lastUsedPin.worldType;
                        GameControl.control.SavePlayerData();
        ExploreController.world = lastUsedPin.worldType;
        ExploreController.level = selected.Stage;
        selected.transform.parent.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;

        GameControl.control.GetComponent<GraphicRaycaster>().enabled = false;
        StageTransition(0, 1, lastUsedPin.worldType, () =>
         {
             SceneManager.LoadScene("Explore");
         });
    }

    public void PlayRandom()
    {
        //selected.transform.parent.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;

        GameControl.control.GetComponent<GraphicRaycaster>().enabled = false;
        StageTransition(0, 1, MapCat.CurrentPin.worldType, () =>
         {
             SceneManager.LoadScene("Explore");
         });
    }

    public static void StageTransition(float from, float to, WorldType world, Action onComplete)
    {
        StageTransition ST = Camera.main.GetComponent<StageTransition>();
        ST.changeTexture(Resources.Load<Texture2D>("LevelAssets/Textures/" + world.ToString()));
        LeanTween.value(from, to, 1.2f).setOnUpdate((float value) =>
        {
            ST.changeCutoff(value);
        }).setEaseInOutQuad().setOnComplete(onComplete);
    }

    public void EnterLevelSelect(WorldPin worldPin)
    {
        lastUsedPin = worldPin;
        levelSelect.SetActive(true);
        LevelAsset asset = DataUtils.loadLevelAsset(worldPin.worldType);
        TextMeshProUGUI title = levelSelect.transform.GetChild(0).Find("title").GetComponent<TextMeshProUGUI>();
        title.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(worldPin.worldType.ToString());
        int i = 0;
        int lastUnlocked = 0;
        for (; i < asset.stageNames.Length; i++)
        {
            buttons[i].Init(this, asset.stageNames[i], title);
            if (buttons[i].Unlocked && i > lastUnlocked)
            {
                lastUnlocked = i;
            }
        }
        for (; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        buttons[lastUnlocked].GetComponent<Button>().onClick.Invoke();

    }
    // public void unlockWorlds()
    // {
    //     GameObject.FindGameObjectWithTag("LevelSelect").SetActive(false);
    //     MapCat.CurrentPin.transform.parent.gameObject.SetActive(false);
    //     MapCat.CurrentPin.transform.parent.gameObject.SetActive(true);
    // }
    public void hidePrompt()
    {
        Prompt.SetActive(false);
    }

    /// <summary>
    /// Update the GUI text
    /// </summary>
    public void updatePrompt()
    {
        RectTransform prompt = (RectTransform)Prompt.transform;
        //haracter.CurrentPin.transform
        prompt.SetParent(MapCat.CurrentPin.transform, false);
        prompt.anchoredPosition = new Vector2(0, 5);
        prompt.Find("description").GetComponent<TextMeshProUGUI>().text = MapCat.CurrentPin.Description;
        Button button = prompt.Find("button").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => EnterLevelSelect(MapCat.CurrentPin));
        Prompt.SetActive(true);
    }
}
