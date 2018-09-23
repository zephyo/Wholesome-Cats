using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorldPin : MonoBehaviour
{

    [Header("NOTICE: WORLD PIN MUST BE NAMED AFTER ITS WORLD TYPE")]
    [Header("Options")]
    [HideInInspector]
    public bool Unlocked;
    public WorldType worldType;
    public string Description;

    [Header("Pins")] //
    [SerializeField]
    public PinHashSet ClosePins;


    protected void setUnlocked()
    {
        Unlocked = GameControl.control.getWorldLevel(worldType) <= -1 ? false : true;
    }


    /// <summary>
    /// Use this for initialisation
    /// </summary>
    private void Awake()
    {
        setUnlocked();
        setUI();
    }
    private void setUI()
    {
        if (Unlocked)
        {
            string world = worldType.ToString();
            transform.Find("banner").Find("text").GetComponent<TextMeshProUGUI>().text = world.First().ToString().ToUpper() + world.Substring(1);
            Transform clouds = transform.Find("clouds");
            if (clouds != null && GameControl.control.getWorldLevelHasPlayed(worldType, 0))
            {
                Destroy(clouds.gameObject);
            }
            else if (clouds != null)
            {
                LeanTween.color((RectTransform)transform.Find("clouds").transform, new Color(1, 1, 1, 0), 3).setDelay(0.2f).setEaseOutSine();
            }
        }
        else
        {
            transform.Find("banner").gameObject.SetActive(false);
            Shader sh = Shader.Find("GUI/Text Shader");
            Image im = GetComponent<Image>();
            im.material = new Material(sh);
            im.material.color = DataUtils.getDisabledWorldColor(worldType);
        }
    }

    private void OnDestroy() {
        LeanTween.cancel(gameObject);
    }

    /// <summary>
    /// Draw lines between connected pins
    /// </summary>
    private void OnDrawGizmos()
    {
        if (ClosePins == null) return;
        foreach (WorldPin pin in ClosePins)
        {
            DrawLine(pin);
        }
    }

    /// <summary>
    /// Draw one pin line
    /// </summary>
    /// <param name="pin"></param>
    protected void DrawLine(WorldPin pin)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, pin.transform.position);
    }
}
