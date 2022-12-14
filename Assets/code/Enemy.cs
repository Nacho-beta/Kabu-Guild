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

    // name_agent : Name of the Agent
    private string name_agent;

    // move : Method to move to a position
    private Movement move;

    // position : Vector3 (x,y,z) for position of agent
    public Vector2 position;

    // facing : Bool that indicate if is facing to left or right
    private bool facing_left;

    // collision: Bool indicate if collision happen
    bool stop;

    // path: Stack with direction for pathing
    private Queue<Vector2> path;

    // speed to move: Constant needed for move method
    private Vector2 speed_move = new Vector2(1, 1);

    // next_step: Next position for pathing
    private Vector2 next_step;


    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */
    /*
     * Start
     * Start is called before the first frame update
     */
    void Start()
    {
        name_agent = "Enemy";
        position = new Vector2(3, 0);
        facing_left = true;
        stop = false;
        path = new Queue<Vector2>();
        next_step = new Vector2(1, 0);

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
                    collision = move.move(ref position, speed_move, ref facing_left, name_agent);
                }

                if (collision)
                {
                    stop = true;
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
