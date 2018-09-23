
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class DynamicScaler : MonoBehaviour
{
    public Canvas canvas;
    public Image bg;
    void Start()
    {
        if (Camera.main.aspect >= 2f)
        {
            string bgName = bg.sprite.name;
            Debug.Log("adjust bg "+bgName);
            HashSet<string> dontScale = new HashSet<string>(){
                "computer0",
                "computer1"
                };
            if (!dontScale.Contains(bgName))
            {
                HashSet<string> Scale15 = new HashSet<string>(){
                "house0",
                "house1",
                "fields0"
                };
                if (Scale15.Contains(bgName))
                {
                    canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.15f;
                    bg.rectTransform.offsetMax = new Vector2(0, 60);
                }
                else
                {
                    canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.22f;
                }
            }
            //house 0, house1 :  .15, top: -60
        }
        canvas.renderMode = RenderMode.WorldSpace;
        bg = null;
        canvas = null;
    }

}
