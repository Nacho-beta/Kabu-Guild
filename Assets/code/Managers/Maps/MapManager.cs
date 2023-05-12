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
    private string key;             // Key needed for communication
    private (int, int) zero_point,  // Position equivalent to 0,0
                       player;

    // Array var   
    private Dictionary<int, (int,int)> enemies; // Position in map for enemies
    private List<Vector3Int> tiles_changed;     // List with highlighted tiles  

    // Class var
    private CellType[,] map;   // Matrix with map info
    private Tilemap co_tilemap;  // Tile map of the scene


    //-------------------------------------------------
    //-------UNITY-------------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Execution of start in the first frame
    /// </summary>
    public void Start()
    {
        GameObject l_go_floor;

        l_go_floor = GameObject.FindGameObjectWithTag("Floor");
        this.co_tilemap = l_go_floor.GetComponent<Tilemap>();

        enemies = new Dictionary<int, (int, int)>();
        tiles_changed = new List<Vector3Int>();
    }


    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------
    /// <summary>
    /// Remark a Tile when it is needed
    /// </summary>
    /// <param name="x"> Width position </param>
    /// <param name="y"> Hight postiion </param>
    private void HighlightTile(int x, int y)
    {
        Vector3Int grid_position = new Vector3Int();

        // Get position of tile in the world
        grid_position.x = x - zero_point.Item2;
        grid_position.y = y - zero_point.Item1;

        // Change color
        co_tilemap.SetTileFlags(grid_position, TileFlags.None);
        co_tilemap.SetColor(grid_position, Color.red);

        // Save as modified
        tiles_changed.Add(grid_position);
    }

    /// <summary>
    /// Parse scene position to map position
    /// </summary>
    /// <param name="pos"> Position in scene </param>
    /// <returns> Position in map </returns>
    private (int,int) ParsePosToMap(Vector2 pos_scene)
    {
        (int, int) map_pos;
        map_pos.Item2 = (int)Mathf.RoundToInt(pos_scene.x + this.zero_point.Item2 - 0.5f );
        map_pos.Item1 = (int)Mathf.RoundToInt(pos_scene.y + this.zero_point.Item1 - 0.75f);

        return map_pos;
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
        pos_scene.x = pos_map_enemy.Item2 - this.zero_point.Item2  + 0.5f;
        pos_scene.y = pos_map_enemy.Item1 - this.zero_point.Item1 + 0.75f;

        return pos_scene;
    }

    /// <summary>
    /// Get enemy map position 
    /// </summary>
    /// <param name="index"> Index of enemy</param>
    /// <returns> Enemy's map position </returns>
    public (int, int) GetEnemyMapPos(int index) { return enemies[index]; }

    /// <summary>
    /// Get the enemy index in that position, if not enemy in this position return -1
    /// </summary>
    /// <param name="pos_enemy"> Pos to check</param>
    /// <returns> Index of the enemy, -1 if error</returns>
    public int GetEnemyIndexByPos(Vector2 pos_enemy)
    {
        (int,int) enemy_map_pos = this.ParsePosToMap(pos_enemy);

        foreach(var enemy_actual in enemies)
        {
            if(enemy_actual.Value == enemy_map_pos)
            {
                return enemy_actual.Key;
            }
        }

        return -1;
    }

    /// <summary>
    /// Get player scene position
    /// </summary>
    /// <returns>Player's position</returns>
    public Vector2 GetPlayerPos()
    {
        Vector2 pos_scene = new Vector2();
        
        pos_scene.x = player.Item2 - this.zero_point.Item2 + 0.5f;
        pos_scene.y = player.Item1 - this.zero_point.Item1 + 0.75f;

        return pos_scene;
    }

    /// <summary>
    /// Get Heigth of map
    /// </summary>
    /// <returns>Heigth of map</returns>
    public int GetHeight() { return this.height; }

    /// <summary>
    /// Get height equal to y = 0.0f
    /// </summary>
    /// <returns>Zero height</returns>
    public int GetZeroHeight() { return this.zero_point.Item1; }

    public float GetRightEdge() { return this.width - this.zero_point.Item2 + 0.5f; }
    
    public float GetLeftEdge() { return 0 - this.zero_point.Item2 + 0.5f; }

    //-------SETTERS-----------------------------------

    public void AddEnemy(int id)
    {
        (int, int) new_entry;
        new_entry.Item1 = 0; new_entry.Item2 = 0;

        enemies.Add(id, new_entry);
    }

    /// <summary>
    /// Update player position adding the new position
    /// </summary>
    /// <param name="new_pos"> Pos to add to actual position </param>
    public void SetPlayer((int,int) new_pos)
    {
        map[player.Item2, player.Item1] = CellType.Empty;
        
        player.Item2 += new_pos.Item2;
        player.Item1 += new_pos.Item1;

        map[player.Item2, player.Item1] = CellType.Player;
    }

    /// <summary>
    /// Update enemy pos in map 
    /// </summary>
    /// <param name="new_pos"> New position of enemy</param>
    /// <param name="index"> Index of enemy </param>
    public void SetEnemy((int, int) new_pos, int id)
    {
        (int, int) enemy_actual = enemies[id];
        map[enemy_actual.Item2, enemy_actual.Item1] = CellType.Empty;

        enemy_actual.Item2 = new_pos.Item2;
        enemy_actual.Item1 = new_pos.Item1;

        map[enemy_actual.Item2, enemy_actual.Item1] = CellType.Enemy;
        enemies[id] = enemy_actual;
    }


    //-------------------------------------------------
    //-------PUBLIC------------------------------------
    //-------------------------------------------------
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
        (int, int) new_pos_enemy;
        // Key
        key = "Kobold Hill";

        // Fill map array
        width = 11; height = 12;
        map = new CellType[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                map[i, j] = CellType.Empty;
            }
        }

        player.Item1 = 5; player.Item2 = 3;
        map[player.Item2, player.Item1] = CellType.Player;

        new_pos_enemy.Item1 = 1; new_pos_enemy.Item2 = 8;
        enemies[0] = new_pos_enemy;

        new_pos_enemy.Item1 = 3; new_pos_enemy.Item2 = 4;
        enemies[1] = new_pos_enemy;

        new_pos_enemy.Item1 = 8; new_pos_enemy.Item2 = 9;
        enemies[2] = new_pos_enemy;

        new_pos_enemy.Item1 = 10; new_pos_enemy.Item2 = 2;
        enemies[3] = new_pos_enemy;

        foreach ( var enemy in enemies.Values)
        {
            map[enemy.Item2, enemy.Item1] = CellType.Enemy;
        }

        // Point relative to zero
        zero_point.Item2 = 5;
        zero_point.Item1 = 6;

    }

    /// <summary>
    /// Count enemies in player's range
    /// </summary>
    /// <param name="range">Player's range</param>
    /// <returns>Number of enemies in the range</returns>
    public int CheckEnemiesInRange(int range)
    {
        float lv_hypotenuse = 0.0f;
        int lv_enemies_found = 0;

        foreach (var enemy_actual in enemies.Values)
        {
            lv_hypotenuse = (float)Mathf.Pow(enemy_actual.Item2 - player.Item2, 2) + (float)Mathf.Pow(enemy_actual.Item1 - player.Item1, 2);
            lv_hypotenuse = (float)Mathf.Sqrt(lv_hypotenuse);

            if (lv_hypotenuse <= range)
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
        float lv_hypotenuse = 0.0f;
        bool lv_ret_found = false;
        (int, int) ls_enemy_actual = enemies[index];
        
        lv_hypotenuse = (float)Mathf.Pow(this.player.Item2 - ls_enemy_actual.Item2, 2) + (float)Mathf.Pow(player.Item1 - ls_enemy_actual.Item1, 2);
        lv_hypotenuse = Mathf.Sqrt(lv_hypotenuse);

        if(lv_hypotenuse <= range)
        {
            lv_ret_found = true;
        }      

        return lv_ret_found;
    }    
    
    /// <summary>
    /// Restart cells and restart their colors
    /// </summary>
    public void RestartCells()
    {
        foreach(Vector3Int cell in this.tiles_changed)
        {
            co_tilemap.SetTileFlags(cell, TileFlags.None);
            co_tilemap.SetColor(cell, Color.white);
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
