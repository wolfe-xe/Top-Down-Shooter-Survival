using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent (typeof(Renderer))]
public class Enemy : Entity
{
    NavMeshAgent pathFindingEnemy;
    Transform target;
    Material skinMaterial;
    Entity targetEntity;

    public ParticleSystem deathFX;
    public static event System.Action OnDeathStatic;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1f;
    float nextAttackTime = 1f;
    float damage = 1f;

    float enemyCollisionRadius;
    float targetCollisionRadius;

    public enum State { Idle, Chasing, Attacking};
    State currentState;

    bool hasTarget;

    private void Awake()
    {
        pathFindingEnemy = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<Entity>();

            enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        }

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
        
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathFindingEnemy.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHP / hitsToKillPlayer);

        }
        startingHP = enemyHealth;

        deathFX.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathFX.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathFX.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float squaredDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (squaredDistanceToTarget < Mathf.Pow(attackDistanceThreshold + enemyCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());

                }
            }
        }
        
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathFindingEnemy.enabled = false;
        
        skinMaterial.color = Color.red;

        StartCoroutine(InterpolBtwAttack());

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFindingEnemy.enabled = true;
        yield return null;

    }

    IEnumerator InterpolBtwAttack()
    {
        Vector3 originalPosition = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (enemyCollisionRadius);

        float attackSpeed = 3f;
        float attackPrecent = 0f;

        bool hasAppliedDamage = false;

        while (attackPrecent < 1)
        {
            if (attackPrecent >= 0.5 && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            attackPrecent += Time.deltaTime * attackSpeed;
            float interpolationValue = (-Mathf.Pow(attackPrecent, 2) + attackPrecent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolationValue);

            yield return null;
        }

    }

    /*    void Attack()
        {
            currentState = State.Attacking;
            pathFindingEnemy.enabled = false;

            Vector3 originalPosition = transform.position;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 attackPosition = target.position - directionToTarget * (enemyCollisionRadius);

            float attackSpeed = 3f;
            float attackPrecent = 0f;

            skinMaterial.color = Color.red;
            bool hasAppliedDamage = false;

            if (attackPrecent <= 1)
            {
                if (attackPrecent >= 0.5 && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    targetEntity.TakeDamage(damage);
                    Debug.Log("Damaged");
                }
                attackPrecent += Time.deltaTime * attackSpeed;
                float interpolationValue = (-Mathf.Pow(attackPrecent, 2) + attackPrecent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolationValue);
            }

            skinMaterial.color = originalColor;
            currentState = State.Chasing;
            pathFindingEnemy.enabled = true;
        }*/

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget) 
        {
            if(currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (enemyCollisionRadius + enemyCollisionRadius + attackDistanceThreshold/2);

                if (!dead)
                {
                    pathFindingEnemy.SetDestination(targetPosition);
                    yield return new WaitForSeconds(refreshRate);
                }
            }
           
        }
    }
}
