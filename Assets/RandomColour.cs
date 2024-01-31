using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomColour : NetworkBehaviour
{
    // Start is called before the first frame update
    public List<Color> colors;
    public NetworkVariable<Color> picked_color = new NetworkVariable<Color>();
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GenerateColor();     
    }
    void GenerateColor()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            picked_color.Value = colors[Random.Range(0, colors.Count)];
        }
        GetComponent<SpriteRenderer>().color = picked_color.Value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
