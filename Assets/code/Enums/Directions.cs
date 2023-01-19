using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions : MonoBehaviour
{
    public enum Direction { up, left, right, down };

    // Get Agent by string
    static public Direction getDirection(int num)
    {
        Direction get_direction;

        switch (num)
        {
            case 0:
                get_direction = Direction.up;
                break;
            case 1:
                get_direction = Direction.left;
                break;
            case 2:
                get_direction = Direction.right;
                break;
            default:
                get_direction = Direction.down;
                break;
        }

        return get_direction;
    }

}
