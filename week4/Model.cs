using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class Model : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

//定义了UFO的相关属性
public class UFOAttributes
{
    public Color color;
    public float scale;
    public float speed;

    public UFOAttributes(Color _color, float _scale, float _speed)
    {
        color = _color; scale = _scale; speed = _speed;
    }
}

public class UFOScript : MonoBehaviour
{
    public UFOController ctrl;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            g.AddComponent<UFOScript>().ctrl = ctrl;
        }
    }
}