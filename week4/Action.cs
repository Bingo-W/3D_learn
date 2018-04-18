using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class Action : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

namespace mygame
{
    public interface ActionCallback
    {
        void actionDone(ObjAction source);
    }

    public class ActionManager : MonoBehaviour, ActionCallback
    {
        private Dictionary<int, ObjAction> actions = new Dictionary<int, ObjAction>();
        private List<ObjAction> waitingToAdd = new List<ObjAction>();
        private List<int> watingToDelete = new List<int>();

        protected void Update()
        {
            foreach (ObjAction ac in waitingToAdd)
            {
                actions[ac.GetInstanceID()] = ac;
            }
            waitingToAdd.Clear();

            foreach (KeyValuePair<int, ObjAction> kv in actions)
            {
                ObjAction ac = kv.Value;
                if (ac.destroy)
                {
                    watingToDelete.Add(ac.GetInstanceID());
                }
                else if (ac.enable)
                {
                    ac.Update();
                }
            }

            foreach (int key in watingToDelete)
            {
                ObjAction ac = actions[key];
                actions.Remove(key);
                DestroyObject(ac);
            }
            watingToDelete.Clear();
        }

        public void addAction(GameObject gameObject, ObjAction action, ActionCallback whoToNotify)
        {
            action.gameObject = gameObject;
            action.transform = gameObject.transform;
            action.whoToNotify = whoToNotify;
            waitingToAdd.Add(action);
            action.Start();
        }

        public void removeActionOf(GameObject gameObj)
        {
            foreach (KeyValuePair<int, ObjAction> kv in actions)
            {
                if (kv.Value.gameObject == gameObj)
                {
                    kv.Value.destroy = true;
                    kv.Value.enable = false;
                }
            }
        }

        public void actionDone(ObjAction source)
        {
            print("actionDone: " + source);
        }

    }

    public class FirstSceneActionManager : ActionManager
    {
        public void addRandomAction(GameObject gameObj, float speed)
        {
            Vector3 currentPos = gameObj.transform.position;
            Vector3 randomTarget1 = new Vector3(
                Random.Range(currentPos.x - 7, currentPos.x + 7),
                Random.Range(1, currentPos.y + 5),
                Random.Range(currentPos.z - 7, currentPos.z + 7)
                );
            MoveToAction moveAction1 = MoveToAction.getAction(randomTarget1, speed);

            Vector3 randomTarget2 = new Vector3(
                Random.Range(currentPos.x - 7, currentPos.x + 7),
                Random.Range(1, currentPos.y + 5),
                Random.Range(currentPos.z - 7, currentPos.z + 7)
                );
            MoveToAction moveAction2 = MoveToAction.getAction(randomTarget2, speed);

            SequenceAction sequenceAction = SequenceAction.getAction(new List<ObjAction> { moveAction1, moveAction2 }, -1);

            addAction(gameObj, sequenceAction, this);
        }

        public void addRandomActionForArr(UFOController[] arr, float speed)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                addRandomAction(arr[i].getObj(), speed);
            }
        }
    }

    public class MoveToAction : ObjAction
    {
        public Vector3 target;
        public float speed;

        private MoveToAction() { }
        public static MoveToAction getAction(Vector3 target, float speed = 5F)
        {
            MoveToAction action = ScriptableObject.CreateInstance<MoveToAction>();
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
                this.whoToNotify.actionDone(this);
            }
        }

        public override void Start()
        {
            //
        }

    }

    public class ObjAction : ScriptableObject
    {

        public bool enable = true;
        public bool destroy = false;

        public GameObject gameObject;
        public Transform transform;
        public ActionCallback whoToNotify;

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }
    public class SequenceAction : ObjAction, ActionCallback
    {
        public List<ObjAction> sequence;
        public int repeat = 1; // 1->only do it for once, -1->repeat forever
        public int currentActionIndex = 0;

        public static SequenceAction getAction(List<ObjAction> sequence, int repeat = 1, int currentActionIndex = 0)
        {
            SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
            action.sequence = sequence;
            action.repeat = repeat;
            action.currentActionIndex = currentActionIndex;
            return action;
        }

        public override void Update()
        {
            if (sequence.Count == 0) return;
            if (currentActionIndex < sequence.Count)
            {
                sequence[currentActionIndex].Update();
            }
        }

        public void actionDone(ObjAction source)
        {
            source.destroy = false;
            this.currentActionIndex++;
            if (this.currentActionIndex >= sequence.Count)
            {
                this.currentActionIndex = 0;
                if (repeat > 0) repeat--;
                if (repeat == 0)
                {
                    this.destroy = true;
                    this.whoToNotify.actionDone(this);
                }
            }
        }

        public override void Start()
        {
            foreach (ObjAction action in sequence)
            {
                action.gameObject = this.gameObject;
                action.transform = this.transform;
                action.whoToNotify = this;
                action.Start();
            }
        }

        void OnDestroy()
        {
            foreach (ObjAction action in sequence)
            {
                DestroyObject(action);
            }
        }
    }
}