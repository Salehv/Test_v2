using UnityEngine;
using Mono.Data.SqliteClient;
using System.Data;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    internal static DatabaseManager instance;

    public Text debug;
    public InputField input;

    public InputField input2;
    public InputField input3;

    private string wordsConnectionString;
    private string progressConnectionString;


    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }

    private IDbConnection dbConnection;

    internal void Init()
    {
        wordsConnectionString = "URI=file:" + Application.persistentDataPath + "/words.db3";
        progressConnectionString = "URI=file:" + Application.persistentDataPath + "/progress.db3";


        try
        {
            dbConnection = new SqliteConnection(wordsConnectionString);
            dbConnection.Open();
        }
        catch (Exception e)
        {
            debug.text = e + "";
        }

        using (IDbConnection pConn = new SqliteConnection(progressConnectionString))
        {
            pConn.Open();
            using (IDbCommand cmd = pConn.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM `datas` WHERE `option`='gem';";
                cmd.CommandText = sqlQuery;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        int all = 0;
                        cmd.CommandText = "SELECT * FROM `progress`;";
                        using (IDataReader reader2 = cmd.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                all += reader2.GetInt32(2);
                            }
                        }

                        cmd.CommandText = string.Format("INSERT INTO `datas` VALUES ('gem',{0});", all);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }


    public bool IsCorrect(string word)
    {
        try
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM `words` WHERE `word`='" + word + "'";
                cmd.CommandText = sqlQuery;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    bool result = false;
                    while (reader.Read())
                    {
                        result = true;
                    }

                    return result;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void Dispose()
    {
        dbConnection.Close();
        dbConnection.Dispose();
    }


    public void ShowSimilar()
    {
        GetAllSimilarWords(input.text);
    }

    internal string[] GetAllSimilarWords(string word)
    {
        string related = "";
        List<string> similars = new List<string>();
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            string sqlQuery = string.Format("SELECT * FROM `words` WHERE `word`='{0}';", word);
            cmd.CommandText = sqlQuery;

            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    related = reader.GetString(3);
                }
            }

            string[] lds = related.Split(',');

            sqlQuery = "SELECT * FROM `words` WHERE FALSE";

            for (int i = 0; i < lds.Length; i++)
            {
                sqlQuery += " OR `id`='" + lds[i] + "'";
            }

            sqlQuery += ";";
            cmd.CommandText = sqlQuery;

            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    similars.Add(reader.GetString(1));
                }
            }
        }

        return similars.ToArray();
    }

    internal char[] GetPossibleChars(string s)
    {
        try
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM `words` WHERE `word`='" + s + "'";
                cmd.CommandText = sqlQuery;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string relateds = reader.GetString(2);
                        relateds = relateds.Replace(",", "");
                        return relateds.ToCharArray();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw e;
        }

        return null;
    }


    /**********  Progress  ************/
    internal static void CreateProgressDatabase(Text debug)
    {
        try
        {
            File.Create(Application.persistentDataPath + "/progress.db3");
            string connectionString = "URI=file:" + Path.Combine(Application.persistentDataPath, "progress.db3");

            IDbConnection dbConnection;
            using (dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();

                using (IDbCommand cmd = dbConnection.CreateCommand())
                {
                    string sqlQuery =
                        "CREATE TABLE `progress` ( `levelid` INTEGER UNIQUE, `solvedsteps` INTEGER, `startaken` INTEGER, `hint_used_1` INTEGER, `hint_used_2` INTEGER, `hint_used_3` INTEGER, `time_taken` REAL, `used_words` TEXT )";

                    cmd.CommandText = sqlQuery;
                    cmd.ExecuteNonQuery();

                    sqlQuery = "CREATE TABLE `datas` ( `option` TEXT UNIQUE, `value` INTEGER)";
                    cmd.Prepare();
                }
            }
        }
        catch (Exception e)
        {
            debug.text += e;
            throw e;
        }
    }

    internal static void CheckProgressDataBase(Text debug)
    {
        string connectionString = "URI=file:" + Path.Combine(Application.persistentDataPath, "progress.db3");

        IDbConnection dbConnection;
        using (dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "CREATE TABLE `datas` ( `option` TEXT UNIQUE, `value` INTEGER);";
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
        }
    }

    internal GameProgression GetProgressData()
    {
        GameProgression gp = new GameProgression();
        using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM `progress`;";
                cmd.CommandText = sqlQuery;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int lvlid = reader.GetInt32(0);
                        int chapid = lvlid / 100;
                        lvlid = lvlid % 100;
                        int ss = reader.GetInt32(1);
                        int st = reader.GetInt32(2);
                        bool unlocked = reader.GetInt32(4) == 1;
                        gp.UpdateLevelProgress(new LevelProgression(chapid, lvlid, st, ss, unlocked));
                    }
                }
            }
        }

        return gp;
    }

    internal void UpdateLevelProgress(LevelProgression lp)
    {
        /*
        if (!update)
        {
            using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
            {
                dbConnection.Open();

                using (IDbCommand cmd = dbConnection.CreateCommand())
                {
                    string sqlQuery = "INSERT INTO `progress` VALUES (";
                    sqlQuery += lp.chapterid * 100 + lp.levelid + ", ";
                    sqlQuery += lp.solvedsteps + ", ";
                    sqlQuery += lp.gemTaken + ", ";
                    sqlQuery += "0, ''";
                    sqlQuery += ");";

                    cmd.CommandText = sqlQuery;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        else
        {
        */
        using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "UPDATE `progress` SET `solvedSteps`=" + lp.solvedsteps + ", ";
                sqlQuery += "`star_taken`=" + lp.gemTaken + ", ";
                sqlQuery += "`unlocked`=" + lp.unlocked + " ";
                sqlQuery += "WHERE `levelid`=" + (lp.chapterid * 100 + lp.levelid) + ";";

                // print(sqlQuery);

                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
        }
    }

    internal void AddOrRemoveCoins(int count)
    {
        int coins = GetCoins();
        coins += count;

        if (coins < 0)
            throw new NoMoneyException();

        SetOption("coin", coins + "");
    }

    internal void AddOrRemoveGems(int count)
    {
        int gems = GetGems();
        gems += count;

        if (gems < 0)
            throw new NoMoneyException();

        SetOption("gem", gems + "");
    }

    private void SetOption(string name, string value)
    {
        using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = string.Format("UPDATE `datas` SET `value`='{0}' WHERE `option`='{1}'", value, name);

                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
        }
    }

    internal int GetCoins()
    {
        return GetOption("coin");
    }

    internal int GetGems()
    {
        return GetOption("gem");
    }

    private int GetOption(string name)
    {
        using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM `datas` WHERE `option`='" + name + "'";

                cmd.CommandText = sqlQuery;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        return reader.GetInt32(1);
                }
            }
        }

        return 0;
    }


    public void ResetProgress()
    {
        using (IDbConnection dbConnection = new SqliteConnection(progressConnectionString))
        {
            dbConnection.Open();

            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "UPDATE `progress` SET `solvedSteps`=-1, `star_taken`=0 WHERE TRUE;";
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
        }

        SetOption("gem", "0");
        SetOption("coin", "0");
    }
}

public class NoMoneyException : Exception
{
}