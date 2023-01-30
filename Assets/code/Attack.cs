using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------
    private float dmg;

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    // Constructor
    public Attack()
    {
        dmg = 1.0f;
    }

    // SetDamage
    public void SetDamage(float damage) { dmg = damage; }
    // GetDamage
    public float GetDamage() { return dmg; }
}
