using UnityEngine;
using UnityEngine.UI;

internal class HintWordHandler : MonoBehaviour
{
    internal void Init(string v)
    {
        GetComponent<Text>().text = v;
    }
}