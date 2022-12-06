using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Global Data
    private Vector2 speed = new Vector2(16, 16);
    private bool facing_left = false;
    private Rigidbody2D rigid_body;

    // Update is called once per frame
    public void move()
    {
        // Local Data
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");

        Vector3 move,
                scale_flip = transform.localScale;

        // Function
            //Facing
        if(input_x > 0 & facing_left) // If facing left and movement right, flip
        {
            scale_flip.x *= -1;
            transform.localScale = scale_flip;
            facing_left = false;
        } else if(input_x < 0 & !facing_left) // If facing right and movement left, flip
        {
            scale_flip.x *= -1;
            transform.localScale = scale_flip;
            facing_left = true;
        }

            // Movement
        move = new Vector3(speed.x * input_x, speed.y * input_y, 0);
        move *= Time.deltaTime;
        transform.Translate(move);
    }

    void check_collision(Rigidbody2D rb)
    {
        // Data

        // Function
        rigid_body = rb;

    }
}
