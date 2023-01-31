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
    private float shield;

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

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

}
