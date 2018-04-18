using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class UFOFactory : MonoBehaviour {

    Queue<UFOController> freeUFO;  //free UFO
    List<UFOController> usingUFO; //using UFO

    GameObject originalUFO; // the original of UFO
    int count = 0;

    private void Awake()
    {
        freeUFO = new Queue<UFOController>();
        usingUFO = new List<UFOController>();

        originalUFO = Instantiate(Resources.Load("UFO", typeof(GameObject))) as GameObject;
        originalUFO.SetActive(false);// this one is not active at the begining
    }

    //product one UFO
   public UFOController produceUFO(UFOAttributes attr)
    {
        UFOController newUFO;
        if(freeUFO.Count == 0)
        {
            GameObject newone = GameObject.Instantiate(originalUFO);

            newUFO = new UFOController(newone);
            newone.transform.position += Vector3.forward * Random.value * 5;//set the position
            count++;
        }
        else
        {
            newUFO = freeUFO.Dequeue();
        }
        newUFO.setAttr(attr);   //set the property of the new UFO 
        usingUFO.Add(newUFO);
        newUFO.appear();
        return newUFO;
    }

    //product several UFOs
    public UFOController[] produceUFOs(UFOAttributes attr, int n)
    {
        UFOController[] some = new UFOController[n];
        for (int i = 0; i < n; i++)
        {
            some[i] = produceUFO(attr);
        }
        return some;
    }

    public void recyle(UFOController UFOone)
    {
        UFOone.disappear();
        usingUFO.Remove(UFOone);
        freeUFO.Enqueue(UFOone);
    }

	public void recyleAll()
    {
        while(usingUFO.Count != 0)
        {
            recyle(usingUFO[0]);
        }
    }

    public List<UFOController> getListUFO()
    {
        return usingUFO;
    }


	// Update is called once per frame
	void Update () {
	    
	}
}
