using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : AttackObj
{
    public float speed = 25;
    private SecondaryType sprite;
    protected Rigidbody2D rigidBody;
    bool hasHit = false;

    public Rigidbody2D getRigidBody2D()
    {
        if (rigidBody != null)
        {
            return rigidBody;
        }
        rigidBody = this.GetComponent<Rigidbody2D>();
        return rigidBody;
    }

    public void Init(ExploreCat owner, float speed, SecondaryType sprite)
    {
        this.owner = owner;
        this.speed = speed;
        this.sprite = sprite;
        if (sprite != SecondaryType.Music)
        {
            transform.rotation = owner.transform.rotation;
        }
        gameObject.layer = getLayer(owner.owner.enemy);
    }
    private int getLayer(bool enemy)
    {
        if (enemy)
        {
            return LayerMask.NameToLayer("EnemyRange");
        }
        else
        {
            return LayerMask.NameToLayer("UserRange");
        }
    }
    public void Target(ExploreCat targ)
    {
        transform.position = new Vector3(owner.transform.position.x + 0.3f, owner.transform.position.y + 0.2f, 0);
        hasHit = false;
        setMiss();
        setup(targ);
    }

    private void setMiss()
    {
        if (damage > 0)
        {
            GetComponent<CircleCollider2D>().isTrigger = false;
        }
        else
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hasHit || owner.target == null || !callOnHit)
        {
            return;
        }
        moveTowardsPosition(owner.target.transform.position - transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canHitCol(other.collider))
        {
            ExploreCat cat = Hit(other.gameObject);
            if (cat != null)
            {
                particleEffect();
                cat.hitByObj(damage, this);
            }
            GetComponent<CircleCollider2D>().isTrigger = true;

            onHit();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canHitTrig(other.gameObject))
        {
            ExploreCat cat = Hit(other.gameObject);
            if (cat != null)
            {
                cat.hitByObj(damage, this);
            }
            onHit();
        }
    }

    private bool canHitCol(Collider2D enemy)
    {
        return !hasHit
        && !enemy.isTrigger &&
        (enemy.gameObject.layer == getLayer(!owner.owner.enemy) ||
        enemy.gameObject.layer == owner.owner.getLayer(!owner.owner.enemy));
    }
    private bool canHitTrig(GameObject enemy)
    {
        return !hasHit && enemy.gameObject.layer == owner.owner.getLayer(!owner.owner.enemy);
    }
    private ExploreCat Hit(GameObject other)
    {
        hasHit = true;

        if (owner.target != null && other == owner.target.gameObject)
        {
            return owner.target;
        }
        //else, in case it hit something like another projectile or a different cat other than the target
        else if (other.gameObject.layer == getLayer(!owner.owner.enemy))
        {
            Debug.Log(owner.cat.Name + "'s projectile hit an enemy projectile");
            if (owner.owner.enemy)
            {
                owner.getRigidBody2D().AddForce(Vector2.down * UnityEngine.Random.Range(1, 2), ForceMode2D.Impulse);
                owner.getRigidBody2D().AddForce(Vector2.right * (owner.realizedStats.speed / 2), ForceMode2D.Impulse);
            }
            else
            {
                owner.getRigidBody2D().AddForce(Vector2.up * UnityEngine.Random.Range(1, 2), ForceMode2D.Impulse);
                owner.getRigidBody2D().AddForce(Vector2.left * (owner.realizedStats.speed / 2), ForceMode2D.Impulse);
            }
        }
        else
        {
            ExploreCat otherCat = other.GetComponent<ExploreCat>();
            if (otherCat != null && otherCat.currentHealth > 0)
            {
                return otherCat;
            }
        }
        return null;
    }

    void Done()
    {
        gameObject.SetActive(false);
        owner.attackObjs.Enqueue(this);
    }

    private void setup(ExploreCat targ)
    {
        Vector2 targetPos = new Vector2(targ.getRigidBody2D().position.x, targ.getRigidBody2D().position.y + 0.5f);
        Vector2 dir = targetPos - getRigidBody2D().position;
        dir.Normalize();
        dir *= this.speed;
        switch (sprite)
        {
            case SecondaryType.Star:
            case SecondaryType.Gem:
            case SecondaryType.Nebula:
            case SecondaryType.Snowflake:
            case SecondaryType.Rose:
                //rotate motion 
                getRigidBody2D().angularVelocity = speed + 50f;
                getRigidBody2D().velocity = dir;
                getRigidBody2D().gravityScale = 0;
                break;
            case SecondaryType.Bubbles:
            case SecondaryType.Butterfly:
            case SecondaryType.Leaf:
            case SecondaryType.Rainbow:

                // direct hit
                getRigidBody2D().gravityScale = 0;
                getRigidBody2D().velocity = dir;
                transform.localEulerAngles = Vector3.zero;
                break;

            case SecondaryType.Heart:
            case SecondaryType.Music:
            case SecondaryType.Arrow:
                getRigidBody2D().gravityScale = 0;
                getRigidBody2D().velocity = dir;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
            case SecondaryType.Bat:
            case SecondaryType.Skull:
                getRigidBody2D().gravityScale = UnityEngine.Random.Range(0.7f, 0.9f);
                getRigidBody2D().velocity = calculateArc(targ);
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
            default:
                //arc motion -> simulate gravity?
                getRigidBody2D().gravityScale = UnityEngine.Random.Range(0.7f, 0.9f);
                getRigidBody2D().velocity = calculateArc(targ);
                break;
        }
    }

    private void moveTowardsPosition(Vector2 dir)
    {
        switch (sprite)
        {
            case SecondaryType.Bubbles:
            case SecondaryType.Music:
            case SecondaryType.Butterfly:
                //wave motion 

                dir.Normalize();
                getRigidBody2D().velocity = GetWaveVelocity(dir, speed * 0.5f, Time.time, 5, 0.6f);
                break;
            case SecondaryType.Leaf:
            case SecondaryType.Rainbow:
                //wave and rotate

                dir.Normalize();
                getRigidBody2D().velocity = GetWaveVelocity(dir, speed * 0.5f, Time.time, 5, 0.6f);
                getRigidBody2D().angularVelocity = getRigidBody2D().velocity.x * speed * speed;
                break;
        }
    }

    void OnBecameInvisible()
    {
        Done();
    }

    private Vector2 calculateArc(ExploreCat targ)
    {
        float v = speed + 5;
        Debug.Log("Projectile v: " + v);
        float x = targ.transform.position.x - transform.position.x;
        float y = targ.transform.position.y - transform.position.y;
        float g = getRigidBody2D().gravityScale * 10f;
        float dis = Mathf.Pow(v, 4) - 2 * v * v * g * y - g * g * x * x;
        if (dis > 0)
        {
            float plusminus = Mathf.Sqrt(dis);
            float dividend = v * v - plusminus;
            //For once we actually don't want atan2 - it'd mess with our results.
            float theta = Mathf.Atan(dividend / (g * x));
            //Instead we just flip the vector if the target is on the left
            return new Vector2((x > 0 ? 1 : -1) * v * Mathf.Cos(theta),
            (x > 0 ? 1 : -1) * v * Mathf.Sin(theta));
        }
        else
        {
            Debug.Log("dir is less than 0");
            return Vector2.one * v;
        }
    }

    private Vector2 GetWaveVelocity(Vector2 _forward, float _speed, float _time, float _frequency, float _amplitude)
    {
        Vector2 up = new Vector2(-_forward.y, _forward.x);
        float up_speed = Mathf.Cos(_time * _frequency) * _amplitude * _frequency;
        return up * up_speed + _forward * _speed;
    }
}
