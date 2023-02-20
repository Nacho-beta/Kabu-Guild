using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Parameters
    private float   speed        = 1.0f,
                    time_to_move = 0.2f;
    private bool is_moving = false, // Bool indicate if object is moving
                    pos_eq_dest = true;
    private Vector2 destination = new Vector2(0, 0),
                    origin      = new Vector2(0, 0);

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    // Getters & Setters
    public void setSpeed(float set_speed) => speed = set_speed;

    public bool getPosOriDest() { return pos_eq_dest; }

    public Vector2 getDestination() { return destination; }
    // Public

    /* Move: Move the actor around the map
     * Return: Bool indicate if collision happen or not */
    public bool Move(ref Vector2 position, Vector2 direction, ref bool facing_left, string name_agent)
    {
        if(!is_moving)
        {
            origin.x = position.x;
            origin.y = position.y;

            destination.x = position.x;
            destination.y = position.y;

            // Move object        
            if (direction.x != 0.0f)
            {
                this.facing(ref facing_left, direction.x);
                destination.x += speed * direction.x;
            }
            else if (direction.y != 0.0f)
            {
                destination.y += speed * direction.y;
            }

            if (direction.x != 0.0f | direction.y != 0.0f)
            {
                if (!CheckCollision(origin, destination, name_agent))
                {
                    pos_eq_dest = false;
                    StartCoroutine("MoveInTileMap");
                    return true;
                }
            }
        }        

        return false;
    }

    /*
     * Method: Facing
     * Determinate the facing of actor, and change sprite
     */
    public void facing(ref bool facing_left, float input_x)
    {
        // Local Data
        Vector3 scale_flip = transform.localScale;

        // Function
        if(input_x != 0)
        {
            if (input_x > 0 & facing_left) // If facing left and movement right, flip
            {
                scale_flip.x *= -1;
                transform.localScale = scale_flip;
                facing_left = false;
            }
            else if (input_x < 0 & !facing_left) // If facing right and movement left, flip
            {
                scale_flip.x *= -1;
                transform.localScale = scale_flip;
                facing_left = true;
            }
        }        
    }


    // Private

    /* CheckCollision: Determinate if collision exist
     * Return: Bool indicate if collision happen or not */
    private bool CheckCollision(Vector2 position, Vector2 destination, string name_agent)
    {
        RaycastHit2D hit;
        Vector2 direction = destination - position;
        LayerMask mask_wall = new LayerMask();

        switch (AgentEnum.getAgent(name_agent))
        {
            case AgentEnum.Agent.Player:
                mask_wall = LayerMask.GetMask("Wall", "Enemy", "Item");
                break;
            case AgentEnum.Agent.Enemy:
                mask_wall = LayerMask.GetMask("Wall", "Player", "Item");
                break;
            default:
                break;
        }

        hit = Physics2D.Raycast(position, direction, speed, mask_wall);
        if(hit.collider != null )
        {
            return true;
        }

        return false;
    }

    private IEnumerator MoveInTileMap()
    {
        float elapsed_time = 0.0f;

        is_moving = true;

        while(elapsed_time < time_to_move)
        {
            transform.position = Vector2.Lerp(origin, destination, (elapsed_time / time_to_move));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        origin = transform.position;
        is_moving = false;
        pos_eq_dest = true;
    }
}
