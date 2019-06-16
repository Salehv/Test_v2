using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioCorrector : MonoBehaviour
{
    static Camera backgroundCam;

    /**/
    [Range(0.1f, 2.5f)] public float desiredAspect;
    
    // Use this for initialization
    void Awake() 
    {
        
        float targetaspect = desiredAspect;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {  
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
        
            camera.rect = rect;
        }
        
        if (!backgroundCam) 
        {
            // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
            backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
            backgroundCam.depth = int.MinValue;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;
        }
    }
    /**/
}