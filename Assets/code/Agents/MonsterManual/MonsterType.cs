using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MonsterType
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    // Standard var
    protected float hp; // Life of monster

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */
    //-------ABSTRACT----------------------------------
    abstract public Attack DealAttack();
    abstract public int GetRange();


    //-------GETTERS-----------------------------------
    /// <summary>
    /// Get for HP
    /// </summary>
    /// <returns> float : hp</returns>
    public float GetHp() { return hp; }

    //-------PUBLIC------------------------------------
    public bool ReceiveAttack(Attack atck)
    {
        this.hp -= atck.GetDamage();
        if (this.hp <= 0.0f)
        {
            return true;
        }
        return false;
    }
}
