using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HowToPlay : MonoBehaviour
{

    private float[] heights = new float[] {
524,
1745,
1238,
275.7f,
2239
};
    private string[] infoText = new string[]{
"<b>Welcome to <color=#e16666>Wholesome Cats</color></b>!\nCollect strong cats in your <b><color=#666ee1>deck</b></color> to put on your <b><color=#666ee1>team</b></color>."
+"\nThen, <b><color=#666ee1>explore </b></color>Half Moon Island, playing with other cats and restoring harmony between the Factions - Manmade, Earth, and Life!",

"<b><color=#666ee1>Explore</b></color> Half Moon Island by selecting an unlocked World. Every world has <color=#BF1F38>stages</color> to beat before continuing the adventure."
+"\n\n<color=#BF1F38><b><u>the stage:</color></b></u>\nEvery <color=#BF1F38>stage</color> has opponents you must defeat to move on."
+"\n<color=#608D8F><b>How to win:</color></b>\nHave a strong <b><color=#666ee1>team</b></color>, <color=#e1aa66>Faction advantages</color>, and/or <color=#d08071>Acts</color>."
+"\n<color=#e1aa66><b>Faction advantages:</color></b>\nCertain Factions are stronger in certain Worlds. It's up to you to figure out which Faction'll work best!\n<color=#d08071><b>Acts:</color></b>"
+"\nEvery cat has an <color=#d08071>Act</color>, described in their <b><color=#666ee1>deck</b></color> card. Each cat can only use their <color=#d08071>Act</color> once per stage. Depending on your opponent, an <color=#d08071>Act</color> could have no effect, give an advantage, or automatically win!"
+"\n\n<color=#608D8F><b>After winning,</color></b> you're gifted fishbones, and, occasionally, gold and/or a cat. The better you do = more rewards. You're more likely to be gifted a cat if the stage was won by an <color=#d08071>Act</color>.",

"The <b><color=#666ee1>deck</b></color> is where all your cats are! You can have up to 8 cats before needing to upgrade. Clicking on a cat shows their <color=#608D8F>deck card."
+"\n\n<b>the deck card:</b></color>\nEvery cat's info is in their card.\nHere, you can edit their name and see their Faction, level, rarity, and stats."
+"\nYou can also <color=#608D8F>sell</color> them or <color=#608D8F>level up</color> them with fishbones.\nLastly, you can tap the cat to see their battle animation."
+"\n\n<color=#BF1F38><b>a cat tip:</b></color>\nEven the rarest cat isn't stronger than the most common cat if their levels are far apart!",

"Your team is who you choose to <b><color=#666ee1>explore</b></color> the Island. Up to 4 cats from your <b><color=#666ee1>deck</b></color> can go on your team.",

"The <b><color=#666ee1>shop</b></color> is managed by Box Cat! Box Cat offers 3 options;\n\n<color=#608D8F><b>random cat:</b></color>"
+"\nPay 1 coin to get a cat, randomly selected from the cats you've unlocked so far."
+"\nNote: Higher rarities are harder to get, and the cat's level is randomized from a certain range!\n"
+"\n<color=#608D8F><b>pick cat:</b></color>\nPay from 3 to 27 coins for any cat from the unlocked cats.\n"
+"\n<color=#BF1F38><b>cat tip:</b></color>\nCats are unlocked as you <b><color=#666ee1>explore</b></color> the Island and see cats never seen before!"
+"\n\n<color=#608D8F><b>coin shop:</b></color>\nGet coins through in-app purchases, trading fishbones, or watching videos."
+"\n\n<color=#BF1F38><b>cat tip:</b></color>\nWith time, your cats may gift you fishbones and even gold! But <color=#EF1F38>be careful - if your clock is found to be messed with, you'll be locked out.\n<color=#728670>~~~"
+"\nCoins are for getting more cats, and fishbones are for leveling up the cats you already have."
+"\n~~~</color>"
    };
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI aboutText;
    private TextMeshProUGUI lastText;
    private const int aboutTextHeight =2119;

    public void changeText(int i)
    {
        GameControl.control.getSoundManager().playButton();
        if (lastText != null)
        {
            lastText.text = "";
            lastText.rectTransform.sizeDelta = new Vector2(lastText.rectTransform.sizeDelta.x, 15);
			aboutText.rectTransform.sizeDelta = new Vector2(aboutText.rectTransform.sizeDelta.x, aboutTextHeight);
        }
        if (lastText == texts[i])
        {
            lastText = null;
            return;
        }
        lastText = texts[i];
        lastText.text = infoText[i];
        lastText.rectTransform.sizeDelta = new Vector2(lastText.rectTransform.sizeDelta.x, heights[i]);
        aboutText.rectTransform.sizeDelta = new Vector2(aboutText.rectTransform.sizeDelta.x, aboutTextHeight + heights[i]);
    }

}
