using System.Collections.Generic;
using Initialize;

[System.Serializable]
public class Chapter
{
    private Dictionary<int, Level> lvls;
    internal int id;

    public Level[] levels;

    public int cost;
    public string name;

    private void AddLevel(int i, Level lvl)
    {
        if ((!lvls.ContainsKey(i - 1) && i != 0) || i != lvl.id || id != lvl.chapterId)
        {
            throw new System.Exception(string.Format("Bad Level: Chapter {0:d} Level {1:d}", id, lvl.id));
        }

        lvls.Add(i, lvl);
    }

    internal Level GetLevel(int id)
    {
        return lvls[id];
    }

    internal void InitChapter(XMLChapter chapter)
    {
        name = chapter.name;
        id = int.Parse(chapter.id);
        cost = int.Parse(chapter.cost);


        lvls = new Dictionary<int, Level>();
        levels = new Level[chapter.levels.Count];

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = new Level(
                i,
                id,
                0,
                chapter.levels[i].start,
                chapter.levels[i].end,
                0);

            if (chapter.levels[i].type == "1")
                levels[i].SetDynamicFlags(DynamicsFlag.DF_ONLY_CHANGE);
            else if (chapter.levels[i].type == "2")
                levels[i].SetDynamicFlags(DynamicsFlag.DF_CHANGE_ADD);
            else
                levels[i].SetDynamicFlags(DynamicsFlag.DF_FULL);

            levels[i].SetWay(chapter.levels[i].way.words.ToArray());

            AddLevel(i, levels[i]);
        }
    }
}