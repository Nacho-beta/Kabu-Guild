using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Class
{
    /*
     * ------------------------------------------------------
     * Parameters
     * ------------------------------------------------------
     */
    // Standard types
    private float shield;

    // Class types
    private Attack long_sword;   // Attack

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------GETTERS-----------------------------------
    /// <summary>
    /// Get Attack Great Sword
    /// </summary>
    /// <returns>Great Sword attack</returns>
    public Attack LongSword() { return long_sword; }

    //-------PRIVATE------------------------------------

    // PutShield : Add hp 
    private void PutShield()
    {
        this.player.SetHP(this.player.GetHP() + this.shield);
    }


    //-------PUBLIC------------------------------------

    public Warrior() 
    {
        this.class_type = Classes.Warrior;
        this.shield = 10.0f;

        long_sword = new Attack();
        long_sword.CreateLongSword();

        this.NewSprite();
    }

    // UseSkill: Use specific class skill
    override public void UseSkill()
    {
        this.PutShield();
    }

    // NewSprite: Change Sprite to Warrior Sprite
    override public void NewSprite() 
    {
        this.sprites = Resources.LoadAll<Sprite>("warrior");
        this.sprites_size = this.sprites.Length;
    }

    /// <summary>
    /// Override abstract method Attack
    /// </summary>
    /// <returns> Attack from Long Sword </returns>
    override public Attack Attack()
    {
        return this.long_sword;
    }

}
