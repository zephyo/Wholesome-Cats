using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatThoughts : MonoBehaviour
{

    private void OnEnable()
    {
        string[] catThoughts = new string[]{
            "Every cat is worth something.",
            "<size=130%><b>NOTICE:</b></size>\nTaking good care of your pets is cool!",
            "<b>Cats</b> may not solve your problems,\nbut they might make it better",
            "<b>Please do not leave items unattended.</b>\n(They will be pushed)",
            "<size=130%><b>CAT FACT:</b></size>\nAll you need is a cat. Or two. Or twenty.",
            "Do not disturb unless you have money or my box is on fire.",
            "A box is a luxurious bed.",
            "A lick a day keeps unkempt fur away.",
            "Cat Time TM",
            "Have a good day!",
            "How are mew today?",
            "Support your local cats!",
            "Non-GMO, organic cats!",
            "What's better than a cat shop?\nA cat cafe.", 
            "First cats, then more cats, then the world.",
            "<size=130%><b>CAT FACT:</b></size>\nPurring can relieve stress!",
            "<size=130%><b>CAT FACT:</b></size>\nWe love nice hoomans :p",
            "<size=110%><b>BREAKING NEWS:</b></size>\nHoomans are big clumsy kittens.",
            "May your cats be strong and yourself be cute.",
           // todo
        };
        GameControl.GetTextBox(transform, "text").text = catThoughts[UnityEngine.Random.Range(0, catThoughts.Length)];
    }
}
