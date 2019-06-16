using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDestroyer : MonoBehaviour
{
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}