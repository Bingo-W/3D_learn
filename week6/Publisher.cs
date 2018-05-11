using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Subject
{
    void notify(StateOfActor state, int pos, GameObject actor);
    //发布函数

    void add(Observer_one observer);
    //委托添加函数

    void delete(Observer_one observer);
    //委托取消函数
}

public interface Observer_one
{
    void notified(StateOfActor state, int pos, GameObject actor);
    //实现接收函数
}


public enum StateOfActor { ENTER_AREA, DEATH }
//状态


public class Publisher : Subject
{

    private delegate void ActionUpdate(StateOfActor state, int pos, GameObject actor);
    private ActionUpdate updatelist;
    //存储状态,委托定义

    private static Subject _instance;
	// Use this for initialization
	
    public static Subject getInstance()
    {
        if(_instance == null)
        {
            _instance = new Publisher();
        }
        return _instance;
    }

    public void notify(StateOfActor state, int pos, GameObject actor)
    {
        if(updatelist != null)
        {
            updatelist(state, pos, actor);
        }
    }
    //发布函数

    public void add(Observer_one observer)
    {
        updatelist += observer.notified;
    }
    //委托添加函数

    public void delete(Observer_one observer)
    {
        updatelist -= observer.notified;
    }

}
