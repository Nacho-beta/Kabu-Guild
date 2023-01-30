using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------
    Player player;

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------
    
    // Start
    public void Start()
    {
        // Variables
        GameObject l_go_player;

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();
    }

    // Update
    public void Update()
    {
        transform.position = new Vector3( player.GetPosition().x, player.GetPosition().y, this.transform.position.z );
    }
}
