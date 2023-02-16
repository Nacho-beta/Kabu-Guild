using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private (int,int) player;               // Position in map for player
    private (int,int)[] enemies;            // Position in map for enemies
   // private Vector3Int[] tile_highlighted;  // Array for position of highlighted tiles  

    // Class var
    private CellType[,] map;   // Matrix with map info
    private Tilemap co_tilemap;  // Tile map of the scene
    //private Tile 

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    //-------UNITY-------------------------------------
    /// <summary>
    /// Execution of start in the first frame
    /// </summary>
    public void Start()
    {
        GameObject l_go_floor;

        l_go_floor = GameObject.FindGameObjectWithTag("Floor");
        this.co_tilemap = l_go_floor.GetComponent<Tilemap>();
    }


    //-------PRIVATE-----------------------------------
    /// <summary>
    /// Remark a Tile when it is needed
    /// </summary>
    /// <param name="x"> Width position </param>
    /// <param name="y"> Hight postiion </param>
    private void HighlightTile(int x, int y)
    {
        (int, int) medium_point;
        Vector3Int grid_position = new Vector3Int();

        medium_point.Item1 = (int)Mathf.Floor(width / 2.0f);
        medium_point.Item2 = (int)Mathf.Floor(height / 2.0f);

        // Get position of tile in the world
        //grid_position.x = 1;
        grid_position.x = (int)Mathf.Ceil(x - (this.width / 2.0f) - 1.5f);        
        grid_position.y = (int)(y - (this.height / 2.0f));
        //grid_position.y = -1;

        co_tilemap.SetTileFlags(grid_position, TileFlags.None);
        co_tilemap.SetColor(grid_position, Color.red);
    }

    //-------GETTERS-----------------------------------
    public string GetKey() { return key; }

    public Vector2 GetEnemyPos(int index)
    {
        (int, int) pos_map_enemy = enemies[index];
        
        Vector2 pos_scene = new Vector2();

        //pos_scene.x = pos_map_enemy.Item2 - Mathf.Ceil(this.width / 2.0f) + 0.5f;
        pos_scene.x = pos_map_enemy.Item2 - Mathf.Floor(this.width/2.0f)  - 1.5f;
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
        width = 25; height = 21;
        map = new CellType[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 9 & j == 13)
                {
                    player.Item1 = 9; player.Item2 = 13;
                    map[i, j] = CellType.Player;
                }
                else
                {
                    map[i, j] = CellType.Empty;
                }
            }
        }

        enemies[0].Item1 = 9; enemies[0].Item2 = 14;
        map[11, 9] = CellType.Enemy;

        enemies[1].Item1 = 10; enemies[1].Item2 = 16;
        map[11, 11] = CellType.Enemy;
    }

    public int CheckEnemiesInRange(int range)
    {
        int lv_enemies_found = 0,
            lv_hypotenuse = 0;

        foreach((int, int) enemy_actual in enemies)
        {
            lv_hypotenuse = (int)Mathf.Pow(enemy_actual.Item2 - player.Item2, 2) + (int)Mathf.Pow(enemy_actual.Item1 - player.Item1, 2);
            lv_hypotenuse = (int)Mathf.Sqrt(lv_hypotenuse);

            if(lv_hypotenuse <= range)
            {
                lv_enemies_found++;
                this.HighlightTile(enemy_actual.Item2, enemy_actual.Item1);
                print("Enemigo en casilla " + enemy_actual);
            }
        }

        return lv_enemies_found;
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
