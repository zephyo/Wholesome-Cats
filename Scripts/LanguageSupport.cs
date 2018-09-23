// using System;
// public enum Language
// {
//     English,
//     Japanese,
//     Korean
// }

public static class LanguageSupport
{
    //     public static Language language = Language.English;
    public static string NotEnoughGold()
    {
        return "not enough " + CatIAP.goldStr + "! Buy more?";
    }
    public static string NotEnoughSilver()
    {
        return "not enough " + CatIAP.silverStr + "!";
    }
    public static string BuyPrompt(string one, string two)
    {
        return "Buy " + one + " for " + two + "?";
    }

    public static string ExchangePrompt(string one, string two)
    {
        return "Exchange " + one + " for " + two + "?";
    }

    //     public static string PurchaseFailed(string coinAmt)
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "couldn't buy " + coinAmt + " (‘д`)! \n..";

    //         }
    //     }


    //     public static string Ok()
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "ok";

    //         }
    //     }

    //     public static string Yes()
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "yes";

    //         }
    //     }
    //     public static string No()
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "no";

    //         }
    //     }


    //     public static string Ready()
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "READY";

    //         }
    //     }


    //     public static string Enter()
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "ENTER";

    //         }
    //     }


    //     public static string Scene(string sceneName)
    //     {
    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return sceneName;

    //         }
    //     }

    //     // public static string getAction(string[] translatedActions)
    //     // {

    //     //     switch (language)
    //     //     {
    //     //         case Language.English:
    //     //         default:
    //     //             return translatedActions[0];

    //     //     }
    //     // }

    //     public static string FleeFail()
    //     {

    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "failed to flee! :<";

    //         }
    //     }
    //     public static string FleeSuccess()
    //     {

    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "managed to flee!";

    //         }
    //     }

    //     public static string NeedCatsOnTeam()
    //     {

    //         switch (language)
    //         {
    //             case Language.English:
    //             default:
    //                 return "you need a cat on your team to explore!";

    //         }
    //     }



}
