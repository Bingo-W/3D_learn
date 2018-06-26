using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyFire : NetworkBehaviour
{

    public GameObject bulletPrefab;
    // Use this for initialization
    private int num = 0;
	
	// Update is called once per frame
	void Update () {

        num++;
        num = num % 100;

        if(num == 0) CmdFire();
    }

    [Command]
    void CmdFire()
    {
        // This [Command] code is run on the server!

        // create the bullet object locally
        var bullet = (GameObject)Instantiate(
             bulletPrefab,
             transform.position - transform.forward,
             Quaternion.identity);

        bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4;

        //behind
        var bullet1 = (GameObject)Instantiate(
             bulletPrefab,
             transform.position + transform.forward,
             Quaternion.identity);

        bullet1.GetComponent<Rigidbody>().velocity = transform.forward * 4;

        //left
       var bullet2 = (GameObject)Instantiate(
             bulletPrefab,
             transform.position - transform.right,
             Quaternion.identity);

        bullet2.GetComponent<Rigidbody>().velocity = -transform.right * 4;

        //right
        var bullet3 = (GameObject)Instantiate(
             bulletPrefab,
             transform.position + transform.right,
             Quaternion.identity);

        bullet3.GetComponent<Rigidbody>().velocity = transform.right * 4;

        // spawn the bullet on the clients
        NetworkServer.Spawn(bullet);
        NetworkServer.Spawn(bullet1);
        NetworkServer.Spawn(bullet2);
        NetworkServer.Spawn(bullet3);

        // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
        Destroy(bullet, 2.0f);
        Destroy(bullet1, 2.0f);
        Destroy(bullet2, 2.0f);
        Destroy(bullet3, 2.0f);
    }
}
