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
    private GameState actual_state;     // Actual state of the gme
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
            ret_next_state = GameState.player_turn;
        }

        enemy_actual = enemies[enemy_index];
        return ret_next_state;
    }

    private void InitializeEnemies()
    {

    }

    // Awake
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        actual_state = GameState.game_start;
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

    // Start Game
    GameState StartGame()
    {
        // Variables
        GameObject l_go_player,
                   l_go_action_menu,
                   l_go_map,
                   l_go_communication;
        GameObject[] lr_go_enemies;

        // Class variables
        enemy_index = 0;
        enemies = new List<Enemy>();


        //map_manager = gameObject.AddComponent(typeof(MapManager)) as MapManager;
        l_go_map = GameObject.FindGameObjectWithTag("Map");
        this.map_manager = l_go_map.GetComponent<MapManager>();
        map_manager.GenerateDesertHill();

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();

        // Get references for enemy
        lr_go_enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject l_go_enemy in lr_go_enemies)
        {
            enemy_actual = l_go_enemy.GetComponent<Enemy>();
            enemy_actual.SetPosition(map_manager.GetEnemyPos(this.enemy_index));
            enemy_index++;
            this.enemies.Add(enemy_actual);            
        }

        // Get reference for menu
        l_go_action_menu = GameObject.FindGameObjectWithTag("ActionMenu");
        this.action_menu = l_go_action_menu.GetComponent<ActionMenu>();

        // Get reference for menu
        l_go_communication = GameObject.FindGameObjectWithTag("Communication");
        this.communication = l_go_communication.GetComponent<CommunicationManager>();        

        this.communication.SetKey(map_manager.GetKey());     

        // Clear var
        enemy_index = 0;
        enemy_actual = enemies[enemy_index];

        return GameState.player_turn;
    }

    // PlayerTurn: Action in player's turn
    GameState PlayerTurn()
    {
        bool kill_enemy = false;
        int enemies_hit = 0;

        GameState state_to_return = GameState.player_turn;
        Actions player_action = Actions.none; // Action for player

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
                if(player_move_happen)
                {
                    map_manager.SetPlayer(player.GetMapPosition());
                    player_move_happen = false;
                }
                //state_to_return = GameState.enemy_turn;
                this.player.SetAction(Actions.none);
                state_to_return = GameState.player_turn;
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

        // Check if enemy is range
        if(!enemy_hit_happen)
        {
            if (map_manager.CheckPlayerInRange(enemy_actual.GetRange(), enemy_index))
            {
                enemy_actual.SetAction(Actions.fight);
            }
        }        

        enemy_action = enemy_actual.GetAction();
        switch (enemy_action)
        {
            case Actions.none:
                communication.Subscribe(enemy_actual.GetId());
                enemy_actual.SetAction(Actions.plan);
                break;
            case Actions.plan:
                //print("Plan");
                communication.CollaborativePlan();
                break;
            case Actions.move:
                enemy_actual.Move();
                map_manager.SetEnemy(enemy_actual.GetMapPosition(), enemy_index);
                enemy_move_happen = true;
                break;
            case Actions.fight:
                //print("Ataco");
                enemy_hit_happen = true;
                enemy_actual.SetAction(Actions.pass_turn);
                break;
            case Actions.pass_turn:
                if (enemy_move_happen)
                {
                    map_manager.SetEnemy(enemy_actual.GetMapPosition(), enemy_index);
                    enemy_move_happen =false;
                }
                enemy_actual.SetAction(Actions.plan);
                enemy_hit_happen = false;
                state_to_return = this.UpdateEnemyActual();
                break;
        }

        if(state_to_return == GameState.player_turn)
        {
            this.action_menu.SetStatusMenu(true);
        }

        return state_to_return;
    }
}

// State fo the game
public enum GameState
{
    game_start,
    player_turn,
    enemy_turn,
    end_fight
}
