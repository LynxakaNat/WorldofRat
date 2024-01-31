using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class YeetYeetYeeet : NetworkBehaviour
{
    public GameObject bullet;
    
    public float speed;
    public float fire_rate; // amount of bullets per second 
    private float time_last_shot;
    // Start is called before the first frame update
    void Update()
    {
        time_last_shot = time_last_shot + Time.deltaTime;
    }

    // Update is called once per frame
    void Shoot()
    {
        if (time_last_shot > 1/fire_rate)
        {
            GameObject newbullet = Instantiate(bullet, this.transform); // init a new bullet
            newbullet.GetComponent<NetworkObject>().Spawn();
            Rigidbody2D rigid = newbullet.GetComponent<Rigidbody2D>();
            Vector3 temp_vec = new Vector3(0, speed, 0);
            rigid.velocity = temp_vec;
            time_last_shot = 0;
        }

    }
    void OnFire()
    {   if (IsOwner)
        {
            if (NetworkManager.Singleton.IsServer)
            {


                //position.Value = MoveCh(direction);
                Shoot();
            }
            else
            {
                //Debug.Log("I am not the server");
                SubmitShootReqServerRpc();
            }
        }
    }
    [ServerRpc]
    void SubmitShootReqServerRpc(ServerRpcParams rpcParams = default)
    {
        //Debug.Log("Submit the server ROC");
        //Debug.Log(direction);
        Shoot();

        //Debug.Log("I have set the pos");
    }
}

