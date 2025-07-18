using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
    public float targetaspect = 9.0f / 16.0f;
    public float lwa = 9.0f / 16.0f;
    void Start()
    {
        InvokeRepeating("Check", 0.25f, 0.5f);
        InvokeRepeating("Adjust", 0.1f, 5f);
    }
    public void Check()
    {
        float windowaspect = (float)Screen.width / (float)Screen.height;
        if(windowaspect != lwa)
        {
            Adjust();
        }
    }
    public void Adjust()
    {
        float windowaspect = (float)Screen.width / (float)Screen.height;
        lwa = windowaspect;
        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();

        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

    }
}