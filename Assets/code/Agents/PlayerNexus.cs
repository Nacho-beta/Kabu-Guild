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
    private bool facing_left,   // Bool that indicate if is facing to left or right
                 pos_ok;        // Bool indicate if current position and value of position it's ok
    private string name_agent;  // Name of the Agent
    private float speed;        // Speed of movement

    // Array Var
    private Vector2 movement_direction;   // Vector2 (x,y) for position of agent                             
    // Class var
    private Movement move;
    private Class my_class;
    private SpriteRenderer sprite_renderer; // Sprite Renderer
    private BoxCollider2D hitbox;           // Hit box
    private Rigidbody2D rb;
    public Animator animator;

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
        speed = 2f;

        // Array Var
        movement_direction = new Vector2(0.0f, 0.0f);

        // Initialization class var
        my_class = new Warrior();

        // Get reference
        move = gameObject.AddComponent(typeof(Movement)) as Movement;

        this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        this.hitbox = gameObject.GetComponent<BoxCollider2D>();

        this.rb = gameObject.GetComponent<Rigidbody2D>();

        // Methods called in the start
        this.ChangeSprite();
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit;
        LayerMask mask_wall = new LayerMask();
        Vector3 target = new Vector3();
        Vector2 direction = new Vector2(),
                pos = new Vector2();

        movement_direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(movement_direction.x != 0)
        {
            // Flip sprite if needed
            if(this.move.facing(ref facing_left, movement_direction.x))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if(movement_direction.x != 0 | movement_direction.y !=0)
        {
            animator.SetFloat("Speed", 1);
            pos.x = this.transform.position.x;
            pos.y = this.transform.position.y;
            
            target.x = this.transform.position.x + movement_direction.x * speed;
            target.y = this.transform.position.y + movement_direction.y * speed;
            
            direction.x = target.x - pos.x;
            direction.y = target.y - pos.y;

            mask_wall = LayerMask.GetMask("Wall", "Item");

            hit = Physics2D.Raycast(pos, direction, 1.0f, mask_wall);
            if (hit.collider != null)
            {
                print("Colision detectada");
                movement_direction.x = 0; movement_direction.y = 0;                
            }
            else
            {
                print("Sin colision");
                movement_direction.x *= speed;
                movement_direction.y *= speed;
            }
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void FixedUpdate()
    {        
        rb.velocity = movement_direction;
    }

    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Change actual sprite to class sprite
    /// </summary>
    private void ChangeSprite()
    {
        sprite_renderer.sprite = my_class.GetSprite();
    }

}
