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
    private string channel_key; // Key needed to access to the chanel   
    private int map_height;     // Height of the map

    // Array Var
    private Vector2 player_position;    // Position of the player
    private List<Enemy> my_agents;      // List of enemies
    private List<bool> agents_active;   // Bool list indicate if enemies are alive
    private List<bool> player_pos_enemy;// Bool list indicate if this enemy has found the player

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

        my_agents = new List<Enemy>();
        agents_active = new List<bool>();
        player_pos_enemy = new List<bool>();
        
        lr_go_enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject l_go_enemy in lr_go_enemies)
        {
            enemy = l_go_enemy.GetComponent<Enemy>();
            my_agents.Add(enemy);     
            agents_active.Add(false);
            player_pos_enemy.Add(false);
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
        ret_target.y = height - Mathf.Floor(this.map_height / 2.0f) + 0.75f;

        return ret_target;
    }

    //-------GETTERS-----------------------------------
    /// <summary>
    /// Getter for player pos
    /// </summary>
    /// <returns>Player's position</returns>
    public Vector2 GetPlayer() { return player_position; }

    //-------SETTERS-----------------------------------
    /// <summary>
    /// Initialize all var of the channel
    /// </summary>
    /// <param name="key"> Key required by the channel </param>
    /// <param name="height"> Height of the map</param>
    public void Initialize(string key, int height)
    {
        this.map_height = height;
        this.channel_key= key;
    }

    /// <summary>
    /// Set player position
    /// </summary>
    /// <param name="pos">New position of player</param>
    /// <param name="id">Enemy's Id</param>
    public void SetPlayerPos(Vector2 pos, int id)
    {
        for (int i = 0; i < my_agents.Count; i++)
        {
            if (my_agents[i].GetId() == id)
            {
                player_pos_enemy[i] = true;
            }
            else
            {
                if(pos != player_position)
                {
                    player_pos_enemy[i] = false;
                }
            }
        }

        this.player_position= pos;
        print("El jugador está en " + this.player_position);
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
            }
        }
    }    

    /// <summary>
    /// Set 
    /// </summary>
    /// <param name="id"></param>
    public void DeletePlayerPos(int id)
    {
        /*
        bool found = false;

        for (int i = 0; i < my_agents.Count & !found; i++)
        {
            if (my_agents[i].GetId() == id)
            {
                player_pos_enemy[i] = false;
                found = true;
            }
        }*/
    }
    
    public void CollaborativePlan()
    {
        int lv_division_heigth,
            lv_mid_height,
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
            lv_mid_height = (int) Mathf.Floor( ((i+1)* lv_division_heigth + i* lv_division_heigth) / 2 );
            print("Punto medio " + lv_mid_height);

            // Search for best agent
            for(int j=0; j<lt_agents_copy.Count; j++)
            {
                lv_distance = (int)Mathf.Abs(lt_agents_copy[j].GetPosInMap().Item1 - lv_mid_height);
                if (lv_distance < lv_best_distance)
                {
                    lv_best_distance = lv_distance;
                    lv_iter_best_agent = j;
                }
            }

            lt_agents_copy[lv_iter_best_agent].SetTarget(this.ParseMapToPos(lv_iter_best_agent, lv_mid_height));
            lt_agents_copy.RemoveAt(lv_iter_best_agent);
        }       

        // Call agents to plan
        foreach(Enemy actual_agent in my_agents) 
        {
            actual_agent.SetAction(Actions.plan);
        }
    }
}
