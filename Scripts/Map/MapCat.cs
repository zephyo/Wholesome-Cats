using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
public class
MapCat : ExploreCatHolder
{
    private float Speed = 1.5f;
    public bool IsMoving { get; private set; }

    public WorldPin CurrentPin { get; private set; }
    private MapManager _mapManager;

    public void Initialise(MapManager mapManager, WorldPin startPin)
    {
        cat = GameControl.control.playerData.team[0];

        if (cat == null)
        {
            cat = new Cat();
        }
        Speed = cat.getCatAsset().dynamicStats.getRealizedSpeedFromLevel(cat.catLvl, cat.catLvl.getPercentageToMaxLevel()) / 3;
        cat.SetCat(transform.GetChild(0));

        _mapManager = mapManager;

        //if got caught in a random event
        if (GameControl.control.playerData.lastPos.x != 0 ||
        GameControl.control.playerData.lastPos.y != 0)
        {
            transform.position = new Vector2(GameControl.control.playerData.lastPos.x, GameControl.control.playerData.lastPos.y);
            CurrentPin = mapManager.FindPin(GameControl.control.playerData.lastWorld);
            GameControl.control.playerData.lastPos = new Vector2Ser(0, 0);
            foreach (WorldPin pin in CurrentPin.ClosePins)
            {
                if (pin.worldType == GameControl.control.playerData.currentWorld)
                {
                    MoveToPin(pin);
                    return;
                }
            }
            SetCurrentPin(CurrentPin);
        }
        else
        {
            SetCurrentPin(startPin);
        }

    }

    /// <summary>
    /// This runs once a frame
    /// </summary>
    private IEnumerator Walk(WorldPin _targetPin)
    {

        GameControl.control.playerData.currentWorld = _targetPin.worldType;
        GameControl.control.SavePlayerData();
        ExploreController.world = CurrentPin.worldType;
        ExploreController.level = -1;

        Animator a = transform.Find("body").GetComponent<Animator>();
        a.SetBool("walk", true);
        yield return null;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = _targetPin.transform.position;

        getRigidBody2D().velocity = (targetPosition - currentPosition).normalized * Speed;
        if (targetPosition.x < currentPosition.x)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
        float distance = Vector2.Distance(currentPosition, targetPosition);
        float time = 0;
        while (distance > 0.15f)
        {
            currentPosition = transform.position;
            time += Time.deltaTime;
            if (time > 0.8f && UnityEngine.Random.value > 0.97f)
            {
                GameControl.control.playerData.lastPos = new Vector2Ser(transform.position.x, transform.position.y);
                GameControl.control.SavePlayerData();
                _mapManager.PlayRandom();
                rigidBody.velocity = Vector2.zero;
                a.SetBool("walk", false);
                yield break;
            }
            yield return null;
            distance = Vector2.Distance(currentPosition, targetPosition);
        }
        rigidBody.velocity = Vector2.zero;
        a.SetBool("walk", false);
        SetCurrentPin(_targetPin);
        GameControl.control.getSoundManager().playExploreMusic(null);

        ExploreController.level = 0;
    }


    /// <summary>
    /// Check the if the current pin has a reference to another in a direction
    /// If it does the move there
    /// </summary>
    /// <param name="direction"></param>
    public void TestPin(WorldPin pin)
    {
        if (IsMoving || pin.Unlocked == false || !CurrentPin.ClosePins.Contains(pin)) return;

        // Try get the next pin
        MoveToPin(pin);
    }


    /// <summary>
    /// Move to a new pin
    /// </summary>
    /// <param name="pin"></param>
    private void MoveToPin(WorldPin pin)
    {
        _mapManager.hidePrompt();
        IsMoving = true;
        StartCoroutine(Walk(pin));
    }


    /// <summary>
    /// Set the current pin
    /// </summary>
    /// <param name="pin"></param>
    public void SetCurrentPin(WorldPin pin)
    {
        CurrentPin = pin;
        GameControl.control.playerData.lastWorld = pin.worldType;
        GameControl.control.SavePlayerData();
        transform.position = pin.transform.position;
        IsMoving = false;

        // Tell the map manager that
        // the current pin has changed
        _mapManager.updatePrompt();
    }


}