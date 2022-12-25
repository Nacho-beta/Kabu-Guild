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
    private int max_move = 6;       // Max of move can be made
    private int move_made = 0;      // Actual move that have been made
    private float   input_x = 0.0f,   // Input in horizontal axis
                    input_y = 0.0f; // Input in vertical axis
    private string name_agent;      // name_agent : Name of the Agent

    // move : Method to move to a position
    private Movement move;

    // position : Vector3 (x,y,z) for position of agent
    public Vector2 position;

    // facing : Bool that indicate if is facing to left or right
    private bool facing_left;


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
        return Actions.move;
    }
    

    // Move: Move agent using control
    public bool Move()
    {
        Vector2 old_pos = new Vector2(position.x, position.y),
                target = new Vector2(0, 0);

        print("Rutina");

        StartCoroutine("WaitForMovementInput");

        print("Después de rutina");

        if(input_x < 0)
        {
            target.x = -1;
        } else if (input_x > 0)
        {
            target.x = 1;
        }

        if (input_y < 0)
        {
            target.y = -1;
        }
        else if (input_y > 0)
        {
            target.y = 1;
        }

        if (target != Vector2.zero)
        {
            this.move.Move(ref position, target, ref facing_left, name_agent);
            this.move_made++;
            input_x = input_y = 0.0f;
            return true;
        } else
        {
            return false;
        }
    }

    // WaitForMovementInput: Wait for input in control to move
    IEnumerator WaitForMovementInput()
    {
        while (input_x == 0.0f & input_y == 0.0f ) 
        {
            input_x = Input.GetAxis("Horizontal");
            input_y = Input.GetAxis("Vertical");

            yield return null;
        }
        
    }
}
