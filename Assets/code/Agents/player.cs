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
    // Standard var
    private int max_move,       // Max of move can be made
                move_made,      // Actual move that have been made
                range;          // Range for Attack
    private float   input_x,    // Input in horizontal axis
                    input_y;    // Input in vertical axis
    private string name_agent;  // Name of the Agent
    private bool pos_ok ,       // Bool indicate if current position and value of position it's ok
                 facing_left;   // Bool that indicate if is facing to left or right

    // Array Var
    public Vector2 position;    // Vector2 (x,y) for position of agent

    // Class Var
    private Movement move;          // Method to move to a position
    private Actions action_actual;  // Action to return to GM
    private Attack my_attack;       // Attack


    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------PRIVATE-----------------------------------
    
    // CleanInput : Clean var for input in movement
    private void CleanInput()
    {
        input_x = 0.0f;
        input_y = 0.0f;
    }

    //-------PUBLIC------------------------------------
    
    // Get for Position
    public Vector2 GetPosition() { return position; }
    
    // Get for action_actual
    public Actions GetAction()
    {
        if (action_actual == Actions.move)
            this.CleanInput();
        return action_actual; 
    }
    // Set for action_actual
    public void SetAction(Actions act) { action_actual = act; }

    // Get for range
    public int GetRange() { return range; }

    // Get Attack
    public Attack GetAttack() { return my_attack; }

    // Start : Start is called before the first frame update
    public void Start()
    {
        max_move = 6;
        move_made = 0;

        input_x = 0.0f;
        input_y = 0.0f;

        range = 2;

        name_agent = "Player";

        position = new Vector2(-0.5f, -0.25f);
        pos_ok = false;
        facing_left = false;
        
        action_actual = Actions.none;

        move = gameObject.AddComponent(typeof(Movement)) as Movement;

        my_attack = new Attack();
        my_attack.SetDamage(2.0f);
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
            this.CleanInput();

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


    /*
     * Coroutines
     */
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
        if(this.move_made >= max_move-1)
        {
            action_actual = Actions.pass_turn;
            this.move_made = 0;
        }
    }
}
