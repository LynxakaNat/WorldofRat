using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.Netcode;

public class NetworkMovement : NetworkBehaviour 
{
    public float speed;
    //private bool moving = false;
    public NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>(default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private float dtime;
    public GameObject arena;
    private Vector2 arenatrans;
    public NetworkVariable<Vector2> position = new NetworkVariable<Vector2>();  

    // Start is called before the first frame update
    void Start()
    {
        arenatrans = arena.transform.localScale;
    }

    // Update is called once per frame
    // 
    void Update()
    {
        if (IsOwner)
        {
            Move();
        }
        //ftransform.position = position.Value;

    }
    public void Move()
    {

        if (NetworkManager.Singleton.IsServer)
        {

            
            //position.Value = MoveCh(direction);
            transform.position = MoveCh(direction.Value);
        }
        else
        {
            //Debug.Log("I am not the server");
            SubmitPosReqServerRpc();
        }
       
    }
    public void OnMove(InputValue val)
    {
        //Debug.Log("I AM HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
        if (IsOwner)
        {
            direction.Value = val.Get<Vector2>();
            //Debug.Log(direction);
            
        }
    }
    private Vector2 MoveCh(Vector2 dir_v)

    {
        //Debug.Log("MoveCH is getting called");
        Vector2 oldPos = transform.position; // take the character coordinates 
        Vector2 move = dir_v;
        dtime = Time.deltaTime;
        Vector2 delta_move = move * (speed * dtime);
        Vector2 new_pos = oldPos + delta_move;
        if ((Math.Pow(new_pos.x, 2) / (Math.Pow((arenatrans.x / 2), 2))) + (Math.Pow(new_pos.y, 2) / (Math.Pow((arenatrans.y / 2), 2))) < 1)
        {
            //Debug.Log("If st");
            return new_pos;
            
            // change the position (vector)
        }

        return oldPos;
    }
    [ServerRpc]
    void SubmitPosReqServerRpc(ServerRpcParams rpcParams = default)
    {
        //Debug.Log("Submit the server ROC");
        //Debug.Log(direction);
        transform.position = MoveCh(direction.Value);
        //Debug.Log("I have set the pos");
    }
}
