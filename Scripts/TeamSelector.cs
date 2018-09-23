using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;


public class TeamSelector : DeckCard
{
    public ushort position;

    public void SetCatInTeam(Cat newCat)
    {
        GameControl.control.RemoveFromTeam(this.cat);
        GameControl.control.playerData.team.Insert(Mathf.Clamp(position, 0, GameControl.control.playerData.team.Count), newCat);
        GameControl.control.SavePlayerData();
        this.cat = newCat;
    }
}
