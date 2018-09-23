using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ExploreController : MonoBehaviour
{
    public const WorldType lastWorld = WorldType.space;
    public const int lastLevel = 0;
    [Header("Prefabs")]
    public GameObject catGameObjectPrefab;
    public GameObject exploreNotif;
    [Header("Helpers")]
    public ExploreEnd exploreEnd;
    public ExploreStory exploreStory;
    public ExploreChoice exploreChoice;

    public ExplorePlayer userPlayer;
    public ExplorePlayer enemyPlayer;

    public AudioClip melee, projectile;
    public RuntimeAnimatorController RightAnimator;
    public static readonly Vector2 userPos = new Vector2(-6.5f, -1);
    public static readonly Vector2 enemyPos = new Vector2(6.5f, -1);

    [HideInInspector]
    public StageAsset stage;
    [HideInInspector]
    public static int level;
    [HideInInspector]
    public static WorldType world;
    [HideInInspector]
    public static bool justWon = false;


    public void Notify(string[] notifications, Action onComplete)
    {
        GameControl.control.getSoundManager().playNotif();
        ExploreNotif n = GameObject.Instantiate(exploreNotif, transform.root, false).GetComponent<ExploreNotif>();
        if (n == null)
        {
            return;
        }
        n.InitNotify(notifications, onComplete);
    }

    private void Awake()
    {
        if (level == -1)
        {
            //random stage
            stage = DataUtils.getRandomStage(world, GameControl.control.getWorldLevel(world));
        }
        else
        {
            stage = Instantiate(Resources.Load<StageAsset>("LevelAssets/" + world.ToString() + "/" + world.ToString() + level));
            if (wonLastLevel())
            {
                stage = DataUtils.WonLastLvlStage(stage, world, level);
            }
        }

        GameControl.control.GetComponent<Canvas>().enabled = false;
        GameControl.control.GetComponent<GraphicRaycaster>().enabled = true;

        setBackground();
        PlayerData enemyPlayerData = getEnemyData();

        userPlayer = gameObject.AddComponent<ExplorePlayer>();

        enemyPlayer = gameObject.AddComponent<ExplorePlayer>();

        float CatScale = GameControl.control.playerData.team.Count > enemyPlayerData.team.Count ?
        getCatScale(GameControl.control.playerData.team.Count) : getCatScale(enemyPlayerData.team.Count);

        userPlayer.init(GameControl.control.playerData, enemyPlayer, catGameObjectPrefab, CatScale, userPos, Vector3.zero, checkGameOver);
        enemyPlayer.setAsEnemy(userPlayer);
        enemyPlayer.init(enemyPlayerData, userPlayer, catGameObjectPrefab, CatScale, enemyPos, new Vector3(0, 180, 0), checkGameOver);

        setUpStart();


        MapManager.StageTransition(1, 0, world, () =>
         {

             if (level == -1)
             {
                 string[] randomEncounters = new string[]{
                    enemyPlayerData.team[0].Name + " is happy to see you!",
                    enemyPlayerData.team[0].Name + " is excited to see you!",
                     enemyPlayerData.team[0].Name + " stops you to say hi!",
                      enemyPlayerData.team[0].Name + " wants to play with you!",
                     enemyPlayerData.team[0].Name + " interrupts your journey!",
                      enemyPlayerData.team[0].Name + " confronts you!",
                      "You've been stopped by "+   enemyPlayerData.team[0].Name + "!",
                       enemyPlayerData.team[0].Name + " saw you and wants to play!",
                 };
                 Notify(new string[]{
                     randomEncounters[UnityEngine.Random.Range(0, randomEncounters.Length)]
                 }, Init);
                 return;
             }
             Init();

         });
    }

    IEnumerator Start()
    {
        yield return null;
        GetComponent<DynamicScaler>().enabled = true;
    }
    private void Init()
    {
        Debug.Log("STAGE LEVEL: " + level + " beat level " + GameControl.control.getWorldLevel(world));
        //if random level
        if ((level == -1 ||
        //if never won level
        GameControl.control.getWorldLevel(world) <= level ||
        //if won last level
        wonLastLevel()
        )
         && stage.beforeDialogues != null && stage.beforeDialogues.Length > 0)
        {
            InitStory();
        }
        else
        {
            stage.Responses = null;
            InitAdvantages();
        }
    }

    private float getCatScale(int numCats)
    {
        switch (numCats)
        {
            case 1:
                return 0.9f;
            case 2:
                return 0.85f;
            case 3:
                return 0.81f;
            case 4:
                return 0.76f;
            default:
                return 1;
        }
    }

    private void setBackground()
    {
        GameObject.FindGameObjectWithTag("Background").GetComponent<Image>().sprite =
          stage.background;
    }
    private void setUpChoice()
    {
        //setting enemy Event Triggers and layers logic, and enabling healthBars

        int enemyLayer = enemyPlayer.getLayer(true);

        foreach (ExploreCat cat in userPlayer.aliveCats)
        {
            cat.onHit = OnHit;
            cat.healthBar.gameObject.SetActive(true);
            //meow
            cat.transform.GetChild(0).GetComponent<RandomCatNoises>().enableNoises = true;
        }

        foreach (ExploreCat cat in enemyPlayer.aliveCats)
        {
            if (enemyPlayer.aliveCats.Count > 1)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => exploreChoice.setFight(cat));
                cat.gameObject.AddComponent<EventTrigger>().triggers.Add(entry);
            }
            cat.onHit = OnHit;
            //health
            cat.healthBar.gameObject.SetActive(true);
            //physics
            cat.gameObject.layer = enemyLayer;
            //meow
            cat.transform.GetChild(0).GetComponent<RandomCatNoises>().enableNoises = true;
        }

        //test if need tutorial
        if (world == WorldType.house && level == 0 && !GameControl.control.getWorldLevelHasPlayed(world, level))
        {
            GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/tutorialExp"), transform, false).GetComponent<TutorialExplore>().Init(this);
        }
    }

    private void InitStory()
    {
        exploreStory.Finished += InitAdvantages;
        exploreStory.InitStory(stage.beforeDialogues, userPlayer.allCats[0], enemyPlayer.allCats);
    }
    private void InitAdvantages()
    {

        setUpChoice();
        InitPlayerAdvantage(0, userPlayer, () => InitPlayerAdvantage(0, enemyPlayer, InitChoices));
    }
    private void InitChoices()
    {
        exploreChoice.Init(this);
    }
    //check if any cat has advantage in the field due to their faction; if so, notify and add
    public void InitPlayerAdvantage(int i, ExplorePlayer player, Action onComplete)
    {
        for (; i < player.aliveCats.Count; i++)
        {
            if (DataUtils.isFactionCompatible(player.aliveCats[i].cat.getCatAsset().faction, world))
            {
                player.aliveCats[i].factionAdvantage();
                Notify(getAdvantageString(player.aliveCats[i].cat.Name, world.ToString()),
                () => InitPlayerAdvantage(i + 1, player, onComplete));
                return;
            }
        }
        if (onComplete != null)
        {
            onComplete();
        }
    }
    private string[] getAdvantageString(string name, string world)
    {
        string[] rand;
        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                rand = new string[] { name + " feels stronger in the " + world + "!" };
                break;
            case 1:
                rand = new string[] { name + " feels energized by their love for the " + world + "!" };
                break;
            case 2:
            default:
                rand = new string[] { name + " grows stronger in the " + world + "!" };
                break;
        }
        return rand;
    }
    PlayerData getEnemyData()
    {
        PlayerData playerData = new PlayerData();
        int totalLevels = DataUtils.getTotalLevels();
        int progressThroughWorld = MathUtils.progressThroughWorld();
        for (int i = 0; i < stage.enemyCats.Length; i++)
        {
            Cat enemyCat = new Cat(stage.enemyCats[i]);
            uint level = MathUtils.FairEnemyCatLevel(totalLevels, progressThroughWorld);
            if (enemyCat.getCatAsset().rarity > 3)
            {
                Debug.Log("level before: " + level);
                level = (uint)Mathf.Clamp(level + 5, 0, 80);
                Debug.Log("level after: " + level);
            }
            enemyCat.catLvl = new CatLevel(level);
            playerData.team.Add(enemyCat);
        }
        return playerData;
    }


    public void playSound(AttackType a)
    {
        if (a == AttackType.Melee)
        {
            GameControl.control.getSoundManager().sfx.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            GameControl.control.getSoundManager().playOneShot(melee, UnityEngine.Random.Range(0.8f, 1.2f));
        }
        else if (a == AttackType.Ranged)
        {
            GameControl.control.getSoundManager().sfx.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            GameControl.control.getSoundManager().playOneShot(projectile, UnityEngine.Random.Range(0.8f, 1.2f));
        }
    }

    //always used for game over, unless game finished via a CatAction
    public void checkGameOver(ExplorePlayer p)
    {
        if (p.aliveCats.Count == 0)
        {
            Debug.Log("set active false");
            exploreChoice.gameObject.SetActive(false);
            exploreChoice.selector.gameObject.SetActive(false);
            GameOver(p.enemy);
            // return false;
        }
        else if (exploreChoice != null && !exploreChoice.gameObject.activeSelf)
        {
            exploreChoice.gameObject.SetActive(true);
        }
    }
    public void OnHit(AttackType a, ExploreCat attacker)
    {
        playSound(a);
        attacker.Disable();
        if (attacker.owner.enemy)
        {
            ExploreCat user = exploreChoice.getCurrentUser();
            if (user != null && user.enabled)
            {
                user.Disable();
            }
        }
        else
        {
            ExploreCat e = exploreChoice.getCurrentEnemy();
            if (e != null)
            {
                e.Disable();
            }
        }

        Debug.Log("try to set active true");
        if (!Finished() && !exploreChoice.gameObject.activeSelf)
        {
            Debug.Log("set active true");
            StartCoroutine(enableChoice());
        }
    }
    IEnumerator enableChoice()
    {
        yield return null;
        if (exploreChoice != null )
        exploreChoice.gameObject.SetActive(true);
    }
    // IEnumerator openChoice()
    // {
    //     yield return new WaitForSeconds(0.2f);
    //     Debug.Log("set active true");
    //     if (!Finished())
    //     {
    //         Debug.Log("set active true2");
    //         exploreChoice.gameObject.SetActive(true);
    //     }
    // }
    public void AcceptedGameOver(ActionType action, Cat actioner, Cat enemy)
    {
        string[] randomAcceptance = new string[]{
    enemy.Name + " looks happy with " + actioner.Name + "'s "+action.ToString() + " and backs off.",
    enemy.Name + " seems satisfied with " + actioner.Name + "'s "+action.ToString() + ".",
    enemy.Name + " glows with happiness from " + actioner.Name + "'s "+action.ToString() + ".",
    enemy.Name + " rests easy with " + actioner.Name + "'s "+action.ToString() + ".",
    enemy.Name + " looks at " + actioner.Name + " with care and affection after their "+action.ToString() + ".",
    enemy.Name + " looks more loving towards " + actioner.Name + " after the "+action.ToString() + ".",
    actioner.Name + "'s "+action.ToString() + " seems to lift a burden off of "+ enemy.Name+ "'s paws.",
        };
        Notify(new string[]{
            randomAcceptance[UnityEngine.Random.Range(0, randomAcceptance.Length)]
            }, () => GameOver(true, true));
    }
    public void GameOver(bool won)
    {
        GameOver(won, false);
    }

    public bool Finished()
    {
        return exploreEnd.isActiveAndEnabled || exploreStory == null || exploreStory.isActiveAndEnabled;
    }

    private ushort getRating()
    {
        float team = GameControl.control.playerData.team.Count;
        float alive = team <= 2 ? userPlayer.aliveCats.Count + 0.5f : userPlayer.aliveCats.Count;
        ushort r = (ushort)(Mathf.Clamp(alive / team * LevelButton.MAX_STARS, 1, 3));
        return r;
    }

    public void GameOver(bool won, bool wonByAction = false)
    {
        if (Finished())
        {
            Debug.LogWarning("duplicate call to game over!");
            return;
        }
        ushort rating = won ? getRating() : (ushort)1;
        bool neverWonLvl = GameControl.control.getWorldLevel(world) <= level;

        if (won)
        {
            justWon = true;
        }

        if ((level == -1 ||
        //if never won level
       neverWonLvl ||
        //if won last level
        wonLastLevel()
        )
         && stage.afterDialogues != null && stage.afterDialogues.Length > 0 && won)
        {
            exploreStory.Finished += () =>
            {
                showGameOverUI(won, wonByAction, rating, neverWonLvl);
            };
            Debug.Log("init end story");
            exploreStory.InitStory(stage.afterDialogues, userPlayer.allCats[0], enemyPlayer.allCats);
        }
        else
        {
            showGameOverUI(won, wonByAction, rating, neverWonLvl);

        }
    }
    private void showGameOverUI(bool won, bool wonByAction, ushort rating, bool neverWonLvl)
    {
        Destroy(exploreChoice.gameObject);
        Destroy(exploreStory.gameObject);
        if (isLastLevel() && won && neverWonLvl)
        {
            exploreEnd.wonLastLevel(this, wonByAction, catGameObjectPrefab, rating);
        }
        else
        {
            exploreEnd.init(won, this, wonByAction, rating);
        }
        StartCoroutine(setData(won, rating));
    }

    private IEnumerator setData(bool won, ushort rating)
    {
        Debug.Log("set data");
        if (level == -1)
        {
            yield break;
        }

        yield return null;
        if (won)
        {
            Debug.Log("heya");
            GameControl.control.setWorldLevel(world, level + 1, level, true, DataUtils.loadLevelAsset(world).stageNames.Length);
            GameControl.control.setWorldRating(world, level, rating);//leftover cats / original Cats * 3, rounded up
        }
        else
        {
            GameControl.control.setWorldLevel(world, level, level, false, DataUtils.loadLevelAsset(world).stageNames.Length);
        }
        GameControl.control.SavePlayerData();
    }

    public static bool wonLastLevel()
    {
        return GameControl.control.getWorldLevel(lastWorld) > lastLevel;
    }
    private bool isLastLevel()
    {
        return level == lastLevel && world == lastWorld;
    }
    #region BUTTON LISTENERS ----
    public void buttonClick()
    {
        GameControl.control.getSoundManager().playButton();
    }
    public void errorClick()
    {
        GameControl.control.getSoundManager().playError();
    }

    #endregion

    // private bool hasPlayerWon()
    // {
    //     if (enemyPlayer.getNumLivingCats() == 0)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // private bool hasPlayerLost()
    // {
    //     if (userPlayer.getNumLivingCats() == 0)
    //     {
    //         return true;
    //     }
    //     return false;
    // }


    private void sideEffectReact(SideEffect effect, ExploreCat effectee)
    {
        string[] effectNotifs = null;
        string secondstring = null;
        float damage = 0;
        // if (cat.owner.enemy)
        // {
        //     exploreChoice.getCurrentUser().Disable();
        // }
        switch (effect.effect)
        {
            case Fx.Love:
                effectNotifs = new string[] {
                    effectee.cat.Name + " loves their foe too much to focus!",
                    effectee.cat.Name + " likes their foe so much that they refuse to battle.",
                    effectee.cat.Name + " is too distracted by their love for their foe to pay attention!",
                    effectee.cat.Name + " is too busy feeling adoration for their foe!",
                    effectee.cat.Name + " wants to make friends, not war!",
                };
                if (UnityEngine.Random.value > 0.6f)
                {
                    damage = effectee.realizedStats.maxHealth * 0.025f;
                    secondstring = effectee.cat.Name + " feels hurt at the thought of fighting!";
                }
                break;
            case Fx.Confused:
                effectNotifs = new string[] {
                    effectee.cat.Name + " is too confused by the "+effect.effector.ToString().ToLower()+" to fight!",
                    effectee.cat.Name + " is too bewildered by the "+effect.effector.ToString().ToLower()+" to attack!",
                    effectee.cat.Name + " is caught off guard by the "+effect.effector.ToString().ToLower()+"!",
                    effectee.cat.Name + " is confused; they've never seen a "+effect.effector.ToString().ToLower()+" before!",
                 };
                if (UnityEngine.Random.value > 0.75f)
                {
                    damage = effectee.realizedStats.maxHealth * 0.05f + 1;
                    secondstring = effectee.cat.Name + " got hurt in their confusion!";
                }
                break;
            case Fx.Distracted:
                effectNotifs = new string[] {
                     effectee.cat.Name + " is too distracted by the "+effect.effector.ToString().ToLower()+" to attack!",
                    effectee.cat.Name + " bats the "+effect.effector.ToString().ToLower()+" happily, forgetting to attack!",
                    effectee.cat.Name + " is too busy playing with the "+effect.effector.ToString().ToLower()+" to attack!",
                    effectee.cat.Name + " ignores the fight to paw at the "+effect.effector.ToString().ToLower()+"!",
                 };
                break;
            case Fx.Scared:
                effectNotifs = new string[] {

                    effectee.cat.Name + "'s too scared of the "+effect.effector.ToString().ToLower()+" to move!",
                       effectee.cat.Name + " is frozen in fear of the "+effect.effector.ToString().ToLower()+"!",
                    effectee.cat.Name + " is too frightened of the "+effect.effector.ToString().ToLower()+" to do anything!",
                };
                if (UnityEngine.Random.value > 0.45f)
                {
                    damage = effectee.realizedStats.maxHealth * 0.05f + 2;
                    secondstring = effectee.cat.Name + " got hurt in their fright!";
                }
                break;
        }
        if (effectNotifs != null)
        {
            if (effectee.currentHealth <= 0)
            {
                effectee.onHit(AttackType.None, effectee);
                return;
            }
            if (effectee.owner.enemy)
            {
                exploreChoice.getCurrentUser().Disable();
            }

            string[] notifs = (secondstring == null) ? new string[] {
                effectNotifs[UnityEngine.Random.Range(0, effectNotifs.Length)]
            } : new string[] {
                effectNotifs[UnityEngine.Random.Range(0,effectNotifs.Length)],
                secondstring
            };

            Notify(notifs, () =>
            {
                effectee.onHit(AttackType.None, effectee);
                if (damage > 0)
                {
                    effectee.takeDamage(damage);
                }
            });
            effectee.setEffect();
        }
    }
    private void setUpStart()
    {
        //  uint catLayer = 0;
        Shader catSprite = Shader.Find("Sprite");

        foreach (ExploreCat cat in enemyPlayer.aliveCats)
        {
            setCatMaterial(cat, catSprite);
            cat.setAnimator(RightAnimator);
        }
        foreach (ExploreCat cat in userPlayer.aliveCats)
        {
            setCatMaterial(cat, catSprite);
        }
        RightAnimator = null;
    }

    private void setCatMaterial(ExploreCat cat, Shader catSprite)
    {
        Material newMaterial = new Material(catSprite);
        SpriteRenderer[] spriteRenderers = cat.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = newMaterial;
        }
        cat.spriteMaterial = newMaterial;
        cat.onEffect = sideEffectReact;
    }

}
