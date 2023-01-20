using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------
    
    // Private
    private Player player;  // Player of game   

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------
    
    //Start
    public void Start()
    {
        GameObject l_go_player;

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();
    }

    public void setActionMove()
    {
        this.player.SetAction(Actions.move);
    }
}
