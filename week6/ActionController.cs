using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]


public class ActionController : MonoBehaviour {

    private Animator ani;
    private AnimatorStateInfo currentState;
    private Rigidbody rig;
    //用来得到物体本身的组件，再对组件进行相关的操作

    private Vector3 velocity;

    private float rotateSpeed = 15f; //旋转速度
    private float runSpeed = 5f; //奔跑速度

    // Use this for initialization
    void Start () {
        Debug.Log("233");
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!ani.GetBool("isLive")) return;
        //死亡不进行任何动作

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        ani.SetFloat("Speed", Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)));
        //设置速度
        ani.speed = 1 + ani.GetFloat("Speed") / 3;
        //???

        velocity = new Vector3(x, 0, z);

        //如果在运动中输入，则转向
        if(x != 0 || z != 0)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            if(transform.rotation != rotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * rotateSpeed);
            }
        }

        this.transform.position += velocity * Time.fixedDeltaTime * runSpeed;
    }

    //检测是否进入某一区域
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Area"))
        {
            Subject publish = Publisher.getInstance();
            int patrolType = other.gameObject.name[other.gameObject.name.Length - 1] - '0';
            publish.notify(StateOfActor.ENTER_AREA, patrolType, this.gameObject);
            //发布消息
        }
    }

    //检测死亡
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Patrol") && ani.GetBool("isLive"))
        {
            ani.SetBool("isLive", false);
            ani.SetTrigger("toDie");
            //执行死亡动作

            Subject publish = Publisher.getInstance();
            publish.notify(StateOfActor.DEATH, 0, null);

        }
    }
}
