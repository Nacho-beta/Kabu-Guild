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

        // Get reference for Action menu
        this.menu = GameObject.FindGameObjectWithTag("ActionMenu");        

        status_menu = true;
        is_showing = true;
    }

    //Update
    public void Update()
    {
        this.CheckButtonVisibility();       
    }

    // SetActionMove : Function for button move
    public void SetActionMove()
    {
        this.player.SetAction(Actions.move);
    }

    // SetActionAttack : Function for button attack
    public void SetActionFight()
    {
        this.player.SetAction(Actions.fight);
    }

    public void SetActionSkill()
    {
        this.player.SetAction(Actions.Skill);
    }

    // CheckButtonVisibility
    private void CheckButtonVisibility()
    {

        if (status_menu != is_showing)
        {
            if(status_menu == false)
            {
                this.menu.transform.localScale = new Vector3(0, 0, 0);
                is_showing = false;
            } else
            {
                this.menu.transform.localScale = new Vector3(1, 1, 1);
                is_showing = true;
            }
        }
        
    }
}
