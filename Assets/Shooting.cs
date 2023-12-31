using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Shooting : NetworkBehaviour
{
    public GameObject bullet;
    public NetworkObject target;
    private Vector3 tar_pos;
    public float speed;
    public float fire_rate; // amount of bullets per second 
    private float time_last_shot;
    // Start is called before the first frame update
    void Start()
    {
        // Shoot();
    }

    // Update is called once per frame 
    // if time_last_shot >= 1s / fire_rate shoot 
    void Update()
    {
        time_last_shot = time_last_shot + Time.deltaTime;
        if (NetworkManager.Singleton.IsServer) {
            if (time_last_shot > 1f / fire_rate)
            {
                Shoot();
            } }
        
    }
    void Shoot()
    {
        int picked = Random.Range(0,NetworkManager.Singleton.ConnectedClientsList.Count);
        GameObject newbullet = Instantiate(bullet, this.transform); // init a new bullet
        newbullet.GetComponent<NetworkObject>().Spawn();
        target = NetworkManager.Singleton.ConnectedClientsList[picked].PlayerObject;
        tar_pos = target.transform.position;
        Rigidbody2D rigid = newbullet.GetComponent<Rigidbody2D>();
        Vector3 temp_vec = tar_pos - this.transform.position;
        temp_vec = temp_vec / temp_vec.magnitude;
        rigid.velocity = speed * temp_vec;
        time_last_shot = 0;

    }
}
