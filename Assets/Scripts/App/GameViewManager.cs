using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewManager : MonoBehaviour
{
    public GameObject gamePanelsObject;
    
    public void ShowGame()
    {
        gamePanelsObject.SetActive(true);
    }

    public void Escape()
    {
        
    }
}
