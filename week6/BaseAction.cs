using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tem.Action
{
    public enum SSActionState : int { STARTED, COMPLETED}

    public interface ISSActionCallback
    {
        void SSEventAction(SSAction source, SSActionState events = SSActionState.COMPLETED, int intParam = 0, string strParam = null, Object obj = null);
    }

    public class SSAction : ScriptableObject
    {
        public bool enable = true;
        public bool destory = false;

        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }
        public ISSActionCallback callback { get; set; }


        //只能调用子类的重写函数，否则将会报错
        public virtual void Start()
        {
            throw new System.NotImplementedException("Action Start Error!");
        }

        public virtual void FixedUpdate()
        {
            throw new System.NotImplementedException("Physics Action Start Error!");
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException("Action Update Error!");
        }
    }

    //???
    public class CCSequenceAction : SSAction, ISSActionCallback
    {
        public List<SSAction> sequence;
        public int repeat = -1;
        public int start = 0;

        public static CCSequenceAction GetSSAction(List<SSAction> _sequence, int _start = 0, int _repead = 1)
        {
            CCSequenceAction actions = ScriptableObject.CreateInstance<CCSequenceAction>();
            actions.sequence = _sequence;
            actions.start = _start;
            actions.repeat = _repead;
            return actions;
        }

        public override void Start()
        {
            foreach (SSAction ac in sequence)
            {
                ac.gameObject = this.gameObject;
                ac.transform = this.transform;
                ac.callback = this;
                ac.Start();
            }
        }

        public override void Update()
        {
            if (sequence.Count == 0) return;
            if (start < sequence.Count) sequence[start].Update();
        }

        public void SSEventAction(SSAction source, SSActionState events = SSActionState.COMPLETED,
            int intParam = 0, string strParam = null, Object objParam = null) //通过对callback函数的调用执行下个动作
        {
            source.destory = false; // 当前动作不能销毁（有可能执行下一次）
            this.start++;
            if (this.start >= this.sequence.Count)
            {
                this.start = 0;
                if (this.repeat > 0) repeat--;
                if (this.repeat == 0)
                {
                    this.destory = true;
                    this.callback.SSEventAction(this);
                }
            }
        }

        private void OnDestroy()
        {
            this.destory = true;
        }
    }


    //站立的动作
    public class IdleAction : SSAction
    {
        private float time;
        //站立的时间
        private Animator ani;

        public static IdleAction GetIdleAction(float time, Animator ani)
        {
            IdleAction currentAction = ScriptableObject.CreateInstance<IdleAction>();
            currentAction.time = time;
            currentAction.ani = ani;
            return currentAction;
        }

        public override void Start()
        {
            ani.SetFloat("Speed", 0);
            // 进入站立状态
        }

        public override void Update()
        {
            if (time == -1) return;
            // 永久站立
            time -= Time.deltaTime;
            // 减去时间
            if (time < 0)
            {
                this.destory = true;
                this.callback.SSEventAction(this);
            }
        }
    }

    //巡逻时的动作
    public class WalkAction : SSAction
    {
        private float speed;
        private Vector3 target;
        private Animator ani;
        // 移动速度和目标的地点

        public static WalkAction GetWalkAction(Vector3 target, float speed, Animator ani)
        {
            WalkAction currentAction = ScriptableObject.CreateInstance<WalkAction>();
            currentAction.speed = speed;
            currentAction.target = target;
            currentAction.ani = ani;
            return currentAction;
        }

        public override void Start()
        {
            ani.SetFloat("Speed", 0.5f);
            // 进入走路状态
        }

        public override void Update()
        {
            Quaternion rotation = Quaternion.LookRotation(target - transform.position);
            if (transform.rotation != rotation) transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed * 5);
            // 进行转向，转向目标方向

            this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
            if (this.transform.position == target)
            {
                this.destory = true;
                this.callback.SSEventAction(this);
            }
        }
    }

    //追击时的动作
    public class RunAction : SSAction
    {
        private float speed;
        private Transform target;
        private Animator ani;
        // 移动速度和人物的transform

        public static RunAction GetRunAction(Transform target, float speed, Animator ani)
        {
            RunAction currentAction = ScriptableObject.CreateInstance<RunAction>();
            currentAction.speed = speed;
            currentAction.target = target;
            currentAction.ani = ani;
            return currentAction;
        }

        public override void Start()
        {
            ani.SetFloat("Speed", 1);
            // 进入跑步状态
        }

        public override void Update()
        {
            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            if (transform.rotation != rotation) transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed * 5);
            // 转向

            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(this.transform.position, target.position) < 0.5)
            {
                this.destory = true;
                this.callback.SSEventAction(this);
            }
        }
    }

    //动作管理类
    public class SSActionManager : MonoBehaviour
    {
        //用字典来储存相关的指令
        private Dictionary<int, SSAction> diction = new Dictionary<int, SSAction>();
        private List<SSAction> AddAction = new List<SSAction>();
        private List<int> DeleteAction = new List<int>();

        protected void Start()
        {
            
        }

        protected void Update()
        {
            foreach(SSAction ac in AddAction)
            {
                diction[ac.GetInstanceID()] = ac;
            }
            AddAction.Clear();
            //将要进行的动作加入到执行的字典中

            //将要删除的加到删除列表中
            foreach(KeyValuePair<int, SSAction> dic in diction)
            {
                SSAction ac = dic.Value;
                if(ac.destory == true)
                {
                    DeleteAction.Add(ac.GetInstanceID());
                }
                else if(ac.enable == true) {
                    ac.Update();
                }
            }

            //将删除列表中的元素进行删除
            foreach(int id in DeleteAction)
            {
                SSAction ac = diction[id];
                diction.Remove(id);
                DestroyObject(ac);
            }
            DeleteAction.Clear();
        }

        //追赶的时候，由于玩家的位置不断变化，因此需要不断地进行更新
        public void runAction(GameObject gameObject, SSAction action, ISSActionCallback callback)
        {
            action.gameObject = gameObject;
            action.transform = gameObject.transform;
            action.callback = callback;
            AddAction.Add(action);
            action.Start();
        }
    }

    public class PYActionManager : MonoBehaviour
    {
        private Dictionary<int, SSAction> dictionary = new Dictionary<int, SSAction>();
        private List<SSAction> watingAddAction = new List<SSAction>();
        private List<int> watingDelete = new List<int>();

        protected void Start()
        {

        }

        protected void FixedUpdate()
        {
            foreach (SSAction ac in watingAddAction) dictionary[ac.GetInstanceID()] = ac;
            watingAddAction.Clear();
            // 将待加入动作加入dictionary执行

            foreach (KeyValuePair<int, SSAction> dic in dictionary)
            {
                SSAction ac = dic.Value;
                if (ac.destory) watingDelete.Add(ac.GetInstanceID());
                else if (ac.enable) ac.FixedUpdate();
            }
            // 如果要删除，加入要删除的list，否则更新

            foreach (int id in watingDelete)
            {
                SSAction ac = dictionary[id];
                dictionary.Remove(id);
                DestroyObject(ac);
            }
            watingDelete.Clear();
            // 将deletelist中的动作删除
        }

        public void runAction(GameObject gameObject, SSAction action, ISSActionCallback callback)
        {
            action.gameObject = gameObject;
            action.transform = gameObject.transform;
            action.callback = callback;
            watingAddAction.Add(action);
            action.Start();
        }
    }
}
