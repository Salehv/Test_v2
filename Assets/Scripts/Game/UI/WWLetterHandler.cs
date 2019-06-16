using UnityEngine;
using UnityEngine.UI;

class WWLetterHandler : MonoBehaviour
{
    public int code;
    public Image letterImage;

    internal void Init(int code)
    {
        this.code = code;
        letterImage.sprite = GameManager.instance.GetLetterSprite(code, SpriteMode.NORMAL);
    }
}