using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaptersContentFitter : MonoBehaviour {
    public float contentWidthPercent;
    public float contentHeightPercent;

    void Awake()
    {
        Init();    
    }

    public void Init()
    {
        int n = transform.childCount;
        float width = Screen.width * contentWidthPercent;
        float height = Screen.height * contentHeightPercent;

        for (int i = 0; i < n; i++)
            transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        GetComponent<RectTransform>().sizeDelta = new Vector2(width * (n + 1), height);
	}
}
