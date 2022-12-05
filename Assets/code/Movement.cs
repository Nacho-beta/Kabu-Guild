using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Global Data
    public Vector2 speed = new Vector2(16, 16);
    public bool facing_left = false;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
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
}
