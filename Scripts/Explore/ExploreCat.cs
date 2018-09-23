using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum Fx
{
    Love,
    Confused,
    Distracted,
    Scared,
}
public class SideEffect
{
    public Fx effect;
    public float chance;
    public SecondaryType effector;
    public SideEffect(Fx effect, SecondaryType effector)
    {
        this.effect = effect;
        this.effector = effector;
        switch (effect)
        {
            case Fx.Love:
                chance = 0.5f;
                break;
            case Fx.Distracted:
                chance = 0.8f;
                break;
            case Fx.Confused:
                chance = 0.8f;
                break;
            case Fx.Scared:
                chance = 0.5f;
                break;
        }
    }
}
public class ExploreCat : ExploreCatHolder
{
    public ExplorePlayer owner;
    public ExploreCat target;
    public HealthBar healthBar;
    private Animator animator;
    public void setAnimator(RuntimeAnimatorController a)
    {
        getAnimator().runtimeAnimatorController = a;
    }
    public Animator getAnimator()
    {
        if (animator != null)
        {
            return animator;
        }
        animator = transform.GetChild(0).GetComponent<Animator>();
        return animator;
    }
    public float currentHealth;
    private float coolDown;
    public CatDynamicStatsRealized realizedStats;
    private Coroutine IAttack;
    public Action<AttackType, ExploreCat> onHit;
    public Action<ActionType, ExploreCat> onAction;
    public Action<SideEffect, ExploreCat> onEffect;

    #region POWERUPS/EFFECTS ------
    private bool flying = false;
    public Button Action;
    public bool usedAction;
    ushort boost = 0;
    public SideEffect sideEffect;
    public Material spriteMaterial;
    #endregion
    #region MOVEMENT/VISUALS --- 
    private int increment;
    private float blockedTime;
    #endregion
    public Queue<AttackObj> attackObjs = new Queue<AttackObj>();
    public AttackObj cAttack;

    //object pooling
    public void getAttackObj(ExploreCat enemyCat, float damage)
    {
        if (attackObjs.Count == 0)
        {
            CreateAttackObject(enemyCat, damage);
            return;
        }
        cAttack = attackObjs.Dequeue();
        cAttack.setOnHit(true);
        cAttack.setDamage(damage);
    }

    public void init(Cat cat, ExplorePlayer owner, int i, HealthBar hb)
    {
        this.cat = cat;
        this.owner = owner;
        this.healthBar = hb;
        healthBar.initHealthBar(this);

        setStats();
        cat.SetCat(transform.GetChild(0));
        gameObject.name = cat.Name;

        Debug.Log("FIGHTER " + cat.Name + " at Lv." + cat.catLvl.level);
        //wait for user input to enable
        increment = i + (!owner.enemy ? 5 : 0);
        setPositionZ(transform.position);
        enabled = false;

    }
    public void setStats()
    {
        realizedStats = cat.getRealizedStats();
        currentHealth = realizedStats.maxHealth;
    }

    public void checkRemoveEffect()
    {
        Debug.Log(cat.Name + "'s checking if we remove effect at chance " + sideEffect.chance);
        if (UnityEngine.Random.value < sideEffect.chance)
        {
            Debug.Log(cat.Name + "'s side effect removed!");
            sideEffect = null;
            if (boost > 0)
            {
                healthBar.fromEffectToFactionAdvantage();
            }
            else
            {
                healthBar.removeEffect();
            }
        }
        else
        {
            sideEffect.chance += 0.3f;
        }
    }
    public void Enable()
    {
        Debug.Log("trying to enable " + cat.Name);
        if (enabled)
        {
            Debug.Log("already enabled!: " + cat.Name);
            return;
        }
        Debug.Log("enable " + cat.Name);
        enabled = true;
        if (sideEffect != null)
        {
            StartCoroutine(initSideEffect());
            return;
        }
        if (owner.enemy && (target == null || target.currentHealth <= 0))
        {
            findTarget();
        }

        this.getAnimator().SetBool("walk", true);

        coolDown = 0.25f;

        IAttack = StartCoroutine(Attack());
    }

    public void findTarget()
    {
        if (owner.enemyPlayer.aliveCats.Count == 0)
        {
            return;
        }
        ExploreCat closestCat = null;
        float distance = float.PositiveInfinity;
        foreach (ExploreCat c in owner.enemyPlayer.aliveCats)
        {
            float d2 = Vector2.Distance(transform.position, c.transform.position);
            if (d2 < distance)
            {
                distance = d2;
                closestCat = c;
            }
        }
        target = closestCat;
    }

