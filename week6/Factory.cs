using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour {


    private static List<GameObject> usedPatrol = new List<GameObject>();
    //正在使用的巡逻兵列表
    private static List<GameObject> freePatrol = new List<GameObject>();
    //空闲巡逻兵列表

    //在指定的位置上放置巡逻兵
    public GameObject setOnPos(Vector3 pos, Quaternion direction)
    {
        if(freePatrol.Count == 0)
        {
            GameObject aGameObject = Instantiate(Resources.Load("prefabs/Patrol")
                , pos, direction) as GameObject;
            // 新建实例，将位置设置成为targetposition，将面向方向设置成faceposition
            usedPatrol.Add(aGameObject);
            Debug.Log(aGameObject);
        }
        else
        {
            usedPatrol.Add(freePatrol[0]);
            freePatrol.RemoveAt(0);
            usedPatrol[usedPatrol.Count - 1].SetActive(true);
            usedPatrol[usedPatrol.Count - 1].transform.position = pos;
            usedPatrol[usedPatrol.Count - 1].transform.localRotation = direction;
        }
        return usedPatrol[usedPatrol.Count - 1];
    }

    public void removeUsed(GameObject obj)
    {
        obj.SetActive(false);
        usedPatrol.Remove(obj);
        freePatrol.Add(obj);
    }
}
