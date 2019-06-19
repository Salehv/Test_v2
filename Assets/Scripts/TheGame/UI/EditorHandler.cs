using System;
using System.Collections;
using System.Collections.Generic;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

public class EditorHandler : MonoBehaviour
{
    public HorizontalList contentList;
    public PlusPositionHandler pluses;

    public GameObject recycler;
    private string currentText;
    private List<GameObject> currentLetters;

    [Header("Prefabs")] [SerializeField] protected GameObject listItemPrefab;


    private DynamicsFlag _flag;
    private DynamicsHandler _dynamics;

    private void Awake()
    {
        typeToTile = new Dictionary<SpriteMode, Sprite>()
        {
            {SpriteMode.NORMAL, normalTile},
            {SpriteMode.CORRECT, correctTile},
            {SpriteMode.ERROR, errorTile}
        };
    }

    internal void Initialize(DynamicsHandler dynamics, string text, DynamicsFlag flag)
    {
        _flag = flag;
        _dynamics = dynamics;

        text = Utilities.GetNormalizedFarsi(text);

        currentText = text;
        currentLetters = new List<GameObject>();

        for (int i = 0; i < contentList.transform.childCount; i++)
            Destroy(contentList.transform.GetChild(i).gameObject);

        for (int i = text.Length - 1; i >= 0; i--)
        {
            currentLetters.Add(CreateLetter(IT_to_X(i), text[i], _flag.canRemoveLetter));
        }
    }


//    internal bool EditorLetterDropped(Vector2 pos, EditorLetterHandler handler)
//    {
//        if((pos - (Vector2) recycler.transform.position).magnitude < 100)
//        {
//            if (GameManager.instance.RemoveLetter(X_to_IT(handler.position)))
//            {
//                Destroy(handler.gameObject);
//                contentList.ReArrange();
//                return true;
//            }
//        }
//        return false;
//    }


    internal bool ChangeLetterRequest(int code, int xPosition)
    {
        print(string.Format("Letter Change Request at ITPosition:{0} and code:{1}", X_to_IT(xPosition), code));
        return _dynamics.ChangeLetter(X_to_IT(xPosition), code);
    }

    internal bool AddLetterRequest(int code, int xPosition)
    {
        print(string.Format("Letter Add Request at ITPosition:{0} and code:{1}", X_to_IT(xPosition), code));
        return _dynamics.AddLetter(X_to_IT(xPosition), code);
    }

    public bool RemoveLetterRequest(int xPosition)
    {
        print(string.Format("Letter Remove Request at ITPosition:{0}", X_to_IT(xPosition)));
        return _dynamics.RemoveLetter(X_to_IT(xPosition));
    }


    /*********************/
    internal void ChangeLetter(int inTextPosition, int code)
    {
        contentList.ChangeChild(IT_to_AP(inTextPosition), IT_to_X(inTextPosition), code);
        currentText = _dynamics.GetLastWord();
    }

    internal void AddLetter(int inTextPosition, int code, bool canBeRemoved)
    {
        contentList.AddChild(IT_to_AP(inTextPosition) + 1,
            CreateLetter(IT_to_X(inTextPosition), Utilities.dic_letterToChar[code], canBeRemoved));
        contentList.ReArrange();
        currentText = _dynamics.GetLastWord();
        CalculateXPositions();
    }


    public void RemoveLetter(int xPosition)
    {
        contentList.RemoveChild(IT_to_AP(X_to_IT(xPosition)));
        currentText = _dynamics.GetLastWord();
        CalculateXPositions(xPosition);
    }


    private void CalculateXPositions(int removed)
    {
        RectTransform[] t = contentList.GetChildren();

        int ap = IT_to_AP(X_to_IT(removed));
        bool passed = false;
        for (int i = 0; i < t.Length; i++)
        {
            if (!passed && i == ap)
                passed = true;
            t[i].GetComponent<EditorLetterHandler>().position = IT_to_X(AP_to_IT(i + (passed ? -1 : 0)));
        }
    }


    private void CalculateXPositions()
    {
        RectTransform[] t = contentList.GetChildren();
        for (int i = 0; i < t.Length; i++)
        {
            t[i].GetComponent<EditorLetterHandler>().position = IT_to_X(AP_to_IT(i));
        }
    }


    private GameObject CreateLetter(int position, char l, bool canRemoveLetter)
    {
        GameObject g = Instantiate(listItemPrefab, contentList.transform, true);
        g.transform.localScale = Vector3.one;
        g.transform.localPosition = Vector3.zero;
        g.GetComponent<EditorLetterHandler>()
            .Init(_dynamics, this, position, Utilities.dic_charToLetter[l], canRemoveLetter);
        return g;
    }


    /***************************************************************************/


    public void ShowPluses()
    {
        if (_flag.canAddLetter)
            pluses.Show(this);
    }

    public void HidePluses()
    {
        pluses.Hide();
    }

    /***** UI ******/
    [Header("Tile Sprites")] private Dictionary<SpriteMode, Sprite> typeToTile;
    public Sprite normalTile;
    public Sprite errorTile;
    public Sprite correctTile;

    public void Correct()
    {
        for (int i = 0; i < contentList.Count; i++)
        {
            CorrectLetter(IT_to_X(i));
        }

        AudioManager.instance.PlaySFX(SFX.CORRECT_WORD);
    }

    public void Error()
    {
        for (int i = 0; i < contentList.Count; i++)
        {
            ErrorLetter(IT_to_X(i));
        }

        AudioManager.instance.PlaySFX(SFX.WRONG_WORD);
    }


    private void CorrectLetter(int xPosition)
    {
        TurnTileTo(SpriteMode.CORRECT, xPosition);
        StartCoroutine(TurnBackTile(xPosition));
    }


    private void ErrorLetter(int xPosition)
    {
        TurnTileTo(SpriteMode.ERROR, xPosition);
        CharacterCommentHandler.instance.Call("کلمه بی معنی");
        StartCoroutine(TurnBackTile(xPosition));
    }

    private void TurnTileTo(SpriteMode type, int xPosition)
    {
        try
        {
            RectTransform tile = contentList.GetChildren()[IT_to_AP(X_to_IT(xPosition))];
            var code = tile.GetComponent<EditorLetterHandler>().code;
            tile.GetComponent<Image>().sprite = typeToTile[type];
            tile.GetChild(1).GetComponent<Image>().sprite =
                GameManager.instance.GetLetterSprite(code, type);
        }
        catch (IndexOutOfRangeException e)
        {
        }
    }


    private readonly WaitForSeconds t = new WaitForSeconds(1);

    private IEnumerator TurnBackTile(int xPosition)
    {
        yield return t;

        TurnTileTo(SpriteMode.NORMAL, xPosition);
    }


    /*****************************/
    /**
     * Converters
     * xPosition - inText - absolute
     *
     *
     * xPosition:
     *        س     ر     ا     ب       
     *   0  1  2  3  4  5  6  7  8 
     * */

    internal int X_to_IT(int xPosition)
    {
        return currentText.Length - ((xPosition + 1) / 2);
    }

    internal int IT_to_X(int inTextPosition)
    {
        return currentText.Length * 2 - (inTextPosition * 2 + 1);
    }

    internal int IT_to_AP(int inTextPosition)
    {
        return currentText.Length - inTextPosition - 1;
    }

    internal int AP_to_IT(int absPosition)
    {
        return currentText.Length - absPosition - 1;
    }


    public void ResetPluses()
    {
        pluses.Reset();
    }
}