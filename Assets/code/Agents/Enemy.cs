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
    private string name_agent;      // Name of the Agent
    private bool facing_left,       // Bool indicate if is facing to left or right
                 stop,              // Bool indicate if is in movement
                 player_in_range,   // Bool indicate if player is in attack range
                 action_made,       // Bool indicate if action was made
                 is_moving;
    private int max_move,       // Max of move can be made
                move_made,      // Actual move that have been made
                id;

    // Array Var
    private Vector2 next_step,      // Next position for pathing
                    new_pos,        // Vector2 to move
                    target,         // Vector2 to reach
                    position;       // Vector3(x, y, z) for position of agent
    private Queue<Vector2> path;    // Queue for pathing
    private (int, int) map_position;// Position in map


    // Class var
    private Movement move;          // Class for move to a position
    private MonsterType agent_type; // Type of agent 
    private Actions action_actual;  // Actual action
    private CommunicationManager channel;

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
        GameObject l_go_channel;

        name_agent = "Enemy";
        facing_left = true;
        stop = false;
        player_in_range = false;
        action_made = false;
        max_move = 3;
        move_made = 0;

        path = new Queue<Vector2>();
        next_step = new Vector2(0, 0);
        position = new Vector2(3, 0);
        
        move = gameObject.AddComponent(typeof(Movement)) as Movement;
        agent_type = new Kobold();
        action_actual = Actions.none;

        this.FillPath();

        id = id_generator;
        id_generator++;

        // Class Reference
        l_go_channel = GameObject.FindGameObjectWithTag("Communication");
        this.channel = l_go_channel.GetComponent<CommunicationManager>();
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
        ret_enemy.position = new Vector2(3, 0);

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
        Vector2 new_step = new Vector2();

        if(target.y < position.y)
        {
            new_step.x = 0;
            new_step.y = -1;
            for(int i = (int)position.y; i>=(int)target.y; i--)
            {
                path.Enqueue(new_step);
            }
        } else if(target.y > position.y)
        {
            new_step.x = 0;
            new_step.x = 1;
            for (int i = (int)position.y; i <= (int)target.y; i++)
            {
                path.Enqueue(new_step);
            }
        }

        if(target.x < position.x)
        {
            new_step.x = -1;
            new_step.y = 0;            
            for (int i = (int)position.x; i >= (int)target.x; i--)
            {
                path.Enqueue(new_step);
            }
        } else if(target.x > position.x)
        {
            new_step.x = 1;
            new_step.y = 0;
            for (int i = (int)position.x; i <= (int)target.x; i++)
            {
                path.Enqueue(new_step);
            }
        }
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
        if (action_made)
        {
            action_made = false;
            action_actual = Actions.pass_turn;
            return Actions.pass_turn;
        } else
        {
            return action_actual;
        }
    }

    /// <summary>
    /// Get for ID
    /// </summary>
    /// <returns> Id of the enemy </returns>
    public int GetId() { return id; }   

    /// <summary>
    /// Return position in the map
    /// </summary>
    /// <returns> Position in map </returns>
    public (int, int) GetPosInMap() { return map_position; }

    //-------SETTERS-----------------------------------   
    /// <summary>
    /// Update position
    /// </summary>
    /// <param name="new_pos"> new position</param>
    public void SetPosition( (int, int) new_map_pos, Vector2 new_pos) {
        map_position = new_map_pos;

        position= new_pos;
        this.transform.position = position;

        print("Posicion en el mapa " + this.map_position);
    }

    /// <summary>
    /// Set Action for enemy
    /// </summary>
    /// <param name="new_action"> New action </param>
    public void SetAction(Actions new_action) { action_actual = new_action; }

    /// <summary>
    /// Set new position for target
    /// </summary>
    /// <param name="new_target">New position to be the target</param>
    public void SetTarget(Vector2 new_target) { print("Mi nuevo objetivo es " + new_target); this.target= new_target; }

    //-------PUBLIC------------------------------------
    /// <summary>
    /// Enemy's move 
    /// </summary>
    public void Move()
    {
        Vector2 ls_new_position;

        if (!is_moving)
        {
            // Update Direction        
            next_step = path.Dequeue();
            // If next_step is not empty
            if(next_step != Vector2.zero)
            {
                path.Enqueue(next_step);

                ls_new_position = new Vector2(this.position.x, this.position.y);
                if (next_step.x != 0.0f)
                {
                    // Update position in axis x
                    ls_new_position.x += next_step.x;

                    // If movement in axis X, check if flip is needed
                    if (this.move.facing(ref this.facing_left, next_step.x))
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                }
                else if (next_step.y != 0.0f)
                {
                    // Update position in axis y
                    ls_new_position.y += next_step.y;
                }

                // Check collision
                if( !this.move.CheckCollision(this.position, ls_new_position, this.name_agent) )
                {
                    new_pos = ls_new_position;
                    is_moving = true;
                    StartCoroutine("UpdatePosition");
                }
            }                    
        }                  
    }
    
    /// <summary>
    /// Attacks receiver
    /// </summary>
    /// <param name="atck"> Attack to receive</param>
    /// <returns> Bool indicate if the agent is dead </returns>
    public bool ReceiveAttack(Attack atck)
    {
        print("Vida actual = " + agent_type.GetHp());
        stop = agent_type.ReceiveAttack(atck);
        print("Vida despues del ataque = " + agent_type.GetHp());
        return stop;
    }

    public void FoundEnemy(bool enemy_found, Vector2 player)
    {        
        if (enemy_found) 
        {
            player_in_range = true;
            this.action_actual = Actions.fight;

            //Update player position in channel
            this.channel.SetPlayerPos(player,this.id);

        } else
        {
            if (player_in_range)
            {
                player_in_range = false;
                this.channel.DeletePlayerPos(this.id);
            }
        }
    }

    public Actions PathFinding()
    {
        print("Mi objetivo es " + this.target);
        this.FillPath();

        if (path.Count != 0)
            action_actual = Actions.move;
        else
            action_actual = Actions.pass_turn;
        return action_actual;
    }

    public void Attack()
    {
        this.action_made = true;
    }

    /// <summary>
    /// Clear var for the actual turn
    /// </summary>
    public void InitTurn()
    {
        this.move_made = 0;
        this.action_actual = Actions.plan;
        this.action_made = false;
    }

    /// <summary>
    /// Update actual position with move controller
    /// </summary>
    /// <returns>Ienumerator needed for asynchronous operations</returns>
    IEnumerator UpdatePosition()
    {
        const float lv_time_move = 0.2f;
        float elapsed_time = 0.0f;

        while (elapsed_time < lv_time_move)
        {
            transform.position = Vector2.Lerp(this.position, this.new_pos, (elapsed_time / lv_time_move));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        is_moving = false;
        this.position = this.new_pos;
        this.move_made++;

        // Pos relative to map: Axis X
        if (next_step.x < 0)
            map_position.Item2 -= 1;
        else if (next_step.x > 0)
            map_position.Item2 += 1;

        // Pos relative to map: Axis Y
        if (next_step.y < 0)
            map_position.Item1 -= 1;
        else if (next_step.y > 0)
            map_position.Item1 += 1;

        if (this.move_made >= max_move - 1)
        {
            action_made = true;
            action_actual = Actions.pass_turn;
        }
    }
}
