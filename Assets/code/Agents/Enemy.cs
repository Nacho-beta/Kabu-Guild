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
                 pos_ok,            // Bool indicate if position is updated
                 action_made;       // Bool indicate if action was made
    private int max_move,       // Max of move can be made
                move_made,      // Actual move that have been made
                id;

    // Array Var
    private Vector2 next_step,  // Next position for pathing
                    last_step,  // Last position in pathfinding
                    position;   //Vector3(x, y, z) for position of agent
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
        action_made = false;
        max_move = 3;
        move_made = 0;

        path = new Queue<Vector2>();
        next_step = new Vector2(0, 0);
        last_step = new Vector2(0, 0);
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


    //-------PUBLIC------------------------------------
    /// <summary>
    /// Enemy's move 
    /// </summary>
    /// <returns> Bool indicate success or failure </returns>
    public bool Move()
    {        
        bool move_happen = false;

        if(last_step != next_step)
        {
            move_happen = this.move.Move(ref position, next_step, ref facing_left, name_agent);
            if (move_happen)
            {
                last_step = next_step;
                pos_ok = false;
                StartCoroutine("UpdatePosition");
                return true;
            }
        }
       
        return false;
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

    public void FoundEnemy(bool enemy_found)
    {        
        if (enemy_found) 
        {
            player_in_range = enemy_found;
            pos_ok = true;
            this.action_actual = Actions.fight;
        }
    }

    public Actions PathFinding()
    {
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
    /// Update actual position with move controller
    /// </summary>
    /// <returns>Ienumerator needed for asynchronous operations</returns>
    IEnumerator UpdatePosition()
    {
        while (!pos_ok)
        {
            pos_ok = this.move.getPosOriDest();
            yield return null;
        }

        // Update pos
        position = this.move.getDestination();
        
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

        // Update next step        
        next_step = path.Dequeue();
        path.Enqueue(next_step);

        this.move_made++;
        if (this.move_made > max_move)
        {
            action_actual = Actions.pass_turn;
        }
    }
}
