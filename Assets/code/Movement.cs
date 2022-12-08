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
     */
    public void move(ref Vector2 position)
    {
        // Local Data
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(position.x, position.y);

        // Move object
        if (input_x != 0.0f)
        {
            move.x += speed*input_x;
        }
        else if (input_y!= 0.0f)
        {
            move.y += speed*input_y;
        }

        if (input_x != 0.0f | input_y != 0.0f)
        {
            if (!checkCollision(position, move))
            {
                transform.position = Vector2.MoveTowards(position, move, speed * Time.deltaTime);
                position = transform.position;
            }            
        }            
    }

    /*
     * Method: Facing
     * Determinate the facing of actor, and change sprite
     */
    public void facing(ref bool facing_left)
    {
        // Local Data
        Vector3 scale_flip = transform.localScale;

        float input_x = Input.GetAxis("Horizontal");

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
    private bool checkCollision(Vector2 position, Vector2 destination)
    {
        RaycastHit2D hit;

        LayerMask mask_wall = LayerMask.GetMask("Wall");

        hit = Physics2D.Raycast(position, destination, 0.25f, mask_wall);
        if(hit.point != Vector2.zero )
        {
            return true;
        }

        return false;
    }
}
