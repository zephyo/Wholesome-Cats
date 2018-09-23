using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Tutorial : ExploreCatHolder
{
    private Action onComplete;
    private Transform tutorialUI;
    private Button skip;
    private Transform pawGraphic;
    Camera mainCamera;
    private TextMeshProUGUI tutorialText;
    private TextMeshProUGUI getTutorialText()
    {
        if (tutorialText == null)
        {
            tutorialText = GameControl.GetTextBox(tutorialUI, "text");
        }
        return tutorialText;
    }
    Stack<DialogueToAction> tutorialDialogue;
    private void initTutorialDialogue()
    {
        tutorialDialogue = new Stack<DialogueToAction>(new DialogueToAction[]{
new DialogueToAction("", walkOff),
new DialogueToAction("Here's " + CatIAP.silverStr + " and " + CatIAP.goldStr + " to support your journey. I'll see you soon ^-^", increaseToDefault),
new DialogueToAction("Yay! Now your <b>team</b> can <b>explore</b> <u>Half Moon Island</u> <s>and rescue us from our past</s>!", heartParticles),
new DialogueToAction("", addToTeam),
new DialogueToAction("Add your cat to your <b>team</b>~", afterShop),
new DialogueToAction("", getRandomCat),
new DialogueToAction("Now you can adopt a cat from the <b>shop</b>~", null),
new DialogueToAction("Here's one " + CatIAP.goldStr + "!", increaseCoin),
new DialogueToAction("Welcome to <u>Half Moon Island</u>, an island of wholesome cats ♡", Welcome),
new DialogueToAction("..Oh, hello! (Don't mind the bread..)", playMeow),

//tod
});
    }
    public struct DialogueToAction
    {
        public string dialogue;
        public Action action;
        public DialogueToAction(string dialogue, Action action)
        {
            this.dialogue = dialogue;
            this.action = action;
        }
    }
    private void setTutorialUIPosition()
    {
        if (tutorialUI != null)
        {
            Vector3 pos = mainCamera.WorldToScreenPoint(new Vector3(transform.position.x + 2.9f, transform.position.y + 0.7f, 0));
            tutorialUI.position = pos;
        }
    }

    private Animator getAnimator()
    {
        return transform.GetChild(0).GetComponent<Animator>();
    }
    private void playMeow()
    {
        transform.GetChild(0).GetComponent<RandomCatNoises>().playMeow();
    }
    IEnumerator movePosition(Vector2 targetPos, Action complete = null, float pace = 1)
    {
        while (getRigidBody2D().velocity != Vector2.zero)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Vector2 speed = targetPos - (Vector2)transform.position;
        speed.Normalize();
        getRigidBody2D().velocity = speed * pace;
        getAnimator().SetBool("walk", true);
        while (Vector2.Distance(transform.position, targetPos) > .1f)
        {
            yield return null;
            setTutorialUIPosition();
        }
        getAnimator().SetBool("walk", false);
        getRigidBody2D().velocity = Vector2.zero;
        if (complete != null)
        {
            complete();
        }
    }
    private void walkIntoView()
    {
        //<walks fully into view>

        StartCoroutine(movePosition(new Vector2(mainCamera.ViewportToWorldPoint(new Vector3(0.1f, 0, 0)).x, transform.position.y), ContinueTutorial, 2.2f));
    }
    private void Welcome()
    {
        heartParticles();
        StartCoroutine(movePosition(new Vector2(mainCamera.ViewportToWorldPoint(new Vector3(0.38f, 0, 0)).x, transform.position.y), null, 2.2f));
    }
    private void heartParticles()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/love"), transform, false);
        playMeow();
    }
    private void increaseCoin()
    {
        GameControl.control.playerData.gold = 1;
        GameControl.control.UpdatePlayerUI();
        getAnimator().SetTrigger("throw");
    }

    private void getRandomCat()
    {
        turnOnGraphicRaycaster();
        /*
        paw graphic
        <press shop>
        <press random>
        <press back
        */
        pawGraphic = GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/tutorialPaw"), GameControl.control.getMainUI().mainButtons.transform.Find("Shop"), false).transform;
        Button button = pawGraphic.parent.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            //button.onClick.RemoveAllListeners();
            button = GameControl.control.getMainUI().gacha.shopPanel.transform.Find("Random").GetComponent<Button>();
            pawGraphic.SetParent(button.transform, false);
            button.onClick.AddListener(() =>
            {
                button.onClick.RemoveAllListeners();
                GameControl.control.GetComponent<GraphicRaycaster>().enabled = false;
                button = GameControl.control.getMainUI().detailCard.transform.Find("exit").GetComponent<Button>();
                skip.gameObject.SetActive(false);
                pawGraphic.SetParent(button.transform, false);
                button.onClick.AddListener(() =>
                {
                    button.onClick.RemoveAllListeners();
                    GameControl.control.GetComponent<GraphicRaycaster>().enabled = true;
                    skip.gameObject.SetActive(true);
                    button = GameControl.control.getBackButton();
                    button.onClick.AddListener(Continue);
                    pawGraphic.SetParent(button.transform, false);
                });

            });

        });
    }
    private void afterShop()
    {
        turnOffGraphicRaycaster();
        GameControl.control.getMainUI().mainButtons.transform.Find("Shop").GetComponent<Button>().onClick.RemoveAllListeners();
        // GameControl.control.getMainUI().gacha.shopPanel.transform.Find("Random").GetComponent<Button>().onClick.RemoveAllListeners();
    }
    private void Continue()
    {
        pawGraphic.gameObject.SetActive(false);
        tutorialUI.transform.localScale = Vector3.zero;
        tutorialUI.gameObject.SetActive(true);
        GameControl.control.getBackButton().onClick.RemoveListener(Continue);
        ContinueTutorial();
    }
    private void turnOffGraphicRaycaster()
    {
        GameControl.control.getMainUI().GetComponent<GraphicRaycaster>().enabled = false;
    }
    private void turnOnGraphicRaycaster()
    {
        GameControl.control.getMainUI().GetComponent<GraphicRaycaster>().enabled = true;
    }

    private void addToTeam()
    {/*
        paw graphic
        <press team>
        <press first slot>
        <press the cat you bought>
        <press back>
        */

        turnOnGraphicRaycaster();
        pawGraphic.gameObject.SetActive(true);
        pawGraphic.SetParent(GameControl.control.getMainUI().mainButtons.transform.Find("Team"), false);
        Button button = pawGraphic.parent.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            button.onClick.RemoveAllListeners();
            button = GameControl.control.getMainUI().transform.Find("Team").GetChild(0).GetComponent<Button>();
            pawGraphic.SetParent(button.transform, false);
            button.onClick.AddListener(() =>
            {
                button.onClick.RemoveAllListeners();
                button = GameControl.control.getMainUI().deck.transform.GetChild(1).GetComponent<Button>();
                pawGraphic.SetParent(button.transform, false);
                button.onClick.AddListener(() =>
                {
                    // button.onClick.RemoveAllListeners();
                    button = GameControl.control.getBackButton();
                    pawGraphic.SetParent(button.transform, false);
                    button.onClick.AddListener(Continue);
                });
            });
        });
    }
    private void increaseToDefault()
    {

        turnOffGraphicRaycaster();
        GameControl.control.playerData.silver = PlayerData.defaultSilver;
        GameControl.control.SetGold(PlayerData.defaultGold);
        getAnimator().SetTrigger("throw");
    }
    private void walkOff()
    {
        unBlockExplore();
        turnOnGraphicRaycaster();
        playMeow();
        StartCoroutine(movePosition(new Vector2(mainCamera.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x, transform.position.y),
       () => Finish(false), 2.5f));
    }

    private void Finish(bool skipped)
    {
        if (pawGraphic != null)
        {
            Destroy(pawGraphic.gameObject);
        }
        if (tutorialUI != null)
        {
            if (!skipped)
            {
                Destroy(gameObject);
            }
            Destroy(tutorialUI.gameObject);
            Destroy(skip.gameObject);
            onComplete();
        }

    }

    public void StartTutorial(Action onComplete)
    {
        initTutorialDialogue();
        Canvas mainUI = GameControl.control.getMainUI().GetComponent<Canvas>();
        mainUI.renderMode = RenderMode.ScreenSpaceCamera;
        mainCamera = Camera.main;
        mainUI.worldCamera = mainCamera;
        this.onComplete = onComplete;
        MainMenuCat MMCat = GetComponent<MainMenuCat>();
        cat = new Cat(CatType.bread);
        transform.localScale = Vector3.one * 0.7f;
        turnOffGraphicRaycaster();
        transform.GetChild(0).GetComponent<RandomCatNoises>().enableNoises = false;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        MMCat.init(cat, new Vector3(mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 0.5f, -3.5f, 0), "FX");
        Destroy(MMCat);
        transform.eulerAngles = Vector3.zero;
        gameObject.SetActive(true);
        initTutorialObject();
        blockExplore();
    }
    private void unBlockExplore()
    {
        Button explore = GameControl.control.getMainUI().mainButtons.transform.Find("Explore").GetComponent<Button>();
        explore.onClick.RemoveAllListeners();
        explore.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);

        explore.onClick.AddListener(() => Finish(false));

    }

    private void blockExplore()
    {
        Button explore = GameControl.control.getMainUI().mainButtons.transform.Find("Explore").GetComponent<Button>();
        explore.onClick.AddListener(() =>
{
    tutorialUI.transform.localScale = Vector3.zero;
    tutorialUI.gameObject.SetActive(true);
    string[] Prevent = new string[]{
                "You can't go there yet :<",
                "Still some things to do :<",
                "Not yet :<",
                "We'll get there!",
    };

    tutorialDialogue.Push(new DialogueToAction(
  "", turnOnGraphicRaycaster
    ));
    tutorialDialogue.Push(new DialogueToAction(
        Prevent[UnityEngine.Random.Range(0, Prevent.Length)], turnOffGraphicRaycaster
    ));
    ContinueTutorial();
});
        explore.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);

    }

    private void initTutorialObject()
    {
        tutorialUI = GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/tutorial"), GameControl.control.transform, false).transform;
        tutorialUI.transform.localScale = Vector3.zero;
        setTutorialUIPosition();
        skip = GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/tutorialSkip"), GameControl.control.transform, false).GetComponent<Button>();
        skip.onClick.AddListener(() =>
        {
            Finish(true);
            SceneManager.LoadScene(0);
        });
        tutorialUI.GetComponent<Button>().onClick.AddListener(ContinueTutorial);
        walkIntoView();
    }
    private void ContinueTutorial()
    {
        GameControl.control.getSoundManager().playButton();
        AnimateDown(() =>
        {
            if (tutorialDialogue.Count == 0)
            {
                return;
            }
            DialogueToAction DA = tutorialDialogue.Pop();
            if (DA.dialogue == "")
            {
                tutorialUI.gameObject.SetActive(false);
            }
            else
            {
                getTutorialText().text = DA.dialogue;
            }
            if (DA.action != null)
            {
                DA.action();
            }
            AnimateUp();
        });
    }
    private void AnimateDown(Action onDone)
    {
        if (tutorialUI.transform.localScale == Vector3.zero)
        {
            onDone();
            return;
        }
        LeanTween.scale(tutorialUI.gameObject, Vector3.zero, 0.35f).setEaseOutSine().setOnComplete(onDone);
    }
    private void AnimateUp()
    {
        LeanTween.scale(tutorialUI.gameObject, Vector3.one, 0.3f).setEaseInSine();
    }
}
