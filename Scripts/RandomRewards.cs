using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class RandomRewards : MonoBehaviour
{
    //Reward data

    #region REWARDS -----


    public struct RandomReward
    {
        public string dialogue;
        public string yesString;
        public string noString;
        public UnityAction yesAction;
        public UnityAction noAction;
        public Sprite catFace;
        public RandomReward(string dialogue, string yes, string no, UnityAction yesAct, UnityAction noAct, Sprite face)
        {
            this.dialogue = dialogue;
            this.yesString = yes;
            this.noString = no;
            this.yesAction = yesAct;
            this.noAction = noAct;
            this.catFace = face;
        }
    }

    private Cat getRandomCat()
    {
        return GameControl.control.playerData.deck[UnityEngine.Random.Range(0, GameControl.control.playerData.deck.Count)];
    }

    private uint getRandomSilver()
    {
        uint silver = MathUtils.FairSilver();
        Debug.Log("reward of average silver: " + silver);
        return (uint)UnityEngine.Random.Range(silver * 0.5f, silver * 1.75f);
    }
    public RandomReward RandomMainReward()
    {
        Cat speaker = getRandomCat();
        uint silver = getRandomSilver();
        UnityAction incrementGold = () => { GameControl.control.IncrementGold(1); };
        UnityAction incrementSilver = () =>
        {
            GameControl.control.IncrementSilver(silver);
            GameControl.control.SavePlayerData();
        };

        RandomReward[] rewards = new RandomReward[]{
            new RandomReward("\"A fun fact about me: I love you lots!\" "+ speaker.Name + " gives you "+silver+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " found a"+CatIAP.goldStr+" on the floor!", null, null, incrementGold,null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " found "+silver+""+CatIAP.silverStr+" on the floor!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " boops you, drops "+silver+""+CatIAP.silverStr+" at your feet, then runs away!", null, null,incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Hooman! I demand a hug!\" "+speaker.Name + " meows.", "Hug","Ignore", ()=>{
               GameControl.control.Notify(speaker.Name + " wraps around you and gifts you "+silver+""+CatIAP.silverStr+" in affection.", GameControl.control.transform,incrementSilver).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward("\"Meow?\" "+speaker.Name + " asks.", "Answer","Ignore", ()=>{
               GameControl.control.Notify("\"What?\" You ask.\n\"For you!\" "+speaker.Name + " offers "+silver+CatIAP.silverStr+"!", GameControl.control.transform,incrementSilver).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " is pushing something out of sight..", "check out","Ignore", ()=>{
               GameControl.control.Notify("It's a hidden stash of dust bunnies, hair ties, and "+silver+CatIAP.silverStr+"!", GameControl.control.transform,incrementSilver).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward(speaker.Name+ " is chewing on something!", "check out","Ignore", ()=>{
               GameControl.control.Notify(speaker.Name+" chokes, then coughs up " +silver+""+CatIAP.silverStr+"!", GameControl.control.transform,incrementSilver).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " pulls your face in for a boop!", "Boop","Turn away", ()=>{
               GameControl.control.Notify(speaker.Name + " gifts you "+silver+""+CatIAP.silverStr+" in affection!", GameControl.control.transform,incrementSilver).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward(speaker.Name +" sits on you.", "Move","Wait", ()=>{
               GameControl.control.Notify(speaker.Name + " humphs and runs away.", GameControl.control.transform,null);
            }, ()=>{
               GameControl.control.Notify(speaker.Name+" gifts "+silver+""+CatIAP.silverStr+" as thanks for being a good pillow!", GameControl.control.transform,incrementSilver).RewardBackground();
            }, speaker.getCatAsset().meow),
            new RandomReward("..! "+speaker.Name + " is playing with something!", "check out","Ignore", ()=>{
               GameControl.control.Notify("It's a"+CatIAP.goldStr+"!", GameControl.control.transform,incrementGold).RewardBackground();
            }, null, speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " walks toward you..", "Pet","Ignore", ()=>{
               GameControl.control.Notify("\"What? Can't I enjoy a good meal in peace?\" "+speaker.Name+" walks away haughtily, taking the"+CatIAP.silverStr+" with them.", GameControl.control.transform,null);
            }, ()=>{
               GameControl.control.Notify(speaker.Name + " avoids your pat, drops "+silver+""+CatIAP.silverStr+" at your feet, and runs away.", GameControl.control.transform,incrementSilver).RewardBackground();
            }, speaker.getCatAsset().meow),
            new RandomReward("..BURP. "+speaker.Name + " suddenly burps up "+silver+""+CatIAP.silverStr+".", "Stare","Pet", ()=>{
               GameControl.control.Notify("\"What? Can't I enjoy a good meal in peace?\" "+speaker.Name+" walks away haughtily, taking the"+CatIAP.silverStr+" with them.", GameControl.control.transform,null);
            }, ()=>{
               GameControl.control.Notify(speaker.Name+" purrs affectionately.", GameControl.control.transform,incrementSilver).RewardBackground();
            }, speaker.getCatAsset().meow),
            new RandomReward("\"Hey. Hey. Hey,\" "+ speaker.Name + " pats your face. \"I'm hungry.\"" , "Feed", "Ignore", ()=>{
                GameControl.control.Notify("\"Oooh, thank you!\" "+speaker.Name+" chomps up "+silver+CatIAP.silverStr+".", GameControl.control.transform,()=>{GameControl.control.DecrementSilver(silver);});
            },null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " is staring at you.", "Pet", "Ignore", ()=>{
              GameControl.control.Notify(speaker.Name + " ducks and runs away, leaving "+silver+""+CatIAP.silverStr+" behind.", GameControl.control.transform,incrementSilver).RewardBackground();
            },null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " is covering something with their paws!", "check out", "Ignore", ()=>{
              GameControl.control.Notify("It's a secret stash of"+CatIAP.silverStr+"! \"I didn't steal them,\" "+ speaker.Name + " meows. ..Suspicious.", GameControl.control.transform,incrementSilver).RewardBackground();
            },null,speaker.getCatAsset().meow),
            new RandomReward("56585983ulllcipppp[o;;;;e;e;;;;;;. "+speaker.Name + " goes to step on the keyboard again - ", "Shoo", "Ignore", ()=>{
              GameControl.control.Notify(speaker.Name + " humphs and runs away, leaving "+silver+""+CatIAP.silverStr+".", GameControl.control.transform,incrementSilver).RewardBackground();
            },()=>{
               GameControl.control.Notify("fght4ljkhghgfr5656aaaaaaa", GameControl.control.transform, null);
            },speaker.getCatAsset().meow),
        };
        return rewards[UnityEngine.Random.Range(0, rewards.Length)];
    }
    public RandomReward RandomWonExploreReward()
    {
        Cat speaker = getRandomCat();
        uint silver = getRandomSilver();
        // UnityAction incrementGold = () => { GameControl.control.IncrementGold(1); };
        UnityAction incrementSilver = () =>
        {
            GameControl.control.IncrementSilver(silver);
            GameControl.control.SavePlayerData();
        };

        RandomReward[] rewards = new RandomReward[]{
              new RandomReward("..Oh no! "+speaker.Name + " is running away!", "Wait", "call", ()=>{
              GameControl.control.Notify(speaker.Name + " returns with "+silver+""+CatIAP.silverStr+"!", GameControl.control.transform, incrementSilver).RewardBackground();
             }, ()=>{
                 GameControl.control.Notify(speaker.Name + " hears and ignores your call.", GameControl.control.transform);
             }, speaker.getCatAsset().meow),
            new RandomReward("Upon returning, "+ speaker.Name + " gives you "+silver+CatIAP.silverStr+"! \"My hooman must be hungry!\" "+speaker.Name + " yelps.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " greets your return with "+silver+CatIAP.silverStr+"! \"I must feed my clumsy hooman,\" "+ speaker.Name + " meows.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"You did so good! Let's celebrate!\" "+ speaker.Name + " throws "+silver+""+CatIAP.silverStr+" your way in celebration!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Great job, cats ♡ I'm so proud of you!\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Yay, welcome back! You did so good!\" "+ speaker.Name + " throws "+silver+""+CatIAP.silverStr+" in the air in celebration!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Thanks for your hard work ♡ You deserve this!\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"You've done so much ♡ You cats must be tired! Here!\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("You come home to "+silver+ ""+CatIAP.silverStr+"!\n\"Just for you,\" "+speaker.Name+" meows.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"My hooman returns! I love my hooman.\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+" out of love.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
        };
        return rewards[UnityEngine.Random.Range(0, rewards.Length)];
    }
    public void InitRandomWelcomeReward()
    {
        Cat speaker = getRandomCat();
        uint silver = getRandomSilver();
        UnityAction incrementGold = () => { GameControl.control.IncrementGold(1); };
        UnityAction incrementSilver = () =>
        {
            GameControl.control.IncrementSilver(silver);
            GameControl.control.SavePlayerData();
        };

        RandomReward[] rewards = new RandomReward[]{
            new RandomReward("\"Welcome back!\" "+speaker.Name + " greets you with "+silver+""+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward(speaker.Name + " greets you by burping up "+silver+""+CatIAP.silverStr+" at your feet!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"The hooman's back!\" "+ speaker.Name + " attacks your feet, then coughs up "+silver+""+CatIAP.silverStr+" on the floor.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
             new RandomReward("\"Yay, you're back!\" "+speaker.Name + " gifts you "+silver+""+CatIAP.silverStr+" as thanks for being a good owner!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"You're back! I missed you,\" "+speaker.Name + " meows.", "Pet", "Feed", ()=>{
               GameControl.control.Notify(speaker.Name + " purs and gifts you " +silver+ ""+CatIAP.silverStr+".", GameControl.control.transform, incrementSilver).RewardBackground();
             },()=>{
               GameControl.control.Notify(speaker.Name + " gobbles " + silver+ CatIAP.silverStr+" and gifts a"+CatIAP.goldStr+" in thanks!", GameControl.control.transform, ()=>{ GameControl.control.DecrementSilver(silver); incrementGold();}).RewardBackground();
             },speaker.getCatAsset().meow),
            new RandomReward("\"Welcome back!\" "+ speaker.Name + " says, hacks up "+silver+""+CatIAP.silverStr+", then walks away.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Hi hooman! These represent my love for you!\" "+speaker.Name + " offers "+silver+""+CatIAP.silverStr+".", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Hello, pillow!\" "+speaker.Name + " meows.", "Pet", "Shoo", ()=>{
               GameControl.control.Notify(speaker.Name + " gifts "+silver+""+CatIAP.silverStr+" in thanks. \"Good pillow.\"", GameControl.control.transform, incrementSilver).RewardBackground();
             },null,speaker.getCatAsset().meow),
            new RandomReward("\"Hooman, look, look at what I found!\" "+ speaker.Name + " reveals a pile of "+silver+""+CatIAP.silverStr+".", null, null, incrementGold,null,speaker.getCatAsset().meow),
             new RandomReward("\"Hooman, please accept my gift of love.\" "+speaker.Name + " pushes "+silver+""+CatIAP.silverStr+" to you.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"I missed you. Pet me!\" "+speaker.Name + " demands.", "Pet", "Shoo", ()=>{
               GameControl.control.Notify(speaker.Name + " purs and leaves you "+silver+""+CatIAP.silverStr+"!", GameControl.control.transform, incrementSilver).RewardBackground();
             },null,speaker.getCatAsset().meow),
            new RandomReward("\"I missed you. Pet me!\" "+speaker.Name + " demands.", "Pet", "Ignore", ()=>{
               GameControl.control.Notify("\"<i>With your eyes!</i>\" " + speaker.Name + " runs away, leaving "+silver+""+CatIAP.silverStr+" behind.", GameControl.control.transform, incrementSilver).RewardBackground();
             },null,speaker.getCatAsset().meow),
            new RandomReward("\"Welcome back! Have you eaten enough today?\" "+ speaker.Name + " gives you "+silver+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"A fun fact about me: I hope you have a good day and I love you!\" "+ speaker.Name + " gives you "+silver+CatIAP.silverStr+"!", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"My hooman came back! I love my hooman.\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+" out of love.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"Favorite hooman! How are you?\" "+ speaker.Name + " gifts "+silver+""+CatIAP.silverStr+" out of love.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"The hooman who makes me happy is back,\" "+ speaker.Name + " gifts you "+silver+""+CatIAP.silverStr+" in appreciation.", null, null, incrementSilver,null,speaker.getCatAsset().meow),
            new RandomReward("\"!! My hooman! I'm less lonely now,\" "+ speaker.Name + " meows, and drops a"+CatIAP.goldStr+" at your feet.", null, null, incrementGold,null,speaker.getCatAsset().meow),
            new RandomReward("\"My hooman's back! I thought I lost you,\" "+ speaker.Name + " meows, gifting you "+silver+""+CatIAP.silverStr+".", null, null, incrementGold,null,speaker.getCatAsset().meow),
            new RandomReward("\"Hooman, where did you go? It got lonely,\" "+ speaker.Name + " meows, offering "+silver+""+CatIAP.silverStr+".", null, null, incrementGold,null,speaker.getCatAsset().meow),
        };
        RewardPrompt(rewards[UnityEngine.Random.Range(0, rewards.Length)]);
    }
    public void RewardPrompt(RandomReward reward)
    {
        GameControl.control.getSoundManager().playExploreButton();
        Notification n = GameObject.Instantiate(GameControl.control.notificationPrefab, GameControl.control.transform, false).GetComponent<Notification>();
        n.RewardPrompt(reward, !string.IsNullOrEmpty(reward.yesString));
    }
    #endregion

}
