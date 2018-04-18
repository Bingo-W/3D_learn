using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class Shooter : MonoBehaviour {

    public Camera cam;
    private FirstController sceneController;
    LayerMask layer;

    //层次的界面
    public GameObject muzzleFlash;
    //枪口火焰的预制

    bool muzzleFlashEnable = false; //whether the fire is appear
    float muzzleFlashTimer = 0;//the time of the appearing time
    const float muzzleFlashMaxTime = 0.1F;

    private void Awake()
    {
        muzzleFlash.SetActive(false);
        layer = LayerMask.GetMask("Shoottable", "RayFinish");//指定这两个层，飞碟跟UFO
    }

    // Use this for initialization
    void Start () {

        cam = Camera.main;
        sceneController = SSDirector.getInstance().currentScenceController as FirstController;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Fire1"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
            {
                if(hit.transform.gameObject.layer == 8)
                {
                    UFOController UFOone = hit.transform.GetComponent<UFOScript>().ctrl;//拿到撞到的UFO的控制器
                    sceneController.UFOIsShot(UFOone);
                }
            }
        }
        if(muzzleFlashEnable == false)//显示枪口火焰
        {
            muzzleFlashEnable = true;
            muzzleFlash.SetActive(true);
        }
        if(muzzleFlashEnable)//判断是否消失
        {
            muzzleFlashEnable = false;
            muzzleFlash.SetActive(false);
            muzzleFlashTimer = 0;
        }
	}
}
