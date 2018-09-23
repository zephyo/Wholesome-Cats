using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
public class Notification : MonoBehaviour
{
    public Button ok;
    public TextMeshProUGUI text;
    public void buttonClick()
    {
        GameControl.control.getSoundManager().playButton();
    }
    public void RewardPrompt(RandomRewards.RandomReward reward, bool YesNo)
    {
        if (YesNo)
        {
            YesNoPrompt(reward.dialogue, reward.yesAction, reward.noAction, reward.yesString, reward.noString);
        }
        else
        {
            Notify(reward.dialogue, reward.yesAction);
        }
        //add cat face
        setCatUI(reward.catFace);
        RewardBackground();
    }
    public void setCatUI(Sprite catFace)
    {
        Image cat = new GameObject("cat").AddComponent<Image>();
        RectTransform child = (RectTransform)transform.GetChild(0);
        child.anchoredPosition = new Vector2(0, -142.9f);
        cat.rectTransform.SetParent(child, false);
        cat.sprite = catFace;
        cat.rectTransform.anchorMin = new Vector2(0.5f, 1);
        cat.rectTransform.anchorMax = cat.rectTransform.anchorMin;
        cat.rectTransform.anchoredPosition = new Vector2(0, 61.4f);
        cat.SetNativeSize();
    }
    public void RewardBackground()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/shop");
    }

    public void Notify(string prompt, UnityAction listener)
    {
        Transform child = getChild();
        text.text = prompt;
        if (listener != null)
        {

            ok.onClick.AddListener(listener);
        }
        Scale(child);
    }

    public void YesNoPrompt(string prompt, UnityAction yesListener, UnityAction noListener, string customYesString, string customNoString = "no")
    {
        Transform child = getChild();
        text.text = prompt;
        GameObject No = GameObject.Instantiate(ok.gameObject, child, false);
        GameControl.GetTextBox(ok.transform, "text").text = customYesString;
        GameControl.GetTextBox(No, "text").text = customNoString;
        ok.GetComponent<Image>().color = buttonColor(0);
        SetRect((RectTransform)ok.transform, 0, 2);
        SetRect((RectTransform)No.transform, 1, 2);
        if (yesListener != null)
        {
            ok.onClick.AddListener(yesListener);
        }
        if (noListener != null)
        {
            No.GetComponent<Button>().onClick.AddListener(noListener);
        }
        Scale(child);
    }
    //open prompt with multiple buttons. Listeners.Length must equal buttonLabels.Length
    public void RewardCatPrompt(string prompt, UnityAction[] Listeners, string[] buttonLabels, UnityAction noListener)
    {
        Transform child = getChild();
        text.text = prompt;
        text.rectTransform.offsetMin = new Vector2(text.rectTransform.offsetMin.x, 423.3f);

        //set no thank mew button
        Transform noThankMew = GameObject.Instantiate(ok.gameObject, child, false).transform;
        GameControl.GetTextBox(noThankMew, "text").text = "no thank mew";
        RectTransform RT = (RectTransform)noThankMew;
        RT.anchoredPosition = new Vector2(0, 103);
        RT.sizeDelta = new Vector2(614.5f, 141.2f);
        if (noListener != null)
        {
            noThankMew.GetComponent<Button>().onClick.AddListener(noListener);
        }
        //set option buttons
        int i = 0;
        GameControl.GetTextBox(ok.transform, "text").text = buttonLabels[i];
        ok.onClick.AddListener(Listeners[i]);
        ok.GetComponent<Image>().color = buttonColor(i + 1);
        ((RectTransform)ok.transform).anchoredPosition = new Vector2(0, 293.6f);
        SetRect((RectTransform)ok.transform, i, buttonLabels.Length);
        i++;
        for (; i < buttonLabels.Length; i++)
        {
            Transform button = GameObject.Instantiate(ok.gameObject, child, false).transform;
            GameControl.GetTextBox(button, "text").text = buttonLabels[i];
            button.GetComponent<Button>().onClick.AddListener(Listeners[i]);
            SetRect((RectTransform)button, i, buttonLabels.Length);
            button.GetComponent<Image>().color = buttonColor(i + 1);
        }
        setWidth((RectTransform)child, buttonLabels.Length);
        Scale(child);
    }

    Transform getChild()
    {
        Transform child = transform.GetChild(0);
        child.localScale = new Vector3(0.5f, 0.5f, 1);
        return child;
    }
    private void Scale(Transform child)
    {
        LeanTween.scale(child.gameObject, Vector3.one, 0.1f).setEaseInSine();
    }

    public void Close()
    {
        LeanTween.scale(transform.GetChild(0).gameObject, new Vector3(0.5f, 0.5f, 1), 0.1f).setEaseOutSine().setOnComplete(
            () =>
            {
                Destroy(gameObject);
            });
    }

    private void SetRect(RectTransform button, int index, int totalButtons)
    {
        if (totalButtons == 2)
        {
            button.anchoredPosition = new Vector2(-200 + 400 * index, button.anchoredPosition.y);
        }
        else if (totalButtons >= 3)
        {
            button.anchoredPosition = new Vector2(-350 + 350 * index, button.anchoredPosition.y);
        }
    }
    private void setWidth(RectTransform backdrop, int totalButtons)
    {
        if (totalButtons == 2)
        {
            backdrop.sizeDelta = new Vector2(backdrop.sizeDelta.x, 809.7f);
        }
        else if (totalButtons >= 3)
        {
            backdrop.sizeDelta = new Vector2(1150.6f, 809.7f);
        }
    }
    private Color32 buttonColor(int index)
    {
        switch (index)
        {
            case 0:
                return new Color32(168, 195, 40, 255);
            case 1:
                return new Color32(238, 147, 184, 255);
            case 2:
                return new Color32(190, 144, 217, 255);
            case 3:
                return new Color32(126, 168, 221, 255);
            default:
                return new Color32(255, 143, 156, 255);
        }
    }
    // private void SetLeft(GameObject left)
    // {
    //     Image leftImg = left.GetComponent<Image>();
    //     leftImg.color = new Color32(168, 195, 40, 255);
    //     leftImg.rectTransform.anchoredPosition = new Vector2(-200, leftImg.rectTransform.anchoredPosition.y);
    // }

    // private void SetRight(GameObject right)
    // {
    //     RectTransform rect = (RectTransform)right.transform;
    //     rect.anchoredPosition = new Vector2(200, rect.anchoredPosition.y);
    // }

}
