using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public const int MAX_STARS = 3;
    public int Stage;
    public bool Unlocked;

    // private int maxLevels;
    public void Init(MapManager manager, string stageName, TextMeshProUGUI title)
    {
        gameObject.SetActive(true);
        Unlock(manager);
        Button b = GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        string originalTitle = title.text;
        b.onClick.AddListener(() =>
        {
            title.text = originalTitle + ": " + (Unlocked ? stageName : "??");
        });
    }

    private void Unlock(MapManager manager)
    {
        setUnlocked(manager.lastUsedPin.worldType);
        // if (Unlocked && Stage + 1 == maxLevels)
        // {

        //     foreach (WorldPin pin in manager.lastUsedPin.ClosePins)
        //     {
        //         GameControl.control.setWorldLevel(pin.worldType, 0, -1, 0, maxLevels);
        //     }
        //     manager.unlockWorlds();
        // }
    }

    protected void setUnlocked(WorldType world)
    {
        int worldLevel = GameControl.control.getWorldLevel(world);
        Image i = GetComponent<Image>();
        i.color = Color.white;
        Unlocked = worldLevel >= Stage;
        if (Unlocked)
        {
            setStars(world);
            i.sprite = Resources.Load<Sprite>("miscUI/SUnlock");
        }
        else
        {
            i.sprite = Resources.Load<Sprite>("miscUI/SLock");
            clearStarText();
        }
    }

    private void clearStarText()
    {
        GameControl.GetTextBox(transform, "text").text = "";
    }

    private void setStars(WorldType world)
    {
        ushort rating = GameControl.control.getWorldRating(world, Stage);
        if (rating > 0)
        {
            int i = 0;
            TextMeshProUGUI starText = GameControl.GetTextBox(transform, "text");
            starText.text = "";
            for (; i < rating; i++)
            {
                starText.text += "<sprite=0>";
            }
            for (; i < MAX_STARS; i++)
            {
                starText.text += "<sprite=0 color="+GameControl.GREY_HEX+">";
            }
        }
        else
        {
            clearStarText();
        }
    }
}