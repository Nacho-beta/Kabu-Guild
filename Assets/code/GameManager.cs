using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------

    // Private
    private static GameManager instance; // Instance needed for GameManager
    private GameState actual_state;

    private int enemy_index;        // Index to get enemy for array

    private List<Enemy> enemies;    // Array(List) of Enemies

    private Player player;          // Player of game
    private Enemy enemy_actual;     // Enemy of actual turn

    private AgentEnum agents;


    // --------------------------------------------------
    // Methods
    // --------------------------------------------------

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
                    l_go_enemy;

        // Class variables
        enemy_index = 0;
        enemies = new List<Enemy>();

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();

        // Get references for enemy
        l_go_enemy = GameObject.FindGameObjectWithTag("Enemy");
        this.enemy_actual = l_go_enemy.GetComponent<Enemy>();
        this.enemies.Add(enemy_actual);
        return GameState.player_turn;
    }

    // PlayerTurn: Action in player's turn
    GameState PlayerTurn()
    {
        bool move_happen = false;
        GameState state_to_return = GameState.player_turn;
        Actions player_action; // Action for player

        // Get Action provide by player
        player_action = player.GetAction();

        // Switch for action
        switch (player_action)
        {
            case Actions.move:
                move_happen = player.Move();
                if ( move_happen)
                {
                    print("Cambio estado enemigo");
                    state_to_return = GameState.enemy_turn;
                }
                break;
            default:
                break;
        }

        return state_to_return;
    }

    // PlayerTurn: Action in player's turn
    GameState EnemiesTurn()
    {
        GameState state_to_return = GameState.player_turn;
        Actions enemy_action; // Action for enemy

        // Update actual enemy
        this.enemy_actual = this.enemies[enemy_index];

        // Get Action provide by enemy actual
        enemy_action = enemy_actual.SetAction();

        // Switch for action
        switch (enemy_action)
        {
            case Actions.move:
                enemy_actual.Move();
                break;
            default:
                break;
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
