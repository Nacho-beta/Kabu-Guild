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

    static private bool player_turn;// Bool that determinate if actual turn == player turn
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
        // Game Object Local for player
        

        // Class variables
        player_turn = true;
        enemy_index = 0;
        actual_state = GameState.game_start;

        // References
             
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
                print("Inicio de juego");
                actual_state = this.StartGame();
                break;
            case GameState.player_turn:
                print("Turno del jugador");
                actual_state = this.PlayerTurn();
                break;
            case GameState.enemy_turn:
                print("Turno del enemigo");
                break;
            case GameState.end_fight:
                print("Fin pelea");
                break;
            default:
                print("Valor por defecto de actualizar estado");
                break;
        }
    }

    // Start Game
    GameState StartGame()
    {
        GameObject  l_go_player,
                    l_go_enemy;

        // Get reference for player
        l_go_player = GameObject.FindGameObjectWithTag("Player");
        this.player = l_go_player.GetComponent<Player>();

        // Get references for enemy
        l_go_enemy = GameObject.FindGameObjectWithTag("Player");

        return GameState.player_turn;
    }

    // Player turn
    GameState PlayerTurn()
    {
        GameState state_to_return = GameState.player_turn;
        Actions player_action; // Action for player
        
        // Get Action provide by player
        player_action = player.SetAction();

        // Switch for action
        switch(player_action)
        {
            case Actions.move:
                player.Move();
                break;
            default:
                state_to_return = GameState.enemy_turn;
                break;
        }

        return state_to_return;
    }

    

    

    static public bool canMove(string agent_name)
    {

        if (AgentEnum.getAgent(agent_name) == AgentEnum.Agent.Player)
        {
            if (player_turn)
            {
                return true;
            }
        }
        else if (AgentEnum.getAgent(agent_name) == AgentEnum.Agent.Enemy)
        {
            if (!player_turn)
            {
                return true;
            }
        }

        return false;
    }

    static public void endTurn(string agent_name)
    {
        if (AgentEnum.getAgent(agent_name) == AgentEnum.Agent.Player)
        {
            player_turn = false;
        }
        else if (AgentEnum.getAgent(agent_name) == AgentEnum.Agent.Enemy)
        {
            player_turn = true;
        }
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
