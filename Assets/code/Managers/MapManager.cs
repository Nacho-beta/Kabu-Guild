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
    private string key; // Key needed for communication

    // Array var
    private (int,int) player;     // Position in map for player
    private (int,int)[] enemies;  // Position in map for enemies

    // Class var
    private CellType[,] map;   // Matrix with map info

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    //-------GETTERS-----------------------------------
    public string GetKey() { return key; }

    public Vector2 GetEnemyPos(int index)
    {
        (int, int) pos_map_enemy = enemies[index];
        
        Vector2 pos_scene = new Vector2();

        pos_scene.x = pos_map_enemy.Item2 - Mathf.Floor(this.width / 2) + 0.5f;
        pos_scene.y = pos_map_enemy.Item1 - Mathf.Floor(this.height / 2) + 0.75f;

        return pos_scene;
    }

    //-------SETTERS-----------------------------------

    /// <summary>
    /// Update player position adding the new position
    /// </summary>
    /// <param name="new_pos"> Pos to add to actual position </param>
    public void SetPlayer((int,int) new_pos)
    {
        map[player.Item2, player.Item1] = CellType.Empty;

        player.Item1 += new_pos.Item2;
        player.Item2 += new_pos.Item1;
                
        map[player.Item2, player.Item1] = CellType.Player;
    }

    public void SetEnemy((int, int) new_pos, int index)
    {
        if(new_pos.Item1 != 0 | new_pos.Item2 != 0)
        {
            (int, int) enemy_actual = enemies[index];
            map[enemy_actual.Item2, enemy_actual.Item1] = CellType.Empty;

            enemy_actual.Item1 += new_pos.Item2;
            enemy_actual.Item2 += new_pos.Item1;

            //print("Posicion del agente "+index+" = " + enemy_actual);

            map[enemy_actual.Item2, enemy_actual.Item1] = CellType.Enemy;
            enemies[index] = enemy_actual;
        }        
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
        enemies = new (int,int)[2];

        // Key
        key = "Kobold Hill";

        // Fill map array
        width = 19; height = 20;
        map = new CellType[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 9 & j == 9)
                {
                    player.Item1 = 9; player.Item2 = 9;
                    map[i, j] = CellType.Player;
                }
                else
                {
                    map[i, j] = CellType.Empty;
                }
            }
        }

        enemies[0].Item1 = 9; enemies[0].Item2 = 11;
        map[11, 9] = CellType.Enemy;

        enemies[1].Item1 = 11; enemies[1].Item2 = 11;
        map[11, 11] = CellType.Enemy;
    }

    public int CheckEnemiesInRange(int range)
    {
        int enemies_detected = 0;
        //print("Jugador: x = " + player.x+" ; y = "+player.y);
        //print("Enemigo: x = " + enemies[0].x + " ; y = " + enemies[0].y);
        CellType cell_actual = CellType.Empty;
        for(int i=0; i<= range; i++)
        {
            for(int j=0; j<= (range-i); j++)
            {                
                if (i!=0 | j!=0)
                {
                    cell_actual = this.GetMapCell(i + player.Item2, j + player.Item1);
                    if(cell_actual == CellType.Enemy)
                    {
                        enemies_detected++;
                    }
                    //print("Casilla["+(player.y+i)+","+(player.x+j)+"] - Tipo " + cell_actual);

                    if(i != 0)
                    {
                        cell_actual = this.GetMapCell(-i + player.Item2, j + player.Item1);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }
                        //print("Casilla[" + (player.y - i) + "," + (player.x + j) + "] - Tipo " + cell_actual);
                    }


                    if (j != 0)
                    {
                        cell_actual = this.GetMapCell(i + player.Item2, -j + player.Item1);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }

                        //print("Casilla[" + (player.y + i) + "," + (player.x - j) + "] - Tipo " + cell_actual);
                    }
                    

                    if ( i!=0 & j != 0)
                    {
                        cell_actual = this.GetMapCell(-i + player.Item2, -j + player.Item1);
                        if (cell_actual == CellType.Enemy)
                        {
                            enemies_detected++;
                        }

                        //print("Casilla[" + (player.y - i) + "," + (player.x - j) + "] - Tipo " + cell_actual);
                    }
                    

                }
            }
        }

        return enemies_detected;
    }

    /// <summary>
    /// Detect if player is in range of the enemy
    /// </summary>
    /// <param name="range">Raneg of the enemy</param>
    /// <param name="index">Index for array of enemies</param>
    /// <returns></returns>
    public bool CheckPlayerInRange(int range, int index)
    {
        bool ret_player_found = false;
        (int,int) enemy_actual = enemies[index];
        CellType cell_actual = CellType.Empty;

        //print("Posicion del jugador = " + player.Item2 + " , " + player.Item1);

        for (int i = 0; i <= range & !ret_player_found; i++)
        {
            for (int j = 0; j <= (range - i) & !ret_player_found; j++)
            {
                if (i != 0 | j != 0)
                {
                    
                    cell_actual = this.GetMapCell(i + enemy_actual.Item2, j + enemy_actual.Item1);
                    if (cell_actual == CellType.Player)
                    {
                        ret_player_found = true;
                    }
                    //print("Cell [" + (enemy_actual.Item2+i) + "," + (enemy_actual.Item1+j) + "] = " + cell_actual);

                    if (i != 0)
                    {
                        cell_actual = this.GetMapCell(-i + enemy_actual.Item2, j + enemy_actual.Item1);
                        if (cell_actual == CellType.Player)
                        {
                            ret_player_found = true;
                        }
                        //print("Cell [" + (enemy_actual.Item2 - i) + "," + (enemy_actual.Item1 + j) + "] = " + cell_actual);
                    }


                    if (j != 0)
                    {
                        cell_actual = this.GetMapCell(i + enemy_actual.Item2, -j + enemy_actual.Item1);
                        if (cell_actual == CellType.Player)
                        {
                            ret_player_found= true;
                        }
                        //print("Cell [" + (enemy_actual.Item2 + i) + "," + (enemy_actual.Item1 - j) + "] = " + cell_actual);
                    }


                    if (i != 0 & j != 0)
                    {
                        cell_actual = this.GetMapCell(-i + enemy_actual.Item2, -j + enemy_actual.Item1);
                        if (cell_actual == CellType.Player)
                        {
                            ret_player_found = true;
                        }
                        //print("Cell [" + (enemy_actual.Item2 - i) + "," + (enemy_actual.Item1 - j) + "] = " + cell_actual);
                    }


                }
            }
        }

        return ret_player_found;
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
