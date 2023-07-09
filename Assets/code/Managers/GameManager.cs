using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --------------------------------------------------
    // Attributes
    // --------------------------------------------------

    // Private
    //  Standard Var
    private int enemy_index;            // Index to get enemy for array
    private bool player_move_happen,    // Bool indicate if player move happen
                 player_hit_happen,     // Bool indicate if player made action Attack
                 player_doing_action,   // Bool indicate if player is doing an action
                 enemy_move_happen,     // Bool indicate if enemy move happen
                 enemy_hit_happen;      // Bool indicate if enemy attack
    
    // Array Var
    private List<Enemy> enemies;    // Array(List) of Enemies
    
    // Class Type Var
    public static GameManager instance; // Instance needed for GameManager
    private GameState actual_state,     // Actual state of the game
                      last_state;       // Last state of the game
    private EndGame result; 
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

    //-------------------------------------------------
    //-------PRIVATE-----------------------------------
    //-------------------------------------------------
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

    /// <summary>
    /// Remove the enemy from all managers
    /// </summary>
    /// <param name="pos"> Position of the enemy in the array</param>
    private void RemoveEnemy(int pos)
    {
        Enemy lc_enemy_to_delete = enemies[pos];

        // Deactivate enemy
        lc_enemy_to_delete.KillEnemy();

        // Remove from map manager
        map_manager.RemoveEnemy(lc_enemy_to_delete.GetId());

        // Remove enemy from communication channel
        this.communication.Unsubscribe(lc_enemy_to_delete.GetId());

        enemies.RemoveAt(pos);
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

    /// <summary>
    /// Update actual state of game
    /// </summary>
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
                actual_state = this.FinishBattle();
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
    
    /// <summary>
    /// Actions in player's turn
    /// </summary>
    /// <returns> Next state of the game</returns>
    GameState PlayerTurn()
    {
        // Var
        bool kill_enemy = false;
        int enemy_to_hit = -1;
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

                enemy_to_hit = player.Attack();

                if (enemy_to_hit >= 0)
                {
                    for(int i=0; i<enemies.Count; i++)
                    {
                        if( enemies[i].GetId() == enemy_to_hit)
                        {
                            kill_enemy = enemies[i].ReceiveAttack(player.GetAttack());
                            if (kill_enemy)
                            {
                                this.RemoveEnemy(i);
                            }
                        }
                    }
                    this.map_manager.RestartCells();
                }
                break;

            case Actions.Skill:
                this.action_menu.SetStatusMenu(false);
                this.player.Skill();
                break;

            case Actions.pass_turn:
                state_to_return = GameState.update_battle;
                break;
                
            default:                
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
        bool player_dead = false;
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
                enemy_hit_happen = true;
                enemy_actual.SetAction(Actions.pass_turn);
                
                player_dead = player.ReceiveAttack(enemy_actual.Attack());
                print("Vida del jugador = " + player.GetHP());

                if (player_dead)
                {
                    state_to_return = GameState.end_fight;
                }

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
        GameState ret_gm_st = GameState.update_battle;
        Vector2 player_pos;

        // Check end of game
        if(player.GetHP() <= 0) 
        {
            ret_gm_st = GameState.end_fight;
            this.result = EndGame.defeat;
        }
        else if (enemies.Count <= 0)
        {
            ret_gm_st = GameState.end_fight;
            this.result = EndGame.victory;
        }
        else
        {
            // Update game after player turn
            if (last_state == GameState.player_turn)
            {
                // Update player var
                player.Clear();

                // Clear enemies
                enemy_index = 0;
                foreach (Enemy enemy_to_update in enemies)
                {
                    enemy_to_update.InitTurn();
                }

                // Check if player move
                if (player_move_happen)
                {
                    player_move_happen = false;

                    map_manager.SetPlayer(player.GetMapPosition());
                    player_pos = map_manager.GetPlayerPos();

                    this.communication.DeletePlayerPos();

                    foreach (Enemy enemy in this.enemies) { enemy.InitTurn(); }
                }

                // Update enemy
                enemy_index = 0;
                enemy_actual = enemies[0];

                // Continue game
                ret_gm_st = GameState.enemy_turn;

                // Update map
                if (player_hit_happen)
                {
                    this.map_manager.RestartCells();
                }

                // Clear var
                player_hit_happen = false;
                player_doing_action = false;

                StartCoroutine("WaitSeconds");
            }

            // Update game after enemies turn
            if (last_state == GameState.enemy_turn)
            {
                // Clear var
                foreach (Enemy enemy in this.enemies)
                {
                    enemy.InitTurn();
                }
                enemy_hit_happen = false;

                // Continue game
                ret_gm_st = GameState.player_turn;
            }

            last_state = GameState.update_battle;
        }
               
        return ret_gm_st;
    }

    /// <summary>
    /// Finish battle and return to lobby
    /// </summary>
    public GameState FinishBattle()
    {
        GameState ret_state = GameState.end_fight;

        if(this.result == EndGame.defeat)
        {
            if (player.GetAnimationEnd())
            {
                ret_state = GameState.game_start;
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
       
        return ret_state;
    }

    IEnumerator WaitSeconds()
    {
        // your process
        yield return new WaitForSeconds(2);
        // continue process
    }
}

// State needed by the game
public enum GameState
{
    game_start,
    update_battle,
    player_turn,
    enemy_turn,
    end_fight
}

// States needed by end of game
public enum EndGame
{
    victory,
    defeat,
    draw
}