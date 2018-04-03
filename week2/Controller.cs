using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

namespace mygame
{
    public enum State { Start, SE_move, ES_move, End, Win, Lose };

    public interface UserAction {
        void priestSOnB();
        void priestEOnB();
        void devilSOnB();
        void devilEOnB();
        void moveShip();
        void offShipL();
        void offShipR();
        void reset();
    }

    public class SSDirector : System.Object, UserAction
    {
        private static SSDirector _instance;

        public Controller currentScenceController;
        public State state = State.Start;
        private Model game_obj;

        public static SSDirector GetInstance()
        {
            if(_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }

        public Model getModel()
        {
            return game_obj;
        }
        
        internal void setModel(Model someone)
        {
            if(game_obj == null)
            {
                game_obj = someone;
            }
        }

        public void priestSOnB()
        {
            game_obj.priS();
        }
        public void priestEOnB()
        {
            game_obj.priE();
        }
        public void devilSOnB()
        {
            game_obj.delS();
        }
        public void devilEOnB()
        {
            game_obj.delE();
        }
        public void moveShip()
        {
            game_obj.moveShip();
        }
        public void offShipL()
        {
            game_obj.getOffTheShip(0);
        }
        public void offShipR()
        {
            game_obj.getOffTheShip(1);
        }
        public void reset()
        {
            //暂时不写，待会回来进行定义
            //_instance = null;
            state = State.Start;
            game_obj.Reset_all();

        }
}

        
}

public class Controller : MonoBehaviour {

    // Use this for initialization

    void Start()
    {
        SSDirector one = SSDirector.GetInstance();
    }

}
