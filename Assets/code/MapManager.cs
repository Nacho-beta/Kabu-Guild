using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------
    
    // Standard Var
    private int width, 
                height;

    // Array var
    private Vector2 player;     // Position in map for player
    private Vector2[] enemies;  // Position in map for enemies

    // Class var
    private CellType[,] map;   // Matrix with map info

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------
    
    // Set for player position
    public void SetPlayer(Vector2 new_position)
    {
        map[(int)player.y, (int)player.x] = CellType.Empty;

        if(new_position.x > 0)
        {
            player.x = player.x + (int)new_position.x + 1;
        } else
        {
            player.x = player.x + (int)new_position.x;
        }


        if (new_position.y > 0)
        {
            player.y = player.y + (int)new_position.y + 1;
        } else {
            player.y = player.y + (int)new_position.y;
        }
                
        map[(int)player.y, (int)player.x] = CellType.Player;
    }

    // Get for a cell in the map
    public CellType GetMapCell(int i, int j)
    {
        if(i<0 || i>=height || j<0 || j >= width)
        {
            print("Error access map position");
            return CellType.Error;            
        }
        else
        {
            return map[(int)i, (int)j];
        }
    }

    // GenerateDesertHill : Generate map "Desert Hill"
    public void GenerateDesertHill()
    {
        enemies = new Vector2[1];

        // Fill map array
        width = 25; height = 21;
        map = new CellType[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 10 & j == 14)
                {
                    player.x = 14; player.y = 10;
                    map[i, j] = CellType.Player;
                }
                else if (i == 10 & j == 15)
                {
                    enemies[0].x = 15; enemies[0].y = 10;
                    map[i, j] = CellType.Enemy;
                }
                else
                {
                    map[i, j] = CellType.Empty;
                }
            }
        }                     
    }

    public int CheckEnemiesInRange(int range)
    {
        int enemies_detected = 0;
        CellType cell_actual = CellType.Empty;
        for(int i=0; i<= range; i++)
        {
            for(int j=0; j<= (range-i); j++)
            {                
                if (i!=0 | j!=0)
                {
                    cell_actual = this.GetMapCell(i + (int)player.y, j + (int)player.x);
                    if(cell_actual == CellType.Enemy)
                    {
                        enemies_detected++;
                    }

                    if(i != 0)
                    {
                        cell_actual = this.GetMapCell(-i + (int)player.y, j + (int)player.x);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }
                    }
                    
                    if(j != 0)
                    {
                        cell_actual = this.GetMapCell(i + (int)player.y, -j + (int)player.x);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }
                    }

                    if( i!=0 & j != 0)
                    {
                        cell_actual = this.GetMapCell(-i + (int)player.y, -j + (int)player.x);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }
                    }                                       
                }
            }
        }

        return enemies_detected;
    }

    public void PrintMap()
    {
        for(int i=0; i<height;i++)
        {
            for(int j=0; j<width;j++)
            {
                print(map[i, j]);
            }
        }
    }

}

public enum CellType
{
    Empty,
    Player,
    Enemy,
    Item,
    Wall,
    Error
}
