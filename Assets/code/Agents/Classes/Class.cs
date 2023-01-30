using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Class
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    // Class Var
    protected Classes class_type;
    protected Player player;

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------PUBLIC------------------------------------
    
    public Class()
    {
        GameObject l_go_player;
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();
    }

    // UseSkill: Use specific class skill
    abstract public void UseSkill();
}
