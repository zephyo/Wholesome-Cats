using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class DataUtils
{
    public static WorldType[] getConnectedWorlds(WorldType world)
    {
        switch (world)
        {
            case WorldType.city:
                return new WorldType[]{
                    WorldType.woods,
                    WorldType.fields,
                    WorldType.computer,
                };
            case WorldType.computer:
                return new WorldType[]{
                        WorldType.city,
                        WorldType.woods,
                };
            case WorldType.woods:
            case WorldType.fields:
                return new WorldType[]{
                    WorldType.ocean,
                };
            case WorldType.house:
                return new WorldType[]{
                    WorldType.city,
                    WorldType.computer,
                };
            case WorldType.ocean:
                return new WorldType[]{
                    WorldType.sky,
                };
            case WorldType.sky:
                return new WorldType[]{
                    WorldType.space
                };
            default:
                return new WorldType[]{
                };
        }

    }
    public static bool isFactionCompatible(FactionType faction, WorldType world)
    {
        switch (faction)
        {
            case FactionType.Life:
                if (world == WorldType.space)
                {
                    return true;
                }
                break;
            case FactionType.Earth:
                if (world == WorldType.fields || world == WorldType.woods)
                {
                    return true;
                }
                break;
            case FactionType.Manmade:
                if (world == WorldType.city || world == WorldType.computer)
                {
                    return true;
                }
                break;
        }
        return false;
    }
    public static Vector2[] getOffsets(int numCats)
    {
        //ADJUST GET HEALTH BAR IF MOVING TO DRASTICALLY DIFF POSITON!
        switch (numCats)
        {
            case 1:
                return new Vector2[]{
                Vector2.zero
            };
            case 2:
                return new Vector2[]{
                new Vector2(3.1f, 0.5f),
                new Vector2(-1.1f, -1f)
            };
            case 3:
                return new Vector2[]{
                new Vector2(3.1f, -0.2f),
                new Vector2(-1.1f, 1.2f),
                new Vector2(-1.1f, -2f),
            };
            case 4:
                return new Vector2[] {
                    new Vector2(3.1f, 1.45f),
                    new Vector2(3.1f, -1.3f),
                    new Vector2(-1.1f, 0.35f),
                    new Vector2(-1.1f, -2.1f)
                 };
            default:
                return null;
        }
    }
    public static Vector2[] getLoseOffsets(int numCats)
    {
        //ADJUST GET HEALTH BAR IF MOVING TO DRASTICALLY DIFF POSITON!
        switch (numCats)
        {
            case 1:
                return new Vector2[]{
                 new Vector2(-1f, 0f),
            };
            case 2:
                return new Vector2[]{
                new Vector2(-1f, 1.5f),
                 new Vector2(-1f, -1f),
            };
            case 3:
                return new Vector2[]{
                new Vector2(-1f, -0.3f),
                 new Vector2(-1f, 1.5f),
                 new Vector2(-1f, -2.35f),
            };
            case 4:
                return new Vector2[] {
                    new Vector2(-1.3f, 1.5f),
                 new Vector2(1f, 0.25f),
                  new Vector2(-1.3f, -1.2f),
                 new Vector2(1f, -2.35f),
                 };
            default:
                return null;
        }
    }
    public static StageAsset getRandomStage(WorldType worldFrom, int levelFrom)
    {
        StageAsset stage = new StageAsset();
        //set background
        stage.background = Resources.Load<Sprite>("LevelAssets/Backgrounds/" +
        worldFrom.ToString() + UnityEngine.Random.Range(0, getBackgroundMax(worldFrom) + 1));
        //set enemy cats
        int max = UnityEngine.Random.Range(1, 4);
        stage.enemyCats = new CatType[max];
        List<CatType> randomCats = getRandomRelevantCats(worldFrom, levelFrom);
        for (int i = 0; i < max; i++)
        {
            stage.enemyCats[i] = randomCats[UnityEngine.Random.Range(0, randomCats.Count)];
        }
        //set rewards
        stage.silver = (uint)(MathUtils.FairSilver() * (max + 2) / 4.0f);
        if (UnityEngine.Random.value > 0.96f)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                stage.gold = 1;
            }
            stage.gold = (uint)UnityEngine.Random.Range(1, 6);
        }
        if (UnityEngine.Random.value < 0.05f)
        {
            stage.rewardCat = stage.enemyCats[UnityEngine.Random.Range(0, max)];
        }
        else
        {
            stage.rewardCat = CatType.none;
        }
        //set smol story, if any
        Debug.Log("getRandomStage: shall we set a smol story?");
        if (UnityEngine.Random.value > 0.4f)
        {
            Debug.Log("getRandomStage: YES! get dialogue");
            Dialogue[][] dialogues = getRandomDialogues(stage.enemyCats[0]);
            stage.beforeDialogues = dialogues[0];
            stage.afterDialogues = dialogues[1];
        }
        return stage;
    }

    public static StageAsset WonLastLvlStage(StageAsset stage, WorldType world, int level)
    {
        //set rewards
        CatType randomCat = stage.enemyCats[UnityEngine.Random.Range(0, stage.enemyCats.Length)];
        if (UnityEngine.Random.value < 0.2f)
        {
            stage.rewardCat = randomCat;
        }
        else
        {
            stage.rewardCat = CatType.none;
        }
        Dialogue[][] dialogues = WonLastLvlDialogues(randomCat);
        stage.beforeDialogues = dialogues[0];
        stage.afterDialogues = dialogues[1];
        stage.Responses = null;
        return stage;
    }
    #region RANDOM DIALOGUES ------

    public static string randomLvlUp(CatType cat)
    {
        string[] lvlUps;
        switch (cat)
        {
            case CatType.anime:
                lvlUps = new string[]{
                "I feel my magic growing stronger!",
                "Can't have magic without proper nutrients!"
            };
                break;
            case CatType.artist:
                lvlUps = new string[]{
                "I feel so motivated to paint!",
                "My paintbrush and I feel powered up!",
                "I will paint something beautiful in your honor!"
            };
                break;
            case CatType.blep:
                lvlUps = new string[]{
                "Bpppfft! Nom nom nom.",
                "Mmm, yum, pffbt!"
            };
                break;
            case CatType.bot:
                lvlUps = new string[]{
                "I ENJOY THIS EXQUISITE FELINE TREAT.",
                "I AM A CAT ENJOYING FISH.",
                "I CAN FEEL MY CARBON STRENGTHEN.",
                "SO EXCITED TO DIGEST THIS FISH.",
                "WHAT A GREAT FOOD-MEAL. THANK MEW.",
                "THANK MEW FOR FUEL. BEEP."
            };
                break;
            case CatType.cloud:
                lvlUps = new string[]{
               "Ah, a full belly makes me feel powerful.",
               "I will work hard for you in thanks."
            };
                break;
            case CatType.donutpink:
            case CatType.donutchoc:
            case CatType.choco:
            case CatType.cream:
                lvlUps = new string[]{
               "I'm ready to defeat foes with frosting!",
            };
                break;
            case CatType.dog:
                lvlUps = new string[]{
               "BORK! <3",
               "Bork! bork bork <3",
               "B-bork!!!"
            };
                break;
            case CatType.water:
            case CatType.fire:
            case CatType.gem:
            case CatType.ice:
            case CatType.lantern:
            case CatType.star:
            case CatType.doot:
            case CatType.plush:
            case CatType.night:
            case CatType.old:
            case CatType.rainbow:
                lvlUps = new string[]{
               "I won't fail you.",
               "I'll fight hard for you.",
               "Thank mew for your generosity.",
               "I'll do my best for you.",
               "Thank mew for believing in me.",
               "I'll fight by your side as long as you let me.",
               "I'll remember this favor.",

            };
                break;
            case CatType.ghost:
                lvlUps = new string[]{
               "boo! I'm ready to fight!",
               "I have my boos locked and loaded!",
               "I'm ready to scare the heck outta some cats!"
            };
                break;
            case CatType.grass:
                lvlUps = new string[]{
               "I'm honoured by your gift.",
               "I'll make you proud.",
               "Fish? For me? Thank mew so much.",
            };
                break;
            case CatType.grumpy:
                lvlUps = new string[]{
               "...Thank mew.",
               "Thank mew, I guess.",
               "For me? ..Thanks.",
            };
                break;
            case CatType.head3:
                lvlUps = new string[]{
               "By the name of heck, I won't be defeated!",
               "The guardian of heck will win!",
            };
                break;
            case CatType.music:
                lvlUps = new string[]{
               "Bro, I'll do you proud!",
               "Bro! I'll fight hard!",
            };
                break;
            case CatType.business:
            case CatType.mustache:
                lvlUps = new string[]{
               "I'll fit this growth in my schedule.",
               "A pleasant interruption to my schedule.",
               "I see profit in our future!",
                "A worthy investment!",
            };
                break;
            case CatType.nerd:
                lvlUps = new string[]{
               "This fish oil will fuel my brain!",
               "Fish is great for the body!",
               "A good diet leads to a good body!",
            };
                break;
            case CatType.persian:
            case CatType.phat:
                lvlUps = new string[]{
               "Oh, yeah.. that hit the spot.",
               "Just the thing I was looking for.",
               "I feel so much stronger now.",
               "I'd eat this every day if I could.",
               "I love being fed.",
               "I love free food."
            };
                break;
            case CatType.niceeyes:
            case CatType.pink:
                lvlUps = new string[]{
               "*floofs* Thank mew so much!",
               "*floofs* Thank mew!",
               "*floofs* nom nom!"
            };
                break;
            case CatType.pixel:
                lvlUps = new string[]{
               "Mm, tasty! ^-^",
               "NOM NOM NOM! :D",
               "I feel my pixels strengthening! :p",
               "Oh yus! :D"
            };
                break;
            case CatType.scottish:
                lvlUps = new string[]{
               "T-thank mew..",
               "O-oh, fish for me? I'm grateful..",
               "Y-you chose me? Out of all the cats?",
               "I-I get fish? Thank mew so much.."
            };
                break;
            case CatType.shadow:
                lvlUps = new string[]{
               "...",
               "...Our foes will shiver before me.",
               "...I will defeat anyone standing in your way.",
               "....Good."
            };
                break;
            case CatType.sleepy:
                lvlUps = new string[]{
               "..ZzZz..",
               "..ZZz.. Huh? Is that fish I smell?",
               "...zZz.. fish? For my lazy self? Thank mew!",
               "ZzZz.. *eats in sleep* ..yum.."
            };
                break;
            case CatType.sprout:
                lvlUps = new string[]{
               "My sprout and I shall grow stronger!",
               "My sprout is healthier than ever!",
            };
                break;
            case CatType.sushi:
                lvlUps = new string[]{
               "I'm thankful for this high quality fish.",
               "High quality fish for a high quality sushi cat!",
               "Thank mew for this gift.",
            };
                break;
            case CatType.unicorn:
                lvlUps = new string[]{
               "weeee <3 I love being fed!",
               "Weee I'll not let us get defeated again!",
               "Weeeee I'll try my best!",
            };
                break;
            case CatType.uwu:
                lvlUps = new string[]{
               "you'resokind\niloveyoualot\nwowthisfishis\nsogood!!",
               "aaaa my love for you has increased tenfold!",
               "aaa i'll cherish and remember this gift forever!",
               "oof i'm so happyyyyy, thank mew so much!",
            };
                break;
            case CatType.wood:
                lvlUps = new string[]{
               "I is stronger.",
               "I is strong Wood.",
               "I is good Wood.",
            };
                break;
            default:
                lvlUps = new string[]{
                "Thank mew for your kindness!",
                "NOM NOM. Thank mew!",
                "Thank mew!",
                "Mmm, delicious~",
                "Meow~ I feel stronger!",
                "Yumm, nom nom nom.",
                "Mm, I love a good meal!",
                "I'll work hard for you!",
                "I feel like I can defeat any cat!",
                "My foe won't know what hit 'em!"
            };
                break;
        }
        return adjustCosmeticString(lvlUps[UnityEngine.Random.Range(0, lvlUps.Length)], cat);
    }
    private static Dialogue[][] WonLastLvlDialogues(CatType cat)
    {
        Dialogue[][][] randomDialogues;
        switch (cat)
        {
            case CatType.anime:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Meow! I'm so glad you brought the Life cats back.", "Now I can play multiplayer games with my Life friends~"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Those Life cats are so fun, the computer life is about to get a lot more exciting~"}),
                },
            },
        };
                break;
            case CatType.artist:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I see you brought the Life cats back."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I'm proud of you.","I'll paint something in your honor!",}),
                },
            },
        };
                break;
            case CatType.blep:
                randomDialogues = new Dialogue[][][]{
        new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bppppffft! The hero came back from their journey <3!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Blllwwfff bfpt. Will you stay home now?",}),
                },
            },
        };
                break;
            case CatType.bread:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You brought the Life cats back!", "I knew you could do it."}),
                      new Dialogue(CatType.none, false, new string[]{"Thanks for believing in me."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I bet you can do anything you want!","What are you going to do next?"}),
                },
            },
        };
                break;

            case CatType.mustache:
            case CatType.business:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"What's this?", "You brought the Life cats back?"}),
                      new Dialogue(CatType.none, false, new string[]{"Yep!"}),
                      new Dialogue(cat, false, new string[]{"Why, I don't believe it.", "Cancel my meeting - I got to see if you're really that strong!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..So strong. I believe it.","I've got to get in touch with my old Life friends!"}),
                },
            },


        };
                break;
            case CatType.choco:
            case CatType.sushi:
            case CatType.cream:
            case CatType.donutchoc:
            case CatType.donutpink:
            case CatType.pocky:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh hey!!","The hero that brought the Life cats back!", "Time to test our strength!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"As I thought..", "The hero is probably the strongest cat in the world!"}),
                },
            },
        };
                break;
            case CatType.dog:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"bork bork bork bork bork! boooorrRrRrRRRRK!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"bork! booooooooooooOOOOoooRrk! <3 <3 <3 <3 <3"}),
                    new Dialogue(CatType.none, true, new string[]{":)"}),
                },
            },
        };
                break;

            case CatType.fire:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey! You're back, and you brought the Life cats with you!", }),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I knew you could do it.","I've got better at controlling my fire while you were gone; I'm proud of us."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Guess what?", }),
                      new Dialogue(CatType.none, true, new string[]{"What?", }),
                      new Dialogue(cat, true, new string[]{"I, uh, I talked to Princess Leaf, and..", }),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"..Princess Leaf forgave me for burning her plant."}),
                      new Dialogue(CatType.none, true, new string[]{"That's awesome!", }),
                      new Dialogue(cat, true, new string[]{"Thanks! I couldn't have done it without you.", }),
                },
            },
        };
                break;
            case CatType.ghost:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hey, I haven't seen you in awhile."}),
                      new Dialogue(CatType.none, true, new string[]{"Hey! I was busy with an adventure."}),
                      new Dialogue(cat, true, new string[]{"Now that you're back, let me test my boos on you!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"How was that?"}),
                      new Dialogue(CatType.none, true, new string[]{"You've improved a lot!"}),
                       new Dialogue(cat, true, new string[]{"Thanks! So have you; must've been quite the adventure!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hey there!! You're back to play!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"So I thought about what you said, about the teaching thing, you know.."}),
                      new Dialogue(CatType.none, true, new string[]{"What'd you think?"}),
                       new Dialogue(cat, true, new string[]{"I've begun teaching cats about ghosts and it's going well!", "I'm thankful to you!"}),
                },
            },
        };
                break;
            case CatType.grumpy:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Oh, the so-called heroes!", "Are you really as good as they say you are?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I admit, I made a mistake. I misjudged you.", "You're not weak after all.", "Thanks for bringing Life back."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..Hello. Good job with the.. you know."}),
                      new Dialogue(CatType.none, false, new string[]{"What?"}),
                      new Dialogue(cat, false, new string[]{"Ah, don't make me say it."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, false, new string[]{"You were saying? Good job with the..?"}),
                      new Dialogue(cat, false, new string[]{"Agh. Good job with bringing Life cats back.", "I didn't believe in you, and I should've."}),
                },
            },
        };
                break;
            case CatType.head3:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Cerbercat, gatekeeper of heck, has an announcement!", "Our announcement: Thank you for bringing Life back!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Roarr! Heck will throw parties in your memory!"}),
                },
            },
        };
                break;
            case CatType.doot:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"...I dare you to look me in the eyes and say you're not scared."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, true, new string[]{"I'm not scared."}),
                      new Dialogue(cat, true, new string[]{".....Even a little bit?"}),
                      new Dialogue(CatType.none, true, new string[]{"Not really."}),
                      new Dialogue(cat, true, new string[]{"Is it the doot?","It's the doot, isn't it."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"How's the reuniting-with-Life going?"}),
                    new Dialogue(cat, true, new string[]{"...."}),
                    new Dialogue(CatType.none, true, new string[]{"..?"}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, true, new string[]{"So reuniting isn't going good?", "You don't have to tell me."}),
                      new Dialogue(cat, true, new string[]{".....", "..It's going well.","I just didn't know they'd be so nice."}),
                      new Dialogue(CatType.none, true, new string[]{"Whew! Yep, they're sweet cats.", "I'm happy for you, dooters."}),
                        new Dialogue(cat, true, new string[]{"Don't call me that. doot "}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"..Hey. What is up."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"..is 'what is up' the correct terminology?", "Still trying to get a hang of this Island lingo."}),
                },
            },
        };
                break;
            case CatType.gem:
            case CatType.rainbow:
            case CatType.ice:
            case CatType.water:
            case CatType.lantern:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Thank you for getting those stubborn Life cats to return.", "We all wanted them to, but were too scared to stand up to them."}),
                    new Dialogue(CatType.none, true, new string[]{"You're welcome!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You're so inspiring!", "I've learned from you that I shouldn't let fear stop me."}),
                },
            },
             new Dialogue[][]{
                 new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"Hi again!"}),
                    new Dialogue(cat, true, new string[]{"Hey there! I can't believe you convinced the Life cats to return!"}),

                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I've spent so much time trying to convince them, but failed.",}),
                    new Dialogue(cat, true, new string[]{"That's ok. I did it, but it took a lot of hard work."}),
                    new Dialogue(cat, true, new string[]{"I see now why they were convinced.",}),
                },
            },
        };
                break;
            case CatType.music:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, bro! Great job!","I met my Life friend yesterday; I missed her so much, so glad she's back."}),
                       new Dialogue(cat, true, new string[]{"I'm happy for you cats."}),
                        new Dialogue(cat, true, new string[]{"^_^ Play with me? For old time's sake?"})
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"That was great.", "I'll go to you if I need strength, honor, and courage."}),
                },
            },
              new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Bro! I see you've been doing a lot lately!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I heard whispers of your adventure, you know.", "News travels fast in the city.", "You did awesome."}),
                },
            },
        };
                break;

            case CatType.nerd:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hey, so good to see you!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"I went back to the city after you left,", "And I saw my city friends again.", "I'm glad you inspired me to go back."}),
                },
            },
              new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Oh, hello!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Do you know what you're gonna do next?"}),
                    new Dialogue(CatType.none, true, new string[]{"Not sure; maybe visit old friends, rest a bit.."}),
                    new Dialogue(cat, true, new string[]{"Sounds good. You deserve a rest."}),
                },
            },
        };
                break;
            case CatType.old:
                randomDialogues = new Dialogue[][][]{
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Adventurers, you've come back.", "We missed you at the house."}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"Stay home, rest for a while.", "You've done well."}),
                        },
                    },
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Adventurers, have you come back to entertain this old man?"}),
                            new Dialogue(CatType.none, true, new string[]{"I've missed you."}),
                            new Dialogue(cat, true, new string[]{"I've missed you too."}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"We supported you silently while you were gone, ya know.", "I knew you could bring Life home."}),
                        },
                    },

                };
                break;
            case CatType.oxo:
                randomDialogues = new Dialogue[][][]{
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"YOU! IT'S YOU! THE HERO OF THE ISLAND!", "PLAY WITH US!!"}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"OOOOH AS EXPECTED!","SO POWERFUL!"}),
                        },
                    },


                };
                break;
            case CatType.persian:
            case CatType.phat:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Oof, want to battle?",
                      "..Hold up, gotta mentally prepare myself for exercise.."}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, false, new string[]{"How was that?"}),
                      new Dialogue(cat, false, new string[]{"Not bad.","Maybe I'll do this exercise thing more often..",}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Meow! Fight me and I'll tell you my story!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Ok, you wanna hear my story?"}),
                      new Dialogue(CatType.none, false, new string[]{"Sure!"}),
                      new Dialogue(cat, false, new string[]{"I wasn't always this size.",
                      "A long time ago, I ate a whole melon.",
                      "I haven't been the same since."}),
                      new Dialogue(CatType.none, false, new string[]{
                          "That doesn't make sense."}),
                      new Dialogue(cat, false, new string[]{
                          "It's true. I think. ..Was it a dream?"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"So I've heard you went on quite the journey.", "Let me ask you something.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"On your journey, did you find any good food?", "If so, where, and can I have some?"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"The cats on the island seem happier, less lonely..", "Is this your doing?"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, false, new string[]{"I brought Life back!", "Maybe that's why the cats are happier."}),
                      new Dialogue(cat, false, new string[]{"Ooooh! You fought well, I can believe it!", "Congrats!"}),
                },
            },

        };
                break;
            case CatType.pink:
                return getRandomDialogues(cat);
            case CatType.pixel:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"hello ^_^ Friend, you've come back!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Are you gonna leave again?"}),
                      new Dialogue(CatType.none, true, new string[]{"Maybe.."}),
                      new Dialogue(cat, true, new string[]{"Nooooooo :(","Well, a hero's gotta do what a hero's gotta do :("}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Friend I'm so glad you're back~", "I've read about your adventures through space on the internet!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"My adventures are on the internet?"}),
                      new Dialogue(CatType.none, true, new string[]{"Oh yes, all over Catbook and Purrter and Pawmblr~", "You're a cat legend!"}),
                },
            },
        };
                // do
                break;
            case CatType.plush:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hiya, you're back. We should catch up!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Whew, you've grown stronger.","I'm glad you finished your adventure.", "You can stay here and rest now."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh hello, how's it going?"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"A lot has happened since we last talked!"}),
                      new Dialogue(cat, true, new string[]{"Yeah, you brought Life back, right?", "Aaah, you rock!", "While you were gone, I reunited with my hooman."}),// We've both accomplished a lot!
                    new Dialogue(CatType.none, true, new string[]{"Congrats! We've both accomplished a lot!"}),
                },
            },
        };
                break;
            case CatType.shadow:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"...You brought Life back?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"...I can finally reunite with my only friend, Night.."}),
                      new Dialogue(CatType.none, false, new string[]{"Aren't we friends?"}),
                      new Dialogue(cat, false, new string[]{"...You want to be friends?"}),
                      new Dialogue(CatType.none, false, new string[]{"Of course!"}),
                      new Dialogue(cat, false, new string[]{"...thank you :')"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"...Life?"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, false, new string[]{"...Life? Return?"}),
                     new Dialogue(CatType.none, false, new string[]{"Yes, the Life cats have returned."}),
                      new Dialogue(cat, true, new string[]{"...much excite.."}),
                },
            },
        };
                break;
            case CatType.sleepy:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"...ZZZ..","..*mumble* Princess Leaf, thank.. nom nom.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"...ZZzz ... WHAT!"}),
                      new Dialogue(CatType.none, true, new string[]{"..Are you awake now?"}),
                      new Dialogue(cat, true, new string[]{"...Eh? Where's Princess Leaf?", "I had a dream she gave me a yummy flower..", "Aw, I just <i>had</i> to wake up.."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..zzz -", "O-oh hey, the.. you're the hero, right?"}),
                      new Dialogue(CatType.none, true, new string[]{"I helped the Life cats come back, yes."}),
                       new Dialogue(cat, false, new string[]{"oh? You deserve my full, awake attention."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Awesome.", "Definitely worth waking up from my slumber for!"}),
                },
            },

        };
                break;
            case CatType.unicorn:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You guys! You brought Life back!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"How can this caticorn repay you for bringing Life back?", "I can give you a rainbow, or a high five, or -", "I know! My friendship and fish!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Ahhhh you brought Life back~", }),
                },
                new Dialogue[]{
                      new Dialogue(CatType.unicorn, true, new string[]{"Princess Leaf is really thankful you convinced the Life cats!", "She's always wanted to stay on the Island, but her family said no."}),
                },
            },

        };

                break;
            case CatType.uwu:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oo have you come to play? i'd love to!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"YEsS iconic cat hero who ends all competition! i love you!"}),
                      new Dialogue(CatType.none, true, new string[]{"I love you too!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oooh hello hero and friend!! wanna play?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"im so blessedt?? graced with your presence and skill ?"}),
                      new Dialogue(CatType.none, true, new string[]{"I love you too~"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hewwo!! i'm so very very very proud of you you did so well i love you so much i-","*huff huff* sorry, i was rambling, anyways; you're super awesome!"}),
                    new Dialogue(CatType.none, true, new string[]{"Deep breaths, and thank you, you're awesome too!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"YAY i love spending time with you don't forget me once you're famous ok?!"}),
                      new Dialogue(CatType.none, true, new string[]{"Of course!!"}),
                },
            },

        };
                break;
            case CatType.wood:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I is Wood. You is Life bringer."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I is impressed."}),
                },
            },

        };
                break;
            case CatType.bot:
                randomDialogues = new Dialogue[][][]{

                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"GREETINGS."}),
                            new Dialogue(CatType.none, false, new string[]{"Why are you yelling?"}),
                             new Dialogue(cat, false, new string[]{"WHAT ARE YOU TALKING ABOUT?", "I DO NOT KNOW OF WHAT YOU ARE SPEAKING, FELLOW CAT FLESH."}),
                        },
                        new Dialogue[]{
                            new Dialogue(CatType.none, false, new string[]{"Please don't yell, it hurts my ears."}),
                            new Dialogue(cat, false, new string[]{"I APOLOGIZE. SHUTTING DOWN YELLING..",
                            "ERROR: REQUEST PROHIBITED. BEEP BOOP BEEP BEEEEEP"
                            }),
                            new Dialogue(CatType.none, false, new string[]{"Ah, oh no, Robocat!","Forget it. Continue yelling."}),
                        },
                    },
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"GREETINGS. HAVE YOU COME BACK TO TEST YOUR SKILLS?"}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"I MUST EXTEND GRATITUDE FOR THE RESTORATION OF LIFE.",
                                "I MET MY FRIENDS, LEAF AND STAR, AND EMITTED 'HAHA' MP3s OVER PLEASANT CONVERSATION.",
                                "IT WOULDN'T HAVE HAPPENED WITHOUT YOU ^_^"
                            }),
                        },
                    },
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, false, new string[]{"WELCOME BACK!","I EXTEND MY PAWS AND MULTIPLE SWEETS OUT IN THANKS."}),
                        },
                        new Dialogue[]{
                            new Dialogue(CatType.none, false, new string[]{"What were you thanking me for?"}),
                            new Dialogue(cat, false, new string[]{"FOR THE RESTORATION OF LIFE OF COURSE.",
                                "I EVEN WROTE A SMALL SONG OF THANKS FOR YOU.",
                            }),
                            new Dialogue(CatType.none, false, new string[]{"That's so sweet! Can I hear it?"}),
                            new Dialogue(cat, false, new string[]{
                                "THIS IS MY FIRST SONG. I HOPE YOU LIKE IT.",
                                "1001000100010101101010111010101",
                                "HOW WAS IT?"
                            }),
                            new Dialogue(CatType.none, false, new string[]{"I couldn't understand it, but it sounded beautiful."}),
                        },
                    },
                };
                break;
            case CatType.night:
                randomDialogues = new Dialogue[][][]{
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"I must thank you for opening my eyes."}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Before, I thought I knew best.", "I thought I was hiding us for the greater good.",
                        "But I was wrong. The island needs us, and deserves a second chance."
                            }),
                    },
                },
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Hello, my hero. Have you come to check on us?"}),
                    },
                    new Dialogue[]{
                           new Dialogue(cat, false, new string[]{"Where will you go now? After reuniting the cats of the island?"}),
                            new Dialogue(CatType.none, false, new string[]{"I'm not sure."}),
                            new Dialogue(cat, false, new string[]{"You can stay with us. You'll always be welcome with the Life cats."}),
                    },
                },
                 new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"I can't wait to see the island again, feel the wind on my face, hear the city bustling..",
                        "But I sure will miss space."}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Space is beautiful, isn't it?", "Silent, infinite.."}),
                        new Dialogue(CatType.none, false, new string[]{"It is! Even when you no longer live here, you can always visit."}),
                        new Dialogue(cat, false, new string[]{"Thank you. I sure will."}),
                    },
                },
             };
                break;
            case CatType.cloud:
                randomDialogues = new Dialogue[][][]{
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"The moment I laid eyes on you, I knew."}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"You were full of bravery and good intentions.","I knew you were destined to bring us back."
                            }),
                    },
                },
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Hello again, young one."}),
                    },
                    new Dialogue[]{
                           new Dialogue(CatType.none, false, new string[]{"You've done good.","Take care of yourself, my hero."}),
                    },
                },
             };
                break;
            case CatType.star:
                randomDialogues = new Dialogue[][][]{
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Whew, I can't wait to go back to the island."}),
                    },
                    new Dialogue[]{
                           new Dialogue(CatType.none, false, new string[]{"What will you do when you get back?"}),
                        new Dialogue(cat, false, new string[]{"Oh, I'll definitely go play games with my old buddies.",
                        "You know xXx_AngelNeko_xXx?", "Man, we defeated so many dungeons together.. can't wait to play with her again!"
                            }),
                    },
                },
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Talk about tough, I still can't believe you defeated my mom!"}),
                    },
                    new Dialogue[]{
                           new Dialogue(CatType.none, false, new string[]{"Your mom is definitely one tough cat!"}),
                        new Dialogue(cat, false, new string[]{"For sure, I got my toughness from her!",
                            }),
                    },
                },
                 new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"I'm just so excited to go back to the island!"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"I kind of suppressed my love for my friends back on the Island, you know.",
                        "'For the greater good', and all that.", "But I finally get to see them again!"
                            }),
                    },
                },
             };
                break;
            case CatType.grass:
                randomDialogues = new Dialogue[][][]{
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"I can't thank you enough for convincing my Life cats to return."}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"My parents are so stubborn.", "But I believed in you. I knew you could convince them."
                            }),
                    },
                },
                new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Now that Life is coming back, I don't need to pretend I'm not a Life cat anymore."}),
                    },
                    new Dialogue[]{
                           new Dialogue(CatType.none, false, new string[]{"You had to pretend you weren't a Life cat?"}),
                        new Dialogue(cat, false, new string[]{"If anyone found out I was in the fields, and not hiding in space," ,
                        "and if word got back to my parents, I'd be in huge trouble.",
                         "Pretending was worth it. I love the fields too much."
                            }),
                    },
                },
                 new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Now that Life is back, want to hang out with me?"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Aah, don't go - you should stay in the fields with me..", "We can grow flowers and water plants; it'll be fun."}),
                        new Dialogue(CatType.none, false, new string[]{"Aw, Leaf, I have to go eventually."}),
                        new Dialogue(cat, false, new string[]{"I know :("}),
                    },
                },
             };
                break;

            default:
                //basic cats
                randomDialogues = new Dialogue[][][]{
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Ah, hello, *reads scribbled writing on paw* herb of the Lime cats."}),
                            new Dialogue(CatType.none, true, new string[]{"..that doesn't sound right.."}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Wait, so you're not the herb of the Lime cats?"}),
                            new Dialogue(CatType.none, true, new string[]{"No, we're just some adventurers having fun!"}),
                            new Dialogue(cat, true, new string[]{"Oh, likewise; have lots of fun, new friend."}),
                        },
                    },
                     new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Meow! I heard you brought back the Life cats! Is it true?"}),
                            new Dialogue(CatType.none, true, new string[]{"It's true!"}),
                            new Dialogue(cat, true, new string[]{"No way.."}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"I can't wait to meet my Life friends again~"}),
                        },
                    },
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Ah, can I have your autograph?"}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"I just can't believe you brought Life back!", "You're such a legend!"}),
                        },
                    },
                    new Dialogue[][]{
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"Eek, it's the hero!"}),
                        },
                        new Dialogue[]{
                            new Dialogue(cat, true, new string[]{"I'm so nervous, you're just so famous.."}),
                            new Dialogue(CatType.none, true, new string[]{"I may be famous, but I'm still just a cat with thoughts and worries."}),
                            new Dialogue(cat, true, new string[]{"Ah, of course, I shouldn't forget!", "Good day, hero!"}),
                        },
                    },
                };
                break;
        }
        randomDialogues = randomDialogues.Concat(getRDArray(cat)).ToArray();
        return randomDialogues[UnityEngine.Random.Range(0, randomDialogues.Length)];
    }











    private static Dialogue[][][] getRDArray(CatType cat)
    {

        Dialogue[][][] randomDialogues;

        switch (cat)
        {
            case CatType.doot:
            case CatType.bot:
            case CatType.cloud:
            case CatType.night:
            case CatType.grass:
            case CatType.star:
                randomDialogues = new Dialogue[][][] { };
                break;
            case CatType.anime:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Meow! I'm a magical cat in training; time to put my skills to the test!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Even though I lost, I'm grateful for the practice!"}),
                },
            },
        };
                break;
            case CatType.artist:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hello again."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Wow, you're so much stronger!","My art may not be great, but I'll work hard to improve, just like you!",}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hello there."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You cats have improved so much!","It's like comparing new art to old art, seeing how much it's improved.",}),
                },
            },
        };
                break;
            case CatType.blep:
                randomDialogues = new Dialogue[][][]{
        new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bppppffftt."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Blllwweep. Gwood gwame.",}),
                },
            },
        new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bppfffwing it on."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bllllep. Bwou're so inspiring!",}),
                },
            },
        };
                break;
            case CatType.bread:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oof, hey, can you help me get this thing off my head?",
                      "Maybe knock me around a bit, that might work!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Argh, it's still there *shakes rapidly*","Ah, I'll submit to my bread fate.."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Help me out!"}),
                      new Dialogue(CatType.none, false, new string[]{"With what?"}),
                            new Dialogue(cat, true, new string[]{"Oh, it's embarrassing.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Wow, incredible, I think you can help me out after all!", "So..","Be honest, is there bread on my head?",}),
                       new Dialogue(CatType.none, false, new string[]{"Yes."}),
                       new Dialogue(cat, true, new string[]{"No! Why does this keep happening!",}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bet you've never seen a pure bread cat like me before!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Gee, I might be pure bread, but you're something else!"}),
                },
            },
        };
                break;

            case CatType.mustache:
            case CatType.business:
                randomDialogues = new Dialogue[][][]{

            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Today is a very busy day for me.","After my midday snooze and cuddle with my stuffed friend,", "I must watch through the window for the return of my hooman."}),
                      new Dialogue(CatType.none, false, new string[]{"You sound quite busy! Want to take a break?"}),
                      new Dialogue(cat, false, new string[]{"Hmm.. that'd be lovely.",}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"What a marvelous break, thank mew.","I must go snooze, but best of luck with your adventures."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(CatType.none, false, new string[]{"Hello, how are you doing?"}),
                      new Dialogue(cat, false, new string[]{"Busy busy busy.", "Filing reports, chasing birds.. you know how it is."}),
                      new Dialogue(CatType.none, false, new string[]{"Make sure to take breaks and take care!"}),
                      new Dialogue(cat, false, new string[]{"Thank mew!",}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"That was an absolutely delightful break.","Just in time for my scheduled snooze in the sun.", "Thank mew!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Adventurers, eh? Well, let's see what you've got."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Not perfect, but..","You're definitely heading in the right direction."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Have you seen my secretary? Are you hiding her?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"My secretary's not here, eh?","She better not be hiding in a box again.."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I need reports, and I need them meow!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"My report of this performance: Outstanding!"}),
                },
            },
        };
                break;
            case CatType.choco:
            case CatType.cream:
            case CatType.donutchoc:
            case CatType.donutpink:
            case CatType.pocky:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey, you. Where do you think you're going?"}),
                      new Dialogue(CatType.none, true, new string[]{"Did you hear something?"}),
                      new Dialogue(cat, true, new string[]{"I said - "}),
                      new Dialogue(CatType.none, true, new string[]{"Is the food talking?"}),
                      new Dialogue(cat, true, new string[]{"Humph, true, but also how dare you!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"We're going to find the Life cats."}),
                      new Dialogue(cat, true, new string[]{"It's about time they come back! Good luck."}),
                      new Dialogue(CatType.none, true, new string[]{"Thanks. For food, you sure are aggressive."}),
                      new Dialogue(cat, true, new string[]{"Hmph. I have a reputation to maintain."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey! Wanna know the secret of chocolate?", "I'm not telling you.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Ok ok, <i>fine</i>, I'll tell you! Just get those claws away from me!","Eating chocolate cures Dementor attacks."}),
                      new Dialogue(CatType.none, false, new string[]{"A what?"}),
                      new Dialogue(cat, true, new string[]{"Dementor? Scary shadow things that suck your soul?", "..Nevermind."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Come closer.. let me whisper something very important.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"<size=70%>I LOVE FOOD."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"A balanced diet is chocolate and cream in both paws."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{
                          "Oh my, you're so energetic!", "You must also follow the sacred diet of pure sugar and sweets!"
                          }),
                },
            },

            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Nom nom!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Wow, you're fiercer than the fiercest pointiest fork!"}),
                },
            },
        };
                break;
            case CatType.dog:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"b-bork?"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"bork! boork! <3"}),
                    new Dialogue(CatType.none, true, new string[]{"Good boy!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"bork! mrrrrrrrbORK!"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"MRORK! (is that how a cat acts? do they think i'm a cat?)"}),
                    new Dialogue(CatType.none, true, new string[]{"Aww, you're doing your best."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"bork!! (i'm full of excitement that can't be contained", "and oh won't you play with me??)"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"BORK! (that was so fun you have my devoted love and affection!)"}),
                    new Dialogue(CatType.none, true, new string[]{"Aww, look at this cute borker," , "I just wanna snuggle them until they fall asleep."}),
                },
            },
        };
                break;

            case CatType.fire:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"There are winners, and there are losers!","Which one will you be?!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Well! You're a winner tonight, congrats!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oooh, I'm feeling so FIRED UP!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Whew, I'm out of energy..","It's napping time, my favorite time!"}),
                },
            },
        };
                break;
            case CatType.ghost:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"bo<size=60%>o</size>o<size=120%>o</size>o!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"did you think I was good at booing?"}),
                      new Dialogue(CatType.none, true, new string[]{"You're getting better!"}),
                       new Dialogue(cat, true, new string[]{"Yay, hard work never betrays!"}),
                },
            },
        };
                break;
            case CatType.grumpy:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, false, new string[]{"If I'm rude, I apologize.", "It's just natural to me."}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, false, new string[]{"Apology accepted!"}),
                    new Dialogue(cat, false, new string[]{"Thank mew. I just don't know how to handle sunshine, rainbows, and hope.", "Woops, was that rude? I apologize."}),

                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Ugh, an idealistic housecat."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"You only beat me by chance.","How are you still hopeful?",}),
                        new Dialogue(CatType.none, false, new string[]{"I have a dream and the determination to achieve it."}),
                        new Dialogue(cat, false, new string[]{"The city is where dreams go to die, I thought."}),
                        new Dialogue(CatType.none, false, new string[]{"I guess not."}),
                        new Dialogue(cat, false, new string[]{"Hm.. indeed."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"You are the only light in this cursed world we live in."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, false, new string[]{"Why do you think this is a cursed world?"}),
                      new Dialogue(cat, false, new string[]{"I remember the battle between Manmade and Life, many moons ago.","So much hate and death.","Without Life, it's only gotten worse."}),
                },
            },
                   new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"... *silent nod of acknowledgement*"}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, false, new string[]{"This was fun, I guess, but I've had enough of socializing now."}),
                },
            },
        };
                break;
            case CatType.eyes3:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"What do you call a cat with 3 eyes?"}),
                },
                new Dialogue[]{
                     new Dialogue(CatType.none, false, new string[]{"So what do you call a cat with 3 eyes?"}),
                      new Dialogue(cat, true, new string[]{"Oh..! You still wanna know?","Caiiit! Heh!"}),
                      new Dialogue(CatType.none, false, new string[]{"A what now? A kite?"}),
                      new Dialogue(cat, false, new string[]{"You know - a caIIIt - ack, nevermind."}),
                      new Dialogue(CatType.none, false, new string[]{"Haha, I'm just messing with you."}),
                },

            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Wanna play with me? *stares unblinkingly with all 3 eyes*"}),
                },
                new Dialogue[]{
                     new Dialogue(CatType.none, true, new string[]{"I'm glad you played with me!", "I sometimes scare new cats.. but I know it's ok to look different."}),
                },
            },
        };
                break;
            case CatType.head3:

                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"We are Cerbercat, gatekeeper of heck!", "Feel our wrath!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Roarr! You may pass heck!"}),
                },
            },

                       new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"We are Cerbercat!", "Feer us! You shall not pass!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Uhhh.. you shall pass after all!"}),
                },
            },
        };
                break;

            case CatType.ice:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Greetings. Get ready to bow before me."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"How was I defeated?"}),
                      new Dialogue(CatType.none, true, new string[]{"You're less skilled."}),
                      new Dialogue(cat, true, new string[]{"I'll be back, and better."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Bow before my greatness."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"How dare you beat me? I'm <i>Ice."}),
                      new Dialogue(CatType.none, true, new string[]{"How did I dare to? Because I'm more skilled :P"}),
                      new Dialogue(cat, true, new string[]{"Aah, you cheeky cat. I admit defeat."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You're strong; I respect that. Keep it up."}),
                },
            },
        };
                break;
            case CatType.gem:
            case CatType.water:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Do you have what it takes to stand against me?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You do have what it takes!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You're strong; I respect that. Keep it up."}),
                },
            },
        };
                break;
            case CatType.lantern:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Happy Halloween!"}),
                },
                new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Good work! Have a great rest of Halloween!"}),
                        new Dialogue(CatType.none, false, new string[]{"It's Halloween?"}),
                        new Dialogue(cat, false, new string[]{"Oh, it's not?", "Well either way, I still look fabulous. Have a great rest of the day!"}),
                },
            },
        };

                break;
            case CatType.music:

                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey, what's up? Got any new music on you?",}),
                       new Dialogue(CatType.none, true, new string[]{"Maaaybe.",}),
                       new Dialogue(cat, true, new string[]{"Not willing to tell me, huh?",}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Can you tell me now?"}),
                    new Dialogue(CatType.none, true, new string[]{"Sorry bro. I don't have any new music.",}),
                    new Dialogue(cat, true, new string[]{"Drats..","I've been listening to Cat Park's In The End on repeat for days now.."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You got any good music? I heard the music across the Ocean is pretty dope."}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Mark me impressed. Someone should make a song about you!"}),
                },
            },
              new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Bro! What's up?"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Smell ya later."}),
                },
            },
        };

                break;

            case CatType.nerd:

                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"What's the difference between a cat and a comma?"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"So, what <i>is</i> the difference between a cat and a comma?"}),
                      new Dialogue(cat, true, new string[]{"One has claws at the end of its paws, the other has a pause at the end of its clause!", "Hehe."}),
                },
            },
              new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I study really hard, so if you wanna know something, ask me!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"So do you know the digits of pi?"}),
                      new Dialogue(cat, true, new string[]{"Pi? Of course!", "There's blueberry, and apple, and chocolate, and oh! My favorite - raspberry cheesecake.."}),
                },
            },
        };

                break;
            case CatType.sphinx:

                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Hey, wanna play?", "..Just be careful with the skin; it's sensitive."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Thanks for being careful with the skin!",
                      "You can't imagine how many baths I need for this coat.."}),
                },
            },
        };

                break;
            case CatType.niceeyes:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I'll have you know, my catliner is <i>all natural."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Ok, I admit it, my owner helps me maintain my fabulous looks by taking really good care of me."}),
                },
            },
        };
                break;
            case CatType.old:
                if (ExploreController.wonLastLevel())
                {
                    randomDialogues = new Dialogue[][][]{
                        new Dialogue[][]{
                            new Dialogue[]{
                                new Dialogue(cat, true, new string[]{"Hello travelers. I too love travelling!"}),
                            },
                            new Dialogue[]{
                                new Dialogue(cat, true, new string[]{"Young 'uns like you inspire me to keep travelling.","I might not be in top health, but without taking risks, how can I experience life?"}),
                            },
                        },
                    };
                }
                else
                {
                    randomDialogues = new Dialogue[][][]{
                        new Dialogue[][]{
                            new Dialogue[]{
                                new Dialogue(cat, true, new string[]{"Ah, adventurers. I remember when I was full of energy."}),
                            },
                            new Dialogue[]{
                                new Dialogue(cat, false, new string[]{"Good luck bringing the Life cats back.","We can't move forward if we linger on the past."}),
                            },
                        },
                    };

                }

                break;
            case CatType.oxo:
                randomDialogues = new Dialogue[][][]{


             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"HEY, I'M..."}),
                      new Dialogue(CatType.none, true, new string[]{"Yes?"}),
                      new Dialogue(cat, true, new string[]{"..WHA? WHOA!", "DIDN'T SEE YOU THERE! WERE YOU TALKING TO ME?"}),
                      new Dialogue(CatType.none, true, new string[]{"Yes.."}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"You were saying? I'm..?"}),
                      new Dialogue(cat, true, new string[]{"OH, YES; I'M.. ", "*sees skybird, gets distracted*"}),
                      new Dialogue(CatType.none, true, new string[]{"Sigh."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"HELLO HELLO HELLO HELLO HELLO!!!"}),
                      new Dialogue(CatType.none, true, new string[]{"Have you been eating catnip again?"}),
                      new Dialogue(cat, true, new string[]{"MAYBE MAYBE MAYBE!!!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"You should lay off the catnip -"}),
                      new Dialogue(cat, true, new string[]{"BUT IT'S SO TASTY SO TASTY SO TASTY!"}),
                },
            },

            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I'LL SHOW YOU WHAT I'VE GOT! RIGHT HERE, RIGHT MEOW!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"OOOOH YOU'RE POWERFUL!"}),
                       new Dialogue(CatType.none, true, new string[]{"Thanks!"}),
                      new Dialogue(cat, true, new string[]{"THAT LEFT ME FELINE SO GOOD! LET'S DO IT AGAIN SOMETIME!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I'M BEAUTY, I'M GRACE, I HAVEN'T SLEPT IN DAYS!!!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"You should get some sleep now!"}),
                      new Dialogue(cat, true, new string[]{"I KNOW I SHOULD, BUT I CAN'T! THE CITY IS SO INTERESTING!"}),
                },
            },
        };
                break;
            case CatType.persian:
            case CatType.phat:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Whatever we do, let it not include exercise."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I'm not lazy, I just don't want to do anything."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey, I should warn you about something..", "..."}),
                      new Dialogue(CatType.none, true, new string[]{"What?"}),
                      new Dialogue(cat, false, new string[]{"I forgot."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"My job here is done."}),
                       new Dialogue(CatType.none, false, new string[]{"You didn't do anything - "}),
                       new Dialogue(cat, true, new string[]{"Yes."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Where is the nearest fluffy bed?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Can't wait to lounge and munch bread now.","Remember kids, time you enjoy wasting is not wasted time."}),
                },
            },
        };
                break;
            case CatType.pink:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Do you like my fluff? I went to the barber's today!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Fluff like this doesn't come naturally, yknow! I have an image to maintain!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I may be nice and fluffy, but I have claws!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hmm.. you're cool! I won't use my claws on you.","Only cats that take advantage of my niceness!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Hey have you seen this other cat? She's got grey fur, little plant on her head, <i>sweetest</i> gal.."}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, false, new string[]{"Why do you want to find that other cat?"}),
                      new Dialogue(cat, true, new string[]{"Because I just love her so much you know?", "And I got a sudden urge to tell her."}),
                },
            },
        };
                break;
            case CatType.pixel:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Want some help with using the computer?"}),
                      new Dialogue(CatType.none, false, new string[]{"I don't want to be any trouble.."}),
                      new Dialogue(cat, false, new string[]{"You can return the favor by playing with me~"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Ok, let me tell you my favorite shortcuts~","There's Alt+Tab for switching apps * blah blah blah blah*"}),
                       new Dialogue(CatType.none, false, new string[]{"O-oh, you sure know a lot..! <size=50%>And I was totally listening!"}),
                      new Dialogue(cat, false, new string[]{"^-^"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"hello, today I will rate you.. Loading.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Loading..", "I rate you: very cute ^-^"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Hmm.. Loading.. please answer this prompt: why are you so cute?"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"What is your answer? :o"}),
                    new Dialogue(CatType.none, true, new string[]{"I don't know, but thank mew!"}),
                     new Dialogue(cat, true, new string[]{"It's true! :p"}),
                },
            },
        };
                // do
                break;
            case CatType.plush:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hiya! Have you seen my hooman?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"It's only with the love of my hooman that I can be this strong!"}),
                },
            },
             new Dialogue[][]{
        new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Where's my hooman?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I hope my hooman hasn't ditched me for some better, popular plush.."}),
                },
            },
        };
                break;
            case CatType.rainbow:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Hey there, cute cats~"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Know what would be more awesome than a single rainbow?", "A <i>double rainbow.</i> And flat pushable things, but that goes without saying~"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hellooooo there~"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oooh! What a <i>brilliant</i> display!!"}),
                },
            },
        };
                break;
            case CatType.shadow:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"....."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{
                          "Do you want the power of the dark and dead?",
                          "..I was born in the dark. You will never know the dark like I do.",
                      }),
                      new Dialogue(CatType.none, false, new string[]{"No, that's ok, you can have the dark and dead."}),
                      new Dialogue(cat, false, new string[]{"..hm. *looks appeased*"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..."}),
                      new Dialogue(CatType.none, true, new string[]{"..hello?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"...I can speak, by the way; I'm just shy.", "Thanks for playing with me.."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..."}),
                },
                new Dialogue[]{
                     new Dialogue(cat, false, new string[]{"..Who are you?"}),
                      new Dialogue(CatType.none, false, new string[]{"I can be your friend!"}),
                      new Dialogue(cat, true, new string[]{"...I have many acquaintances, but my one friend is the deep dark night.", "You can be my second friend."}),
                },
            },

             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"..you're not scared of me?"}),
                      new Dialogue(CatType.none, true, new string[]{"No way.", "Maybe a little bit, but in a 'wow-you're-cool' way."}),
                      new Dialogue(cat, true, new string[]{":)"}),
                },
            },
        };
                break;
            case CatType.sleepy:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"...ZZZ..","..*mumble* i love s - SNORREEE - leeping..."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"...ZZzz ... hwaT?!.."}),
                      new Dialogue(CatType.none, true, new string[]{"..Hi, are you awake?"}),
                      new Dialogue(cat, true, new string[]{"...wh - not anymore, bye.. ZZZZ..."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{".ZZZZZ.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"..oh, hello."}),
                      new Dialogue(CatType.none, true, new string[]{"Hi!", "Why are you always sleeping?"}),
                      new Dialogue(cat, false, new string[]{"Ehh.. I just really love sleep. I'd marry sleep if I could."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oh, hello hello!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"weeee! we should hang out more."}),
                      new Dialogue(CatType.none, false, new string[]{"When are you free?"}),
                      new Dialogue(cat, false, new string[]{"let's see.. i need my 20 hour nap, then i eat, then.."}),
                },
            },
        };
                break;
            case CatType.sushi:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Please don't eat me."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, true, new string[]{"Now can I eat you?"}),
                      new Dialogue(cat, true, new string[]{"No, sorry.","I really need a <b>'DO NOT TOUCH'</b> sign."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Yeah, I'm yummy, but have you seen silver fish sushi?", "They're <i>delicious."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, true, new string[]{"I've never seen a silver fish sushi, but you look delicious too!"}),
                      new Dialogue(cat, true, new string[]{"Thanks!", "I'll take that as a compliment and not a threat."}),
                },
            },
        };
                break;
            case CatType.unicorn:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Wheeee heyyy hey long time no see!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Wow i thought I was fast,","but you're like a ninja, so fast! Nothing can stop you!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"whoppee! will you spend time with me?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"i love spending time with brave cats."}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Do you believe in unicorns? Tell me you do."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.unicorn, true, new string[]{"I believe in you."}),
                      new Dialogue(cat, true, new string[]{"whoppeee! <3"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I love veggies."}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.unicorn, true, new string[]{"What's a unicorn's favorite meal?", "One corn! Uni-corn!", "Hahaha."}),
                },
            },
        };
                break;
            case CatType.uwu:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"ooooh hello! Are you here to play with me?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"thank mew for playing i hope u had a good time! ",
                      "we should cuddle and clean each other sometime!",}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oof hewwo !! seeing you has made my day x445334 better!!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oh my that was amazing i am not even exaggerating",
                      "i am filled with admiration! for your skill!!",
                      "i hope u kill it out there i'll miss u lots !"}),
                      new Dialogue(CatType.none, true, new string[]{"Aah, thank you so much! I'll miss you too~"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hello hello you guys seem nice! wanna play?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"wow you guys <i>are</i> nice! and powerful!", "such powerful cat energy !!","i hope you get plenty of rest and kindess in the future!! <3"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"aaa you're so cute","i just wanna wrap my paws around u and hug tight!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"ugh i'm feeling so blessed spending time with you is so very yay!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"oh! hewwo!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"that was so amazing . friend has 6 letters but so does omgggg"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hellllooo ~"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"o-oh my kittens. i am: in awe of your skills !!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hello friend im ready for the time of my life!!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"aaa i loved that so muuuchhhhhhhh i'm such a stan"}),
                      new Dialogue(CatType.none, true, new string[]{"We can stan each other~"}),
                      new Dialogue(cat, true, new string[]{"how ideal ugh your mind!!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hewwo again!!!"}),
                },
                new Dialogue[]{
                    //"dat was a fun game, altho i got some scratches!"
                    new Dialogue(cat, false, new string[]{"aw i got some scratches :<"}),
                    new Dialogue(CatType.none, true, new string[]{"Oh no, are you okay?"}),
                      new Dialogue(cat, true, new string[]{"yeah, i have no regrets!","i may not have many redeeming qualities, but i try to stay positive!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"hiii!! im so very happy to see you guys again!!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"so does this make us friends?"}),
                      new Dialogue(CatType.none, true, new string[]{"We were friends all this time!"}),
                      new Dialogue(cat, true, new string[]{"ohoho i'm blushing to the stars and back ^-^"}),
                },
            },
        };
                break;
            case CatType.wood:
                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I is Wood. Let us play."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"You is good. I go away."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Hello. Good day.","You are friend. Let's play."}),
                },
            },
        };
                break;

            default:
                //basic cats

                randomDialogues = new Dialogue[][][]{
            new Dialogue[][]{
                  new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Ah, humans confuse me sometimes.."}),
                   new Dialogue(CatType.none, true, new string[]{"Want to forget about it and play?", }),
                   new Dialogue(cat, true, new string[]{"Yea!"}),
                   },
                new Dialogue[]{
                     new Dialogue(CatType.none, true, new string[]{"Why were you confused before?", }),
                   new Dialogue(cat, true, new string[]{"I lied down on this human's leg.. and they moved it!"}),
                    new Dialogue(CatType.none, true, new string[]{"Gasp! The nerve!", }),
                   new Dialogue(cat, true, new string[]{"I know! They're not allowed to move where I'm sitting.","They should've been honoured."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"*sniffs your eye* greetings!"}),
                   new Dialogue(CatType.none, true, new string[]{"*licks their nose* hello!"}),
                },
                new Dialogue[]{
                  new Dialogue(cat, true, new string[]{"*sniffs your cheek* sniff you later."}),
                   new Dialogue(CatType.none, true, new string[]{"*licks paw* toodles."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Have you seen my hooman? Are you hiding my hooman?"}),},
                new Dialogue[]{
                   new Dialogue(CatType.none, true, new string[]{"I don't have your human.",
                   "Why do you need them?" }),
                    new Dialogue(cat, true, new string[]{"It's exactly "+DateTime.Now.ToString("hh:mm tt")+" and I need to make sure they're getting their daily kneading!"}),
                    new Dialogue(CatType.none, true, new string[]{"Good luck!", }),
                   new Dialogue(cat, true, new string[]{"Sigh, thank mew!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Are you the adventurer I keep hearing about?"}),},
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"You're the adventurer, aren't you!",
                   "I bet you've been to some amazing places.",
                   "Have you seen those legless slitherers?",
                   "Oh, oh!",
                   "What about those earless fuzzy quackers? They're so cute, aren't they?"}),
                   new Dialogue(CatType.none, true, new string[]{"I think you've seen more than I have..", }),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Hey, wanna play?"}),
                   new Dialogue(CatType.none, true, new string[]{"Not sure.", }),
                   new Dialogue(cat, true, new string[]{"Whatever you want. We all have different wants."}),
                },
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Yay, I'm happy we played!","Let me know when you wanna play it again!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Hey you! I bet I can beat you!"}),
                },
                new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Agh..", "Why can't I defeat anyone!?","Is it because I'm not one of those magical cats?"}),
                    new Dialogue(CatType.none, true, new string[]{"You can beat anyone you want if you practice enough.", }),
                     new Dialogue(cat, true, new string[]{"Hm, maybe. I'm gonna level up more."}),
                },
            },
            new Dialogue[][]{
                  new Dialogue[]{
                   new Dialogue(cat, true, new string[]{  "Hey, help me!"}),
                    new Dialogue(CatType.none, true, new string[]{  "Why?"}),
                    new Dialogue(cat, true, new string[]{  "I'll play with you if you help me!"}),
                   },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Help me!  My food bowl is empty."}),
                    new Dialogue(CatType.none, true, new string[]{
                    "Not empty!", }),
                    new Dialogue(cat, true, new string[]{"I know. It's the worst."}),
                    new Dialogue(CatType.none, true, new string[]{
                    "Have you tried meowing without pause?", }),
                    new Dialogue(cat, true, new string[]{"Yes, my hoomans won't wake up."}),
                    new Dialogue(CatType.none, true, new string[]{
                    "Have you tried batting their face?", }),
                    new Dialogue(cat, true, new string[]{"Yep, didn't work."}),
                    new Dialogue(CatType.none, true, new string[]{
                    "Sorry, you're doomed. RIP.", }),
                     new Dialogue(cat, true, new string[]{"My worst fear.."}),
                },
            },
             new Dialogue[][]{
                  new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Help, I think my hooman's gone!",
                   "I saw them go into a room with a porcelain chair and they haven't come back!"}),
                    new Dialogue(CatType.none, true, new string[]{ "I'm sure they're fine.", }),
                     new Dialogue(cat, true, new string[]{"No they're not!"}),
                   },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Huff huff.. my hooman.."}),
                    new Dialogue(CatType.none, true, new string[]{ "Your hooman's just doing their necessary business on the chair, see?", }),
                    new Dialogue(cat, true, new string[]{"..they need my support while doing their business.."}),
                },
            },
            new Dialogue[][]{
                  new Dialogue[]{
                   new Dialogue(cat, true, new string[]{"Hey, I'm confused and I'm gonna take my frustrations out on you!",}),
                   },
                new Dialogue[]{
                     new Dialogue(CatType.none, true, new string[]{ "Are you less frustrated now?", }),
                    new Dialogue(cat, true, new string[]{"Yes, sorry for suddenly confronting you.."}),
                    new Dialogue(CatType.none, true, new string[]{ "Why were you confused?", }),
                    new Dialogue(cat, true, new string[]{"My hooman keeps self-torturing by submerging themselves in water..!",
                    "How do I stop them?"}),
                     new Dialogue(CatType.none, true, new string[]{ "I think they're bathing themselves.", }),
                    new Dialogue(cat, true, new string[]{"Without their tongue?"}),
                     new Dialogue(CatType.none, true, new string[]{ "Yes, they're strange.","Rub your scent on them afterwards in case someone kidnaps them." }),
                    new Dialogue(cat, true, new string[]{"Obviously! Thank mew for calming me."}),
                },
            },
            new Dialogue[][]{
                  new Dialogue[]{
                   new Dialogue(cat, true, new string[]{
                    "I've found something that's life-changing."
                    ,"I'll only tell those who deserve it!", }),
                   },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"You deserve to know my discovery.",
                    "I found a porcelain bowl in this thing called the 'bathroom'.",
                    "The water in the porcelain bowl is absolutely delightful.",
                    "You have to try it.", }),
                    new Dialogue(CatType.none, true, new string[]{
                    "Ooh, don't mind if I do!", }),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Hey. Stop right there. I don't like the look of you.."}),
                    },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I take it back. I like you.","No wait. I don't. Nevermind. I don't know.", }),
                    new Dialogue(CatType.none, true, new string[]{"Decide already!", }),
                    new Dialogue(cat, true, new string[]{"Nah, I like you.", }),
                    new Dialogue(CatType.none, true, new string[]{"Yay!", }),
                    new Dialogue(cat, true, new string[]{"Or do I?", }),
                    new Dialogue(CatType.none, true, new string[]{"Sigh.", }),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I won't let anyone else me beat me! Not this time!"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Bah. I just didn't win 'cause I'm feeling down.", }),
                    new Dialogue(CatType.none, true, new string[]{"What's got you down?", }),
                    new Dialogue(cat, true, new string[]{"All my friends can hunt mice and birds,", "but I can't hunt anything.", }),
                    new Dialogue(CatType.none, true, new string[]{"I'm sure you can hunt something. Just start small.","Hunt leaves, then make your way to birds.", }),
                    new Dialogue(cat, true, new string[]{"Hm, thanks. I'll try doing that.", }),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Hey, friend~","It's my favorite time of the day for playing!"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"That was a lot of fun~", "What's your favorite time of the day?", }),
                     new Dialogue(CatType.none, true, new string[]{"I love running around and making noise at midnight.", }),
                     new Dialogue(cat, true, new string[]{"Oh yeah, that's a lot of fun too!", "Sometimes the hooman wakes up and makes noise with me.", "Saying things like, 'I'm trying to sleep' or 'Mr Snuggles, no'. It's funny.", }),
                     new Dialogue(CatType.none, true, new string[]{"Humans are so silly.", }),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Hey hey~ Are you in the mood to play?"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Looks like we were both in the same moods today,","I love when cats align."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"I have a question for you.."}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"What if the hooman is not my pet, but I'm theirs?",}),
                    new Dialogue(CatType.none, true, new string[]{"Then why do they serve you food and not the other way around?",}),
                    new Dialogue(cat, true, new string[]{"Indeed! How silly of me to ask.",}),
                     new Dialogue(CatType.none, true, new string[]{"That's ok. There's no silly questions.",}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Good day. Today is a day full of possibilities! Like me meeting you!"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"That was a good start of my day.","Maybe I'll do things for the rest of the day. Or maybe not."}),
                },
            },

            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Oh, hey! you're such cute cats, nice to meet!"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Sometimes I wonder if there's more to life than napping and being ridiculously cute."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Hello~ meeting you is one of my favorite parts of today!"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Well, this was one of my favorite parts of the day.","There's that time where I cuddled so close with my hooman we became one entity..","Hey, we should cuddle sometime!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Today has been a long day.. Wanna play?"}),},
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"It's been a long day, but playing with you has been relaxing."}),
                    new Dialogue(CatType.none, true, new string[]{"Ah, I'm glad."}),
                    new Dialogue(cat, true, new string[]{"I think everything's gonna be alright."}),
                },
            }, new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Good day."}),
                    new Dialogue(CatType.none, true, new string[]{"Good day to you too.  ..Your nails look funny!"}),
                    new Dialogue(cat, true, new string[]{"That's 'cause they got clipped..","I still want to play though! That is, if you want to!"}),
                },
                new Dialogue[]{
                    new Dialogue(CatType.none, true, new string[]{"Good fight, clipped friend."}),
                    new Dialogue(cat, true, new string[]{"Thanks! You too."}),
                    new Dialogue(CatType.none, true, new string[]{"How do you fight with clipped nails?"}),
                    new Dialogue(cat, true, new string[]{"The strategy when you're handicapped is to grow in other areas.","Like knowing where to hit, or getting better at dodging.","No handicap will stop me."}),
                    new Dialogue(CatType.none, true, new string[]{"Very admirable *takes notes*"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Where is food? Do you have food?"}),
                },
                new Dialogue[]{
                    new Dialogue(cat, true, new string[]{"Hmm, that was fun..", "Maybe there's more to life than eating and sleeping.","Haha, nah, I sure crack myself up."}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Where is human?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Human? Hello?", "Hello, human? I'm right here! Bring me food!"}),

                },
                },

            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Humans are so funny."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Did you know humans steal your poop with a little shovel when you're not looking?","I know, shocking, isn't it?"}),

                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"I'm hungry. Do you have food?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, false, new string[]{"Were you trying to distract me? Clever, but I'm still hungry."}),

                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Oh, hiya!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"We should hang out sometime, watch birds together."}),

                },
            },
              new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Wanna know something tragic?", "I don't know, should I tell you?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Ok, let me tell you the tragic news.", "The red dot can't be caught.","I caught it once, and I felt nothing. It's all in our heads! O_O"}),

                },
            },
              new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Hello! Give me all your money please!"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Ahhh, I was just joking about the money thing. <size=60%>Please forgive me."}),

                    },
                },
            new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Roar! Fear my wrath!"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Oh wow, your wrath is greater than mine; can we be friends?"}),

                    },
                },
                 new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"I have a secret~", "Play with me and I'll tell you!"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Ok, ok, my secret:", "Up north, there are cats that can swim!","Can you believe it?"}),

                    },
                },
            new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Aaaaa~ I'm hungry~ Do you have food?"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Aaaa I'm still hungry! Know any cats that I can bug for food?"}),

                    },
                },
            new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"Hey. Have you heard?"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"So I heard that there are cats that can't talk.. only bork!", "Weird, right? Never heard anything like it."}),

                    },
                },
                 new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"He - h - h - h - hh -"}),
                        new Dialogue(CatType.none, true, new string[]{"..?"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"- h - hhhhh - <b>ACHOO!</b>", "Hey! ..is what I wanted to say."}),

                    },
                },
            new Dialogue[][]{
                    new Dialogue[]{
                        new Dialogue(cat, true, new string[]{"*stares*"}),
                    },
                    new Dialogue[]{
                        new Dialogue(cat, false, new string[]{"Hmph.. not bad."}),

                    },
                },
                 new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Help! My hooman is drowning!"}),
                        new Dialogue(CatType.none, true, new string[]{"They're probably showering - "}),
                        new Dialogue(cat, true, new string[]{"HELP!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"..Ok, maybe they're showering. ..But what if they're not? But - but - but what if -"}),

                },
            },            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Do you realize I am both cute and powerful?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I <i>am</i> cute and powerful, dang it, I <i>am."}),

                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Ugh, what are you looking at?!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Thanks for snapping me out of my bad mood,", "You see, I accidentally stepped in a puddle; it was awful!"}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You! I demand you tell me where the nearest box is!"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Box! I need a tiny box to smush myself in this instant!"}),
                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Do you know where my hooman is?", "They're 8 feet tall, I think, and have large hands, great for scratching."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You don't know where my hooman went?","I can't believe I lost my hooman..",}),
                      new Dialogue(CatType.none, true, new string[]{"It's ok, I'm sure they'll come back!",}),
                      new Dialogue(cat, true, new string[]{"Thank mew, I hope so..",}),
                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Meow?"}),
                },
                new Dialogue[]{
                      new Dialogue(CatType.none, true, new string[]{"Meow."}),
                      new Dialogue(cat, true, new string[]{"!! Meow meow!"}),

                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Ugh, I miss my hooman.."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"You must miss your hooman too."}),

                },
            },
            new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Psst.. I'll let you in on a secret if you play with me."}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"I hoard my hooman's bottle caps under the fridge. Don't tell 'em."}),

                },
            },
             new Dialogue[][]{
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Have you seen that red dot anywhere?"}),
                },
                new Dialogue[]{
                      new Dialogue(cat, true, new string[]{"Red dot always escapes me, not today I say!", "I'm catch it, I'm sure!"}),

                },
            },
                    };
                if (!ExploreController.wonLastLevel())
                {
                    randomDialogues = randomDialogues.Concat(new Dialogue[][][]{
                        new Dialogue[][]{
                            new Dialogue[]{
                                new Dialogue(cat, false, new string[]{"Are you really gonna bring Life back?"}),
                            },
                            new Dialogue[]{
                                new Dialogue(cat, false, new string[]{"I'm not gonna stop you, but how will you bring Life back?", "You know Life and Manmade hate each other, right?"}),
                            },
                    },
                        new Dialogue[][]{
                            new Dialogue[]{
                                new Dialogue(cat, true, new string[]{"I heard you're bringing Life back!", "You can't; you don't know what they're like!"}),
                            },
                            new Dialogue[]{
                                new Dialogue(cat, false, new string[]{"Fine, go on, but take caution.", "I heard the Life cats are huge, scary cats that could destroy the world with one paw."}),

                            },
                        },
                    }).ToArray();
                }
                break;
        }
        return randomDialogues;
    }
    private static Dialogue[][] getRandomDialogues(CatType cat)
    {
        Dialogue[][][] d = getRDArray(cat);
        return d[UnityEngine.Random.Range(0, d.Length)];
    }
    #endregion

    #region random HELPERS -----
    private static List<CatType> getRandomRelevantCats(WorldType worldFrom, int levelFrom)
    {
        List<CatType> unlockedCats = getUnlockedCats(worldFrom, levelFrom);

        //never choose ROBOT, Death, or the royal family - Cloud, Night, Grass, Star
        for (int i = unlockedCats.Count - 1; i > -1; i--)
        {
            CatType c = unlockedCats[i];
            Debug.Log("getRandomRelevantCats: "+c);
            if (c == CatType.doot ||
            c == CatType.bot ||
            c == CatType.cloud ||
            c == CatType.night ||
            c == CatType.grass ||
            c == CatType.star)
            {
                unlockedCats.RemoveAt(i);
            }
        }
        if (worldFrom != WorldType.house)
        {
            Debug.Log("adding house 0 cats");
            unlockedCats.AddRange(getUnlockedCats(WorldType.house, 0));
        }

        return unlockedCats;
    }

    private static int getBackgroundMax(WorldType world)
    {
        switch (world)
        {
            case WorldType.city:
                return 3;
            case WorldType.computer:
                return 0;
            case WorldType.fields:
                return 3;
            case WorldType.house:
                return 1;
            case WorldType.ocean:
                return 2;
            case WorldType.sky:
                return 1;
            case WorldType.space:
                return 0;
            case WorldType.woods:
                return 3;
            default:
                return 1;
        }
    }

    public static LevelAsset loadLevelAsset(WorldType world)
    {
        return Resources.Load<LevelAsset>("LevelAssets/" + world.ToString() + "/" + world.ToString());
    }

    public static int getTotalLevels()
    {
        int totalLevels = 0;
        foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
        {
            totalLevels += loadLevelAsset(world).stageNames.Length;
        }
        return totalLevels;
    }

    #endregion

    #region Gacha Cat BUCKETS ---

    private static List<CatType> getUnlockedCats(WorldType world, int level)
    {
        List<CatType> cats = new List<CatType>();
        //if never played before
        if (world == WorldType.house && level == 0)
        {
            level = 1;
        }
        for (int i = 0; i < level; i++)
        {
            StageAsset stage = Resources.Load<StageAsset>("LevelAssets/" + world.ToString() + "/" + world.ToString() + i.ToString());
            foreach (CatType cat in stage.unlockCats)
            {
                cats.Add(cat);
            }
        }
        return cats;
    }

    public static List<List<CatType>> getCatBuckets()
    {
        List<CatType> rarity1 = new List<CatType>();
        List<CatType> rarity2 = new List<CatType>();
        List<CatType> rarity3 = new List<CatType>();
        List<CatType> rarity4 = new List<CatType>();

        HashSet<CatType> collectedCats = new HashSet<CatType>();

        // Add already owned cats to save performance -> no need to load already-loaded Cat Assets
        foreach (Cat cat in GameControl.control.playerData.deck)
        {
            if (collectedCats.Contains(cat.catType))
            {
                continue;
            }
            switch (cat.getCatAsset().rarity)
            {
                case 1:
                    rarity1.Add(cat.catType);
                    break;
                case 2:
                    rarity2.Add(cat.catType);
                    break;
                case 3:
                    rarity3.Add(cat.catType);
                    break;
                case 4:
                    rarity4.Add(cat.catType);
                    break;
            }
            collectedCats.Add(cat.catType);
        }

        // Add cats that aren't owned but are unlocked for collecting through the CatMachine

        foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
        {
            WorldLevel value;
            if (GameControl.control.playerData.WorldLocks.TryGetValue(world, out value) == false)
            {
                value = new WorldLevel(-1, 0, new uint[1], new ushort[1]);
            }

            List<CatType> unlockedCats = getUnlockedCats(world, value.level);

            foreach (CatType unlockedCat in unlockedCats)
            {
                if (collectedCats.Contains(unlockedCat))
                {
                    continue;
                }
                switch (Resources.Load<CatAsset>("CatAssets/" + unlockedCat.ToString()).rarity)
                {
                    case 1:
                        rarity1.Add(unlockedCat);
                        break;
                    case 2:
                        rarity2.Add(unlockedCat);
                        break;
                    case 3:
                        rarity3.Add(unlockedCat);
                        break;
                    case 4:
                        rarity4.Add(unlockedCat);
                        break;
                }
                collectedCats.Add(unlockedCat);
            }
        }

        List<List<CatType>> catBuckets = new List<List<CatType>>();
        if (rarity1.Count > 0)
            catBuckets.Add(rarity1);
        if (rarity2.Count > 0)
            catBuckets.Add(rarity2);
        if (rarity3.Count > 0)
            catBuckets.Add(rarity3);
        if (rarity4.Count > 0)
            catBuckets.Add(rarity4);
        return catBuckets;
    }

    #endregion

    #region CAT ACTION DATA ---------
    // every Cat Type has their own unique action (sniff: case CatType. meow, etc). 
    //string array carries translated action:
    //0 : English

    public static Dialogue[] AdjustCosmetics(Dialogue[] selected, CatType none)
    {
        foreach (Dialogue d in selected)
        {
            for (int i = 0; i < d.dialogue.Length; i++)
            {
                d.dialogue[i] = adjustCosmeticString(d.dialogue[i], d.speaker == CatType.none ? none : d.speaker);
            }
        }
        return selected;
    }
    public static string adjustCosmeticString(string dialogue, CatType speaker)
    {
        /*
        size
        color
        font
        bold
        italic
        lowercase/uppercase/smallcaps
        <cspace=1em>
         */
        switch (speaker)
        {
            case CatType.anime:
                dialogue = "<b><color=#877aa1>" + dialogue;
                //<color=#C29B1E>
                break;
            case CatType.wood:
                dialogue = "<cspace=-0.15em><color=#6f3116>" + dialogue;
                break;
            case CatType.rainbow:
            case CatType.unicorn:
                dialogue = "<color=#C29B1E>" + dialogue;
                //<color=#C29B1E>
                break;
            case CatType.pixel:
            case CatType.pink:
                dialogue = "<color=#a9547f>" + dialogue;
                break;
            case CatType.pocky:
                dialogue = "<b><color=#a9547f>" + dialogue;
                break;
            case CatType.nerd:
                dialogue = "<color=#a9547f>" + dialogue;
                break;
            case CatType.gem:
                dialogue = "<color=#c35959>" + dialogue;
                break;
            case CatType.ghost:
                dialogue = "<color=#6d7c89>" + dialogue;
                //<color=#>
                break;
            case CatType.lantern:
            case CatType.fire:
                dialogue = "<color=#c14400>" + dialogue;
                //<color=#>
                break;
            case CatType.water:
                dialogue = "<color=#009498>" + dialogue;
                //<color=#>
                break;
            case CatType.ice:
                dialogue = "<color=#0098d2>" + dialogue;
                //<color=#>
                break;
            case CatType.doot:
                dialogue = "<material=\"pixelfont\"><u><mark=#1a00002f>" + dialogue + (dialogue.Length > 8 ? " doot." : "");
                break;
            case CatType.choco:
            case CatType.donutchoc:
                dialogue = "<color=#854a3f>" + dialogue;
                break;
            case CatType.donutpink:
                dialogue = "<color=#e75c71>" + dialogue;
                break;
            case CatType.dog:
                if (dialogue.ToLower().IndexOf("bork") <= -1)
                {
                    string[] borks = new string[]{
                        "bork!",
                        "bork BROK yip",
                        "Bork! BOOF!",
                        "*barks an earth-shaking -* BORK!",
                        "Bork? u_u",
                        "BORK!! <3",
                        "mmeeeooo - BORK",
                        "mrrooOORK!",
                        "meoOBORK!",
                        "*sniffs butt* brok",
                        "bork? <3",
                        "bro kbork",
                        "burk bork"
                    };
                    dialogue = borks[UnityEngine.Random.Range(0, borks.Length)];
                }

                dialogue = "<font=\"cutefont\">" + dialogue;
                break;
            case CatType.sleepy:
                dialogue = "<cspace=0.4em>" + dialogue;
                break;
            case CatType.cloud:
                dialogue = "<color=#2D83AC><smallcaps><b>" + dialogue;
                break;
            case CatType.night:
                dialogue = "<color=#332679><smallcaps><b>" + dialogue;
                break;
            case CatType.plush:
                dialogue = "<color=#676ea5><b>" + dialogue;
                break;
            case CatType.star:
                dialogue = "<color=#69378D><b>" + dialogue;
                break;
            case CatType.grass:
                dialogue = "<color=#678A68><b>" + dialogue;
                break;
            case CatType.uwu:
                dialogue = "<color=#DD819C><size=90%>" + dialogue.ToLower();
                break;
            case CatType.oxo:
                dialogue = "<size=110%>" + dialogue.ToUpper();
                break;
            case CatType.bot:
                dialogue = "<material=\"pixelfont\"><color=#000000><b><i>" + dialogue.ToUpper();
                break;
        }
        return dialogue;
    }

    public static Dialogue[] defaultReaction(ActionType action, Cat speaker2)
    {
        Dialogue[][] dialogues;
        switch (action)
        {
            case ActionType.Swim:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, false, new string[]{"*swims in mid-air*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"..What are you doing?"}),
                    },
                        new Dialogue[]
                    {
                        new Dialogue(CatType.none, false, new string[]{"*does swimming motion*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Oh, cool! You know how to swim!"}),
                    },
                     new Dialogue[]
                    {
                        new Dialogue(CatType.none, false, new string[]{"*swims in mid-air*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"..There's no water nearby to swim in, you know."}),
                    },
                };
                break;
            case ActionType.Boop:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"*boops nose*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*boops back*"}),
                    },
                };
                break;
            case ActionType.Love:
                dialogues = new Dialogue[][]{

                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"I hope you're happy, healthy, and safe!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"You too!"}),
                    },
                     new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"I love how cute you look today!!"}),
                        new Dialogue(speaker2.catType, true, new string[]{":3"}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"On a scale of 1 to 10, I love you this.. this..","What's the biggest number?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"Infinity?"}),
                        new Dialogue(CatType.none, true, new string[]{"Then more than infinity!!"}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"I love spending time with you and hope something good happens to you today!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Yayy :3"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"I'm glad I befriended you.'"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Me too :3"}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"I appreciate you. You're a really good friend."}),
                        new Dialogue(speaker2.catType, true, new string[]{"You're a better friend."}),
                        new Dialogue(CatType.none, true, new string[]{"You're the best friend."}),
                        new Dialogue(speaker2.catType, true, new string[]{"We're both the best!"}),
                    },
                };
                break;
            // case CatAction.sleep:
            //     dialogues = new Dialogue[][]{
            //         new Dialogue[]
            //         {    new Dialogue(CatType.none, false, new string[]{"....ZZz."}),
            //             new Dialogue(speaker2.catType, true, new string[]{"H-hello?", "Did you really just fall asleep?"}),
            //         },
            //         new Dialogue[]
            //         {
            //             new Dialogue(CatType.none, true, new string[]{"Hey there, want to be my pillow?"}),
            //             new Dialogue(speaker2.catType, false, new string[]{"Me? Excuse me?"}),
            //             new Dialogue(CatType.none, false, new string[]{"No? The ground will do then.", "...ZZzz."}),
            //         }
            //     };
            //     break;
            case ActionType.Meow:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*meow*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Yes."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*meow*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*meows back*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Meow!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Hi to you too."}),
                    },
                     new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Meow?"}),
                        new Dialogue(speaker2.catType, true, new string[]{"I don't know."}),
                    },
                };
                break;
            case ActionType.Bork:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"BORK!! bork bork!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"awww."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"bork ruff rufff"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Bork to you too."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"yip bork bork bork bork bork. bork bork bork"}),
                        new Dialogue(speaker2.catType, false, new string[]{"Sorry, what?","..Can someone translate?"}),
                    },


                };
                break;
            case ActionType.Comfort:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"It's ok to be sad."}),
                        new Dialogue(speaker2.catType, true, new string[]{":o"}),
                    },
                     new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"If you're feeling sad or alone you can talk to me!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Uhh, ok, thanks!"}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"No matter how messed up you are, there's some cat that likes you."}),
                        new Dialogue(speaker2.catType, true, new string[]{"Uhh, oh, ok."}),
                    },
                };
                break;
            case ActionType.Deal:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"So, let's make a deal -","You give me 10 silver fish, and I'll give you a thorough coat cleaning with my tongue. Deal?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"I can clean my own coat, thank you very much."}),
                    },
                   new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Would you trade your affection for a coin?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"My love can't be bought!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"I'll give you some fish for a "+speaker2.getCatAsset().secondaryType.ToString().ToLower()+"! Deal?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"No deal. "+speaker2.getCatAsset().secondaryType.ToString() + "'s worth a lot!"}),
                    },
                };
                break;
            case ActionType.Feed:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {   new Dialogue(CatType.none, false, new string[]{"Would you like some of this? Literally?"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Yes!"}),
                        new Dialogue(CatType.none, true, new string[]{"*gives food* It'll regrow and I love seeing a happy cat."}),
                    },
                      new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*gives food*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*noms appreciatively*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*gives food*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*noms like I haven't nommed in years*"}),
                    },
                };
                break;
            case ActionType.Fly:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, false, new string[]{"Can't catch me now."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, false, new string[]{"Out of sight, out of mind."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"It's time to fly!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Watch me take to the sky!"}),
                    },
                     new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Let the winds help me take flight!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Lift off!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Did you know cats can fly?"}),
                    },
                };
                break;
            case ActionType.Ghost:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, false, new string[]{"It's my time to disappear."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, false, new string[]{"Let me blend in with the shadows."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, false, new string[]{"Let me disappear."}),
                    },
                    new Dialogue[]
                    {
                         new Dialogue(CatType.none, false, new string[]{"Let me fade into the darkness."}),
                    },
                };
                break;
            case ActionType.Hug:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*hugs*"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*hugs back*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*hugs* you're so soft!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"*hugs back* you too!"}),
                    },
                };
                break;
            case ActionType.Joke:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"How many cats can fit in a box?","One; after that, the box isn't empty."}),
                    },
                      new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"What do you call a cat that lives in an igloo?","An eskimew, hehe!"}),
                        new Dialogue(speaker2.catType, false, new string[]{"Oh my."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Why did the cat run from the tree?","It was afraid of the bark!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"What do cats like to eat for breakfast?","Mice Krispies."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"What's a cat's way of keeping law & order?","Claw Enforcement."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"What do you call the cat that was caught by the police?","The purrpatrator."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"What's a cat's way of keeping law & order?","Claw Enforcement."}),
                    },
                };
                break;
            case ActionType.Magic:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"I will save the world.. and my friends!",}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Hiya! Help me, magic!",}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"I command thee, magic!",}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Magic, help me heal!",}),
                    },
                     new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Magic, help me take the pain away!",}),
                    },
                     new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Magic, heal those hurt!",}),
                    },
                };
                break;
            // case CatAction.poem:
            //     dialogues = new Dialogue[][]{
            //         new Dialogue[]
            //         {    new Dialogue(CatType.none, false, new string[]{"The problem. With being strong.", "Is no one. Helps you for long."}),
            //         },
            //         new Dialogue[]
            //         {    new Dialogue(CatType.none, false, new string[]{"In the end. We must be heroes.","Because if not us. Then who else?"}),
            //         },
            //         new Dialogue[]
            //         {    new Dialogue(CatType.none, false, new string[]{"I see a storm on the horizon. A big hurricane.","Crashing waves. And drowning rain."}),
            //         },
            //         new Dialogue[]
            //         {    new Dialogue(CatType.none, false, new string[]{"It is no surprise. I enjoy your mews.","I like to spend. Time with you."}),
            //         },
            //     };
            //     break;
            case ActionType.Story:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"You wanna hear a story?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"Uh, maybe some other time."}),
                        new Dialogue(CatType.none, false, new string[]{"Oh."}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"Let me tell you a story..",
                        "Many moons ago, all the cats were friends.",
                        "Manmade cats were wild and dramatic, Earth cats were calm and sensitive,",
                        "And Life cats were like the glue that held us all together: kind and knowing.",
                        "Once Life left, we all kind of.. fell apart.",
                        "..I hope they come back.",
                        }),
                        new Dialogue(speaker2.catType, false, new string[]{"I guess you never appreciate what you have until it's gone."}),
                    },
                     new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"Let me tell you a story..",
                        "Many moons ago, all the cats in Half Moon Island got along happily.",
                        "Manmade cats were like the inventors and scientists,",
                        "Earth cats were like the creators and builders,",
                        "And Life cats handled all the business and government stuff.",
                        }),
                        new Dialogue(speaker2.catType, false, new string[]{"What happens when the government disappears?"}),
                        new Dialogue(CatType.none, false, new string[]{"We'll have to wait to find out."}),
                    },
                    new Dialogue[]
                    {
                        new Dialogue(CatType.none, true, new string[]{"Let me tell you a story..",
                        "Once upon a time, there was a King, a Queen, a Princess and a Prince.",
                        "They all lived together.",
                        "Soon, the Princess and Prince had to move out to start their own life.",
                        "The Princess found her home in the Fields, but the Prince..",
                        "He left, then disappeared; some say he was never seen again. Everyday the King and Queen weep and pray to see him again."
                        }),
                        new Dialogue(speaker2.catType, false, new string[]{":("}),
                    },
                    new Dialogue[]
                    {
                          new Dialogue(CatType.none, true, new string[]{"Want to hear a story?"}),
                        new Dialogue(speaker2.catType, false, new string[]{"Hurray, story time!"}),
                        new Dialogue(CatType.none, false, new string[]{
                        "A long time ago, the first hooman stepped on to this island.",
                        "They brought with them a compwuuter. A big metal box that beeps.",
                        "The hooman left the Island, but not before leaving the compwuuter somewhere on the island, beeping away.",
                        "Years later, the catless Half-Moon Island began having all types of strange cats, never seen before.",
                        "Apparently, we all came from that compwuuter! Crazy, isn't it. I don't believe it.",
                        }),
                        new Dialogue(speaker2.catType, false, new string[]{"Wow, that's spoopy."}),
                    },
                };
                break;
            case ActionType.Teach:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Hello I'd like to teach you about something important!",
                    "Sometimes you will see big metal boxes in all sizes and colors!", "They are <i><b>doom machines</b></i> and they are deadly on impact!",
                    "Avoid them at all costs!"}),
                        new Dialogue(speaker2.catType, true, new string[]{"Ooooooh, that's what they are; doom machines.", "Doom machines go vroom vroom."}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Listen up cats, I'm going to teach you how to cat.",
                    "Step 1: Humans belong to us.", "Step 2: Everything else on this planet also belongs to us.",}),
                        new Dialogue(speaker2.catType, true, new string[]{"Hear ye, hear ye!"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"Ok class, listen now.",
                    "I'm here to teach you about the red dot.","I'm sorry to say, but the red dot is a lie."}),
                        new Dialogue(speaker2.catType, true, new string[]{"Not the red dot!!!!"}),
                    },
                };
                break;
            case ActionType.Think:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about fish*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about favorite hooman*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about red dot*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about dogs and their constant barking*","*sometimes I wish I could bark*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about flat, pushable objects*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*heavy breathing*"}),
                    },
                    new Dialogue[]
                    {    new Dialogue(CatType.none, true, new string[]{"*thinks about best friend cat and how much I love them*"}),
                    },
                };
                break;
            default:
                dialogues = new Dialogue[][]{
                    new Dialogue[]
                    {
                    },
                };
                break;
        }
        return dialogues[UnityEngine.Random.Range(0, dialogues.Length)];
    }
    #endregion

    #region MAP ----
    public static Color32 getDisabledWorldColor(WorldType world)
    {
        switch (world)
        {
            case WorldType.city:
                return new Color32(165, 129, 188, 255);
            case WorldType.computer:
                return new Color32(102, 89, 129, 255);
            case WorldType.fields:
                return new Color32(107, 166, 74, 255);
            case WorldType.ocean:
                return new Color32(27, 199, 236, 255);
            case WorldType.sky:
                return new Color32(233, 240, 255, 255);
            case WorldType.space:
                return new Color32(55, 57, 74, 255);
            case WorldType.woods:
                return new Color32(63, 113, 55, 255);
        }
        return Color.grey;
    }
    #endregion
}
