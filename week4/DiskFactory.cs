using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiskFactory : MonoBehaviour
{

    private List<GameObject> used = new List<GameObject>();//存储正在使用的飞碟
    private List<GameObject> free = new List<GameObject>();//存储使用完了被回收的飞碟

    //颜色数组用于随机分配颜色
    private Color[] color = { Color.red, Color.green, Color.blue, Color.yellow };

    //生产飞碟，先从回收部分取，若回收的部分为空，才从资源加载新的飞碟
    public GameObject GetDisk(int ruler)
    {
        GameObject a_disk;
        if (free.Count > 0)
        {
            a_disk = free[0];
            free.Remove(free[0]);
        }
        else
        {
            a_disk = GameObject.Instantiate(Resources.Load("Prefabs/Disk")) as GameObject;
            Debug.Log(a_disk);
        }
        switch (ruler)
        {
            case 1:
                a_disk.GetComponent<DiskData>().size = UnityEngine.Random.Range(0, 6);//随机大小
                a_disk.GetComponent<DiskData>().color = color[UnityEngine.Random.Range(0, 4)];//随机颜色
                a_disk.GetComponent<DiskData>().speed = UnityEngine.Random.Range(10, 15);//不同关卡速度不同，同一关卡速度在一定范围内

                a_disk.transform.localScale = new Vector3(a_disk.GetComponent<DiskData>().size * 2, a_disk.GetComponent<DiskData>().size * 0.1f, a_disk.GetComponent<DiskData>().size * 2);
                a_disk.GetComponent<Renderer>().material.color = a_disk.GetComponent<DiskData>().color;
                break;
            case 2:
                a_disk.GetComponent<DiskData>().size = UnityEngine.Random.Range(0, 4);
                a_disk.GetComponent<DiskData>().color = color[UnityEngine.Random.Range(0, 4)];
                a_disk.GetComponent<DiskData>().speed = UnityEngine.Random.Range(15, 20);

                a_disk.transform.localScale = new Vector3(a_disk.GetComponent<DiskData>().size * 2, a_disk.GetComponent<DiskData>().size * 0.1f, a_disk.GetComponent<DiskData>().size * 2);
                a_disk.GetComponent<Renderer>().material.color = a_disk.GetComponent<DiskData>().color;
                break;
        }
        a_disk.SetActive(true);
        used.Add(a_disk);
        return a_disk;
    }

    //回收飞碟
    public void FreeDisk(GameObject disk)
    {
        for (int i = 0; i < used.Count; i++)
        {
            if (used[i] == disk)
            {
                disk.SetActive(false);
                used.Remove(used[i]);
                free.Add(disk);
            }
        }
    }
}