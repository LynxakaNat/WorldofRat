using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class soaks : NetworkBehaviour
{
    private GameObject targ;
    private float elapsedtime;
    public float timer = 2; 
    private bool standing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ImStillStanding();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (NetworkManager.Singleton.IsServer)
        {

            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("crossing the soak");
                targ = collision.gameObject;
                standing = !standing;
                Debug.Log(standing);
                elapsedtime = 0;
            }

        }
    }
    private void ImStillStanding()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (standing && elapsedtime > timer)
            {
                Debug.Log(standing);
                targ.GetComponent<Health>().IncreaseHealth(200);
                elapsedtime = 0;
                standing = false;

                this.GetComponent<NetworkObject>().Despawn();
            }
            elapsedtime += Time.deltaTime;
        }
    }
}
