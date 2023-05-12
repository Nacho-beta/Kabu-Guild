using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNexus : MonoBehaviour
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    //Standard var
    private float input_x,  // Input in horizontal axis
                  input_y;  // Input in vertical axis
    private bool facing_left,   // Bool that indicate if is facing to left or right
                 pos_ok;        // Bool indicate if current position and value of position it's ok
    private string name_agent;  // Name of the Agent

    // Array Var
    private Vector2 position;   // Vector2 (x,y) for position of agent                             
    // Class var
    private Movement move;
    private Class my_class;
    private SpriteRenderer sprite_renderer; // Sprite Renderer
    private BoxCollider2D hitbox;           // Hit box

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------------------------------------------------
    //-------UNITY-------------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Start is called before the firs frame update
    /// </summary>
    void Start()
    {
        // Standard var
        name_agent = "Player";
        facing_left = false;

        // Array Var
        position = new Vector2(0.5f, 0.75f);

        // Initialization class var
        my_class = new Warrior();

        // Get reference
        move = gameObject.AddComponent(typeof(Movement)) as Movement;

        this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        this.hitbox = gameObject.GetComponent<BoxCollider2D>();

        // Methods called in the start
        this.CleanInput();
        this.ChangeSprite();
        
    }

    // Update is called once per frame
    void Update()
    {
        this.Move();        
    }

    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------
    private bool Move()
    {
        Vector2 old_pos = new Vector2(position.x, position.y),
                target = new Vector2(0, 0);
        bool move_happen = false;

        // Get Input
        StartCoroutine("WaitForMovementInput");

        if (input_x < 0)
        {
            target.x = -1;
        }
        else if (input_x > 0)
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

    /// <summary>
    /// Clean variables needed for input in movement
    /// </summary>
    private void CleanInput()
    {
        input_x = 0.0f;
        input_y = 0.0f;
    }

    /// <summary>
    /// Change actual sprite to class sprite
    /// </summary>
    private void ChangeSprite()
    {
        sprite_renderer.sprite = my_class.GetSprite();
    }

    //-------------------------------------------------
    //-------COROUTINE---------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Wait for keyboard input
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForMovementInput()
    {
        while (input_x == 0.0f & input_y == 0.0f)
        {
            input_x = Input.GetAxis("Horizontal");
            input_y = Input.GetAxis("Vertical");

            yield return null;
        }
    }

    IEnumerator UpdatePosition()
    {
        while (!pos_ok)
        {
            pos_ok = this.move.getPosOriDest();
            yield return null;
        }
        position = this.move.getDestination();
    }
}