    public void Disable()
    {
        Debug.Log(" trying to disable " + cat.Name);

        if (!enabled)
        {
            Debug.Log("already disabled " + cat.Name);
            return;
        }
        // StartCoroutine(changeZ());
        if (IAttack != null)
        {
            Debug.Log("STOP ATTACK " + cat.Name);
            StopCoroutine(IAttack);
        }
        if (cAttack != null)
        {
            cAttack.setOnHit(false);
        }
        if (!owner.enemy)
        {
            target = null;
        }
        stopWalk();
        blockedTime = 0;
        enabled = false;
    }

    // IEnumerator changeZ()
    // {
    //     WaitForSeconds s = new WaitForSeconds(0.1f);
    //     Vector3 one = transform.position, two;
    //     yield return new WaitForSeconds(0.2f);
    //     two = transform.position;
    //     do
    //     {
    //         one = two;
    //         setPositionZ(one);
    //         yield return s;
    //         two = transform.position;
    //     } while (one.y != two.y);
    // }
    private IEnumerator initSideEffect()
    {
        yield return null;
        target = null;
        onEffect(sideEffect, this);
    }

    public void useAction()
    {
        Debug.Log(cat.Name + " use action");
        enabled = true;
        ActionType action = cat.getCatAsset().action;

        usedAction = true;

        onAction(action, this);

        if (!owner.enemy)
        {
            Action.interactable = false;
        }
    }

    // Update is called once per frame
    IEnumerator Attack()
    {
        if (currentHealth <= 0)
        {
            Disable();
            yield break;
        }
        //so we can get multiple hits in if cooldown is low
        if (target != null)
        {
            while ((coolDown > 0))
            {
                coolDown -= Time.deltaTime;
                yield return null;
            }

            float distance;
            if (cat.getCatAsset().attackType == AttackType.Melee)
            {
                distance = 4.5f;
            }
            else
            {
                distance = this.realizedStats.projectileSpeed;
            }
            if (target == null)
            {
                Disable();
                yield break;
            }
            Vector2 position = transform.position;
            Vector2 dir = (Vector2)target.transform.position - (Vector2)position;
            WaitForFixedUpdate fu = new WaitForFixedUpdate();
            while (dir.magnitude > distance)
            {
                // Debug.Log("distance - " + ((Vector2)target.transform.position - (Vector2)transform.position).magnitude);
                // Debug.Log("velocity - " + getRigidBody2D().velocity);
                dir.Normalize();
                getRigidBody2D().AddForce(dir * this.realizedStats.speed, ForceMode2D.Force);
                setPositionZ(position);
                yield return fu;
                if (target == null)
                {
                    Disable();
                    yield break;
                }
                position = transform.position;
                dir = (Vector2)target.transform.position - (Vector2)position;
            }

            StartCoroutine(attackEnemy());
        }
        stopWalk();
        getRigidBody2D().velocity = Vector2.zero;
        // coolDown = 1.8f + 9 / realizedStats.speed;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        //getRigidBody2D().mass = 0.9f;
        setPositionZ(transform.position);
        blockedTime = 0;
    }
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     getRigidBody2D().mass = 0.4f;
    //     setPositionZ(transform.position);
    // }
    void OnCollisionStay2D(Collision2D col)
    {
        if (!enabled || col.rigidbody == null)
        {
            if (!getRigidBody2D().IsSleeping())
            {
                setPositionZ(transform.position);
            }
            return;
        }
        blockedTime += Time.deltaTime;
        //collision detection
        if (blockedTime > 4f)
        {
            blockedTime = 0;
            TextMeshPro text = owner.getDamageText(0);
            text.text = "BLOCKED";
            RiseText(text, null);
            StartCoroutine(goAway());
            // StartCoroutine(goAway(col));
        }
    }
    IEnumerator goAway()
    {
        yield return null;
        onHit(AttackType.None, this);
    }


    public void moveUp()
    {
        getRigidBody2D().AddForce(Vector2.up, ForceMode2D.Impulse);
    }
    public void moveDown()
    {
        getRigidBody2D().AddForce(Vector2.down, ForceMode2D.Impulse);
    }
    void setPositionZ(Vector3 tempPos)
    {
        float newZ = Mathf.Lerp(-10, 190, (tempPos.y + 4) / 5);
        transform.position = new Vector3(tempPos.x, tempPos.y, newZ + increment);
    }

