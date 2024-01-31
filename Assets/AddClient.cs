using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AddClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("bUTTON worked");
            NetworkManager.Singleton.StartClient();
            Debug.Log(NetworkManager.Singleton.LocalClientId);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
