using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunControlls))]
public class Player : Entity
{
    PlayerController controller;
    GunControlls gunControlls;
    Camera cam;

    public Crosshair crosshair;

    public float moveSpeed = 5f;

    protected override void Start()
    {
        base.Start();
        
    }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunControlls = GetComponent<GunControlls>();
        cam = Camera.main;
        FindObjectOfType<Spawn>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHP;
    }

    void Update()
    {
        //Input
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = movementInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunControlls.GunHeight);// need fixing;
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, hitPoint);

            controller.LookAt(hitPoint);

            crosshair.transform.position = hitPoint;
            crosshair.DetectTarget(ray);
            gunControlls.Aim(hitPoint);

        }

        if (Input.GetMouseButton(0))
        {
            gunControlls.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunControlls.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunControlls.Reload();
        }

        if(transform.position.y < -10)
        {
            TakeDamage(health);   
        }

    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();

    }
}
