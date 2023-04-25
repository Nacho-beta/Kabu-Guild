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
                 action_made,       // Bool indicate if action was made
                 player_in_range,   // Bool indicate if player is in attack range
                 is_moving,
                 facing_up;         // Bool indicate if enemy come from upside
    private int max_move,       // Max of move can be made
                move_made,      // Actual move that have been made
                id;             // Enemy's Id

    private float limit_down_move,   // Inferior limit to move in Y axis
                  limit_up_move,     // Superior limit to move in Y axis
                  limit_right_move,  // Superior limit to move in X axis
                  limit_left_move;   // Inferior limit to move in X axis

    // Array Var
    private Vector2 next_step,      // Next position for pathing
                    new_pos,        // Vector2 to move
                    target,         // Vector2 to reach
                    position;       // Vector3(x, y, z) for position of agent
    private Queue<Vector2> path;    // Queue for pathing
    private (int, int) map_position;// Position in map
    private List<Vector2> direction;// Sensors of agent


    // Class var
    private Movement move;                  // Class for move to a position
    private MonsterType agent_type;         // Type of agent 
    private Actions action_actual;          // Actual action
    private CommunicationManager channel;   // Agent's Channel
    private MapManager map;                 // Map of the Game
    private SpriteRenderer sprite_renderer; // Sprite Renderer

    // Static var
    static int id_generator = 0;    // Id generator

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
        GameObject l_go_channel,
                   l_go_map;
        Vector2 new_sensor;

        name_agent = "Enemy";
        facing_left = false;
        stop = false;
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

        l_go_map = GameObject.FindGameObjectWithTag("Map");
        this.map = l_go_map.GetComponent<MapManager>();

        this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        // Fill sensors vector
        new_sensor.x = this.GetRange();

        // Fill sprite
        this.ChangeSprite();
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

    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Fill pathfinding
    /// </summary>
    private void FillPath()
    {
        path.Clear();
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
            new_step.y = 1;
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

    /// <summary>
    /// Calculate next position
    /// </summary>
    private void CalculateTarget()
    {
        List<Vector2> possible_target = new List<Vector2>();
        float   best_distance = float.MaxValue,
                distance;

        if (this.channel.IsPlayerLocated())
        {
            target = this.channel.GetPlayer();

            possible_target = this.FillSurroundingTarget(target);

            foreach(Vector2 target_to_check in possible_target)
            {
                distance = (float)Mathf.Pow(target_to_check.x - position.x, 2) + (float)Mathf.Pow(target_to_check.y - position.y, 2);
                distance = (float)Mathf.Sqrt(distance);
                if ( distance < best_distance)
                {
                    best_distance = distance;
                    target = target_to_check;
                }
            }
            
            if(this.target == this.position)
            {
                player_in_range = true;
            }
        }
        else
        {
            if (next_step.x != 0)
            {
                // Movement needed up/down
                target.x = position.x;

                if (facing_up)
                {
                    // If facing up = true -> Enemy come from bottom -> Enemy move to top                    
                    target.y = position.y + 2 * this.GetRange();
                }
                else
                {
                    // If facing up = false -> Enemy come from top -> Enemy move to bottom
                    target.y = position.y - 2 * this.GetRange();
                }

                this.TransformPosInLimit(target);
            }
            else
            {
                // Movement needed left/right
                target.y = position.y;

                if (facing_left)
                {
                    // If facing left = true -> Enemy come from right -> Enemy move to right
                    target.x = this.limit_right_move;
                }
                else
                {
                    // If facing left = false -> Enemy come from left -> Enemy move to left
                    target.x = this.limit_left_move;
                }
            }            
        }

        this.FillPath();
    }

    /// <summary>
    /// Search for player using map 
    /// </summary>
    private bool SearchPlayer()
    {
        bool player_found;

        player_found = this.map.CheckPlayerInRange(this.GetRange(), this.id);
        if (player_found)
        {
            channel.SetPlayerPos(map.GetPlayerPos());
            player_in_range = true;
        }

        return player_found;
    }

    /// <summary>
    /// Fill list with surrounding cells to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>List with position</returns>
    private List<Vector2> FillSurroundingTarget(Vector2 target)
    {
        List<Vector2> possibles_targets = new List<Vector2>();
        Vector2 target_aux = new Vector2();

        target_aux = target;

        target_aux.x = target.x - 1;
        possibles_targets.Add(target_aux);

        target_aux.x = target.x + 1;
        possibles_targets.Add(target_aux);

        target_aux = target;
        target_aux.y = target.y - 1;
        possibles_targets.Add(target_aux);

        target_aux.y = target.y + 1;
        possibles_targets.Add(target_aux);

        return possibles_targets;
    }

    /// <summary>
    /// Actions needed to be done after movement
    /// </summary>
    private void PostMovementActions()
    {
        action_actual = Actions.plan;

        // Update map position
        map.SetEnemy(this.map_position, id);

        // Check if player is in range
        if (this.SearchPlayer())
        {
            action_actual = Actions.pass_turn;            
        }

        // If movement was_completed, mark action as complete
        if (this.move_made >= max_move - 1)
        {
            action_made = true;
            action_actual = Actions.pass_turn;
        }
    }
    
    /// <summary>
    /// Surround obstacle in the path
    /// </summary>
    private void SurroundObstacle()
    {
        Vector2 new_step = new Vector2();
        Queue<Vector2> new_path = new Queue<Vector2>();

        if (this.next_step.x > 0)
        {
            new_step.x = 0;
            new_step.y = -1;
        } else if(this.next_step.x < 0)
        {
            new_step.x = 0;
            new_step.y = 1;
        } else if(this.next_step.y > 0)
        {
            new_step.x = 1;
            new_step.y = 0;
        } else if(this.next_step.y < 0)
        {
            new_step.x = -1;
            new_step.y = 0;
        }

        new_path.Enqueue(new_step);
        foreach(Vector2 step in this.path)
        {
            new_path.Enqueue(step);
        }

        path = new_path;
    }

    /// <summary>
    /// If position exceed the limits, this function chnage the position to one next to the limit
    /// </summary>
    /// <param name="position"> Position to transform </param>
    /// <returns> Bool which value represent if change has happen </returns>
    private bool TransformPosInLimit(Vector2 position)
    {
        bool ret = false;

        if(position.y > this.limit_up_move - 0.5)
        {
            position.y = this.limit_up_move;
            ret = true;
            this.facing_up = false;
        }

        if (position.y < this.limit_down_move + 0.5)
        {
            position.y = this.limit_down_move;
            ret = true;
            this.facing_up = true;
        }

        if (position.x < this.limit_left_move)
        {
            position.x = this.limit_left_move;
            ret = true;
        }

        if (position.x > this.limit_right_move)
        {
            position.x = this.limit_right_move;
            ret = true;
        }

        return ret;
    }

    private void ChangeSprite()
    {
        sprite_renderer.sprite = agent_type.GetSprite();
        sprite_renderer.size = new Vector2(1, 1);
    }

    //-------------------------------------------------   
    //-------GETTERS-----------------------------------   
    //-------------------------------------------------   
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
        target = position;
        this.transform.position = position;
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
    public void SetTarget(Vector2 new_target) { this.target= new_target; }

    /// <summary>
    /// Set for limit move up
    /// </summary>
    /// <param name="new_limit"> New limit</param>
    public void SetLimitUp(float new_limit) { this.limit_up_move = new_limit; }

    /// <summary>
    /// Set for limit move down
    /// </summary>
    /// <param name="new_limit"> New limit</param>
    public void SetLimitDown(float new_limit) { this.limit_down_move = new_limit; }

    /// <summary>
    /// Set right limit
    /// </summary>
    /// <param name="new_limit"> New limit</param>
    public void SetLimitRight(float new_limit) { this.limit_right_move = new_limit; }

    /// <summary>
    /// Set left limit
    /// </summary>
    /// <param name="new_limit"> New limit</param>
    public void SetLimitLeft(float new_limit) { this.limit_left_move = new_limit; }

    //-------PUBLIC------------------------------------
    /// <summary>
    /// Enemy's move 
    /// </summary>
    public bool Move()
    {
        Vector2 ls_new_position;  
        
        if(target == position)
        {
            action_actual = Actions.plan;
            return false;
        }
        else if (!is_moving)
        {
            // Update Direction
            if(path.Count == 0)
            {
                this.CalculateTarget();
            }
            next_step = path.Dequeue();
            
            // If next_step is not empty
            if(next_step != Vector2.zero)
            {                
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

                this.TransformPosInLimit(ls_new_position);

                // Check collision
                if ( !this.move.CheckCollision(this.position, ls_new_position, this.name_agent) )
                {
                    new_pos = ls_new_position;
                    is_moving = true;
                    StartCoroutine("UpdatePosition");
                } else
                {
                    if(this.move.CheckCollisionWall(this.position, ls_new_position))
                    {
                        this.CalculateTarget();
                    }
                    else
                    {
                        this.SurroundObstacle();
                    }
                }
            }           
        }

        return true;
    }
    
    /// <summary>
    /// Attacks receiver
    /// </summary>
    /// <param name="atck"> Attack to receive</param>
    /// <returns> Bool indicate if the agent is dead </returns>
    public bool ReceiveAttack(Attack atck)
    {        
        stop = agent_type.ReceiveAttack(atck);        
        return stop;
    }

    public Actions PathFinding()
    {
        action_actual = Actions.move;

        // If player is in range, fight
        if (player_in_range)
        {
            action_actual = Actions.fight;
        }

        if (this.action_made)
        {
            action_actual = Actions.pass_turn;
        }

        // If target was reached, calculate new target
        if(this.position == this.target)
        {
            this.CalculateTarget();
        }        
        
        return action_actual;
    }

    public Attack Attack()
    {
        this.action_made = true;
        return this.agent_type.DealAttack();
    }

    /// <summary>
    /// Clear var for the actual turn
    /// </summary>
    public void InitTurn()
    {
        bool player_found;

        this.move_made = 0;
        this.action_actual = Actions.plan;
        this.action_made = false;
        this.player_in_range = false;

        // Check if player is in range
        this.SearchPlayer();
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

        this.PostMovementActions();
    }
}
