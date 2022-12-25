using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Parameters
    private float speed = 5f;    
    

    // Getters & Setters
    public void setSpeed(float set_speed) => speed = set_speed;

    // Methods
    /*
     * Method: Move
     * Move the actor around the map
     * Return: Bool indicate if collision happen or not
     */
    public bool Move(ref Vector2 position, Vector2 direction, ref bool facing_left, string name_agent)
    {
        // Local Data
        Vector2 move = new Vector2(position.x, position.y);
        
        // Move object        
        if (direction.x != 0.0f)
        {
            this.facing(ref facing_left, direction.x);
            move.x += speed*direction.x;
        }
        else if (direction.y!= 0.0f)
        {
            move.y += speed*direction.y;
        }

        if (direction.x != 0.0f | direction.y != 0.0f)
        {
            if (!checkCollision(position, move, name_agent))
            {
                transform.position = Vector2.MoveTowards(position, move, speed * Time.deltaTime);
                position = transform.position;
            } else
            {
                return true;
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

    /*
     * Method: Collision detect
     * Determinate the facing of actor, and change sprite
     */
    private bool checkCollision(Vector2 position, Vector2 destination, string name_agent)
    {
        RaycastHit2D hit;
        Vector2 direction = destination - position;
        LayerMask mask_wall = new LayerMask();

        switch (AgentEnum.getAgent(name_agent))
        {
            case AgentEnum.Agent.Player:
                mask_wall = LayerMask.GetMask("Wall", "Enemy");
                break;
            case AgentEnum.Agent.Enemy:
                mask_wall = LayerMask.GetMask("Wall", "Player");
                break;
            default:
                break;
        }        

        hit = Physics2D.Raycast(position, direction, 0.5f, mask_wall);
        if(hit.collider != null )
        {
            return true;
        }

        return false;
    }
}
