using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*
     * Atributes of enemy
     */
    // Private
    private string name_agent;  // Name of the Agent
    private bool facing_left;   // Bool indicate if is facing to left or right
    private bool stop;          // Bool indicate if is in movement
    private float hp;

    private Vector2 speed_move; // Vector for direction
    private Vector2 next_step;  // Next position for pathing

    private Queue<Vector2> path;// Queue for pathing

    private Movement move;      // Class for move to a position
    
    // Protected

    // Public
    public Vector2 position; //Vector3(x, y, z) for position of agent


    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

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

        ret_enemy.fillPath();

        return ret_enemy;
    }


    // ---------- Getters & Setters ----------------
    // GetHP : Return actual hp
    public float getHp()
    {
        return hp;
    }


    // ---------- Public ---------------------------
    /*
     * Start
     * Start is called before the first frame update
     */
    void Start()
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

        this.fillPath();
    }

    /*
     * Update
     * Update is called once per frame
     */
    void Update()
    {
        bool collision = false;

        if (GameManager.canMove(name_agent))
        {
            if (!stop)
            {
                this.fillDirection();

                if (speed_move.x == 0 & speed_move.y == 0)
                {
                    next_step = path.Dequeue();
                    path.Enqueue(next_step);
                } else
                {
                    collision = move.Move(ref position, speed_move, ref facing_left, name_agent);
                }

                if (collision)
                {
                    stop = true;
                    hp = 0.0f;
                }
            }

            GameManager.endTurn(name_agent);
        }

    }

    /*
     * fill_path
     * Get path finding for agent
     */
    void fillPath()
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

    /*
     * Private: fillDirection
     * Fill value in vector direction
     */
    void fillDirection()
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
}
