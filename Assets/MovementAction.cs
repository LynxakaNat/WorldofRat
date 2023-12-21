using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementAction : MonoBehaviour
{
    public float speed = 1;
    private bool moving = false;
    private Vector2 direction;
    private float dtime;
    public GameObject arena;
    private Vector2 arenatrans;
   
    // Start is called before the first frame update
    void Start()
    {
        arenatrans = arena.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving) {
            MoveCh(direction);
        }
    }
    public void OnMove(InputValue val)
    {
        direction = val.Get<Vector2>();
        moving = true;
     
    }
    private void MoveCh(Vector2 dir_v)
    {
        Vector2 oldPos = transform.position; // take the character coordinates 
        Vector2 move = dir_v;
        dtime = Time.deltaTime;
        Vector2 delta_move = move * (speed * dtime);
        Vector2 newPos = oldPos + delta_move; 
        if ((Math.Pow(newPos.x,2)/ (Math.Pow((arenatrans.x /2),2))) + (Math.Pow(newPos.y,2) / (Math.Pow((arenatrans.y / 2),2))) < 1)
        {
            transform.position = newPos; // change the position (vector)
        }
        
    }
    
}
