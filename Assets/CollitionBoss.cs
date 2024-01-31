using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollitionBoss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (NetworkManager.Singleton.IsServer)
        {

            if (collision.gameObject.tag == "Boss")
            {
                Debug.Log("colliding the boss");
                collision.GetComponent<BossHealth>().DecreaseHealth(10);
                this.gameObject.GetComponent<NetworkObject>().Despawn();
            }

        }
    }
}
