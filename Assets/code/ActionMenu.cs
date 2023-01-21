using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------

    // Private
    //      Standard var
    private bool status_menu,
                 is_showing;
    //      Class var
    private GameObject menu;
    private Player player;  // Player of game


    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    //Set for status_menu
    public void SetStatusMenu(bool new_status) { status_menu = new_status; }

    //Start
    public void Start()
    {
        GameObject l_go_player;

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();

        status_menu = true;
        is_showing = true;
    }

    //Update
    public void Update()
    {
        if(status_menu != is_showing)
        {
            menu.SetActive(status_menu);
            is_showing = !is_showing;
        }        
    }

    // SetActionMove : Function for button move
    public void SetActionMove()
    {
        this.player.SetAction(Actions.move);
    }    
}
