using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SelfDeath : NetworkBehaviour
{
    // Start is called before the first frame update
    public float timer;
    private float elapsed_time;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time > timer)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
