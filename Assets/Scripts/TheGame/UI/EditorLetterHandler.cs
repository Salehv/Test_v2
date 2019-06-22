using System.Runtime.InteropServices;
using TheGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]
public class EditorLetterHandler : MonoBehaviour, IPointerDownHandler
{
    private static EditorLetterHandler selected = null;
    private EditorHandler _editor;
    private DynamicsHandler _dynamics;

    public int position;
    public int code;
    public Image letterImage;
    public GameObject delete;

    private bool canRemove;

    internal void Init(DynamicsHandler dynamics, EditorHandler editor, int xPosition, int code, bool canBeRemoved)
    {
        _editor = editor;
        _dynamics = dynamics;
        Init(xPosition, code);
        canRemove = canBeRemoved;
    }

    internal void Init(int xPosition, int code)
    {
        position = xPosition;
        this.code = code;
        letterImage.sprite = GameManager.instance.GetLetterSprite(code, SpriteMode.NORMAL);
    }

    private int absPos;


    /*public void OnBeginDrag(PointerEventData eventData)
    {
        if (canRemove)
        {
            absPos = EditorHandler.instance.IT_to_AP(EditorHandler.instance.X_to_IT(position));
            EditorHandler.instance.FloatLetter(position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canRemove)
        {
            transform.position += (Vector3)eventData.delta;
            EditorHandler.instance.contentList.CalculateRemovePosition(transform.position, absPos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canRemove)
        {
            bool correct = EditorHandler.instance.EditorLetterDropped(transform.position, this);
            if (!correct)
            {
                EditorHandler.instance.CancelFloat(this);
            }
        }
    }*/

    public void OnPointerDown(PointerEventData eventData)
    {
        if (AvailableLetter.currentLetter == null && !canRemove)
            return;

        if (canRemove && AvailableLetter.currentLetter == null)
        {
            if (selected == this)
            {
                selected.HideDelete();
                return;
            }

            if (selected != null)
            {
                selected.HideDelete();
            }

            selected = this;
            ShowDelete();
            return;
        }

        GetComponent<Animator>().SetTrigger("flip");
    }

    private void ShowDelete()
    {
        delete.GetComponent<Animator>().SetTrigger("show");
    }

    private void HideDelete()
    {
        selected = null;
        delete.GetComponent<Animator>().SetTrigger("hide");
    }

    public void Delete()
    {
        if (selected == null)
            return;

        if (_editor.RemoveLetterRequest(selected.position))
        {
            _editor.Correct();
            // TODO: REFACTORING
            _editor.RemoveLetter(selected.position);
            HideDelete();
        }
        else
        {
            _editor.Error();
            RemoveDelete();
        }
    }

    public void Flipped()
    {
        if (AvailableLetter.currentLetter == null)
            return;

        if (_editor.ChangeLetterRequest(AvailableLetter.currentLetter.code, position))
        {
            _editor.Correct();
            _editor.HidePluses();
        }
        else
        {
            _editor.Error();
        }
    }


    internal static void RemoveDelete()
    {
        if (selected == null)
            return;

        selected.HideDelete();
        selected = null;
    }
}