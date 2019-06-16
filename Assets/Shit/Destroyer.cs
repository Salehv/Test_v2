using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    public void SetDeactive()
    {
        this.gameObject.SetActive(false);
    }
}