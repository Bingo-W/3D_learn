using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SceneController : MonoBehaviour, Observer_one
{

    public Text scoreText;
    public Text centerText;

    private ScoreRecorder record;
    private UIcontroller UI;
    private Factory fac;

    private float[] posx = { -5, 7, -5, 5 };
    private float[] posz = { -5, -7, 5, 5 };
    //预制加载的位置

	// Use this for initialization
	void Start () {

        record = new ScoreRecorder();
        record.scoreText = scoreText;
        UI = new UIcontroller();
        UI.centerText = centerText;
        fac = Singleton<Factory>.Instance;

        Subject publisher = Publisher.getInstance();
        publisher.add(this);
        // 添加事件

        LoadResources();

    }

    private void LoadResources()
    {
        Instantiate(Resources.Load("prefabs/Ami"), new Vector3(2, 1, -2), Quaternion.Euler(new Vector3(0, 180, 0)));
        // 初始化主角
 
        Factory fac = Singleton<Factory>.Instance;
        for (int i = 0; i < posx.Length; i++)
        {
            
            GameObject patrol = fac.setOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
            patrol.name = "Patrol" + (i + 1);
            // 初始化巡逻兵
        }
    }

    public void notified(StateOfActor state, int pos, GameObject actor)
    {
        if(state == StateOfActor.ENTER_AREA)
        {
            //分数加1
        }
        else
        {
            //失败
        }
    }
}
