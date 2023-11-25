using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetMask;
    Color originalColor;

    public SpriteRenderer dot;
    public Color highlightColor;

    private void Start()
    {
        Cursor.visible = false;
        originalColor = dot.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetectTarget(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = highlightColor;
        }

        else
        {
            dot.color = originalColor;  
        }
    }
}
