using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class TeamSetUp : MonoBehaviour
{
    public Sprite Q;
    public List<TeamSelector> selectPositions;

    private TeamSelector currentChosen;

    private List<GameObject> ready = new List<GameObject>();

    private GameObject removeTeam;

    public void SetTeamGUI()
    {
        GameControl.control.getMainUI().deck.SetDeck(() =>
        {
            setExistingTeam();
            setDeckListeners();
            gameObject.SetActive(true);
            GameControl.control.getBackButton().onClick.AddListener(GoBack);
        }, false);
    }

    public void GoBack()
    {
        GameControl.control.getBackButton().onClick.RemoveAllListeners();
        gameObject.SetActive(false);
        clearReady();
        if (removeTeam != null)
        {
            Destroy(removeTeam);
        }
    }

    private void clearReady()
    {
        foreach (GameObject g in ready) 
        {
            g.transform.parent.GetComponent<Button>().interactable = true;
            Destroy(g);
        }
        ready.Clear();
        Debug.Log("Destroyed ready. ready's Count - " + ready.Count);
    }
    private void setExistingTeam()
    {
        int i = 0;
        for (; i < GameControl.control.playerData.team.Count; i++)
        {
            selectPositions[i].transform.GetChild(0).GetComponent<Image>().sprite = GameControl.control.playerData.team[i].getCatAsset().head;
            selectPositions[i].cat = GameControl.control.playerData.team[i];
        }
        for (; i < 4; i++)
        {
            selectPositions[i].transform.GetChild(0).GetComponent<Image>().sprite = Q;
            selectPositions[i].cat = null;
        }
    }
    private void setDeckListeners()
    {
        foreach (Transform child in GameControl.control.getMainUI().deck.transform)
        {
            if (child.name == "more")
            {
                continue;
            }
            Cat cat = child.GetComponent<DeckCard>().cat;
            if (cat == null)
            {
                Debug.LogWarning("this deck card " + child.name + "'s cat is null");
                //this is the remove cat button, it handles its own listener
                continue;
            }
            Button b = child.GetComponent<Button>();
            b.onClick.RemoveAllListeners();

            b.onClick.AddListener(() =>
               {
                   GameControl.control.getSoundManager().playButton();
                   setCatAvailableUI(currentChosen.cat);
                   SetTeamCat(cat);
                   SetUnavailableCatUI(b);
               });

            if (GameControl.control.playerData.team.FirstOrDefault(x => x.GUID == ((cat != null) ? cat.GUID : null)) != null)
            {
                Debug.Log("hi");
                SetUnavailableCatUI(b);
            }
        }
        //add no team button

        addRemoveTeamButton();
    }

    private void addRemoveTeamButton()
    {
        if (removeTeam != null)
        {
            removeTeam.SetActive(true);
            return;

        }
        //instantiate button if doesn't exist
        removeTeam = GameObject.Instantiate(GameControl.control.getMainUI().deck.deckCardPrefab, GameControl.control.getMainUI().deck.transform, false);
        DeckCard removeTeamDeckCard = removeTeam.GetComponent<DeckCard>();
        removeTeamDeckCard.cat = null; //remove team is not a brown cat!
        removeTeam.transform.SetAsFirstSibling();
        Image head = removeTeam.transform.GetChild(0).GetComponent<Image>();
        head.sprite = Q;
        head.SetNativeSize();
        head.rectTransform.anchorMin = Vector2.one * 0.5f;
        head.rectTransform.anchorMax = head.rectTransform.anchorMin;
        head.rectTransform.anchoredPosition = Vector2.zero;
        //destroy star and cat name text
        for (int i = 1; i < removeTeam.transform.childCount; i++)
        {
            Destroy(removeTeam.transform.GetChild(i).gameObject);
        }
        //add function to remove cat from team
        Button b = removeTeam.GetComponent<Button>();
        b.interactable = true;
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() =>
        {
            if (GameControl.control.RemoveFromTeam(currentChosen.cat))
            {
                Debug.Log("successfully removed!");
                GameControl.control.SavePlayerData();
                setCatAvailableUI(currentChosen.cat);
                currentChosen.transform.GetChild(0).GetComponent<Image>().sprite = Q;
                currentChosen.cat = null;
            }
            returnToTeam();
        });

    }

    private void setCatAvailableUI(Cat cat)
    {
        if (cat == null) return;
        Debug.Log("searching for " + cat.Name + "..");
        for (int i = ready.Count - 1; i > -1; i--)
        {
            Debug.Log(ready[i].transform.parent.name + " .... " + ready[i].transform.parent.GetComponent<DeckCard>().cat.Name);
            Debug.Log("this GUID: " + cat.GUID + " that GUID: " + ready[i].transform.parent.GetComponent<DeckCard>().cat.GUID);
            if (ready[i].transform.parent.GetComponent<DeckCard>().cat == cat)
            {
                ready[i].transform.parent.GetComponent<Button>().interactable = true;
                Destroy(ready[i]);
                ready.RemoveAt(i);
                return;
            }
        }
        Debug.Log("couldn't find " + cat.Name + " to make available!..");
    }

    private void SetUnavailableCatUI(Button b)
    {
        b.interactable = false;
        if (ready.Count == 0)
        {
            TextMeshProUGUI text = GameObject.Instantiate(GameControl.control.getBackButton().transform.GetChild(0), b.transform, false).GetComponent<TextMeshProUGUI>();
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMax = Vector2.zero;
            text.rectTransform.offsetMin = Vector2.zero;
            text.text = "READY";
            text.fontSize = 102;
            text.color = GameControl.control.getBackButton().targetGraphic.color;
            text.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/pixelfont");
            text.fontMaterial = Resources.Load<Material>("Fonts & Materials/pixelfont_outline");
            text.fontStyle = FontStyles.Bold;
            ready.Add(text.gameObject);
        }
        else
        {
            ready.Add(GameObject.Instantiate(ready[0], b.transform, false));
        }
    }

    private void SetTeamCat(Cat cat)
    {
        currentChosen.SetCatInTeam(cat);
        currentChosen.transform.GetChild(0).GetComponent<Image>().sprite = cat.getCatAsset().head;
        returnToTeam();
    }

    private void returnToTeam()
    {

        gameObject.SetActive(true);
        GameControl.control.getMainUI().deck.transform.parent.gameObject.SetActive(false);
    }
    //listener on button
    public void PickTeamCat(TeamSelector chosen)
    {
        GameControl.control.getSoundManager().playButton();
        currentChosen = chosen;
        GameControl.control.getMainUI().deck.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
