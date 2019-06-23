using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;

public class PopupDestroyer : MonoBehaviour
{
    public void DestroyThis()
    {
        ViewManager.instance.SetEscapable();
        Destroy(gameObject);
    }
}