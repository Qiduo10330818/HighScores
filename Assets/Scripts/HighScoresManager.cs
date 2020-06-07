using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class HighScoresManager : MonoBehaviour
{
    private string connectionString;
    private List<HighScore> highScores = new List<HighScore>();

    public GameObject scorePrefab;
    public Transform scoreParent;
    public int topRanks;
    public int saveScores;
    // Start is called before the first frame update
    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/Highscores.db";
        DeleteExtraScores();
        //UpdateScore(19, 1);
        ShowScores();

    }

    private void GetScores()
    {
        highScores.Clear();
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM HighScores";
                dbCmd.CommandText = sqlQuery;
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highScores.Add(new HighScore(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDateTime(3)));
                        //Debug.Log(reader.GetInt32(0)+" "+reader.GetString(1)+" "+reader.GetInt32(2));
                    }
                    dbConnection.Close();
                    reader.Close();
                }
            }
        }
        highScores.Sort();


    }
    
    private void InsertScore(int id, string name, int newScore)
    {
        GetScores();
        int hsCount = highScores.Count;
        if (hsCount > 0)
        {
            HighScore lowestScore = highScores[highScores.Count - 1];
            if(lowestScore!=null && saveScores>0 && highScores.Count >= saveScores && newScore > lowestScore.Score)
            {
                DeleteScore(lowestScore.ID);
                hsCount--;
            }

        }
        if (hsCount < saveScores)
        {
            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    string sqlQuery = String.Format("INSERT INTO HighScores(PlayerID, Username, Score) VALUES(\"{0}\", \"{1}\", \"{2}\")", id, name, newScore);
                    dbCmd.CommandText = sqlQuery;
                    dbCmd.ExecuteScalar();
                    dbConnection.Close();

                }
            }
        }
        highScores.Sort();

    }

    private void DeleteScore(int id)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", id);
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();

            }
        }
    }

    private void ShowScores()
    {
        //highScores.Sort();
        for(int i=0; i<Math.Min(highScores.Count, topRanks); i++)
        {
            //GameObject tmpObject = Instantiate(scorePrefab, scoreParent.transform);
            GameObject tmpObject = Instantiate(scorePrefab);
            HighScore tmpScore = highScores[i];
            tmpObject.GetComponent<HighScoreScript>().SetScore((i+1), tmpScore.Name, tmpScore.Score);
            tmpObject.transform.SetParent(scoreParent);
            tmpObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    private void DeleteExtraScores()
    {
        GetScores();
        
        if (saveScores < highScores.Count)
        {
            int deleteCount = highScores.Count - saveScores;
            highScores.Reverse();
            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                    for (int i = 0; i< deleteCount; i++)
                    {
                        string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", highScores[i].ID);
                        dbCmd.CommandText = sqlQuery;
                        dbCmd.ExecuteScalar();
                       
                    }
                dbConnection.Close();
            }
        }
        highScores.Sort();


    }

    private void UpdateScore(int id, int scoreChange) // enter scoreCHANGE
    {
        int oldScore = 0;
        if (IDAlreadyExists(id))
        {
            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();
                //if (IDAlreadyExists(id))
                //{
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    // get old score
                    string sqlQuery = String.Format("SELECT * FROM HighScores WHERE PlayerID = \"{0}\"", id);
                    dbCmd.CommandText = sqlQuery;
                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oldScore = reader.GetInt32(2);
                        }
                        reader.Close();
                    }
                    Debug.Log("old score = " + oldScore);
                    // update score
                    int newScore = oldScore + scoreChange;
                    sqlQuery = String.Format("UPDATE HighScores SET Score = \"{0}\" WHERE PlayerID = \"{1}\"", newScore, id);
                    dbCmd.CommandText = sqlQuery;
                    dbCmd.ExecuteScalar();
                }
                dbConnection.Close();
            }
        }
        else
        {
            Debug.Log("No player has ID " + id);
        }
        GetScores();
        
    }

    private bool IDAlreadyExists(int id)
    {
        int exists = 0;
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {

                string sqlQuery = String.Format("SELECT EXISTS(SELECT * from HighScores WHERE PlayerID = \"{0}\")", id);
                dbCmd.CommandText = sqlQuery;
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        exists = reader.GetInt32(0);
                    }
                }
            }
            dbConnection.Close();
        }
        if (exists == 1)
        {
            return true;
        }
        return false;
    }
}
