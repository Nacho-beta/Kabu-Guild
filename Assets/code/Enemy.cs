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

    // path: Stack with direction for pathing
    private Queue<Vector2> path;

    // speed to move: Constant needed for move method
    private Vector2 speed_move = new Vector2(1, 1);

    // next_step: Next position for pathing
    Vector2 next_step;


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
        position = new Vector2(1, 0);
        facing_left = true;
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
            if (next_step.x != position.x | next_step.y != position.y)
            {                
                if (next_step.x < position.x)
                {
                    speed_move.x = -1;
                }
                else if (next_step.x > position.x)
                {
                    speed_move.x = 1;
                }
                else
                {
                    speed_move.x = 0;
                }

                if (next_step.y < position.y)
                {
                    speed_move.y = -1;
                }
                else if (next_step.y > position.y)
                {
                    speed_move.y = 1;
                }
                else
                {
                    speed_move.y = 0;
                }

                collision = move.move(ref position, speed_move, ref facing_left, name_agent);
            } else
            {
                next_step = path.Dequeue();
                path.Enqueue(next_step);
            }
                           
            print("Posicion: " + position);
            print("Voy a " + next_step);
            print("Con dirección " + speed_move);

            

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

        new_point.x = 1;
        new_point.y = 0;
        path.Enqueue(new_point);

        new_point.x = 1;
        new_point.y = 1;
        path.Enqueue(new_point);

        new_point.x = 0;
        new_point.y = 1;
        path.Enqueue(new_point);

        new_point.x = -1;
        new_point.y = 1;
        path.Enqueue(new_point);

        new_point.x = -1;
        new_point.y = 0;
        path.Enqueue(new_point);

        new_point.x = 1;
        new_point.y = 0;
        path.Enqueue(new_point);
    }
}
