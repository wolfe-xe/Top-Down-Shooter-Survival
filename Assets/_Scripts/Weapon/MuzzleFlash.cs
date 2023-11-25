using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{

    public GameObject muzzleFlashHolder;
    //public Sprite[] flashSprite;
    //public SpriteRenderer[] spriteRenderer;

    public float flashTime;

    // Start is called before the first frame update
    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        muzzleFlashHolder.SetActive(true);

        /*int flashSpriteInd = Random.Range(0, flashSprite.Length);

        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].sprite = flashSprite[flashSpriteInd];
        }*/

        Invoke("Deactivate", flashTime);
    }

    public void Deactivate()
    {
        muzzleFlashHolder.SetActive(false);
    }
}
