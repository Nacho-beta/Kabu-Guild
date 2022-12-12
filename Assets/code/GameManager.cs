using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AgentEnum agents;
    static private bool player_turn = true;

    // Start is called before the first frame update
    void Start()
    {
        agents = gameObject.AddComponent(typeof(AgentEnum)) as AgentEnum;
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
