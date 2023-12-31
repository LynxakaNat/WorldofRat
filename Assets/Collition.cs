using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Collition : MonoBehaviour
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
            Debug.Log("2nd stage");
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("dec hea");
                collision.GetComponent<Health>().DecreaseHealth(50);
                Debug.Log("COllid");
                Destroy(this.gameObject);
            }

        }
    }
}
