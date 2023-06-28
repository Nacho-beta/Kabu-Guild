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
    private int max_move,           // Max of move can be made
                move_made,          // Actual move that have been made
                input_enemy_index;  // Enemy index for attack
    private float input_x,  // Input in horizontal axis
                  input_y,  // Input in vertical axis
                  hp;       // Hit points for the player
    private string name_agent;  // Name of the Agent
    private bool pos_ok,        // Bool indicate if current position and value of position it's ok
                 facing_left,   // Bool that indicate if is facing to left or right
                 input_mouse_ok,// Bool represents if is reading input for mouse
                 animation_end; // Bool represents if animation of death end

    // Array Var
    public Vector2 position;        // Vector2 (x,y) for position of agent
    private (int,int) map_move;

    // Class Var
    private Class my_class;
    private Movement move;                  // Method to move to a position
    private Actions action_actual;          // Action to return to GM
    private SpriteRenderer sprite_renderer; // Sprite Renderer
    private BoxCollider2D hitbox;           // Hit box
    private MapManager map;                 // Map of the Game
    public Animator animator;               // Animator controller


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
    public void Start()
    {
        GameObject l_go_map;

        // Standard var
        hp = 10.0f;
        max_move = 6;
        move_made = 0;
        name_agent = "Player";
        pos_ok = false;
        facing_left = false;
        input_mouse_ok = false;
        animation_end = false;

        // Array Var
        position = new Vector2(-0.5f, -0.25f);

        // Initialization class var
        my_class = new Warrior();

        action_actual = Actions.none;

        // Get references
        move = gameObject.AddComponent(typeof(Movement)) as Movement;

        this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        this.hitbox = gameObject.GetComponent<BoxCollider2D>();

        l_go_map = GameObject.FindGameObjectWithTag("Map");
        this.map = l_go_map.GetComponent<MapManager>();

        // Methods called in the start
        this.CleanInput();
        this.ChangeSprite();
    }

    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------

    /// <summary>
    /// Clean variables needed for input in movement
    /// </summary>
    private void CleanInput()
    {
        input_x = 0.0f;
        input_y = 0.0f;

        input_mouse_ok = false;
        input_enemy_index = -1;
    }

    /// <summary>
    /// Change actual sprite to class sprite
    /// </summary>
    private void ChangeSprite()
    {
        sprite_renderer.sprite = my_class.GetSprite();
    }

   
    //-------------------------------------------------
    //-------GETTERS-----------------------------------
    //-------------------------------------------------
    
    /// <summary>
    /// Get actual action
    /// </summary>
    /// <returns></returns>
    public Actions GetAction()
    {
        if (action_actual == Actions.move)
            this.CleanInput();
        return action_actual;
    }

    // HP
    public float GetHP() { return this.hp; }

    // Position
    public Vector2 GetPosition() { return position; }           

    // Range
    public int GetRange() { return my_class.Attack().GetRange(); }

    /// <summary>
    /// Get map position
    /// </summary>
    /// <returns>Get player's pos relative to map manager</returns>
    public (int,int) GetMapPosition()
    {
        (int,int) map_position = map_move;
        map_move = (0, 0);

        return map_position;
    }

    public Attack GetAttack() { return my_class.Attack(); }

    public bool GetAnimationEnd() { return animation_end; }

    //-------SETTERS-----------------------------------   
    // Action actual
    public void SetAction(Actions act) { action_actual = act; }

    // HP
    public void SetHP(float new_hp) { this.hp = new_hp; }

    public void SetPosition(Vector2 pa_new_pos) 
    { 
        this.position = pa_new_pos;
        this.transform.position = pa_new_pos;
    }


    //------------------------------------------------- 
    //-------PUBLIC------------------------------------ 
    //-------------------------------------------------    

    /// <summary>
    /// Move agent using controls
    /// </summary>
    /// <returns>True if player moved, false in other case</returns>
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
                animator.SetFloat("Speed", 1);

                pos_ok = false;

                // Pos relative to map
                if(target.x < 0)
                {
                    map_move.Item2 -= 1;
                } else if(target.x > 0)
                {
                    map_move.Item2 += 1;
                }

                // Pos relative to map
                if (target.y < 0)
                {
                    map_move.Item1 -= 1;
                }
                else if (target.y > 0)
                {
                    map_move.Item1 += 1;
                }

                StartCoroutine("UpdatePosition");
                return true;
            }            
        }

        return false;
    }

    public void Skill()
    {
        print("Mi vida: " + hp);
        this.my_class.UseSkill();
        print("Mi vida después de la habilidad: " + hp);
        this.action_actual = Actions.pass_turn;
    }

    /// <summary>
    /// Select attack and calculate damage
    /// </summary>
    /// <returns>Index of enemy to hit</returns>
    public int Attack()
    {
        int lv_enemies_range = 0;

        // If input of mouse is null
        if (!input_mouse_ok)
        {
            // Calculate enemies in the range
            lv_enemies_range = this.map.CheckEnemiesInRange(this.GetRange());
            if(lv_enemies_range > 0)
            {
                // Wait for user select a valid enemy
                StartCoroutine("WaitForClickInput");
            }
            else
            {
                // If there isn't enemies in range, end turn
                this.action_actual = Actions.pass_turn;
            }            
        }
        else
        {
            // If input is correct
            if (input_enemy_index >= 0)
            {                
                // End turn
                this.action_actual = Actions.pass_turn;
                return input_enemy_index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Clear var for player
    /// </summary>
    public void Clear()
    {
        this.CleanInput();

        this.action_actual = Actions.none;

        this.animation_end = false;
    }

    /// <summary>
    /// Receive the attack
    /// </summary>
    /// <param name="atck"> Attack to receive </param>
    public bool ReceiveAttack(Attack atck)
    {
        this.hp -= atck.GetDamage();
        if (this.hp <= 0)
        {
            animator.SetBool("Dead", true);
            StartCoroutine("WaitForDeathAnimation");
            return true;
        }
        else
        {
            return false;
        }
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

    IEnumerator WaitForClickInput()
    {
        Vector2 pos_map = new Vector2();
        Vector3 input_mouse = new Vector3();

        while (this.input_enemy_index < 0)
        {
            if(Input.GetMouseButtonDown(0))
            {
                input_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                pos_map.x = input_mouse.x;
                pos_map.y = input_mouse.y;

                this.input_enemy_index = this.map.GetEnemyIndexByPos(pos_map);
            }            
            
            yield return null;
        }
        input_mouse_ok = true;
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
            animator.SetFloat("Speed", 0);
            action_actual = Actions.pass_turn;
            this.move_made = 0;
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        while(!this.animator.GetCurrentAnimatorStateInfo(0).IsName("player_dead"))
        {
            yield return new WaitForSeconds(0.6f);
        }

        animation_end = true;
    }
}
