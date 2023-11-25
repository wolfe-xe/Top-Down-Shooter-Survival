using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public LayerMask collisonLayerMask;
    //public Color TrailColor;
        
    float speed = 10f;
    float damage = 1f;

    float lifetime = 5f;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] initalCollision = Physics.OverlapSphere(transform.position, 1f, collisonLayerMask);

        if (initalCollision.Length > 0)
        {
            OnHitObject(initalCollision[0], transform.position);
        }

        //GetComponent<TrailRenderer>().material.SetColor("_TintColor", TrailColor);
    }
    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;  
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate (Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisonLayerMask, QueryTriggerInteraction.Collide) )
        {
            OnHitObject(hit.collider, hit.point);
        }

    }

    void OnHitObject(Collider col, Vector3 hitPoint)
    {
        IDamageable damageableObject = col.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject.Destroy(gameObject);
    }
}
