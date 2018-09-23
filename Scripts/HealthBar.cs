using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public void initHealthBar(ExploreCat cat)
    {
        transform.Find("cat").GetComponent<Image>().sprite = cat.cat.getCatAsset().head;
    }

    public void updateHealthBar(ExploreCat cat)
    {
        // 212 is max width for the bar
        float oldW = healthBar.rectTransform.sizeDelta.x;
        float newW = 212 * (cat.currentHealth / cat.realizedStats.maxHealth);
        LeanTween.value(gameObject, (float val) =>
        {
            healthBar.rectTransform.sizeDelta = new Vector2(val, healthBar.rectTransform.sizeDelta.y);
        }, oldW, newW, Mathf.Clamp(Mathf.Abs(oldW - newW) * 0.05f, 0, 2.5f));
    }

    public void setSideEffect(Fx fx, ExploreCat cat)    
    {
        setSprite(fx.ToString());
    }
    public void setDead()
    {
        setSprite("x");
        RectTransform rt = (RectTransform)transform;
        Vector2 orig = rt.anchoredPosition,
        beg = new Vector2(rt.anchoredPosition.x - 17, rt.anchoredPosition.y),
        end = new Vector2(rt.anchoredPosition.x + 17, rt.anchoredPosition.y);
        LeanTween.value(gameObject, (float val) =>
        {
            rt.anchoredPosition = Vector2.Lerp(beg, end, val);
        }, 0, 1, 0.1f).setLoopPingPong(3).setEaseInSine().setOnComplete(() =>
        {
            rt.anchoredPosition = orig;
        });
    }
    public void setFactionAdvantage()
    {
        Vector2 beg = transform.localScale,
        end = transform.localScale * 1.2f;
        transform.Find("status").gameObject.SetActive(true);
        LeanTween.value(gameObject, (float val) =>
       {
           transform.localScale = Vector2.Lerp(beg, end, val);
       }, 0, 1, 0.2f).setLoopPingPong(1).setEaseInSine().setOnComplete(() =>
       {
           transform.localScale = beg;  
       });
    }

    public void fromEffectToFactionAdvantage()
    {
        transform.Find("status").GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/up");
    }
    public void removeEffect()
    {
        transform.Find("status").gameObject.SetActive(false);
    }

    public void setSprite(string sprite)
    {
        Transform effect = transform.Find("status");
        effect.gameObject.SetActive(true);
        effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("miscUI/" + sprite);
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }
}
