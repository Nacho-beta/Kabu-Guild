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
    private int max_move = 6;   // Max of move can be made
    private int move_made = 0;  // Actual move that have been made
    
    // name_agent : Name of the Agent
    private string name_agent;
    
    // move : Method to move to a position
    private Movement move;

    // position : Vector3 (x,y,z) for position of agent
    public Vector2 position;

    // facing : Bool that indicate if is facing to left or right
    private bool facing_left;

    // private Rigidbody2D rigid_body;
    // private BoxCollider2D hitbox;


    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */
    /*
     * Getters & Setters
     */
    // Get Position
    public Vector2 GetPosition() { return position; }


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
    }

    // SetAction : Return action selected by player
    public Actions SetAction()
    {
        if (move_made < max_move)
        {
            return Actions.move;
        }
        else
        {
            return Actions.none;
        }
    }
    

    // Move: Move agent using control
    public bool Move()
    {
        float input_x = 0.0f,
              input_y = 0.0f;
        Vector2 target = new Vector2(0,0);

        input_x = Input.GetAxis("Horizontal");
        input_y = Input.GetAxis("Vertical");

        if (input_x != 0.0f)
        {
            target.x = 1.0f;
        } else if(input_y != 0.0f)
        {
            target.y = 1.0f;
        }

        if(target != Vector2.zero)
        {
            this.move.Move(ref position, target, ref facing_left, name_agent);
            this.move_made++;
            return true;
        } else
        {
            return false;
        }
    }
}
