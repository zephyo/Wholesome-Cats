using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class ExploreStory : MonoBehaviour
{
    private Dialogue[] Dialogues;
    private int currSpeakerIndex = 0;
    private int currDialogueIndex = 0;
    private ExploreCat currentSpeaker;
    public event Action Finished;
    public const int defaultTypeSpeed = 60;

    private float speed = defaultTypeSpeed;

    //Sound + Visual effects
    private ExploreCat userCatRef;
    private ExploreCat[] enemyCatRefs;
    public AudioClip[] meowBeeps;

    private bool isTyping;
    private float nextMeow;
    private TextMeshProUGUI dialogue;
    private TextMeshProUGUI getDialogueText(Transform speech)
    {
        if (dialogue == null)
        {
            dialogue = (speech ?? transform.Find("talk").Find("speech")).Find("text").GetComponent<TextMeshProUGUI>();
        }
        return dialogue;
    }
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI getNameText(Transform speech)
    {
        if (nameText == null)
        {
            nameText = (speech ?? transform.Find("talk").Find("speech")).Find("name").Find("text").GetComponent<TextMeshProUGUI>();
        }
        return nameText;
    }

    private void Awake()
    {
        speed = PlayerPrefs.GetInt("speed", defaultTypeSpeed);
        speed = 1 / speed;
    }
    public void InitStory(Dialogue[] dialogues, ExploreCat user, ExploreCat[] enemies)
    {
        userCatRef = user;
        enemyCatRefs = enemies;
        Debug.Log("init story with " + userCatRef.cat.Name + " as NONE.");

        Dialogues = DataUtils.AdjustCosmetics(dialogues, userCatRef.cat.catType);
        if (dialogues == null || dialogues.Length == 0)
        {
            End();
            return;
        }
        currSpeakerIndex = 0;
        currDialogueIndex = 0;
        gameObject.SetActive(true);
        setNewCharUI();


    }
    public void ContinueStory()
    {
        if (isTyping)
        {
            isTyping = false;
            return;
        }
        if (currSpeakerIndex >= Dialogues.Length)
        {
            return;
        }
        if (currDialogueIndex >= Dialogues[currSpeakerIndex].dialogue.Length)
        {
            currSpeakerIndex++;
            if (currSpeakerIndex >= Dialogues.Length)
            {
                EndDialogue();
                return;
            }
            setNewCharUI();
        }
        else
        {
            StartCoroutine(setSpeech(null, Dialogues[currSpeakerIndex].dialogue[currDialogueIndex]));
            currDialogueIndex += 1;
        }
        GameControl.control.getSoundManager().playSoftButton();

    }

    private void setNewCharUI()
    {
        Transform talk = transform.Find("talk");
        int degree = -75;
        Transform speech = talk.Find("speech");
        CanvasGroup CanvasGroup = speech.GetComponent<CanvasGroup>();
        CanvasGroup.blocksRaycasts = false;
        Action animateUp = () =>
        {
            currDialogueIndex = 0;
            Dialogue current = Dialogues[currSpeakerIndex];
            currentSpeaker = getExploreCat(current.speaker);
            setCat(current.meow, talk);
            getNameText(speech).GetComponent<TextMeshProUGUI>().text = currentSpeaker.cat.Name;
            StartCoroutine(setSpeech(speech, current.dialogue[currDialogueIndex++]));
            checkFlipDialogue(talk, (RectTransform)speech.Find("arrow"));
            speech.localScale = Vector3.zero;
            speech.localEulerAngles = new Vector3(0, speech.localEulerAngles.y, degree);
            LeanTween.value(0.5f, 1, 0.25f).setOnUpdate((float val) =>
            {
                CanvasGroup.alpha = Mathf.Clamp(val, 0, 1);
                speech.localScale = new Vector3(val, val, 1);
                speech.localEulerAngles = new Vector3(0, speech.localEulerAngles.y, degree - val * degree);
            }).setEaseInSine().setOnComplete(() =>
            {
                CanvasGroup.blocksRaycasts = true;
                speech.localScale = Vector3.one;
                speech.localEulerAngles = Vector3.zero;
            });
        };
        if (currSpeakerIndex == 0)
        {
            animateUp();
        }
        else
        {
            animateDown(CanvasGroup, speech, degree, animateUp);
        }

    }
    private ExploreCat getExploreCat(CatType cat)
    {
        if (cat == CatType.none)
        {
            return userCatRef;
        }
        ExploreCat cat2 = enemyCatRefs.FirstOrDefault(x => x.cat.catType == cat);
        if (cat2 == null)
        {
            return userCatRef;
        }
        return cat2;
    }

    private void animateDown(CanvasGroup CanvasGroup, Transform speech, int degree, Action onComplete)
    {
        LeanTween.value(1, 0.5f, 0.25f).setOnUpdate((float val) =>
            {
                CanvasGroup.alpha = Mathf.Clamp(val, 0, 1);
                speech.localScale = new Vector3(val, val, 1);
                speech.localEulerAngles = new Vector3(0, speech.localEulerAngles.y, degree - val * degree);
            }).setEaseOutSine().setOnComplete(onComplete);
    }


    private void setCat(bool meow, Transform talk)
    {
        Image body = talk.Find("body").GetComponent<Image>();
        body.sprite = currentSpeaker.cat.getCatAsset().body;
        Image head = talk.Find("head").GetComponent<Image>();
        if (meow)
        {
            head.sprite = currentSpeaker.cat.getCatAsset().meow;
            currentSpeaker.transform.GetChild(0).GetComponent<RandomCatNoises>().playMeow();
        }
        else
        {
            head.sprite = currentSpeaker.cat.getCatAsset().head;
        }
        head.rectTransform.pivot = new Vector2(head.sprite.pivot.x / head.sprite.rect.width, head.sprite.pivot.y / head.sprite.rect.height);
        head.SetNativeSize();
        body.rectTransform.pivot = new Vector2(body.sprite.pivot.x / body.sprite.rect.width, body.sprite.pivot.y / body.sprite.rect.height);
        body.SetNativeSize();
    }
    private IEnumerator setSpeech(Transform speech, string text)
    {
        //characters per second
        // Debug.Log("setSpeech "+text);
        if (speed < 0.009)
        {
            getDialogueText(speech).text = text;
            // Debug.Log("wtf1");
            yield break;
        }

        isTyping = true;
        getDialogueText(speech).text = "";
        nextMeow = Time.time;
        int currChar = 0;
        float punctuationPause = 0.25f;
        if (text.Length > 0)
        {
            GameControl.control.getSoundManager().changeSfxPitch(currentSpeaker.cat.getCatAsset().pitch);
            while (isTyping)
            {
                //  Debug.Log("typing");
                if (text[currChar] == '<')
                {
                    int length = text.IndexOf('>', currChar);
                    if (length != -1)
                    {
                        getDialogueText(speech).text += text.Substring(currChar, length - currChar);
                        currChar = length;
                        continue;
                    }
                }
                getDialogueText(speech).text += text[currChar];
                if (nextMeow < Time.time)
                {
                    AudioClip randomMeow = meowBeeps[UnityEngine.Random.Range(0, meowBeeps.Length)];
                    GameControl.control.getSoundManager().sfx.PlayOneShot(randomMeow, UnityEngine.Random.Range(0.9f, 1.1f));
                    nextMeow = Time.time + randomMeow.length;
                }
                if (currChar + 1 == text.Length)
                {
                    isTyping = false;
                     yield break;
                }
                if (IsPunctuation(text[currChar]))
                {
                    yield return new WaitForSeconds(punctuationPause);
                }
                else
                {
                    yield return new WaitForSeconds(speed);
                }
                currChar++;
            }
        }
        GameControl.control.getSoundManager().changeSfxPitch(1);
        getDialogueText(speech).text = text;
        //  Debug.Log("setSpeech done "+text);

    }

    protected virtual bool IsPunctuation(char character)
    {
        return character == '.' ||
            character == '?' ||
                character == '!' ||
                character == ',' ||
                character == ':' ||
                character == '~' ||
                character == ';' ||
                character == ')';
    }

    private void checkFlipDialogue(Transform talk, RectTransform arrow)
    {
        if (currSpeakerIndex == 0)
        {
            if (Dialogues[currSpeakerIndex].speaker == CatType.none)
            {
                userFlip(talk, arrow);
            }
            else
            {
                enemyFlip(talk, arrow);
            }
            return;
        }
        if (Dialogues[currSpeakerIndex].speaker == CatType.none || Dialogues[currSpeakerIndex - 1].speaker == CatType.none)
        {
            if (talk.eulerAngles == Vector3.zero)
            {
                enemyFlip(talk, arrow);
            }
            else
            {
                userFlip(talk, arrow);
            }
        }
    }

    private void userFlip(Transform talk, RectTransform arrow)
    {
        Vector3 flip = Vector3.zero;
        if (talk.eulerAngles == flip)
        {
            return;
        }
        talk.eulerAngles = flip;
        arrow.anchorMin = new Vector3(1, 0);
        arrow.anchorMax = arrow.anchorMin;
        arrow.anchoredPosition = new Vector2(-49.5f, arrow.anchoredPosition.y);
        getDialogueText(null).transform.localEulerAngles = talk.eulerAngles;
        getNameText(null).transform.localEulerAngles = talk.eulerAngles;
    }
    private void enemyFlip(Transform talk, RectTransform arrow)
    {
        Vector3 flip = new Vector3(0, 180, 0);
        if (talk.eulerAngles == flip)
        {
            return;
        }
        talk.eulerAngles = flip;
        arrow.anchorMin = Vector3.zero;
        arrow.anchorMax = arrow.anchorMin;
        arrow.anchoredPosition = new Vector2(108, arrow.anchoredPosition.y);
        getDialogueText(null).transform.localEulerAngles = talk.eulerAngles;
        getNameText(null).transform.localEulerAngles = talk.eulerAngles;
    }
    private void End()
    {
        gameObject.SetActive(false);
        if (Finished != null)
        {
            Finished();
            Finished = null;
        }
    }
    private void EndDialogue()
    {
        Transform speech = transform.Find("talk").Find("speech");
        animateDown(speech.GetComponent<CanvasGroup>(), speech, -75, End);
    }

    public void Skip()
    {
        EndDialogue();
    }
}
