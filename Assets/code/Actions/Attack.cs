using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------
    
    //Standard type    
    private float dmg;
    private int range;

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    // Constructor
    public Attack()
    {
        dmg = 1.0f;
        range = 1;
    }


    //-------GETTERS-----------------------------------
    /// <summary>
    /// Get Damage
    /// </summary>
    /// <returns></returns>
    public float GetDamage() { return dmg; }

    /// <summary>
    /// Get Range
    /// </summary>
    /// <returns>range</returns>
    public int GetRange() { return range; }

    //-------SETTERS-----------------------------------
    /// <summary>
    /// Damage
    /// </summary>
    /// <param name="damage"></param>
    public void SetDamage(float damage) { dmg = damage; }

    //-------PUBLIC------------------------------------
    public void CreateLongSword()
    {
        dmg = 5.0f;
        range = 2;
    }

    public void CreateKoboldBite()
    {
        dmg = 2.0f;
        range = 1;
    }
}
