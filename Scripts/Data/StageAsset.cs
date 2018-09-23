using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Dialogue
{
    public CatType speaker;
    public bool meow;
    public string[] dialogue;
    public Dialogue(CatType speaker, bool meow, string[] dialogue)
    {
        this.speaker = speaker;
        this.meow = meow;
        this.dialogue = dialogue;
    }
}
[System.Serializable]
public struct ActionResponse
{
    public ActionType action;
    public Dialogue[] response;
    //if not accepted, just provides hint; else, can end stage in victory without fighting. pacifist
    public bool accepted;
}
[System.Serializable]
public struct rewardPrompt
{
    public string prompt;
    //if reward.Length == 1, yes no prompt; else, options prompt with an ability to choose 'none'
    public CatType[] rewards;
}
public class StageAsset : ScriptableObject
{
    [Header("COMBAT-------------------------------------")]
    public Sprite background;
    public CatType[] enemyCats;
        [Header("RESPONSES---------------------------------------")]
    public ActionResponse[] Responses;
    [Header("REWARDS---------------------------------------")]
    public uint silver = 0;
    public uint gold = 0;
    public CatType rewardCat;
    [Header("UNLOCK")]
    public CatType[] unlockCats;
    [Header("STORY--")]
    public Dialogue[] beforeDialogues;
    public Dialogue[] afterDialogues;
    public rewardPrompt rewardPrompt;




 
}
