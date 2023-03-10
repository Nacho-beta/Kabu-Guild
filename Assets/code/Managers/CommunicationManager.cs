using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------
    
    //Standard var
    private string channel_key;         // Key needed to access to the chanel   
    private int zero_heigth,            // Reference hegith for 0,0
                map_height;             // Height of the map
    private bool enemies_quant_change,  // Bool indicates if quantity of enemies has changed
                 player_pos_ok;         // Position of player is correct

    // Array Var
    private Vector2 player_position;    // Position of the player
    private List<Enemy> my_agents;      // List of enemies
    private List<bool> agents_active;   // Bool list indicate if enemies are alive

    // Class Type Var
    private GameManager gm;

    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

    //-------UNITY-------------------------------------
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        GameObject[] lr_go_enemies;
        Enemy enemy;

        channel_key = "Placeholder";
        player_pos_ok = false;

        my_agents = new List<Enemy>();
        agents_active = new List<bool>();        
        
        lr_go_enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject l_go_enemy in lr_go_enemies)
        {
            enemy = l_go_enemy.GetComponent<Enemy>();
            my_agents.Add(enemy);     
            agents_active.Add(false);
        }
    }

    //-------PRIVATE-----------------------------------
    /// <summary>
    /// Change map heigth to position on map
    /// </summary>
    /// <param name="index">Index of enemy</param>
    /// <param name="height">Height of map</param>
    /// <returns></returns>
    private Vector2 ParseMapToPos(int index, int height)
    {
        Enemy enemy_act = my_agents[index];
        Vector2 ret_target = new Vector2();

        ret_target.x = enemy_act.GetPosition().x;
        ret_target.y = (height + enemy_act.GetRange()) - zero_heigth + 0.75f;

        return ret_target;
    }

    //-------GETTERS-----------------------------------
    /// <summary>
    /// Getter for player pos
    /// </summary>
    /// <returns>Player's position</returns>
    public Vector2 GetPlayer() { return player_position; }

    /// <summary>
    /// Get for enemies quantity change. If var is consulted -> clear var
    /// </summary>
    /// <returns> 0/1 inidcate if enemies quantity has changed </returns>
    public bool GetEnemiesQuantChange()
    {
        bool ret = enemies_quant_change;
        enemies_quant_change = false;
        return ret;
    }

    /// <summary>
    /// Get if the player position is correct
    /// </summary>
    /// <returns>Return player pos ok</returns>
    public bool IsPlayerLocated() { return this.player_pos_ok; }

    //-------SETTERS-----------------------------------
    /// <summary>
    /// Initialize all var of the channel
    /// </summary>
    /// <param name="key"> Key required by the channel </param>
    /// <param name="height"> Height of the map</param>
    public void Initialize(string key, int height, int zero_height)
    {
        this.map_height = height;
        this.channel_key = key;
        this.zero_heigth = zero_height;
    }

    /// <summary>
    /// Set player position
    /// </summary>
    /// <param name="pos">New position of player</param>
    /// <param name="id">Enemy's Id</param>
    public void SetPlayerPos(Vector2 pos, int id)
    {
        this.player_position = pos;
        this.player_pos_ok = true;
    }


    //-------PUBLIC------------------------------------
    /// <summary>
    /// Subscribe an enemy to the channel
    /// </summary>
    /// <param name="id"> ID of the enemy</param>
    public void Subscribe(int id)
    {
        bool found = false;
        for(int i = 0; i< my_agents.Count & !found; i++) 
        {
            if (my_agents[i].GetId() == id)
            {
                agents_active[i] = true;
                found = true;
            }
        }
    }

    /// <summary>
    /// Unsuscribe enemy to the channel
    /// </summary>
    /// <param name="id"> ID of the enemy</param>
    public void Unsubscribe(int id)
    {
        bool found = false;
        for (int i = 0; i < my_agents.Count & !found; i++)
        {
            if (my_agents[i].GetId() == id)
            {
                agents_active[i] = false;
                found = true;
                enemies_quant_change = true;
            }
        }
    }    

    /// <summary>
    /// Set 
    /// </summary>
    /// <param name="id"></param>
    public void DeletePlayerPos(int id)
    {
        this.player_pos_ok = false;      
    }
    
    /// <summary>
    /// Divide map between all agents active
    /// </summary>
    public void DivideMap()
    {
        int lv_division_heigth,
            lv_target,
            lv_distance,
            lv_best_distance,
            lv_iter_best_agent;        
        List<Enemy> lt_agents_copy = new List<Enemy>(my_agents);

        lv_division_heigth = (int) Mathf.Floor(this.map_height / agents_active.Count);

        for(int i=0; i<agents_active.Count; i++)
        {
            // Clean loop var
            lv_best_distance = this.map_height;
            lv_iter_best_agent = 0;

            // Get Distance
            lv_target = i * lv_division_heigth;

            // Search for best agent
            for(int j=0; j<lt_agents_copy.Count; j++)
            {
                lv_distance = (int)Mathf.Abs(lt_agents_copy[j].GetPosInMap().Item1 - lv_target);
                if (lv_distance < lv_best_distance)
                {
                    lv_best_distance = lv_distance;
                    lv_iter_best_agent = j;
                }
            }

            lt_agents_copy[lv_iter_best_agent].SetTarget(this.ParseMapToPos(lv_iter_best_agent, lv_target));
            if( i == agents_active.Count - 1)
            {
                lt_agents_copy[lv_iter_best_agent].SetLimitUp(this.map_height - zero_heigth + 0.75f);
                lt_agents_copy[lv_iter_best_agent].SetLimitDown( (this.map_height - lv_division_heigth) - zero_heigth + 0.75f);
            }
            else
            {
                lt_agents_copy[lv_iter_best_agent].SetLimitUp( (lv_target + lv_division_heigth) - zero_heigth + 0.75f);
                lt_agents_copy[lv_iter_best_agent].SetLimitDown( lv_target - zero_heigth + 0.75f);
            }
            
            lt_agents_copy.RemoveAt(lv_iter_best_agent);
        }       

        // Call agents to plan
        foreach(Enemy actual_agent in my_agents) 
        {
            actual_agent.SetAction(Actions.plan);
        }
    }    
}
