using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecorder {

    public Text scoreText;
    //计分的文本

    private int score = -1;
    //记录分数

    //重置分数，到时重置游戏需要
    public void resetScore()
    {
        score = -1;
    }

    //增加分数
    public void addScore(int add_one)
    {
        score += add_one;
        scoreText.text = "Score:" + score;
    }

    //reset所用
    public void setDisActive()
    {
        scoreText.text = "";
    }

    public void setActive()
    {
        scoreText.text = "Score:" + score;
    }

}
