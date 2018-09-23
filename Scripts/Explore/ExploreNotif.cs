using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class ExploreNotif : MonoBehaviour
{
    private string[] dialogues = null;
    private Action onComplete;
    private int currIndex = 0;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI text;
    public void InitNotify(string[] dialogues, Action onComplete)
    {
        this.onComplete = onComplete;
        if (dialogues == null || dialogues.Length == 0)
        {
            End();
            return;
        }
        this.dialogues = dialogues;
        LeanTween.value(0, 1, 0.15f).setEaseInQuad().setOnUpdate((float val) =>
        {
            canvasGroup.alpha = val;
        });
        Continue();
    }
    public void Continue()
    {
        if (dialogues.Length <= currIndex)
        {
            End();
            return;
        }
        //if not inited / user gave input
        if (currIndex > 0)
        {
            GameControl.control.getSoundManager().playSoftButton();
        }
        text.text = dialogues[currIndex++];
    }
    private void End()
    {
        canvasGroup.blocksRaycasts = false;
        LeanTween.value(1, 0, 0.2f).setEaseInQuad().setOnUpdate((float val) =>
           {
               canvasGroup.alpha = val;
           }).setOnComplete(() =>
           {
               if (onComplete != null)
               {
                   onComplete();
               }
               Destroy(gameObject);
           });
    }
}
