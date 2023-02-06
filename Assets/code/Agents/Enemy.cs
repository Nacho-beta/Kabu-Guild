using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------
    // Private
    public string name_agent;  // Name of the Agent
    private bool facing_left;   // Bool indicate if is facing to left or right
    private bool stop;          // Bool indicate if is in movement
    private float hp;           // Hit Points
    private int max_move = 6;   // Max of move can be made
    private int move_made = 0;  // Actual move that have been made

    private Vector2 speed_move; // Vector for direction
    private Vector2 next_step;  // Next position for pathing

    private Queue<Vector2> path;// Queue for pathing

    private Movement move;      // Class for move to a position
    
    // Protected

    // Public
    public Vector2 position; //Vector3(x, y, z) for position of agent


    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    // ---------- Static ---------------------------
    public static Enemy getEnemyDefault()
    {
        Enemy ret_enemy = new Enemy();

        ret_enemy.name_agent = "Enemy";
        ret_enemy.facing_left = true;
        ret_enemy.stop = false;
        ret_enemy.hp = 15.0f;

        ret_enemy.path = new Queue<Vector2>();
        ret_enemy.next_step = new Vector2(1, 0);
        ret_enemy.speed_move = new Vector2(1, 1);
        ret_enemy.position = new Vector2(3, 0);

        ret_enemy.FillPath();

        return ret_enemy;
    }


    // ---------- Getters & Setters ----------------
    // GetHP : Return actual hp
    public float getHp(){ return hp; }

    // SetAction. Return Enemy Action
    public Actions SetAction()
    {
        if (move_made < max_move)
        {
            return Actions.move;
        }
        else
        {
            return Actions.none;
        }
    }


    // ---------- Private --------------------------
    
    // FillPath: get pathfinding for agent
    private void FillPath()
    {
        Vector2 new_point = new Vector2(0, 0);

        path.Enqueue(new_point);

        for (int i = 0; i < 8; i++)
        {
            new_point.x = 3;
            new_point.y = i;
            path.Enqueue(new_point);
        }

        for (int i = 8; i >= 0; i--)
        {
            new_point.x = 3;
            new_point.y = i;
            path.Enqueue(new_point);
        }
    }

    // FillDirection : Fill vector for direction with position and destination
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
        }

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

    // ---------- Public ---------------------------

    // Start : Only execution in first frame
    public void Start()
    {
        name_agent = "Enemy";
        facing_left = true;
        stop = false;
        hp = 15.0f;

        path = new Queue<Vector2>();
        next_step = new Vector2(1, 0);
        speed_move = new Vector2(1, 1);
        position = new Vector2(3, 0);

        move = gameObject.AddComponent(typeof(Movement)) as Movement;

        this.FillPath();
    }

    // Move : Move enemy to position in pathing
    public void Move()
    {
        bool move_happen = true;

        if (!stop)
        {
            this.FillDirection();

            if (speed_move.x == 0 & speed_move.y == 0)
            {
                next_step = path.Dequeue();
                path.Enqueue(next_step);
            }
            else
            {
                move_happen = move.Move(ref position, speed_move, ref facing_left, name_agent);
                if(!move_happen)
                {
                    print("No me he podido mover, me muero");
                    this.hp = 0.0f;
                    stop = true;
                }
            }
        }
    }
    
    // ReceiveAttack : Receive attack and calculate results
    public bool ReceiveAttack(Attack atck)
    {
        print("Mi vida" + this.hp);
        this.hp -= atck.GetDamage();
        print("Despues del golpe:" + this.hp);
        if (this.hp <= 0.0f)
        {
            stop = true;
            return true;
        }
        return false;
    }
}
