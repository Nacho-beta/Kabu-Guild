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
    private (int,int) player;           // Position in map for player
    private (int,int)[] enemies;        // Position in map for enemies
    private (float,float) zero_point;   // Position equivalent to 0,0

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
        print("Casilla en x = " + x + " y = " + y);
        (int, int) medium_point;
        Vector3Int grid_position = new Vector3Int();

        medium_point.Item1 = (int)Mathf.Floor(width / 2.0f);
        medium_point.Item2 = (int)Mathf.Floor(height / 2.0f);

        // Get position of tile in the world
        grid_position.x = (int)Mathf.Ceil((x-1) - (this.width / 2.0f) - 1.5f);        
        grid_position.y = (int)(y - (this.height / 2.0f));

        print("Equivalente a " + grid_position);

        co_tilemap.SetTileFlags(grid_position, TileFlags.None);
        co_tilemap.SetColor(grid_position, Color.red);
    }

    //-------GETTERS-----------------------------------
    public string GetKey() { return key; }

    /// <summary>
    /// Get enemy scene position 
    /// </summary>
    /// <param name="index"> Index of enemy</param>
    /// <returns> Enemy's position </returns>
    public Vector2 GetEnemyPos(int index)
    {
        (int, int) pos_map_enemy = enemies[index];
        
        Vector2 pos_scene = new Vector2();

        //pos_scene.x = pos_map_enemy.Item2 - Mathf.Ceil(this.width / 2.0f) + 0.5f;
        pos_scene.x = pos_map_enemy.Item2 - Mathf.Floor(this.width/2.0f)  - 1.5f;
        pos_scene.y = pos_map_enemy.Item1 - Mathf.Floor(this.height / 2) + 0.75f;

        return pos_scene;
    }

    /// <summary>
    /// Get player scene position
    /// </summary>
    /// <returns>Player's position</returns>
    public Vector2 GetPlayerPos()
    {
        Vector2 pos_scene = new Vector2();
        
        pos_scene.x = player.Item2 - Mathf.Floor(this.width / 2.0f) - 1.5f;
        pos_scene.y = player.Item1 - Mathf.Floor(this.height / 2.0f) + 0.75f;

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
        print("Posicion actual: "+player);
        
        player.Item2 += new_pos.Item2;
        player.Item1 += new_pos.Item1;

        print("Posicion nueva :"+player);
        map[player.Item2, player.Item1] = CellType.Player;
    }

    /// <summary>
    /// Update enemy pos in map 
    /// </summary>
    /// <param name="new_pos"> New position of enemy</param>
    /// <param name="index"> Index of enemy </param>
    public void SetEnemy((int, int) new_pos, int index)
    {        
        if(new_pos.Item1 != 0 | new_pos.Item2 != 0)
        {            
            (int, int) enemy_actual = enemies[index];
            map[enemy_actual.Item2, enemy_actual.Item1] = CellType.Empty;

            enemy_actual.Item2 += new_pos.Item2;
            enemy_actual.Item1 += new_pos.Item1;

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

        enemies[0].Item1 = 9; enemies[0].Item2 = 16;
        enemies[1].Item1 = 11; enemies[1].Item2 = 16;

        foreach ( (int,int)enemy in enemies)
        {
            map[enemy.Item2, enemy.Item1] = CellType.Enemy;
        }

    }

    /// <summary>
    /// Count enemies in player's range
    /// </summary>
    /// <param name="range">Player's range</param>
    /// <returns>Number of enemies in the range</returns>
    public int CheckEnemiesInRange(int range)
    {
        float lv_hypotenuse = 0.0f,
              lv_range_pow = Mathf.Pow(range, 2);
        int lv_enemies_found = 0;

        foreach((int, int) enemy_actual in enemies)
        {
            lv_hypotenuse = (float)Mathf.Pow(enemy_actual.Item2 - player.Item2, 2) + (float)Mathf.Pow(enemy_actual.Item1 - player.Item1, 2);
            lv_hypotenuse = (float)Mathf.Sqrt(lv_hypotenuse);

            if(lv_hypotenuse <= range)
            {
                lv_enemies_found++;
                this.HighlightTile(enemy_actual.Item2, enemy_actual.Item1);
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
        float lv_hypotenuse = 0.0f,
              lv_range_pow = Mathf.Pow(range,2);
        bool lv_ret_found = false;
        (int, int) ls_enemy_actual = enemies[index];
        
        lv_hypotenuse = (float)Mathf.Pow(this.player.Item2 - ls_enemy_actual.Item2, 2) + (float)Mathf.Pow(player.Item1 - ls_enemy_actual.Item1, 2);
        lv_hypotenuse = Mathf.Sqrt(lv_hypotenuse);

        if(lv_hypotenuse <= lv_range_pow)
        {
            lv_ret_found = true;
        }      

        return lv_ret_found;
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
