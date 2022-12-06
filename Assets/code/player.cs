using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Movement move;
    private Rigidbody2D rigid_body;

    // Start is called before the first frame update
    void Start()
    {
        move = gameObject.AddComponent(typeof(Movement)) as movement;
        rigid_body = gameObject.GetComponent(typeof(Rigidbody2D)) as rigidBody2D;
    }

    // Update is called once per frame
    void Update()
    {
        move.move();
    }
}
