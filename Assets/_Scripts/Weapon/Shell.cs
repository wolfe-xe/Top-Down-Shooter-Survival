using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody shellRb;
    public float forceMin;
    public float forceMax;

    float lifeTime = 1f;
    float fadeTime = 1f;


    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax); 
        shellRb.AddForce(transform.right * force);
        shellRb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        StartCoroutine(Shells());

    }

    IEnumerator Shells()
    {
        float percent = 0f;
        float fadeSpeed = 1 / fadeTime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
