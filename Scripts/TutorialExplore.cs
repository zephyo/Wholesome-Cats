using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TutorialExplore : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    private ExploreController controller;
    Queue<string> dialogues = new Queue<string>(
        new string[]{
        "I'm back and here to help! Try <b>FIGHT</b> to lower your opponent's health!",
        "Select your target!",
        "",
        "Nice! <b>FIGHT</b> and <b>ACT</b> can help you win. Good luck!",
        ""
        }
    );
    private List<EventTrigger.TriggerEvent> enemyTriggers = new List<EventTrigger.TriggerEvent>();
    private Button flee, fight, action;
    public void Init(ExploreController controller)
    {
        this.controller = controller;
        transform.localScale = Vector3.one * 0.3f;
        stage1();
        ContinueTutorial();
    }

    private void stage1()
    {
        flee = controller.exploreChoice.transform.Find("Flee").GetComponent<Button>();
        flee.onClick.AddListener(destroyThis);
        fight = controller.exploreChoice.transform.Find("Fight").GetComponent<Button>();
        fight.onClick.AddListener(stage2);
        action = controller.exploreChoice.transform.Find("Act").GetComponent<Button>();
        action.onClick.AddListener(setInactive);
    }

    private void setInactive()
    {
        AnimateDown(() => gameObject.SetActive(false));
    }
    private void stage2()
    {
        flee.onClick.RemoveListener(destroyThis);
        fight.onClick.RemoveListener(stage2);
        action.onClick.RemoveListener(setInactive);
        if (controller.enemyPlayer.aliveCats.Count == 1)
        {
            destroyThis();
            return;
        }
        foreach (ExploreCat cat in controller.enemyPlayer.aliveCats)
        {
            EventTrigger.TriggerEvent TE = cat.gameObject.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.PointerClick).callback;
            TE.AddListener(stage3);
            enemyTriggers.Add(TE);
        }
        ContinueTutorial();
    }

    private void stage3(BaseEventData bed)
    {
        Action<AttackType, ExploreCat> onHit = controller.userPlayer.aliveCats[0].onHit;
        ExploreCat user = controller.exploreChoice.getCurrentUser();
        ExploreCat enemy = controller.exploreChoice.getCurrentEnemy();
        for (int i = 0; i < controller.enemyPlayer.aliveCats.Count; i++)
        {
            enemyTriggers[i].RemoveListener(stage3);
        }
        user.onHit = (AttackType a, ExploreCat u) =>
            {
                onHit(a, u);
                stage4(user, enemy);
            };
        enemy.onHit = (AttackType a, ExploreCat u) =>
            {
                onHit(a, u);
                stage4(user, enemy);
            };
        ContinueTutorial();
    }
    private void stage4(ExploreCat user, ExploreCat enemy)
    {
        gameObject.SetActive(true);
        user.onHit = controller.OnHit;
        enemy.onHit = controller.OnHit;

        flee.onClick.AddListener(destroyThis);
        fight.onClick.AddListener(finish);
        action.onClick.AddListener(finish);
        ContinueTutorial();
    }

    private void finish()
    {
        flee.onClick.RemoveListener(destroyThis);
        fight.onClick.RemoveListener(finish);
        action.onClick.RemoveListener(finish);
        ContinueTutorial();
    }
    private void destroyThis()
    {
        Destroy(gameObject);
    }

    private void ContinueTutorial()
    {
        AnimateDown(() =>
        {
            if (dialogues.Count == 0)
            {
                destroyThis();
                return;
            }
            string nextDialogue = dialogues.Dequeue();
            if (string.IsNullOrEmpty(nextDialogue))
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            tutorialText.text = nextDialogue;

            AnimateUp();
        });
    }
    private void AnimateDown(Action onDone)
    {
        if (transform.localScale == Vector3.zero)
        {
            onDone();
            return;
        }
        LeanTween.scale(gameObject, Vector3.one * 0.3f, 0.2f).setEaseOutSine().setOnComplete(onDone);
    }
    private void AnimateUp()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEaseInSine();
    }

}
