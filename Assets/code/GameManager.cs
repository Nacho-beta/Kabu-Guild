using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AgentEnum agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = gameObject.AddComponent(typeof(AgentEnum)) as AgentEnum;
    }

    static public bool canMove(string agent_name)
    {
        if(AgentEnum.getAgent(agent_name) == AgentEnum.Agent.Player) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
