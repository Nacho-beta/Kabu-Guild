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
    private float   input_x = 0.0f, // Input in horizontal axis
                    input_y = 0.0f; // Input in vertical axis
    private string name_agent;      // Name of the Agent
    private bool pos_ok = true;     // Bool indicate if current position and value of position it's ok

    
    private Movement move;          // Method to move to a position
    private Actions action_actual;  // Action to return to GM

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
    
    // Action
    public Actions GetAction(){ return action_actual; }
    public void SetAction(Actions act) { action_actual = act; }

    /*
     * Start
     * Start is called before the first frame update
     */
    void Start()
    {
        name_agent = "Player";
        position = new Vector2(-0.5f, -0.25f); // 4, 4, 0
        facing_left = false;
        action_actual = Actions.none;

        move = gameObject.AddComponent(typeof(Movement)) as Movement;
    }

    
    

    // Move: Move agent using control
    public bool Move()
    {
        Vector2 old_pos = new Vector2(position.x, position.y),
                target = new Vector2(0, 0);
        bool move_happen = false;

        // Get Input

        StartCoroutine("WaitForMovementInput");

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

        // Movement
        if (target != Vector2.zero)
        {            
            input_x = input_y = 0.0f;

            move_happen = this.move.Move(ref position, target, ref facing_left, name_agent);
            if (move_happen)
            {
                pos_ok = false;
                StartCoroutine("UpdatePosition");
                return true;
            }            
        }

        return false;
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

    // UpdatePosition: Call get method for movement to update the position
    IEnumerator UpdatePosition()
    {
        while (!pos_ok)
        {
            pos_ok = this.move.getPosOriDest();            
            yield return null;
        }
        
        position = this.move.getDestination();

        this.move_made++;
        print("Me he movido" + this.move_made);
        if(this.move_made >= max_move-1)
        {
            action_actual = Actions.none;
            this.move_made = 0;
        }
    }
}
