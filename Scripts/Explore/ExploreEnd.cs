using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExploreEnd : MonoBehaviour
{
    public Reward reward;
    public TextMeshProUGUI titleText;
    public AudioClip victory, defeat;

    //show credits screen
    public void wonLastLevel(ExploreController controller, bool wonByAction, GameObject cat, ushort rating)
    {
        Credits credits = GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/Credits")).GetComponent<Credits>();
        credits.init(() => init(true, controller, wonByAction, rating), controller, cat);
    }
    public void init(bool won, ExploreController controller, bool wonByAction, ushort rating)
    {

        bool hasWon = GameControl.control.getWorldLevel(ExploreController.world) > ExploreController.level;
        if (!hasWon && won && controller.stage.rewardPrompt.rewards != null && controller.stage.rewardPrompt.rewards.Length > 0)
        {
            setRewardPrompt(controller, wonByAction, rating, hasWon);
        }
        else
        {
            setExploreUI(won, controller, wonByAction, false, rating, hasWon);
        }
    }

    private void setRewardPrompt(ExploreController controller, bool wonByAction, ushort rating, bool hasWon)
    {
        GameControl.control.getSoundManager().playExploreButton();
        if (controller.stage.rewardPrompt.rewards.Length == 1)
        {
            GameControl.control.YesNoPrompt("<font=\"pixelfont\">" + controller.stage.rewardPrompt.prompt, transform.root, () =>
              {
                  controller.stage.rewardCat = controller.stage.rewardPrompt.rewards[0];
                  setExploreUI(true, controller, wonByAction, true, rating, hasWon);
              }, () => { setExploreUI(true, controller, wonByAction, false, rating, hasWon); });
        }
        else
        {
            UnityAction[] listeners = new UnityAction[controller.stage.rewardPrompt.rewards.Length];
            string[] labels = new string[controller.stage.rewardPrompt.rewards.Length];
            for (int i = 0; i < controller.stage.rewardPrompt.rewards.Length; i++)
            {
                CatType rewardCat = controller.stage.rewardPrompt.rewards[i];
                listeners[i] = () =>
                {
                    controller.stage.rewardCat = rewardCat;
                    setExploreUI(true, controller, wonByAction, true, rating, hasWon);
                };
                labels[i] = new Cat(rewardCat).Name;
            }
            GameControl.control.MultiplePrompt("<font=\"pixelfont\">" + controller.stage.rewardPrompt.prompt, transform.root, listeners, labels,
            () =>
            {
                setExploreUI(true, controller, wonByAction, false, rating, hasWon);
                Debug.Log("ADDD");
            });
        }
    }

    private void setExploreUI(bool won, ExploreController controller, bool wonByAction, bool catPrompt, ushort rating, bool hasWonBefore)
    {
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.4f).setEaseInQuad();
        uint silver = MathUtils.AdjustedSilver
        (controller.stage.silver, ExploreController.level > -1 ? GameControl.control.getWorldLevelPlays(ExploreController.world, ExploreController.level) : 0, rating);


        if (won)
        {
            titleText.text = "Victory!";
            if (silver > 0)
            {
                getReward().initsilver(silver);
            }

            if (controller.stage.gold > 0 && !hasWonBefore)
            {
                getReward().initgold(controller.stage.gold);
            }
            Debug.Log("initing cat award.. has won level before? " + hasWonBefore);
            if (controller.stage.rewardCat != CatType.none && !hasWonBefore
             && ((wonByAction && UnityEngine.Random.value > 0.25f) || catPrompt || UnityEngine.Random.value > 1.05f - (rating * 0.1f)))
            {
                getReward().initcat(controller.stage.rewardCat);
            }
            GameControl.control.getSoundManager().playOneShot(victory, 1);
        }
        else
        {
            titleText.text = getDefeat() + getLoss(silver, controller);
            GameControl.control.getSoundManager().playOneShot(defeat, 1);
        }
    }
    string getDefeat()
    {
        string[] defeat = new string[]{
                "Defeat<font=\"cutefont\"><size=66%>(•_•)</font>",
                "Defeat<font=\"cutefont\"><size=66%>(‘д`)</size></font>!",
                 "Defeat<font=\"cutefont\"><size=66%>(´_`)</font>",
                 "Defeat!",
                 "Defeat :(",
        };
        return defeat[UnityEngine.Random.Range(0, defeat.Length)];
    }

    string getLoss(uint silver, ExploreController controller)
    {
        //reduce original reward by (1/3
        silver = (uint)Mathf.Clamp((float)silver*0.25f, 0, GameControl.control.playerData.silver >= 21 ? GameControl.control.playerData.silver / 2 : 0);
        Debug.Log("silver loss - " + silver);
        string lossStr = "";
        if (silver > 0)
        {
            lossStr = silver.ToString() + CatIAP.silverStr;
            GameControl.control.DecrementSilver(silver);
        }
        Cat user = controller.userPlayer.allCats[UnityEngine.Random.Range(0, controller.userPlayer.allCats.Length)].cat;
        Cat enemy = controller.enemyPlayer.allCats[UnityEngine.Random.Range(0, controller.enemyPlayer.allCats.Length)].cat;
        if (!string.IsNullOrEmpty(lossStr))
        {
            string moneyStr = lossStr;
            lossStr = "<line-height=125%>\n<size=58%><line-height=120%>";
            string[] phrases = new string[]{
                    enemy.Name + " took "+moneyStr+ " from you in victory :(",
                    enemy.Name + " won "+moneyStr+ " from you :<",
                    user.Name + " gave "+ enemy.Name + " "+moneyStr+ " for a good game!",
                    user.Name + " gave "+ enemy.Name + " "+moneyStr+ " for a fair play!",
                    user.Name + " rewarded "+ enemy.Name + " "+moneyStr+ " for their win.",
                    user.Name + " gifted "+ enemy.Name + " "+moneyStr+ " for a fun time.",
            };
            lossStr += phrases[UnityEngine.Random.Range(0, phrases.Length)];
        }
        else
        {

            lossStr = "<line-height=120%>\n<size=58%><line-height=100%>";
            string[] tooltips = new string[]{
                "Winning via Acts gives a 60% higher rate of winning a cat!",
                "Try leveling up with fishbones!",
                "The right Act can get an automatic win.",
                "Have you tried using Acts?",
                "Maybe a different team could help.",
                "Perhaps leveling up could help.",
                "A stronger team might do the trick.",
                "Did you know that the right Acts can cause a win?",
                "Run out of silver? Try getting some from past stages!",

            };
            lossStr += tooltips[UnityEngine.Random.Range(0, tooltips.Length)];
        }
        return lossStr;
    }

    public void returnToMainMenu()
    {
        GameControl.control.getSoundManager().playError();
        GameControl.control.getBackButton().gameObject.SetActive(false);
        GameControl.control.loadMain();
    }

    private Reward getReward()
    {
        if (reward.Initiated)
        {
            Reward newReward = GameObject.Instantiate(reward.gameObject, reward.transform.parent, false).GetComponent<Reward>();
            newReward.Initiated = false;
            return newReward;
        }
        return reward;
    }

    public void returnToMap()
    {
        GameControl.control.getSoundManager().playExploreButton();
        GameControl.control.GetComponent<Canvas>().enabled = true;
        SceneManager.LoadScene("Map");
    }
}
