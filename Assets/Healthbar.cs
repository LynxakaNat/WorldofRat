using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Healthbar : NetworkBehaviour

{
    
    public float curr_health;
    public float max_health;
    private float coord;
    // Start is called before the first frame update
    void Start()
    {
        max_health = this.transform.parent.GetComponentInParent<Health>().max_health;
    }

    // Update is called once per frame
    void Update()
    {
           GetHealth();
           HealthThingy();
        
        
    }
    private void GetHealth()
    {
        curr_health = this.transform.parent.GetComponentInParent<Health>().ReadHealth();
    }
    private void HealthThingy()
    {
        coord = curr_health / max_health;
        this.transform.localScale = new Vector3(this.transform.localScale.x, coord, this.transform.localScale.z); // pain
    }
}
