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

        // Get reference for communication channel
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
        this.communication.Initialize(  map_manager.GetKey(), 
                                        map_manager.GetHeight(), 
                                        map_manager.GetZeroHeight()
                                      );

        // Initialization for enemies
        for(int i=0; i<enemies.Count; i++)
        {
            // Get enemy actual
            enemy_actual = enemies[i];

            // Set action
            enemy_actual.SetAction(Actions.plan);

            // Set position
            enemy_actual.SetPosition(map_manager.GetEnemyMapPos(i),
                                     map_manager.GetEnemyPos(i));

            // Set limits
            enemy_actual.SetLimitRight(map_manager.GetRightEdge());
            enemy_actual.SetLimitLeft(map_manager.GetLeftEdge());

            // Subscribe to communication channel
            communication.Subscribe(enemy_actual.GetId());
        }
        this.communication.DivideMap();

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
        //print("Loop en " + enemy_action);
        switch (enemy_action)
        {
            case Actions.none:
                break;
            case Actions.plan:
                enemy_actual.PathFinding();
                break;
            case Actions.move:
                enemy_actual.Move();
                break;
            case Actions.fight:
                print("Id = "+enemy_actual.GetId()+"Ataco");
                enemy_hit_happen = true;
                enemy_actual.SetAction(Actions.pass_turn);
                enemy_actual.Attack();
                break;
            case Actions.pass_turn:                
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
        Vector2 player_pos;

        // Update game after player turn
        if (last_state == GameState.player_turn)
        {
            // Update player var
            player.SetAction(Actions.none);

            // Clear enemies
            enemy_index = 0;
            foreach(Enemy enemy_to_update in enemies)
            {
                enemy_to_update.InitTurn();
            }            


            // Check if player move
            if (player_move_happen)
            {
                player_move_happen = false;

                map_manager.SetPlayer(player.GetMapPosition());
                player_pos = map_manager.GetPlayerPos();

                for (int i=0; i< enemies.Count; i++)
                {
                    lc_enemy_aux = enemies[i];
                    lc_enemy_aux.FoundEnemy(
                                    map_manager.CheckPlayerInRange(lc_enemy_aux.GetRange(), i),
                                    player_pos
                                );                    
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

            // Update map
            for (int i = 0; i < enemies.Count; i++)
            {
                lc_enemy_aux = enemies[i];
                //map_manager.SetEnemy(lc_enemy_aux.GetMapPosition(), i);
                map_manager.SetEnemy(lc_enemy_aux.GetPosInMap(), i);
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
