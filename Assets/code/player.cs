using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

   // private Rigidbody2D rigid_body;
   // private BoxCollider2D hitbox;


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
        name_agent = "Player";
        position = new Vector2(0, 0); // 4, 4, 0
        facing_left = false;

        move = gameObject.AddComponent(typeof(Movement)) as Movement;
    }

    /*
     * Update
     * Update is called once per frame
     */
    void Update()
    {
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");
        Vector2 destination = new Vector2();

        if (GameManager.canMove(name_agent))
        {
            destination.x = input_x;
            destination.y = input_y;

            if(destination != Vector2.zero) 
            {
                move.move(ref position, destination, ref facing_left, name_agent);
                
            }            
        }

        GameManager.endTurn(name_agent);
    }

    /*
     * WaitForKey
     * Wait to the key is pressed
     */
    private IEnumerator waitForKey()
    {
        bool done = false;
        while (!done)
        {
            if (Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.UpArrow) |
                Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.LeftArrow) |
                Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.RightArrow) |
                Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.DownArrow)
                )
            {
                done = true;
            }
            yield return null;
        }
    }
}
