using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    
    // name_agent : Name of the Agent
    private string name_agent;
    
    // move : Method to move to a position
    private Movement move;

    // position : Vector3 (x,y,z) for position of agent
    public Vector2 position;

    // facing : Bool that indicate if is facing to left or right
    private bool facing_left;

    private Rigidbody2D rigid_body;
    private BoxCollider2D hitbox;


    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */
    /*
     * Start
     * Start is called before the first frame update
     */
    void Start()
    {
        name_agent = "Player";
        position = new Vector2(0, 0); // 4, 4, 0
        facing_left = false;

        move = gameObject.AddComponent(typeof(Movement)) as Movement;
        rigid_body = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        // hitbox = GetComponent<BoxCollider2D>();
    }

    /*
     * Update
     * Update is called once per frame
     */
    void Update()
    {
        if (GameManager.canMove(name_agent))
        {
            // Determinates the facing
            move.facing(ref facing_left);            

            move.move(ref position);
        }
    }
}
