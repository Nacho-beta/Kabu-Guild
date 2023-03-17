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
    // Standard var
    protected int   sprite_iter,    // Iterator for sprites array
                    sprites_size;   // Size of sprite array
    
    // Array var
    protected Sprite[] sprites;    

    // Class Var
    protected GameObject go_player;
    protected Classes class_type;
    protected Player player;

    /*
     * ------------------------------------------------------
     * Methods
     * ------------------------------------------------------
     */

    //-------GETTERS-----------------------------------
    // Sprite
    public Sprite GetSprite()
    {
        Sprite ret_sprite = sprites[sprite_iter];

        sprite_iter = (sprite_iter + 1) % sprites_size;

        return ret_sprite;
    }

    public int GetSpritesSize() { return sprites_size; }


    //-------PUBLIC------------------------------------

    public Class()
    {
        go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = go_player.GetComponent<Player>();

        sprite_iter = 0;
    }    

    // UseSkill: Use specific class skill
    abstract public void UseSkill();

    // NewSprite: Change sprite to specific of a class
    abstract public void NewSprite();

    /// <summary>
    /// Calculate damage deal by the class
    /// </summary>
    /// <returns> Attack of the class</returns>
    abstract public Attack Attack();
}
