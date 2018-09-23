using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ExploreChoice : MonoBehaviour
{
    private ExploreController controller;
    [HideInInspector]
    private int currentUser = -1;
    private int currentEnemy = -1;
    [HideInInspector]
    public SpriteRenderer selector;
    private SpriteRenderer getActiveSelector()
    {
        if (selector != null)
        {
            LeanTween.cancel(selector.gameObject);
            selector.gameObject.SetActive(true);
            selector.color = Color.white;
        }
        else
        {
            selector = Instantiate(Resources.Load<GameObject>("miscPrefabs/selector"), controller.enemyPlayer.aliveCats[0].transform, false).GetComponent<SpriteRenderer>();
        }
        return selector;
    }
    public void Init(ExploreController controller)
    {
        this.controller = controller;
        SetActs();
        gameObject.SetActive(true);
    }

    private void SetActs()
    {
        Transform opts = transform.Find("ActOpt");
        int i = 0;
        for (; i < controller.userPlayer.aliveCats.Count; i++)
        {
            ExploreCat cat = controller.userPlayer.aliveCats[i];
            ActionType action = cat.cat.getCatAsset().action;
            Button button = opts.GetChild(getButtonIndex(i, controller.userPlayer.aliveCats.Count)).GetComponent<Button>();
            cat.onAction = (ActionType act, ExploreCat catInstance) => UserAction(button, catInstance, act);
            SetActionButton(button, cat, action);
        }
        Debug.Log("setting enemy onacts.. enemy has " + controller.enemyPlayer.aliveCats.Count + " cats");
        i = 0;
        for (; i < controller.enemyPlayer.aliveCats.Count; i++)
        {
            Debug.Log("setting action of " + controller.enemyPlayer.aliveCats[i].cat.Name);
            controller.enemyPlayer.aliveCats[i].onAction = enemyAction;
        }
    }

    private void OnEnable()
    {
        if (controller.userPlayer.aliveCats.Count == 0 || controller.enemyPlayer.aliveCats.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        if (currentUser > -1)
        {
            getCurrentUser().Action.interactable = false;
        }
        currentUser++;
        currentEnemy++;
        if (currentUser >= controller.userPlayer.aliveCats.Count)
        {
            currentUser = 0;
        }
        if (currentEnemy >= controller.enemyPlayer.aliveCats.Count)
        {
            currentEnemy = 0;
        }
        Debug.Log("currentUser " + currentUser);
        Debug.Log("currentEnemy " + currentEnemy);
        setUI();
    }
    public ExploreCat getCurrentEnemy()
    {
        return controller.enemyPlayer.aliveCats.Count > 0 ? controller.enemyPlayer.aliveCats[currentEnemy % controller.enemyPlayer.aliveCats.Count] : null;
    }
    public ExploreCat getCurrentUser()
    {
        return controller.userPlayer.aliveCats.Count > 0 ? controller.userPlayer.aliveCats[currentUser % controller.userPlayer.aliveCats.Count] : null;
    }

    private void setUI()
    {
        GameControl.GetTextBox(transform, "text").text = getCurrentUser().cat.Name + "'s turn";
        getActiveSelector().transform.SetParent(getCurrentUser().transform, false);
        // current user cat's button is interactable unless they've used their action
        if (!getCurrentUser().usedAction)
        {
            getCurrentUser().Action.interactable = true;
        }
    }
    private int getButtonIndex(int i, int max)
    {
        if (max == 4)
        {
            if (i == 0)
            {
                return 1;
            }
            if (i == 1)
            {
                return 3;
            }
            if (i == 2)
            {
                return 0;
            }
            if (i == 3)
            {
                return 2;
            }
        }
        if (max == 3)
        {
            if (i == 0)
            {
                return 1;
            }
            if (i == 1)
            {
                return 0;
            }
        }
        if (max == 2)
        {
            return i == 0 ? 1 : 0;
        }
        return i;
    }
    private void SetActionButton(Button button, ExploreCat cat, ActionType act)
    {
        button.onClick.AddListener(() =>
        {
            cat.useAction();
        });
        button.transform.Find("text").GetComponent<TextMeshProUGUI>().text = act.ToString();
        cat.Action = button;
    }

    private void enemyAction(ActionType action, ExploreCat cat)
    {
        if (cat.currentHealth <= 0)
        {
            cat.onHit(AttackType.None, cat);
            return;
        }
        Debug.Log("enemy action!");
        Dialogue[] D = DataUtils.defaultReaction(action,
            //random user Cat
            cat.target.cat);
        for (int i = 0; i < D.Length; i++)
        {
            if (D[i].speaker == CatType.none)
            {
                D[i].speaker = cat.cat.catType;
            }
            else
            {
                D[i].speaker = controller.userPlayer.allCats[0].cat.catType;
            }
        }
        ExploreCat user = controller.userPlayer.allCats[0];
        controller.exploreStory.InitStory(D, user, controller.enemyPlayer.allCats);
        controller.exploreStory.Finished += () =>
        {
            cat.onHit(AttackType.None, cat);
            ExploreChoice.defaultResponse(action, cat);
        };


    }

    ExploreCat findActionCat(ExploreCat def, Dialogue[] response)
    {
        foreach (Dialogue d in response)
        {
            Debug.Log("searching dialogue " + d.dialogue[0]);
            if (d.speaker != CatType.none)
            {
                foreach (ExploreCat cat in controller.enemyPlayer.allCats)
                {
                    Debug.Log("enemy cat: " + cat.cat.Name);
                    if (cat.cat.catType == d.speaker)
                    {
                        Debug.Log("enemy cat found");
                        return cat;
                    }
                }

            }
        }
        return def;
    }

    /*    
    use controller.stage.Responses to adjust response to certain actions
    if no response, use DataUtils.defaultReaction
    make that action button uninteractable after use so cant be used again
    */
    private void UserAction(Button button, ExploreCat userCat, ActionType action)
    {
        gameObject.SetActive(false);
        ExploreCat enemyCat = controller.enemyPlayer.aliveCats[UnityEngine.Random.Range(0, controller.enemyPlayer.aliveCats.Count)];
        selector.gameObject.SetActive(false);
        Action victory = () =>
        {
            controller.AcceptedGameOver(action, userCat.cat, enemyCat.cat);
        };
        Action defeat = () =>
        {
            defaultResponse(action, userCat);

            ExploreCat enemy = getCurrentEnemy();
            if (enemy != null)
            {
                enemy.Enable();
            }
        };

        //get always response
        alwaysResponse(action, userCat);

        //check if there's a premediated response
        if (controller.stage.Responses != null)
        {
            foreach (ActionResponse response in controller.stage.Responses)
            {
                if (response.action == action)
                {
                    //find enemy cat that matches response
                    enemyCat = findActionCat(enemyCat, response.response);

                    Action meh = () =>
                    {

                        if (action != ActionType.Think)
                        {
                            string[] appeasedNotifs = new string[]{
                userCat.cat.Name + "'s "+action.ToString()+" seems to make "+enemyCat.cat.Name+" friendlier!",
                enemyCat.cat.Name+" looks slightly happier from " + userCat.cat.Name + "'s "+action.ToString()+"!",
                enemyCat.cat.Name+" seems nicer towards " + userCat.cat.Name + "!",
                enemyCat.cat.Name+" seems appeased by " + userCat.cat.Name + "'s "+action.ToString()+"!",
                enemyCat.cat.Name+" looks kinder towards " + userCat.cat.Name + " after their "+action.ToString()+"!",
                enemyCat.cat.Name+" glows a bit from " + userCat.cat.Name + "'s "+action.ToString()+".",
                enemyCat.cat.Name+" looks lighter after " + userCat.cat.Name + "'s "+action.ToString()+".",

                        };
                            controller.Notify(new string[]{
                appeasedNotifs[UnityEngine.Random.Range(0, appeasedNotifs.Length)]
                            }, () =>
                            {
                                if (enemyCat.currentHealth > 0)
                                {
                                    enemyCat.takeDamage(userCat.realizedStats.attackDamage * 1.75f);
                                }
                                defeat();
                            });
                        }
                        else
                        {
                            defeat();
                        }
                    };

                    if (response.accepted)
                    {
                        controller.exploreStory.Finished += victory;
                    }
                    else
                    {
                        controller.exploreStory.Finished += meh;
                    }

                    controller.exploreStory.InitStory(response.response, userCat, controller.enemyPlayer.allCats);
                    return;
                }
            }
        }
        Debug.Log("CHOICE SPEAKER - " + userCat.cat.Name);
        //else, use default response
        controller.exploreStory.InitStory(DataUtils.defaultReaction(action,
       //random cat from enemy team
       enemyCat.cat), userCat, controller.enemyPlayer.allCats);
        // for magic, fly, etc

        controller.exploreStory.Finished += defeat;
    }

    public static void defaultResponse(ActionType action, ExploreCat userCat)
    {
        switch (action)
        {
            case ActionType.Magic:
                //instantiates sparkle particles; change color to something else; level + 2
                userCat.Magic();
                break;
            case ActionType.Ghost:
                //increases stats; cat fades then disappears from field; harder to land a hit
                userCat.tempBoost(2);
                userCat.FadeAndAvoidHit();
                break;
            case ActionType.Fly:
                // automatically able to flee - on flee, message isn't 'fled successfully', but '[name] used fly to escape!' 
                //if in battle, cat floats above, only ranged cats or other flying cats can land a hit
                userCat.FlyAndAvoidHit();
                break;
        }
    }

    private bool EnemyAction(ExploreCat cat)
    {
        switch (cat.cat.getCatAsset().action)
        {
            case ActionType.Magic:
                if (cat.owner.noMagic)
                {
                    return false;
                }
                if (cat.currentHealth > (float)cat.realizedStats.maxHealth * 0.7f)
                {
                    Debug.Log("current health - " + cat.currentHealth + "max health - " + cat.realizedStats.maxHealth + " 75% of max: " + ((float)cat.realizedStats.maxHealth * 0.75f));
                    return false;
                }
                return true;
            case ActionType.Ghost:
            case ActionType.Fly:
                return true;
        }
        return false;
    }

    private void alwaysResponse(ActionType action, ExploreCat cat)
    {
        switch (action)
        {
            case ActionType.Love:
            case ActionType.Comfort:
            case ActionType.Hug:
                cat.loveParticles();
                break;

        }
    }
    public void Fight()
    {

        gameObject.SetActive(false);
        ExploreCat currentUser = getCurrentUser();
        //if all cats just died
        if (currentUser == null){
            return;
        }
        if (currentUser.sideEffect != null)
        {
            currentUser.checkRemoveEffect();
            if (currentUser.sideEffect != null)
            {
                fadeSelector();
                currentUser.onHit = (AttackType a, ExploreCat c) =>
                {
                    getCurrentEnemy().Enable();
                    c.onHit = controller.OnHit;
                };
                currentUser.Enable();
                return;
            }
        }

        if (controller.enemyPlayer.aliveCats.Count <= 1)
        {
            startFight(getCurrentEnemy());
            return;
        }

        Camera.main.GetComponent<Physics2DRaycaster>().enabled = true;
        getActiveSelector().transform.SetParent(controller.enemyPlayer.aliveCats[0].transform, false);
        // setSelectorSprite(selector, controller.userPlayer.aliveCats[0].cat.getCatAsset().faction);
    }

    public void setFight(ExploreCat enemy)
    {

        if (getCurrentUser().target != null)
        {
            Debug.Log("nope!");
            GameControl.control.getSoundManager().playError  ();
            return;
        }
        Debug.Log("loading..");
        selector.transform.SetParent(enemy.transform, false);

        startFight(enemy);
        Camera.main.GetComponent<Physics2DRaycaster>().enabled = false;
        GameControl.control.getSoundManager().playButton();
    }

    private void fadeSelector()
    {
        LeanTween.value(selector.gameObject, (float val) =>
        {
            selector.color = new Color(1, 1, 1, val);
        }, 1, 0, 0.5f).setEaseOutSine().setDelay(0.5f).setOnComplete(() =>
        {
            selector.gameObject.SetActive(false);
        });
    }
    public void startFight(ExploreCat enemy)
    {
        fadeSelector();
        ExploreCat cEnemy = getCurrentEnemy();
        ExploreCat user = getCurrentUser();

        if (user == null || cEnemy == null)
        {
            return;
        }
        user.target = enemy;
        user.Enable();
        if (cEnemy.sideEffect != null)
        {
            cEnemy.checkRemoveEffect();
            if (cEnemy.sideEffect != null)
            {
                user.onHit = (AttackType a, ExploreCat c) =>
                {
                    controller.playSound(a);
                    cEnemy.Enable();
                    c.onHit = controller.OnHit;
                };
                return;
            }
        }
        if (!cEnemy.usedAction && EnemyAction(cEnemy))
        //&& UnityEngine.Random.value > 0.15f)
        {
            if (cEnemy.target == null)
            {
                cEnemy.findTarget();
            }
            user.onHit = (AttackType a, ExploreCat c) =>
           {
               controller.playSound(a);
               cEnemy.useAction();
               c.Disable();
               c.onHit = controller.OnHit;
           };
            return;
        }
        cEnemy.Enable();
    }
    public void Flee()
    {
        selector.gameObject.SetActive(false);
        controller.exploreChoice.gameObject.SetActive(false);
        controller.Notify(new string[] { "You fled successfully." }, () =>
               {
                   controller.GameOver(false);
               });
    }
}
