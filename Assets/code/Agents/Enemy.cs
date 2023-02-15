using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    // Standard var
    public string name_agent;       // Name of the Agent
    private bool facing_left,       // Bool indicate if is facing to left or right
                 stop,              // Bool indicate if is in movement
                 player_in_range,   // Bool indicate if player is in attack range
                 pos_ok;            // Bool indicate if position is updated
    private int max_move,       // Max of move can be made
                move_made,      // Actual move that have been made
                id;

    // Array Var
    private Vector2 speed_move, // Vector for direction
                    next_step;  // Next position for pathing
    public Vector2 position;    //Vector3(x, y, z) for position of agent
    private Queue<Vector2> path;// Queue for pathing
    private (int, int) map_move;

    // Class var
    private Movement move;          // Class for move to a position
    private MonsterType agent_type; // Type of agent 
    private Actions action_actual;  // Actual action

    // Static var
    static int id_generator = 0;

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */
    //-------UNITY-------------------------------------
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public void Start()
    {
        name_agent = "Enemy";
        facing_left = true;
        stop = false;
        player_in_range = false;
        max_move = 3;
        move_made = 0;

        path = new Queue<Vector2>();
        next_step = new Vector2(1, 0);
        speed_move = new Vector2(1, 1);
        position = new Vector2(3, 0);
        map_move = (0, 0);

        move = gameObject.AddComponent(typeof(Movement)) as Movement;
        agent_type = new Kobold();
        action_actual = Actions.none;

        this.FillPath();

        id = id_generator;
        id_generator++;
    }

    //-------STATIC------------------------------------
    /// <summary>
    /// Get a enemy placeholder
    /// </summary>
    /// <returns>Placeholder for enemy</returns>
    public static Enemy getEnemyDefault()
    {
        Enemy ret_enemy = new Enemy();

        ret_enemy.name_agent = "Enemy";
        ret_enemy.facing_left = true;
        ret_enemy.stop = false;
        ret_enemy.player_in_range = false;
        ret_enemy.max_move = 6;
        ret_enemy.move_made = 0;

        ret_enemy.path = new Queue<Vector2>();
        ret_enemy.next_step = new Vector2(1, 0);
        ret_enemy.speed_move = new Vector2(1, 1);
        ret_enemy.position = new Vector2(3, 0);
        ret_enemy.map_move = (0, 0);

        ret_enemy.agent_type = new Kobold();
        ret_enemy.action_actual = Actions.none;

        ret_enemy.FillPath();

        return ret_enemy;
    }

    //-------PRIVATE-----------------------------------
    /// <summary>
    /// Fill pathfinding
    /// </summary>
    private void FillPath()
    {
        Vector2 new_point = new Vector2(-1, 0);

        path.Enqueue(new_point);

        new_point = new Vector2(0, 1);
        path.Enqueue(new_point);

        new_point = new Vector2(1, 0);
        path.Enqueue(new_point);

        new_point = new Vector2(0, -1);
        path.Enqueue(new_point);

        next_step = path.Dequeue();
        path.Enqueue(next_step);
    }


    //-------GETTERS-----------------------------------   
    /// <summary>
    /// Get HP of Enemy
    /// </summary>
    /// <returns>float : HP</returns>
    public float getHp(){ return agent_type.GetHp(); }

    /// <summary>
    /// Get Position
    /// </summary>
    /// <returns>Vector2 : position</returns>
    public Vector2 GetPosition() { return position; }

    /// <summary>
    /// Get Range for Attacks
    /// </summary>
    /// <returns> Get attack range </returns>
    public int GetRange() { return agent_type.GetRange(); }

    public Actions GetAction() 
    {
        /*
        // If player is in range, always attack
        if (player_in_range) 
        {            
            action_actual = Actions.fight;
        } else
        {            
            // If can't move and can't attack, end turn
            if (move_made >= max_move)
            {
                move_made = 0;
                action_actual = Actions.pass_turn;
            }
            else
            {
                action_actual = Actions.move;
            }
        }*/
        return action_actual; 
    }

    //
    public int GetId() { return id; }

    /// <summary>
    /// Get map position
    /// </summary>
    /// <returns>Get player's pos relative to map manager</returns>
    public (int, int) GetMapPosition()
    {
        (int, int) map_position = map_move;
        map_move = (0, 0);

        return map_position;
    }


    //-------SETTERS-----------------------------------   
    /// <summary>
    /// Update position
    /// </summary>
    /// <param name="new_pos"> new position</param>
    public void SetPosition(Vector2 new_pos) { 
        position = new_pos;
        this.transform.position = new_pos;
    }

    public void SetAction(Actions new_action) { action_actual = new_action; }


    // ---------- Private --------------------------    

    /// <summary>
    /// Fill vector direction
    /// </summary>
    private void FillDirection()
    {        
        // Direction in Axis X
        if (next_step.x < (position.x - 0.05f))
        {
            speed_move.x = -1;
        }
        else if (next_step.x > (position.x + 0.05f))
        {
            speed_move.x = 1;
        }
        else
        {
            speed_move.x = 0;

            // Direction in Axis Y
            if (next_step.y < position.y - 0.05f)
            {
                speed_move.y = -1;
            }
            else if (next_step.y > position.y + 0.05f)
            {
                speed_move.y = 1;
            }
            else
            {
                speed_move.y = 0;
            }
        }

        
    }

    // ---------- Public ---------------------------

    // Move : Move enemy to position in pathing
    public bool Move()
    {        
        bool move_happen = false;

        // Movement
        if (speed_move != Vector2.zero)
        {
            move_happen = this.move.Move(ref position, next_step, ref facing_left, name_agent);
            if (move_happen)
            {
                // Pos relative to map
                if (next_step.x < 0)
                {
                    map_move.Item2 -= 1;
                }
                else if (next_step.x > 0)
                {
                    map_move.Item2 += 1;
                }

                // Pos relative to map
                if (next_step.y < 0)
                {
                    map_move.Item1 -= 1;
                }
                else if (next_step.y > 0)
                {
                    map_move.Item1 += 1;
                }
                
                pos_ok = false;
                StartCoroutine("UpdatePosition");
                return true;
            }
        }

        return false;
    }
    
    // ReceiveAttack : Receive attack and calculate results
    public bool ReceiveAttack(Attack atck)
    {
        print("Vida actual = " + agent_type.GetHp());
        stop = agent_type.ReceiveAttack(atck);
        print("Vida despues del ataque = " + agent_type.GetHp());
        return stop;
    }

    public Actions PathFinding()
    {
        if (path.Count != 0)
            action_actual = Actions.move;
        else
            action_actual = Actions.pass_turn;
        return action_actual;
    }

    // UpdatePosition: Call get method for movement to update the position
    IEnumerator UpdatePosition()
    {
        while (!pos_ok)
        {
            pos_ok = this.move.getPosOriDest();
            yield return null;
        }

        // Update pos
        position = this.move.getDestination();

        // Update next step

        next_step = path.Dequeue();
        path.Enqueue(next_step);

        this.move_made++;
        if (this.move_made >= max_move - 1)
        {
            action_actual = Actions.pass_turn;
        }
    }
}
