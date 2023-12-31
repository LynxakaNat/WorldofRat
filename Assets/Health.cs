using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    public int max_health = 1000; // this variable is public because we want to be able to mod it
    public NetworkVariable<int> health = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            health.Value = max_health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    
    }
    void Death()
    {
 
        Destroy(this.gameObject); // here we will later change it for the "you lost loser" screen
        SceneManager.LoadScene("LoseScreen");

    }
    public int ReadHealth()
    {
        return health.Value;  // we need this method for other scripts to read health

    }
    public void DecreaseHealth(int decrease)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            health.Value -= decrease;
            //if (health.Value <= 0)
            //{
                //SceneManager.LoadScene("LoseScreen");
            //}
        }
    }
    public void IncreaseHealth(int increase)
    {
        health.Value += increase;
    }

}
