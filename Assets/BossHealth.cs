using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : NetworkBehaviour
{ 
public int boss_health; // this variable is public because we want to be able to mod it
public NetworkVariable<int> health = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

// Start is called before the first frame update
void Start()
{
    if (NetworkManager.Singleton.IsServer)
    {
        health.Value = boss_health;
    }
}

// Update is called once per frame
void Update()
{


}
void Death()
{
        if (NetworkManager.Singleton.IsServer)
        {
            Destroy(this.gameObject);
            NetworkManager.SceneManager.LoadScene("WinningScene",LoadSceneMode.Single);
        }
    // we will load a win screen

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
        if (health.Value <= 0)
        {
        Death();
        }
    }
}
public void IncreaseHealth(int increase)
{
    health.Value += increase;
}

}
