using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//产生爆炸的效果，避免多次单例化引发效率过低
public class ExplosionFactory : MonoBehaviour
{

    protected Queue<GameObject> freeQueue = new Queue<GameObject>();
    protected List<GameObject> usingList = new List<GameObject>();
    public GameObject original;


    void Start()
    {
        original = GameObject.Instantiate(Resources.Load("Explosion")) as GameObject;
        original.SetActive(false);
    }
    public void explodeAt(Vector3 pos)
    {
        GameObject newExplosion;
        if (freeQueue.Count == 0)
        {
            newExplosion = GameObject.Instantiate(original);
            newExplosion.AddComponent<SelfRecycle>().factory = this;
        }
        else
        {
            newExplosion = freeQueue.Dequeue();
        }
        usingList.Add(newExplosion);

        SelfRecycle selfRecycle = newExplosion.GetComponent<SelfRecycle>();
        selfRecycle.startTimer(1.2F);

        newExplosion.SetActive(true);
        newExplosion.transform.position = pos;
    }

    public void recycle(GameObject explosion)
    {
        explosion.SetActive(false);
        freeQueue.Enqueue(explosion);
    }
}

public class SelfRecycle : MonoBehaviour
{
    // 这个类挂载在爆炸对象上，让爆炸对象过一段时间以后自动回收自己
    public ExplosionFactory factory;

    public void startTimer(float time)
    {
        Invoke("selfRecycle", time);
    }

    private void selfRecycle()
    {
        factory.recycle(gameObject);
    }
}