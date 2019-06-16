using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlusHandler : MonoBehaviour, IPointerDownHandler
{
    private EditorHandler _editor;
    public int xPosition;

    public void Init(EditorHandler editor, int xpos, Vector2 upos)
    {
        _editor = editor;
        xPosition = xpos;
        GetComponent<RectTransform>().anchoredPosition = upos;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_editor.AddLetterRequest(AvailableLetter.currentLetter.code, xPosition))
        {
            AvailableLetter.Reset();
            _editor.pluses.Hide();
            _editor.Correct();
        }
        else
        {
            Red();
            _editor.Error();
            AudioManager.instance.PlaySFX(SFX.WRONG_WORD);
        }
    }

    public void Red()
    {
        GetComponent<Image>().color = Color.red;
        StartCoroutine(TurnWhite());
    }

    private IEnumerator TurnWhite()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<Image>().color = Color.white;
    }
}