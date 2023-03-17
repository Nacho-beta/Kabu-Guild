using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MonsterType
{
    //-------------------------------------------------
    //-------PARAMETERS--------------------------------
    //-------------------------------------------------

    // Standard var
    protected float hp;             // Life of monster
    protected int   sprite_iter,    // Iterator for sprites array
                    sprites_size;   // Size of sprite array

    // Array type
    protected Sprite[] sprites;

    
    //-------------------------------------------------
    //-------ABSTRACT----------------------------------
    //-------------------------------------------------
    abstract public Attack DealAttack();
    abstract public int GetRange();
    abstract public void NewSprite();

    //-------------------------------------------------
    //-------GETTERS-----------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Get for HP
    /// </summary>
    /// <returns> float : hp</returns>
    public float GetHp() { return hp; }

    /// <summary>
    /// Get numbers of sprites
    /// </summary>
    /// <returns>Quantity of sprites</returns>
    public int GetSpriteCount() { return sprites_size; }

    public Sprite GetSprite()
    {
        Sprite ret_sprite = sprites[sprite_iter];

        sprite_iter = (sprite_iter + 1) % sprites_size;

        return ret_sprite;
    }

    //-------------------------------------------------
    //-------PUBLIC------------------------------------
    //-------------------------------------------------
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
