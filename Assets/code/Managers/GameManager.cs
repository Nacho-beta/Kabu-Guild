using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------

    // Private
    //  Standard Var
    private int enemy_index;        // Index to get enemy for array
    private bool player_move_happen,// Bool indicate if player move happen
                 enemy_move_happen, // Bool indicate if enemy move happen
                 enemy_hit_happen;  // Bool indicate if enemy attack
    // Array Var
    private List<Enemy> enemies;    // Array(List) of Enemies
    // Class Type Var
    public static GameManager instance;// Instance needed for GameManager
    private GameState actual_state,     // Actual state of the game
                      last_state;       // Last state of the game
    private Player player;              // Player of game
    private Enemy enemy_actual;         // Enemy of actual turn
    private ActionMenu action_menu;     // Menu for actions in combat of the player
    private MapManager map_manager;     // Manager for map
    private CommunicationManager communication; // Manager for agents communication

    private AgentEnum agents;


    // --------------------------------------------------
    // Methods
    // --------------------------------------------------
    public string GetChannelKey() { return map_manager.GetKey(); }

    //-------PRIVATE-----------------------------------
    /// <summary>
    /// Update actual enemy for the turn
    /// </summary>
    /// <returns></returns>
    private GameState UpdateEnemyActual()
    {
        GameState ret_next_state = GameState.enemy_turn;
        enemy_index++;
        if (enemy_index >= enemies.Count)
        {
            enemy_index = 0;
            ret_next_state = GameState.update_battle;
            this.action_menu.SetStatusMenu(true);
        }

        enemy_actual = enemies[enemy_index];
        return ret_next_state;
    }

    // Awake
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        GameObject l_go_player,
                   l_go_action_menu,
                   l_go_map,
                   l_go_communication;
        GameObject[] lr_go_enemies;

        actual_state = GameState.game_start;

        // Class variables
        enemy_index = 0;
        enemies = new List<Enemy>();

        // Get reference for map manager
        l_go_map = GameObject.FindGameObjectWithTag("Map");
        this.map_manager = l_go_map.GetComponent<MapManager>();

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();

        // Get references for enemy
        lr_go_enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject l_go_enemy in lr_go_enemies)
        {
            enemy_actual = l_go_enemy.GetComponent<Enemy>();
            
            enemy_index++;
            this.enemies.Add(enemy_actual);
        }

        // Get reference for menu
        l_go_action_menu = GameObject.FindGameObjectWithTag("ActionMenu");
        this.action_menu = l_go_action_menu.GetComponent<ActionMenu>();

        // Get reference for menu
        l_go_communication = GameObject.FindGameObjectWithTag("Communication");
        this.communication = l_go_communication.GetComponent<CommunicationManager>();
    }

    void Update()
    {
        this.UpdateGameState();
    }

    // UpdateGameState
    void UpdateGameState()
    {
        switch (actual_state)
        {
            case GameState.game_start:
                actual_state = this.StartGame();
                break;
            case GameState.update_battle:
                actual_state = this.UpdateBattle();
                break;
            case GameState.player_turn:
                actual_state = this.PlayerTurn();
                break;
            case GameState.enemy_turn:
                // Clear player action var
                player.SetAction(Actions.none);
                // Enemies turn
                actual_state = this.EnemiesTurn();
                break;
            case GameState.end_fight:
                break;
            default:
                print("Valor por defecto de actualizar estado");
                break;
        }
    }

    /// <summary>
    /// Status to execute at the beginning of the game
    /// </summary>
    /// <returns> Next state for the game </returns>
    GameState StartGame()
    {
        // Clear var
        enemy_index = 0;
        last_state= GameState.game_start;

        // Map generation
        map_manager.GenerateDesertHill();               
          
        // Comunication init
        this.communication.SetKey(map_manager.GetKey());

        // Initialization for enemies
        for(int i=0; i<enemies.Count; i++)
        {
            enemy_actual = enemies[i];
            enemy_actual.SetAction(Actions.plan);
            enemy_actual.SetPosition(map_manager.GetEnemyPos(i));

            communication.Subscribe(enemy_actual.GetId());
        }

        // Player init
        this.player.SetPosition(map_manager.GetPlayerPos());

        enemy_actual = enemies[enemy_index];      

        return GameState.player_turn;
    }

    // PlayerTurn: Action in player's turn
    GameState PlayerTurn()
    {
        // Var
        bool kill_enemy = false;
        int enemies_hit = 0;
        GameState state_to_return = GameState.player_turn;
        Actions player_action = Actions.none; // Action for player

        last_state = GameState.player_turn;

        // Get Action provide by player
        player_action = player.GetAction();

        // Switch for action
        switch (player_action)
        {
            case Actions.move:
                this.action_menu.SetStatusMenu(false);
                player.Move();
                player_move_happen = true;
                break;
            case Actions.fight:
                this.action_menu.SetStatusMenu(false);

                enemies_hit = map_manager.CheckEnemiesInRange(player.GetRange());
                if(enemies_hit> 0)
                {
                    kill_enemy = this.enemy_actual.ReceiveAttack(player.Attack());
                    if(kill_enemy) { print("Enemigo derrotado"); }
                }
                this.player.SetAction(Actions.pass_turn);
                break;
            case Actions.Skill:
                this.action_menu.SetStatusMenu(false);
                this.player.Skill();
                break;
            case Actions.pass_turn:
                state_to_return = GameState.update_battle;
                break;            
            default:
                this.action_menu.SetStatusMenu(true);
                break;
        }

        return state_to_return;
    }

    /// <summary>
    /// Enemy turn
    /// </summary>
    /// <returns></returns>
    GameState EnemiesTurn()
    {        
        // Var       
        GameState state_to_return = GameState.enemy_turn;
        Actions enemy_action; // Action for enemy

        last_state = GameState.enemy_turn;      

        enemy_action = enemy_actual.GetAction();

        switch (enemy_action)
        {
            case Actions.none:
                break;
            case Actions.plan:
                //print("Plan");
                communication.CollaborativePlan();
                break;
            case Actions.move:                
                map_manager.SetEnemy(enemy_actual.GetMapPosition(), enemy_index);
                enemy_move_happen = enemy_actual.Move();
                break;
            case Actions.fight:
                print("Id = "+enemy_actual.GetId()+"Ataco");
                enemy_hit_happen = true;
                enemy_actual.SetAction(Actions.pass_turn);
                enemy_actual.Attack();
                break;
            case Actions.pass_turn:                
                /*
                if (enemy_move_happen)
                {
                    map_manager.SetEnemy(enemy_actual.GetMapPosition(), enemy_index);
                    enemy_move_happen =false;
                }*/
                //enemy_actual.SetAction(Actions.plan);
                //enemy_hit_happen = false;
                state_to_return = this.UpdateEnemyActual();                
                break;
        }

        return state_to_return;
    }

    /// <summary>
    /// Update all resources of the game
    /// </summary>
    /// <returns> Next state of the game </returns>
    public GameState UpdateBattle()
    {
        Enemy lc_enemy_aux = null;
        GameState ret_gm_st = GameState.update_battle;

        // Update game after player turn
        if (last_state == GameState.player_turn)
        {
            // Update player var
            player.SetAction(Actions.none);

            // Clear enemies
            enemy_index = 0;            


            // Check if player move
            if (player_move_happen)
            {
                player_move_happen = false;

                map_manager.SetPlayer(player.GetMapPosition());
                for(int i=0; i< enemies.Count; i++)
                {
                    lc_enemy_aux = enemies[i];
                    enemy_actual.FoundEnemy(map_manager.CheckPlayerInRange(enemy_actual.GetRange(), enemy_index));
                }
            }

            // Update enemy
            enemy_index = 0;
            enemy_actual = enemies[0];

            // Continue game
            ret_gm_st = GameState.enemy_turn;

            StartCoroutine("WaitSeconds");
        }

        // Update game after enemies turn
        if(last_state == GameState.enemy_turn) 
        {
            // Clear var
            enemy_actual.SetAction(Actions.plan);
            enemy_hit_happen = false;

            if (enemy_move_happen)
            {
                enemy_move_happen = false;

                map_manager.SetEnemy(enemy_actual.GetMapPosition(), enemy_index);                
            }

            // Continue game
            ret_gm_st = GameState.player_turn;
        }

        last_state = GameState.update_battle;
        return ret_gm_st;
    }

    IEnumerator WaitSeconds()
    {
        // your process
        yield return new WaitForSeconds(2);
        // continue process
    }
}

// State fo the game
public enum GameState
{
    game_start,
    update_battle,
    player_turn,
    enemy_turn,
    end_fight
}
