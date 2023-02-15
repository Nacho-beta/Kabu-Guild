using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------
    
    //Standard var
    private string channel_key; // Key needed to access to the chanel    

    // Array Var
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

    //-------SETTERS-----------------------------------
    /// <summary>
    /// Set key needed to access the channel
    /// </summary>
    /// <param name="key"> Key required by the channel </param>
    public void SetKey(string key) { channel_key= key; }


    //-------PUBLIC------------------------------------
    /// <summary>
    /// Subscribe an enemy to the channel
    /// </summary>
    /// <param name="enemy"></param>
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

    public void CollaborativePlan()
    {        
        foreach(Enemy actual_agent in my_agents) 
        {
            actual_agent.SetAction(Actions.move);
        }
    }


}
