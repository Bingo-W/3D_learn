using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;


namespace mygame
{
    //定义各种接口
    public interface ISceneController
    {
        void loadResource();
    }

    public class SSDirector : System.Object
    {
        private static SSDirector _instance;

        public ISceneController currentScenceController { get; set; }
        public bool running { get; set; }

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
        int score = 0;
        private int difficulty = 0;
        public readonly int UFOnum = 10;

        //难度安排
        float[] sendUFOInterval = { 10, 9, 8 };
        float[] UFOScale = { 0.9F, 0.9F, 0.9F };
        float[] UFOSpeed = { 5, 8, 10 };
        Color[] UFOColor = { Color.red, Color.blue, Color.gray };
        ExplosionFactory explosionFactory;
        FirstSceneActionManager actionManager;


        float timeAfterRoundStart = 10;
        bool roundHasStart = false;

        void Awake()
        {
            //挂载组件

            director = SSDirector.getInstance();
            director.currentScenceController = this;

            //Ufactory = gameObject.AddComponent<UFOFactory>();
            //explosionFactory = gameObject.AddComponent<ExplosionFactory>();
            loadResource();
            
        }

        public void loadResource()
        {
            //load the init resource
            Instantiate(Resources.Load("Terrain"));//load the map
        }



        // Use this for initialization
        void Start()
        {
            roundStart();
        }

        // Update is called once per frame
        void Update()
        {
            if (roundHasStart)
            {
                timeAfterRoundStart += Time.deltaTime;
            }

            if (roundHasStart && checkAllShot())
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

            UFOAttributes newone = new UFOAttributes(UFOColor[difficulty], UFOScale[difficulty], UFOSpeed[difficulty]);
            UFOController[] ufoArr = new UFOController[UFOnum];
            ufoArr = Ufactory.produceUFOs(newone, UFOnum);

            for (int i = 0; i < ufoArr.Length; i++)
            {
                ufoArr[i].appear();
            }

            //动作管理器内容，生产UFO
        }

        bool checkTimeOut()
        {
            if (timeAfterRoundStart > sendUFOInterval[difficulty])
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

            score++;//加多少分后面决定

            Ufactory.recyle(UFOone);

            //传入UFO的位置，进行爆炸处理
        }

        public void GroundIsShot(Vector3 pos)
        {
            //爆炸管理器根据位置进行爆炸
        }
    }

    public class UFOController
    {
        public UFOAttributes attr;
        GameObject gameObject;
        UFOScript script;

        public UFOController(GameObject _gameObject)
        {
            gameObject = _gameObject;
            script = _gameObject.AddComponent<UFOScript>();
            script.ctrl = this;
        }

        public void appear()
        {
            gameObject.SetActive(true);
        }
        public void disappear()
        {
            gameObject.SetActive(false);
        }

        public GameObject getObj()
        {
            return gameObject;
        }

        public void setAttr(UFOAttributes _attr)
        {
            attr = _attr;
            gameObject.transform.localScale = gameObject.transform.localScale * _attr.scale;
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = _attr.color;
            }
        }

    }

}
