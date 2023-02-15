using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kobold : MonsterType
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    // Standard var
    private int range;

    // Class var
    private Attack my_attack;    

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------GETTERS-----------------------------------
    /// <summary>
    /// Get Range
    /// </summary>
    /// <returns> int : range of attacks</returns>
    override public int GetRange() { return range; }

    //-------PUBLIC------------------------------------
    public Kobold()
    {
        my_attack = new Attack();
        my_attack.CreateKoboldBite();

        range = my_attack.GetRange();
    }

    override public Attack DealAttack()
    {
        return my_attack;
    }
}
