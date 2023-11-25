using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    public float health { get; protected set; }
    protected bool dead;

    public float startingHP;
    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHP; 
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage) 
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
