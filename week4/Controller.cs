using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;


namespace mygame
{   
    //定义各种接口
    public interface ISceneController
    {
        //
    }

    public class SSDirector : System.Object
    {
        private static SSDirector _instance;

        public ISceneController currentScenceController { get; set; }
        public bool running { get; set }

        // get instance
        public static SSDirector getInstance()
        {
            if (_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }
    }

    public class FirstController : MonoBehaviour, ISceneController
    {
        SSDirector director;
        UFOFactory Ufactory;

        //爆炸管理样例
        //动作管理样例
        //得分样例
        //难度管理样例

        float timeAfterRoundStart = 10;
        bool roundHasStart = false;
        
        void Awake()
        {
            //挂载组件

            director = SSDirector.getInstance();
            director.currentScenceController = this;

            //动作管理
            //爆炸
            //分数
            //难度管理
            Ufactory = gameObject.AddComponent();
            loadResource();
        }

        public void loadResource()
        {
            //load the init resource
            new FirstController();
            Instantiate(Resources.Load("Terrain"));//load the map
        }



        // Use this for initialization
        void Start()
        {
            roubdStart();
        }

        // Update is called once per frame
        void Update()
        {
            if(roundHasStart)
            {
                timeAfterRoundStart += Time.deltaTime;
            }

            if(roundHasStart && checkAllShot())
            {
                print("All UFO is shot down! Next round in 3 sec");
                roundHasStart = false;
                
            }
        }

        void roundStart()
        {
            // the new game
            roundHasStart = true;
            timeAfterRoundStart = 0;
            UFOController[] ufoArr = UFOFactory.produceUFOs();

            for(int i = 0; i < ufoArr.Length; i++)
            {
                ufoArr[i].appear();
            }

            //动作管理器内容，生产UFO
        }

        bool checkTimeOut()
        {
            if(timeAfterRoundStart > difficultyManager.currentSendInterval)
            {
                return true;
            }
            return false;
        }

        bool checkAllShot()
        {
            return Ufactory.getListUFO().Count == 0;
        }

        public void UFOIsShot(UFOController UFOone)
        {
            //计分系统根据难度加分

            //动作管理系统去掉UFO

            Ufactory.recyle(UFOone);

            //传入UFO的位置，进行爆炸处理
        }

        public void GroundIsShot(Vector3 pos)
        {
            //爆炸管理器根据位置进行爆炸
        }
    }

}