    #region Health and Damage
    public void hitByObj(float damage, AttackObj attacker)
    {
        checkEffect(attacker);
        if (damage > 0)
        {
            owner.getCameraShake().Shake();
        }
        takeDamage(damage);
    }
    public void takeDamage(float damage)
    {
        if (damage > 0 && currentHealth > 0)
        {
            currentHealth -= damage;
            Debug.Log(cat.Name + " took damage. current health - " + currentHealth);
            healthBar.updateHealthBar(this);
            if (currentHealth <= 0)
            {
                owner.aliveCats.Remove(this);
            }
            StartCoroutine(Flicker(damage, () =>
            {
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    die();
                }
            }));
        }
        setDamageUI((int)damage);
    }


    private void checkEffect(AttackObj attacker)
    {

        Fx fx;
        if (attacker.particles == null)
        {
            return;
        }
        if (attacker.particles.gameObject.name == "hypnotized")
        {
            fx = Fx.Distracted;
        }
        else if (attacker.particles.gameObject.name == "confused")
        {
            fx = Fx.Confused;
        }
        else if (attacker.particles.gameObject.name == "love")
        {
            fx = Fx.Love;
        }
        else if (attacker.particles.gameObject.name == "bats")
        {
            fx = Fx.Scared;
        }
        else
        {
            return;
        }
        if (sideEffect != null && sideEffect.effect == fx)
        {
            return;
        }
        healthBar.setSideEffect(fx, this);
        sideEffect = new SideEffect(fx, attacker.owner.cat.getCatAsset().secondaryType);
    }
    private IEnumerator Flicker(float damage, Action onComplete)
    {
        Color orig = spriteMaterial.color;
        yield return null;
        yield return null;
        spriteMaterial.color = Color.clear;
        float yield = Mathf.Clamp(damage / realizedStats.maxHealth / 3, 0.03f, 0.12f);
        yield return new WaitForSeconds(yield);
        spriteMaterial.color = orig;
        onComplete();
    }
    private float filterDamage(float damage, ExploreCat enemyCat, AttackType t)
    {
        if (t == AttackType.Melee && !flying && enemyCat.flying)
        {
            Debug.Log("miss because " + cat.Name + " isn't flying and " + enemyCat.cat.Name + " is");
            return 0;
        }
        //if speed more than enemy's, higher likelihood of enemy missing
        float multiply;
        if (t == AttackType.Melee)
        {
            multiply = 15;
        }
        else
        {
            multiply = 3;
        }
        if (UnityEngine.Random.value > (realizedStats.speed / enemyCat.realizedStats.speed) - (2f / (realizedStats.speed * multiply)))
        {
            Debug.Log("Miss! With " + cat.Name + " having speed " + realizedStats.speed + " and enemy " + enemyCat.cat.Name + " having speed " + enemyCat.realizedStats.speed);
            return 0;
        }
        return damage;

    }
    // show rising numbers on damage
    public void setDamageUI(int damage)
    {
        TextMeshPro text = owner.getDamageText(damage);
        text.text = damage > 0 ? "-" + damage.ToString() : "MISSED";
        RiseText(text, null);
    }

    public void RiseText(TextMeshPro text, Action onComplete)
    {
        text.transform.SetParent(transform, false);
        text.transform.localEulerAngles = transform.eulerAngles;
        text.transform.localPosition = new Vector3(0, 1, 0);
        Color textColor = text.color;
        LeanTween.value(text.gameObject, 0, 1, 0.8f).setOnUpdate((float value) =>
        {
            text.transform.localPosition = new Vector3(0, value * 1.2f + 1, 0);
            textColor.a = 1f - value;
            text.color = textColor;
        }).setEaseInQuart().setDelay(0.7f).setOnComplete(() =>
       {
           text.gameObject.SetActive(false);
           textColor.a = 1;
           text.color = textColor;
           owner.damageTexts.Enqueue(text);
           if (onComplete != null)
           {
               onComplete();
           }
       });
    }
    private void die()
    {
        owner.GameOver(owner);
        onHit(AttackType.None, this);
        target = null;
        getRigidBody2D().velocity = Vector2.zero;
        Destroy(GetComponent<Collider2D>());
        healthBar.setDead();
        RandomCatNoises n = transform.GetChild(0).GetComponent<RandomCatNoises>();
        n.playMeow();
        n.enableNoises = false;

        if (owner.enemy)
        {
            EventTrigger ET = GetComponent<EventTrigger>();
            if (ET != null)
            {
                Destroy(ET);
            }
            gameObject.layer = owner.getLayer(false);
        }
        else
        {
            Action.interactable = false;
        }

        if (getAnimator().GetBool("fly"))
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = owner.getLosePos(this);
            Vector3 bending = Vector3.left;
            LeanTween.value(gameObject, 0, 1, 2.1f).setDelay(0.5f).setOnUpdate((float val) =>
            {
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, val);
                currentPos.x += bending.x * Mathf.Sin(val * Mathf.PI);
                currentPos.y += bending.y * Mathf.Sin(val * Mathf.PI);
                transform.position = currentPos;
                setPositionZ(currentPos);
            }).setOnComplete(() =>
            getAnimator().SetBool("fly", false));
            flying = false;
        }
        else
        {
            StartCoroutine(moveToOrig());
        }
    }
    IEnumerator moveToOrig()
    {
        Vector3 endPos = owner.getLosePos(this), position = transform.position;
        Debug.Log(endPos);
        getAnimator().SetBool("walk", true);
        Vector2 dir = endPos - position;
        WaitForFixedUpdate fu = new WaitForFixedUpdate();
        while (dir.magnitude > 0.3f)
        {
            // Debug.Log("distance - " + ((Vector2)target.transform.position - (Vector2)transform.position).magnitude);
            // Debug.Log("velocity - " + getRigidBody2D().velocity);
            dir.Normalize();
            getRigidBody2D().AddForce(dir * (this.realizedStats.speed / 2), ForceMode2D.Force);
            yield return fu;
            position = transform.position;
            setPositionZ(position);
            dir = endPos - position;
        }
        setPositionZ(position);
        stopWalk();
    }
    #endregion


    #region Attack
    public IEnumerator attackEnemy()
    {
        if (!enabled || target == null)
        {
            yield break;
        }
        ExploreCat targ = target;
        AttackType aType = cat.getCatAsset().attackType;
        Debug.Log(cat.Name + " ATTACK " + target.cat.Name);
        if (aType == AttackType.Melee)
        {
            this.getAnimator().SetTrigger("swipe");
        }
        else if (aType == AttackType.Ranged)
        {
            this.getAnimator().SetTrigger("throw");
        }
        getAttackObj(targ, filterDamage(this.realizedStats.attackDamage, targ, aType));

        //wait for animation
        yield return new WaitForSeconds(0.75f);

        cAttack.gameObject.SetActive(true);
        if (aType == AttackType.Melee)
        {
            cAttack.transform.position = targ.transform.position;
            ((Melee)cAttack).Target(targ);
        }
        else if (aType == AttackType.Ranged)
        {
            ((Projectile)cAttack).Target(targ);
        }

    }

    private void CreateAttackObject(ExploreCat enemyCat, float damage)
    {
        cAttack = Instantiate(Resources.Load<GameObject>("miscPrefabs/" + cat.getCatAsset().attackType.ToString())).GetComponent<AttackObj>();
        Sprite secondarySprite = Resources.Load<Sprite>("fightUI/" + this.cat.getCatAsset().secondaryType.ToString());

        if (cat.getCatAsset().attackType == AttackType.Melee)
        {
            ((Melee)cAttack).Init(this);
            ((Melee)cAttack).getSprite().sprite = secondarySprite;
        }
        else if (cat.getCatAsset().attackType == AttackType.Ranged)
        {
            cAttack.GetComponent<SpriteRenderer>().sprite = secondarySprite;
            ((Projectile)cAttack).Init(this, this.realizedStats.projectileSpeed, cat.getCatAsset().secondaryType);
        }
        cAttack.setDamage(damage);
    }
    // add advantage due to having a faction that is compatible to the WorldType
    //particle effects and stuff
    public void factionAdvantage()
    {
        Transform powerUpParticles = GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/powerUp")).transform;
        powerUpParticles.name = "powerUp";
        powerUpParticles.SetParent(transform, false);
        healthBar.setFactionAdvantage();
        tempBoost(7);
    }

    public void FadeAndAvoidHit()
    {
        Debug.Log("fade");
        LeanTween.value(1, 0.25f, 2.5f).setEaseInSine().setOnUpdate((float val) =>
        {
            //set alpha
            spriteMaterial.color = new Color(1, 1, 1, val);
        });
        //harder to land a hit
        boost += 7;
        realizedStats.speed = cat.getSpeedBoost((uint)(boost));
    }


    public void FlyAndAvoidHit()
    {
        //    todo
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x + (owner.enemy ? -1f : 1f), startPos.y + 1f, 0);
        Vector3 bending = Vector3.left;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("MainMenu").GetComponent<Collider2D>());
        LeanTween.value(gameObject, 0, 1, 2.1f).setDelay(0.5f).setOnUpdate((float val) =>
        {
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, val);
            currentPos.x += bending.x * Mathf.Sin(val * Mathf.PI);
            currentPos.y += bending.y * Mathf.Sin(val * Mathf.PI);
            transform.position = currentPos;
            setPositionZ(currentPos);
        });
        getAnimator().SetBool("fly", true);
        getRigidBody2D().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        flying = true;
        //avoid hit - only ranged cats or other flying cats can land a hit
    }
    public void Magic()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Particles/magic"), transform, false);
        StartCoroutine(groupHeal());
    }
    public IEnumerator groupHeal()
    {
        yield return new WaitForSeconds(2.5f);
        WaitForSeconds pause = new WaitForSeconds(0.5f);
        float maxHealth = realizedStats.maxHealth * 0.85f;
        Color oldColor = Color.clear;
        foreach (ExploreCat cat in owner.aliveCats)
        {
            if (cat.currentHealth == cat.realizedStats.maxHealth)
            {
                continue;
            }
            float increment = maxHealth;
            increment = Mathf.Clamp(increment, 0, cat.realizedStats.maxHealth - cat.currentHealth);
            cat.currentHealth += increment;
            maxHealth -= increment;
            TextMeshPro text = owner.getDamageText((int)increment);

            text.text = "+" + (int)increment;
            if (oldColor == Color.clear)
            {
                oldColor = new Color(text.color.r, text.color.g, text.color.b, 1);
            }
            text.color = new Color32(103, 195, 0, 255);
            cat.RiseText(text, () =>
            {
                text.color = oldColor;
                Debug.Log("HEYY CHANGE BACK THE COLORS " + oldColor);
            });
            cat.healthBar.updateHealthBar(cat);
            if (maxHealth <= 0)
            {
                break;
            }
            yield return pause;
        }
    }
    public void loveParticles()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("miscPrefabs/love"), transform, false);
    }
    public void setEffect()
    {
        string textureName = sideEffect.effect.ToString() + "Tex";
        int overlay = Shader.PropertyToID("_OverlayTex");
        Texture overlayTex = spriteMaterial.GetTexture(overlay);
        if ((overlayTex == null || overlayTex != null && overlayTex.name != textureName))
        {
            spriteMaterial.SetTexture(overlay, Resources.Load<Texture2D>("miscUI/" + textureName));
        }
        moveTexture(overlay);
    }
    private void moveTexture(int overlay)
    {
        //offset X from 0.25 to -0.14
        //ofset Y from  -0.24 to 0.3
        //overlay color from Color.clear to  Color.white
        int color = Shader.PropertyToID("_ColorOverlay");
        LeanTween.value(GameControl.control.gameObject, (float val) =>
        {
            spriteMaterial.SetTextureOffset(overlay, new Vector2(0.25f - (0.25f * val), -0.24f + (0.24f * val)));
            spriteMaterial.SetColor(color, new Color(1, 1, 1, val * 0.4f));

        }, 0, 1, 1.5f).setOnComplete(() =>
        {
            LeanTween.value(GameControl.control.gameObject, (float val) =>
            {
                spriteMaterial.SetTextureOffset(overlay, new Vector2(-0.14f + (0.14f * val), 0.3f - (0.3f * val)));
                spriteMaterial.SetColor(color, new Color(1, 1, 1, val * 0.4f));
            }, 1, 0, 1.5f).setOnComplete(() =>
             {
                 spriteMaterial.SetColor(color, Color.clear);
             });
        });
    }
    public void tempBoost(ushort boost)
    {
        this.boost += boost;
        float prevHealth = realizedStats.maxHealth;
        realizedStats = cat.getTemporaryBoost(this.boost);
        currentHealth = (currentHealth * realizedStats.maxHealth) / prevHealth;
    }
    #endregion

    #region Move

    public void stopWalk()
    {
        this.getAnimator().SetBool("walk", false);
        // getRigidBody2D().velocity = Vector2.zero;
    }

    // public void moveRandom()
    // {
    //     Rigidbody2D rigidbody2D = getRigidBody2D();
    //     rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + UnityEngine.Random.Range(-.2f * realizedStats.speed, .2f * realizedStats.speed),
    //         rigidbody2D.velocity.y + UnityEngine.Random.Range(-.2f * realizedStats.speed, .2f * realizedStats.speed));
    // }
    #endregion
}
