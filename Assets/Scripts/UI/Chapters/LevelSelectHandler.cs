using UnityEngine;
using UnityEngine.UI;

public class LevelSelectHandler : MonoBehaviour
{
    public Text begin;
    public Text end;
    public Text lvlNumber;

    public Image[] stars;

    public Sprite emptyStar;
    public Sprite lightedStar;

    public void Init(Level lvl)
    {
        lvlNumber.text = lvl.id + "";
        begin.text = lvl.begin;
        end.text = lvl.end;
        LightenStars(lvl.gems);
    }

    internal void LightenStars(int s)
    {
        switch (s)
        {
            case 0:
                stars[0].sprite = stars[1].sprite = stars[2].sprite = emptyStar;
                break;
            case 1:
                stars[1].sprite = stars[2].sprite = emptyStar;
                stars[0].sprite = lightedStar;
                break;
            case 2:
                stars[2].sprite = emptyStar;
                stars[0].sprite = stars[1].sprite = lightedStar;
                break;
            case 3:
                stars[0].sprite = stars[1].sprite = stars[2].sprite = lightedStar;
                break;
        }
    }
}