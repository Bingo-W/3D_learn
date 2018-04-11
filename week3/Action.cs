using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;


public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null);
}

public class SSAction : ScriptableObject
{
    public bool enable = true;  //play this action or not
    public bool destroy = false;    //destory or not

    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }

    protected SSAction() { }

    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class SequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence;    
    public int repeat = -1;            //无限循环
    public int start = 0;              

    public static SequenceAction GetSSAcition(int repeat, int start, List<SSAction> sequence)
    {
        SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }

    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (start < sequence.Count)
        {
            sequence[start].Update();     //回调接口函数
        }
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        source.destroy = false;          
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destroy = true;               
                this.callback.SSActionEvent(this); 
            }
        }
    }

    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;                
            action.Start();
        }
    }

    void OnDestroy()
    {
        
    }
}

//动作管理器
public class SSActionManager : MonoBehaviour, ISSActionCallback                      //action管理器
{

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    //将执行的动作的字典集合,int为key，SSAction为value
    private List<SSAction> waitingAdd = new List<SSAction>();                       //等待去执行的动作列表
    private List<int> waitingDelete = new List<int>();                              //等待删除的动作的key                

    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;                                      //获取动作实例的ID作为key
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        //牧师与魔鬼的游戏对象移动完成后就没有下一个要做的动作了，所以回调函数为空
    }
}

//移动的动作基类
public class SSMoveToAction : SSAction
{
    public Vector3 target;
    public float speed;

    private SSMoveToAction() { }
    public static SSMoveToAction GetSSAction(Vector3 target, float speed)
    {
        SSMoveToAction action = ScriptableObject.CreateInstance<SSMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        if (this.transform.position == target)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }

    public override void Start()
    {
        //移动动作建立时候不做任何事情
    }
}

public class MySceneActionManager : SSActionManager
{
    public SSMoveToAction moveBoatToEndOrStart;     //移动船到结束岸，移动船到开始岸
    public SequenceAction moveRoleToLandorBoat;     //移动角色到陆地，移动角色到船上
    public Controller sceneController;             //当前场景的场景控制器

    protected new void Start()
    {
        sceneController = (Controller)SSDirector.GetInstance().currentScenceController;
        sceneController.actionManager = this;     //设置该场景控制器的动作管理者为自己
    }
    public void moveBoat(GameObject boat, Vector3 target, float speed)
    {
        moveBoatToEndOrStart = SSMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, moveBoatToEndOrStart, this);
    }

}



