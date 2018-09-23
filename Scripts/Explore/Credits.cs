using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class Credits : MonoBehaviour
{

    public void init(Action onComplete, ExploreController controller, GameObject cat)
    {
        //fade in, 0.3f
        fadeInOut(0.8f, () =>
        {
            StartCoroutine(credits(onComplete, controller, cat));
        }, false);
    }

    private IEnumerator credits(Action onComplete, ExploreController controller, GameObject cat)
    {
        Canvas c = GetComponent<Canvas>();
        c.worldCamera = Camera.main;
        c.sortingLayerName = "FX";
        c.sortingOrder = 40;
        controller.GetComponent<Canvas>().enabled = false;
        //sleepy walks on screen
        TextMeshProUGUI text = GameControl.GetTextBox(transform, "text");
        text.text = "";
        Image bg = transform.Find("bg").GetComponent<Image>();
        text.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
        CreditsCat creditsCat = GameObject.Instantiate(cat).AddComponent<CreditsCat>();

        SpriteRenderer[] sprites = creditsCat.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingLayerName = "FX";
        }

        creditsCat.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.zero).x - 2, 1.2f, 0);
        creditsCat.init();
        StartCoroutine(creditsCat.walkAcrossScreen());

        //dialogues
        string[] dialogues = new string[]{
            "<color=#7398df><b>Wholesome Cats: The End</b>",
            "<color=#a09cb7><size=80%><u>Created by</u><line-height=110%>"+
            "\nAngela He\n<u>Music by</u>"+
            "\nCityfires",
            "Thank mew for playing <3",
        };
        for (int i = 0; i < dialogues.Length; i++)
        {
            changeBackground(i, bg);
            LeanTween.value(0, 1, 0.2f).setEaseInQuad().setOnUpdate((float value) =>
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, value);
            });
            text.text = dialogues[i];
            float time = 5.5f;
            for (float j = 0; j < time; j += Time.deltaTime)
            {
                bg.rectTransform.offsetMin = new Vector2(Mathf.Lerp(0, -50, j / time), 0);
                bg.rectTransform.offsetMax = new Vector2(50 + bg.rectTransform.offsetMin.x, 176.9f);
                yield return null;
            }
            LeanTween.value(1, 0, 0.2f).setEaseOutQuad().setOnUpdate((float value) =>
             {
                 text.color = new Color(text.color.r, text.color.g, text.color.b, value);
             });
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("IS THE CREDITS CAT DONE YET?!");

        //sleepy walks off screen

        while (creditsCat != null)
        {
            yield return null;
        }
        //fade out
        fadeInOut(2f, () =>
        {
            onComplete();
            bg.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            controller.GetComponent<Canvas>().enabled = true;

        }, true);
    }

    private void changeBackground(int i, Image bg)
    {
        Sprite background;
        switch (i)
        {
            case 0:
                background = Resources.Load<Sprite>("LevelAssets/Backgrounds/house0");
                break;
            case 1:
                background = Resources.Load<Sprite>("LevelAssets/Backgrounds/city2");
                break;
            default:
                background = Resources.Load<Sprite>("LevelAssets/Backgrounds/fields3");
                break;
        }
        bg.sprite = background;
    }

    private void fadeInOut(float time, Action onComplete, bool destroy = false)
    {
        Image fader = transform.Find("fader").GetComponent<Image>();

        LeanTween.value(0, 1, time).setEaseInCubic().setOnUpdate((float value) =>
                 {
                     fader.color = new Color(0, 0, 0, value);
                 }).setOnComplete(() =>
                 {
                     onComplete();
                     LeanTween.value(1, 0, time).setEaseOutCubic().setOnUpdate((float value) =>
                             {
                                 fader.color = new Color(0, 0, 0, value);
                             });
                     // }).setOnComplete(() =>
                     // {
                     //     if (destroy)
                     //     {
                     //         Destroy(gameObject);
                     //     }
                 });

    }


}
