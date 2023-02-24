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
    private bool player_found;

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

        player_found = false;

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

    //-------GETTERS-----------------------------------
    /// <summary>
    /// Getter for player pos
    /// </summary>
    /// <returns>Player's position</returns>
    public Vector2 GetPlayer() { return player_position; }

    //-------SETTERS-----------------------------------
    /// <summary>
    /// Set key needed to access the channel
    /// </summary>
    /// <param name="key"> Key required by the channel </param>
    public void SetKey(string key) { channel_key= key; }

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
        foreach(Enemy actual_agent in my_agents) 
        {
            actual_agent.SetAction(Actions.plan);
        }
    }


}
