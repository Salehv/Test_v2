using TheGame;

public class GameProgression
{
    private ChapterProgression[] chapterProg;

    public GameProgression()
    {
        int chapterCount = GameManager.instance.chapters.Length;
        chapterProg = new ChapterProgression[chapterCount];
        for (int i = 0; i < chapterCount; i++)
        {
            chapterProg[i] = new ChapterProgression(i, GameManager.instance.chapters[i].levels.Length);
        }
    }

    internal void UpdateLevelProgress(LevelProgression lp)
    {
        chapterProg[lp.chapterid].levelProg[lp.levelid] = lp;
    }

    internal LevelProgression GetLevelProgress(int chapid, int lvlid)
    {
        return chapterProg[chapid].levelProg[lvlid];
    }

    internal LevelProgression GetLevelProgress(Level lvl)
    {
        return GetLevelProgress(lvl.chapterId, lvl.id);
    }
    
    internal Level GetLastLevel()
    {
        Level last = GameManager.instance.GetChapter(0).GetLevel(0);
        
        for(int i = 0; i < chapterProg.Length; i++)
            if(chapterProg[i] != null)    
                for(int j = 0; j < chapterProg[i].levelProg.Length; j++)
                    if (chapterProg[i].levelProg[j] != null)
                        last = GameManager.instance.GetChapter(i).GetLevel(j);
                    else
                        return GameManager.instance.GetChapter(i).GetLevel(j);;
        return last;
    }
}

public class ChapterProgression
{
    public LevelProgression[] levelProg;

    public ChapterProgression(int id, int length)
    {
        levelProg = new LevelProgression[length];
    }
}


public class LevelProgression
{
    public int chapterid;
    public int levelid;
    public int gemTaken;
    public int solvedsteps;
    public bool unlocked;

    public LevelProgression(int chapid, int lvlid, int gem, int solvedsteps)
    {
        chapterid = chapid;
        levelid = lvlid;
        gemTaken = gem;
        this.solvedsteps = solvedsteps;
    }
}