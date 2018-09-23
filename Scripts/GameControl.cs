using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Net;
using SimpleFirebaseUnity;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class GameControl : MonoBehaviour
{

    public static string datapath;
    public const string starStr = "<sprite=0 color=#ffdede>";
    public const string GREY_HEX = "#515154";
    public static GameControl control;
    public PlayerData playerData;
    public TextMeshProUGUI silver;
    public TextMeshProUGUI gold;
    private float lastTime;

    [Header("Prefabs")]
    public GameObject notificationPrefab;

    #region FIREBASE ----
    private Firebase firebase;
    public Firebase getFirebase()
    {
        if (firebase == null)
        {
            firebase = Firebase.CreateNew(
               //TYPE IN YOUR REAL-TIME DATABASE LINK HERE - e.g. "mybase.firebaseio.com"
                ).Child("users").Child(playerData.uid);
        }
        return firebase;
    }
    public void setFirebaseNull()
    {
        firebase = null;
    }
    #endregion

    #region GET THINGS: SOUND, MAINUI, BACKBUTTON
    private SoundManager SoundManager;
    public SoundManager getSoundManager()
    {
        if (SoundManager == null)
        {
            SoundManager = GetComponent<SoundManager>();
        }
        return SoundManager;
    }

    [Header("UI")]

    private Button backButton;
    public Button getBackButton()
    {
        if (backButton == null)
        {
            backButton = transform.Find("UI").Find("Back").GetComponent<Button>();
        }
        return backButton;
    }

    public MainUI mainUI;
    public MainUI getMainUI()
    {
        if (mainUI == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                mainUI = player.GetComponent<MainUI>();
            }
        }
        return mainUI;
    }
    // private bool saving;
    #endregion

    public void setControlNull()
    {
        Destroy(getSoundManager().sfx.gameObject);
        StopAllCoroutines();
        control = null;
        Destroy(gameObject);
    }

    void Awake()
    {
        datapath = Application.persistentDataPath + "/playerdata.json";

        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(getSoundManager().sfx.gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(getSoundManager().sfx.gameObject);
            Destroy(gameObject);
            return;
        }
        LoadPlayerData();
    }
    private void Start()
    {
        // playerData.loggedIn = true;
        // WorldLevel WL;
        // if (playerData.WorldLocks.TryGetValue(WorldType.space, out WL) == false)
        // {
        // }
        // else
        // {
        //     WL.level = 1;
        //     WL.hasPlayed = 0;
        //     // WL.dailyPlays = new uint[1];

        // }

        // playerData.WorldLocks[WorldType.space] = new WorldLevel(0,-1, new uint[1], new ushort[1]);

        // AddToDeck(new Cat(CatType.d));
        // AddToDeck(new Cat(CatType.business));
        // AddToDeck(new Cat(CatType.nerd));
        // AddToDeck(new Cat(CatType.phat));
        // AddToDeck(new Cat(CatType.sleepy));
        // AddToDeck(new Cat(CatType.sprout));
        // AddToDeck(new Cat(CatType.artist));
        // AddToDeck(new Cat(CatType.pocky));
        // AddToDeck(new Cat(CatType.head3));
        // AddToDeck(new Cat(CatType.unicorn));
        // AddToDeck(new Cat(CatType.bot));
        // AddToDeck(new Cat(CatType.wood));
        // AddToDeck(new Cat(CatType.ice));
        // AddToDeck(new Cat(CatType.business));
        // AddToDeck(new Cat(CatType.pocky));
        // AddToDeck(new Cat(CatType.artist));
        // IncrementSilver(100000000);
        // IncrementGold(10000);
        // MathUtils
        checkNewDay();
        // checkLoggedIn();
        setFrameRate();
        SetDragThreshold();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        if (next.name == "Main")
        {
            checkForReward(ExploreController.justWon);
            ExploreController.justWon = false;
        }
    }

    private void setFrameRate()
    {
#if UNITY_IOS || UNITY_ANDROID
        Application.targetFrameRate = 30;
#elif !UNITY_WEBGL
        Application.targetFrameRate = 40;
#endif
    }
    private void SetDragThreshold()
    {
        // #if UNITY_WEBGL

        // #else
        int defaultValue = EventSystem.current.pixelDragThreshold;
        EventSystem.current.pixelDragThreshold =
                Mathf.Max(
                     defaultValue,
                     (int)(defaultValue * Screen.dpi / 160f));
        // #endif
    }

    #region FIREBASE ------ 
    public void getUser(Action<FirebaseParam> onComplete, bool silent, bool returnIfNull = false)
    {

        getIdToken((string idToken) =>
        {
            if (returnIfNull && idToken == null)
            {
                return;
            }

            FirebaseParam user = new FirebaseParam();
            if (silent)
            {
                user.PrintSilent();
            }
            if (idToken != null)
            {
                user.Auth(idToken);
            }
            // Debug.LogWarning("testing httpheader");
            // Debug.LogWarning(user.HttpHeader.ToString());
            onComplete(user);
        });
    }
    public void PostRequest(WWWForm form, string url, Action<UnityWebRequest, bool> onComplete)
    {
        StartCoroutine(PostRequestWait(form, url, onComplete));
    }
    private IEnumerator PostRequestWait(WWWForm form, string url, Action<UnityWebRequest, bool> onComplete)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url + FirebaseLogin.API_KEY, form))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                onComplete(www, false);
            }
            else
            {
                onComplete(www, true);
            }
        }
    }
    public void getIdToken(Action<string> onComplete)
    {
        if (string.IsNullOrEmpty(playerData.refreshToken))
        {
            onComplete(null);
        }

        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", playerData.refreshToken);
        PostRequest(form, "https://securetoken.googleapis.com/v1/token?key=", (UnityWebRequest www, bool success) =>
                 {
                     if (!success)//username is taken or not allowed
                     {
                         Debug.LogWarning("getIdToken not successful: " + www.error);
                         onComplete(null);
                         return;
                     }
                     onComplete(JsonConvert.DeserializeObject<Dictionary<string, string>>(www.downloadHandler.text)["id_token"]);
                 });
    }
    // private void OnApplicationFocus(bool focusStatus)
    // {
    //     if (focusStatus)
    //     {
    //         checkLoggedIn();
    //     }

    // }
    // private void checkLoggedIn()
    // {
    //     if (!string.IsNullOrEmpty(playerData.refreshToken) && Application.internetReachability != NetworkReachability.NotReachable)
    //     {
    //         setLoggedIn(true);
    //     }
    //     else
    //     {
    //         setLoggedIn(false);
    //     }
    // }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            checkNewDay();
        }
    }
    #endregion

    public void SavePlayerData()
    {
        Debug.LogWarning("save!");
        string Json = JsonUtility.ToJson(playerData);
        if (playerData.loggedIn && Application.internetReachability != NetworkReachability.NotReachable)
        {
            GameControl.control.getUser((FirebaseParam user) =>
            {
                getFirebase().SetValue(Json, true, user);
            }, true, true);
        }
        File.WriteAllText(datapath, Json);
    }
    private void LoadPlayerData()
    {
        if (File.Exists(datapath))
        {
            playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(datapath));
            // Debug.Log(File.ReadAllText(datapath));
            UpdatePlayerUI();
        }
        else
        {
            playerData = new PlayerData();
            playerData.defPlayerData();
            Action onComplete = () =>
              {

                  playerData.EndTutorialData();
                  UpdatePlayerUI();
              };
            Tutorial t = GameObject.Find("CatManager").transform.Find("MainMenuCat").gameObject.AddComponent<Tutorial>();
            t.StartTutorial(onComplete);
        }
    }
    public void loadMain()
    {
        getSoundManager().playMainMusic(() => { SceneManager.LoadScene(0); });
    }
    public void UpdatePlayerUI()
    {
        silver.text = playerData.silver.ToString();
        Debug.Log("setting silver to " + playerData.silver + " and gold to " + playerData.gold);
        gold.text = playerData.gold.ToString();
    }


    public void BackButton()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Main")
        {
            if (getMainUI() != null)
            {
                getMainUI().detailCard.deckCat.gameObject.SetActive(false);
                getMainUI().detailCard.gameObject.SetActive(false);
                getMainUI().deck.transform.parent.gameObject.SetActive(false);
                getMainUI().mainButtons.SetActive(true);
                getMainUI().gacha.gameObject.SetActive(false);
                checkForReward(false);
            }
        }
        else if (sceneName == "Map")
        {
            if (ExploreController.level == -1)
            {
                //cat is walking
                Vector2 pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                playerData.lastPos = new Vector2Ser(pos.x, pos.y);
                GameControl.control.SavePlayerData();
            }
            loadMain();
        }
        getSoundManager().playNotif();
        getBackButton().gameObject.SetActive(false);

    }

    private void checkForReward(bool fromWonExplore)
    {
        Debug.Log("check for reward! Time.time: " + Time.time + " lastTime: " + lastTime + " fromWonExplore:" + fromWonExplore);
        if (Time.time > lastTime + 500 && UnityEngine.Random.value > 0.75f) //else if 500 seconds have passed
        {
            if (fromWonExplore)
            {
                RandomRewards rr = GetComponent<RandomRewards>();
                rr.RewardPrompt(rr.RandomWonExploreReward());
                lastTime = Time.time;
                //won explore reward
            }
            else
            {
                //random reward
                RandomRewards rr = GetComponent<RandomRewards>();
                rr.RewardPrompt(rr.RandomMainReward());
                lastTime = Time.time;
            }
        }
    }

    public void CoinShopNoReturn()
    {
        Transform shop = Instantiate(Resources.Load<GameObject>("miscPrefabs/CoinShop"), transform, false).transform;
#if !UNITY_IOS && !UNITY_ANDROID
        Transform child = shop.GetChild(0);
        Destroy(child.Find("watch").gameObject);
        RectTransform rectTransform = (RectTransform)child.Find("s4g");
        rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
#endif
        shop.SetAsFirstSibling();
    }

    public static string GenerateStarText(uint s, bool showLeft)
    {
        string stars = "";
        int i = 0;
        for (; i < s; i++)
        {
            stars += starStr;
        }
        if (showLeft)
        {
            for (; i < 4; i++)
            {
                stars += "<sprite=0 color=" + GREY_HEX + ">";
            }
        }
        return stars;
    }

    public static TextMeshProUGUI GetTextBox(GameObject obj, string value)
    {
        return obj.transform.Find(value).gameObject.GetComponent<TextMeshProUGUI>();
    }
    public static TextMeshProUGUI GetTextBox(Transform obj, string value)
    {
        return obj.Find(value).gameObject.GetComponent<TextMeshProUGUI>();
    }



    public void checkNewDay()
    {
        //  if new day [a way to make it not dependent on phone calendar which can be changed],
        //  then reset daily limit
        StartCoroutine(isNewDay((bool yes) =>
        {
            if (yes)
            {
                {
                    List<WorldType> keys = new List<WorldType>(playerData.WorldLocks.Keys);
                    foreach (WorldType key in keys)
                    {
                        playerData.WorldLocks[key].dailyPlays = new uint[playerData.WorldLocks[key].dailyPlays.Length];
                    }
#if UNITY_IOS || UNITY_ANDROID
                    if (playerData.videosLeft == 0)
                    {
                        playerData.videosLeft = W2EManager.maxVideos;
                    }
#endif
                    if (UnityEngine.Random.value > 0.2f)
                    {
                        //welcome back reward
                        GetComponent<RandomRewards>().InitRandomWelcomeReward();
                    }
                    SavePlayerData();
                }
            }
        }));

    }

    private IEnumerator isNewDay(Action<bool> onComplete)
    {
        DateTime now;


        // Debug.Log("hi");
        using (UnityWebRequest response = UnityWebRequest.Head("https://www.google.com"))
        {
            yield return response.SendWebRequest();
            try
            {
                if (response.isNetworkError || response.isHttpError)
                {
                    Debug.LogWarning(response.error);
                    throw new Exception();
                }

                // Debug.Log(response.downloadHandler.text);     //string todaysDates =  response.Headers["date"];
                now = DateTime.ParseExact(
                    response.GetResponseHeader("date"),
                    //JsonConvert.DeserializeObject<Dictionary<string, object>>(response.downloadHandler.text)["date"] as string,
                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat,
                    System.Globalization.DateTimeStyles.AssumeUniversal).Date;
            }
            catch
            {
                now = DateTime.Today;
            }
        }

        DateTime lastDay = DateTime.ParseExact(playerData.lastDay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        Debug.Log("today - " + now.ToString() + "..lastDay - " + lastDay.ToString());
        if (now > lastDay)
        {
            playerData.lastDay = DateTime.Today.ToString("yyyy-MM-dd");
            onComplete(true);
            yield break;
        }
        if ((lastDay - now).Days > 1)
        {
            Notification n = Notify("Your clock has reversed since your last play session. The game will be locked until the date of your last play session: " + playerData.lastDay, transform, null, true);
            n.text.rectTransform.offsetMin = new Vector2(-n.text.rectTransform.offsetMax.x, 140.6f);
            Destroy(n.ok.gameObject);
        }
        onComplete(false);
    }

    public void checkDeckAvailability(Transform notify, Action<bool> onComplete, string add = "")
    {
        if (playerData.maxCats <= playerData.deck.Count)
        {
            uint EightSlotsPrice = playerData.maxCats;
            getSoundManager().playError();
            YesNoPrompt(add + "No more room in deck! Upgrade from " + playerData.maxCats + " to " + (playerData.maxCats + 8) + " slots for " + CatIAP.goldStr + EightSlotsPrice + "?",
           notify, () =>
              {
                  if (playerData.gold < EightSlotsPrice)
                  {
                      NotEnoughGoldPrompt(notify, () =>
                      {
                          if (playerData.gold < EightSlotsPrice)
                          {
                              onComplete(false);
                          }
                          else
                          {
                              onComplete(true);
                          }
                          //AFTER QUITE A STRUGGLE -SUCCESS!!!
                      });
                      return;
                  }
                  playerData.maxCats = playerData.maxCats + 8;

                  onComplete(true);

                  DecrementGold(EightSlotsPrice);

                  if (getMainUI() != null)
                  {
                      getMainUI().deck.setDeckUpgrade();
                  }
                  //SUCCESS!!
              }, () =>
              {
                  onComplete(false);
              });

        }
        else
        {
            onComplete(true);
        }
    }

    public DeckCard AddToDeck(Cat cat, bool showOnUI = true)
    {
        playerData.deck.Add(cat);
        if (showOnUI)
        {
            MainUI mainUI = getMainUI();
            if (mainUI != null)
            {
                return mainUI.deck.AddToDeck(cat, true);
            }
        }
        return null;
    }

    public void RemoveFromDeck(Cat cat)
    {
        getMainUI().deck.RemoveFromDeck(cat);
        playerData.deck.Remove(cat);
        RemoveFromTeam(cat);
    }

    public bool RemoveFromTeam(Cat cat)
    {
        bool ret = playerData.team.Remove(cat);
        return ret;
    }
    public void IncrementSilver(uint silver)
    {
        playerData.silver += silver;
        UpdatePlayerUI();
    }

    public void DecrementSilver(uint silver)
    {
        silver = (uint)Mathf.Clamp(silver, 0, playerData.silver);
        playerData.silver -= silver;
        UpdatePlayerUI();
    }

    public void SetGold(uint gold)
    {
        playerData.gold = gold;
        SavePlayerData();
        UpdatePlayerUI();
    }

    public void IncrementGold(uint gold)
    {
        playerData.gold += gold;
        SavePlayerData();
        UpdatePlayerUI();
    }

    public void DecrementGold(uint gold)
    {
        gold = (uint)Mathf.Clamp(gold, 0, playerData.gold);
        playerData.gold -= gold;
        SavePlayerData();
        UpdatePlayerUI();
    }

    public void setUser(string refreshToken, string email, string uid)
    {
        playerData.refreshToken = refreshToken;
        playerData.email = email;
        playerData.uid = uid;
    }

    public void setLoggedIn(bool loggedIn)
    {
        playerData.loggedIn = loggedIn;
    }

    public void setWorldLevel(WorldType world, int level, int played, bool won, int maxLevels)
    {
        Debug.Log("set world: " + world + " / level: " + level + " / played: " + played + " / max levels: " + maxLevels);
        if (level < 0)
        {
            return;
        }

        WorldLevel WL;

        if (playerData.WorldLocks.TryGetValue(world, out WL) == false)
        {
            uint[] daily = new uint[maxLevels];
            WL = new WorldLevel(
                level,
                -1,
                daily,
                new ushort[maxLevels]);
        }
        else
        {
            WL.level = level > WL.level ? level : WL.level;
            WL.hasPlayed = played > WL.hasPlayed ? played : WL.hasPlayed;
        }

        if (won)
        {
            WL.dailyPlays[level - 1] += 1;
        }

        playerData.WorldLocks[world] = WL;
        //if last level of the world..
        if (level == maxLevels)
        {
            //unlock connected worlds
            foreach (WorldType w in DataUtils.getConnectedWorlds(world))
            {
                setWorldLevel(w, 0, -1, false, DataUtils.loadLevelAsset(w).stageNames.Length);
            }
        }
    }
    public void setWorldRating(WorldType world, int level, ushort rating)
    {
        if (level < 0)
        {
            return;
        }
        WorldLevel WL;
        if (playerData.WorldLocks.TryGetValue(world, out WL) == false)
        {
            return;
        }
        else
        {
            WL.starRating[level] = rating;
            playerData.WorldLocks[world] = WL;
        }

    }

    public ushort getWorldRating(WorldType world, int level)
    {
        WorldLevel WL;
        ushort zero = 0;
        return playerData.WorldLocks.TryGetValue(world, out WL) == true ? WL.starRating[level] : zero;
    }
    public int getWorldLevel(WorldType world)
    {
        WorldLevel WL;
        return playerData.WorldLocks.TryGetValue(world, out WL) == true ? WL.level : -1;
    }
    public bool getWorldLevelHasPlayed(WorldType world, int level)
    {
        WorldLevel WL;
        return playerData.WorldLocks.TryGetValue(world, out WL) == true ? WL.hasPlayed >= level : false;
    }
    public uint getWorldLevelPlays(WorldType world, int level)
    {
        WorldLevel WL;
        if (playerData.WorldLocks.TryGetValue(world, out WL) == true)
        {
            Debug.Log("FOUND at world " + world + " lvl " + level + " : " + WL.dailyPlays[level]);
            return WL.dailyPlays[level];
        }
        return 0;
    }

    public void NotEnoughGoldPrompt(Transform t, UnityAction onComplete = null)
    {

        getSoundManager().playError();
        Notification n = GameObject.Instantiate(notificationPrefab, t, false).GetComponent<Notification>();
        if (n == null)
        {
            return;
        }
        n.YesNoPrompt(LanguageSupport.NotEnoughGold(), () =>
        {

            Transform shop = Instantiate(Resources.Load<GameObject>("miscPrefabs/CoinShop"), t, false).transform;
            Button exit = null;
            if (onComplete != null)
            {
                exit = shop.GetChild(0).Find("exit").GetComponent<Button>();
                exit.onClick.AddListener(onComplete);
            }
            if (exit != null && t != transform)
            {
                GetComponent<Canvas>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(false);
                exit.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive(true));
            }
        }, onComplete, "Buy Gold");
        n.RewardBackground();
    }

    public Notification Notify(string notification, Transform parent, UnityAction listener = null, bool playNotif = false)
    {
        if (playNotif)
        {
            GameControl.control.getSoundManager().playNotif();
        }
        // getSoundManager().playExploreButton();
        Notification n = GameObject.Instantiate(notificationPrefab, parent, false).GetComponent<Notification>();
        n.Notify(notification, listener);
        return n;
    }

    public Notification YesNoPrompt(string notification, Transform parent, UnityAction yesListener = null, UnityAction noListener = null, string yesString = "yes", bool playNotif = false)
    {
        if (playNotif)
        {
            GameControl.control.getSoundManager().playNotif();
        }
        // getSoundManager().playExploreButton();
        Notification n = GameObject.Instantiate(notificationPrefab, parent, false).GetComponent<Notification>();
        n.YesNoPrompt(notification, yesListener, noListener, yesString);
        return n;
    }

    public void MultiplePrompt(string notification, Transform parent, UnityAction[] listeners, string[] labels, UnityAction noListener)
    {
        //getSoundManager().playExploreButton();
        Notification n = GameObject.Instantiate(notificationPrefab, parent, false).GetComponent<Notification>();
        n.RewardCatPrompt(notification, listeners, labels, noListener);
    }

}
