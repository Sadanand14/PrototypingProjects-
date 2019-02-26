//Sadanand
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour {

    private int[] scoreList = { 0, 0, 0, 0, 0, 0 };
    private TimerScript timer;
    public Text[] textList;
    private Tower tower;
    private bool gameOver = false;
    public GameObject EndGame;
    // Use this for initialization
    void Start() {
        tower = GameObject.FindObjectOfType<Tower>();
        setFields();
    }

    // Update is called once per frame
    void Update() {
        if (!gameOver)
        {
            UpdateScores();
        }
    }

    public float Get1() { return scoreList[0]; }
    public float Get2() { return scoreList[1]; }
    public float Get3() { return scoreList[2]; }
    public float Get4() { return scoreList[3]; }
    public float Get5() { return scoreList[4]; }
    public float Get6() { return scoreList[5]; }

    public void Set1(int x) { scoreList[0] += x; }
    public void Set2(int x) { scoreList[1] += x; }
    public void Set3(int x) { scoreList[2] += x; }
    public void Set4(int x) { scoreList[3] += x; }
    public void Set5(int x) { scoreList[4] += x; }
    public void Set6(int x) { scoreList[5] += x; }

    void setFields()
    {
        timer = GameObject.FindObjectOfType<TimerScript>();
        timer.SetMaxtTime(60f); //  Set Timer MaxVALUE

        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = scoreList[i].ToString();
            GameObject player = GameObject.Find("Player" + (i + 1));
            player.GetComponent<TowerCube>().SetID(i);
            if (player.GetComponent<Transform>().parent.gameObject != GameObject.Find("Tower"))
            {
                //player.SetActive(false);
            }
        }
    }

    private void UpdateScores()
    {
        for (int i = 0; i< tower.playerCubes.Count;i++)
        {
            int x = tower.playerCubes[i].GetComponent<TowerCube>().GetID();
            LogScore(x,i);
            setScore();
        }
    }
    private void setScore()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = scoreList[i].ToString();
        }
    }
    public void LogScore(int ID, int score)
    {
        if (ID == 0)
            Set1(score);
        if (ID == 1)
            Set2(score);
        if (ID == 2)
            Set3(score);
        if (ID == 3)
            Set4(score);
        if (ID == 4)
            Set5(score);
        if (ID == 5)
            Set6(score);
    }

    public void GameOver()
    {
        gameOver = true;
        EndGame.SetActive(true);
        EndGame.GetComponent<Text>().text = "GameOver!";
    }

    public void GameComplete()
    {
        gameOver = true;
        EndGame.SetActive(true);
        int x = GetHighest();
        if (x == 0)
            EndGame.GetComponent<Text>().text = "Player 1 Wins!";
        if (x == 1)
            EndGame.GetComponent<Text>().text = "Player 2 Wins!";
        if (x == 2)
            EndGame.GetComponent<Text>().text = "Player 3 Wins!";
        if (x == 3)
            EndGame.GetComponent<Text>().text = "Player 4 Wins!";
        if (x == 4)
            EndGame.GetComponent<Text>().text = "Player 5 Wins!";
        if (x == 5)
            EndGame.GetComponent<Text>().text = "Player 6 Wins!";

    }
    public int GetHighest()
    {
        int x = 0;
        int index = 0;
        for (int i = 0; i<scoreList.Length; i++)
        {
            if (x < scoreList[i])
            {
                x = scoreList[i];
                index = i;
            }
        }
        return index;
    }

}
