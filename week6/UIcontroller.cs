using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIcontroller{

    public Text centerText;

    //失败的UI
    public void loseGame()
    {
        centerText.text = "Lose!";
    }

    //重置
    public void resetGame()
    {
        centerText.text = "";
    }
}
