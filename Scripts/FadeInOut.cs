using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private void OnEnable()
    {
        CanvasGroup CG = GetComponent<CanvasGroup>();
        CG.blocksRaycasts = false;
        LeanTween.value(0, 1, 0.25f).setEaseInSine().setOnUpdate((float val) =>
        {
            CG.alpha = val;
        }).setOnComplete(() =>
        {
            CG.blocksRaycasts = true;
        });
    }
    public void Close()
    {
        CanvasGroup CG = GetComponent<CanvasGroup>();
		CG.blocksRaycasts = false;
        LeanTween.value(1, 0, 0.15f).setEaseOutSine().setOnUpdate((float val) =>
         {
             CG.alpha = val;
         }).setOnComplete(() =>
         {
             transform.parent.gameObject.SetActive(false);
         });
    }
}
