using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{
    public GameObject rank;
    public GameObject scorename;
    public GameObject score;

    public void SetScore(int rank, string name, int score)
    {
        this.rank.GetComponent<Text>().text = rank.ToString();
        this.scorename.GetComponent<Text>().text = name;
        this.score.GetComponent<Text>().text = score.ToString();
    }
}
