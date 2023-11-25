using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { auto, burst, single};
    public FireMode fireMode;

    [SerializeField] private Transform[] muzzle;
    [SerializeField] private Projectiles bullet;
    [SerializeField] private float fireRate = 100f;
    [SerializeField] private float muzzleVelocity = 35f;
    [SerializeField] private int burstCount;
    [SerializeField] private int projectilesPerMag;
    [SerializeField] private float reloadTime;

    public Transform shell;
    public Transform shellEject;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    
    MuzzleFlash muzzleFlash;
    Vector3 recoilSmoothDampVelocity;

    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    float nextShot;
    float recoilXAxis;
    float recoilXAcisSmoothDampVel;
    bool triggerReleased;
    bool isReloading;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;  
    }

    private void LateUpdate()
    {
        //animate Recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity ,0.1f);
        recoilXAxis = Mathf.SmoothDamp(recoilXAxis, 0f, ref recoilXAcisSmoothDampVel, 0.1f);
        //set back rotation **fixing**
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * -recoilXAxis; 

        if(!isReloading && projectilesRemainingInMag == 0) 
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (!isReloading && Time.time > nextShot && projectilesRemainingInMag > 0)
        {
            if (fireMode == FireMode.burst) 
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }

            if(fireMode == FireMode.single)
            {
                if(triggerReleased == false)
                {
                    return;
                }
            }

            for (int i = 0; i < muzzle.Length; i++)
            {
                if(projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShot = Time.time + fireRate / 1000;
                Projectiles newBullet = Instantiate(bullet, muzzle[i].position, muzzle[i].rotation) as Projectiles;
                newBullet.setSpeed(muzzleVelocity);

            }


            Instantiate(shell, shellEject.position, shellEject.rotation);

            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * 0.2f;
            recoilXAxis += 2f;
            recoilXAxis = Mathf.Clamp(recoilXAxis, 0, 3);

            AudioManager.instance.PlaySound(shootAudio, transform.position);

        }

    }

    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(0.2f);

        float percent = 0;
        float reloadSpeed = 1f / reloadTime;
        Vector3 initialRot = transform.eulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolationValue = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolationValue);
            transform.localEulerAngles = initialRot + Vector3 .left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
        {
            transform.LookAt(aimPoint);

        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleased = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleased = true;
        shotsRemainingInBurst = burstCount;
    }

   
}
