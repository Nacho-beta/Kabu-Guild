using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEnum : MonoBehaviour
{
	public enum Agent { Player, Enemy, Null };

	// Get Agent by string
	static public Agent getAgent(string agent)
	{
		Agent get_agent;

		switch (agent)
		{
			case "Player":
				get_agent = Agent.Player;
				break;
			default:
				get_agent = Agent.Null;
				break;
		}

		return get_agent;
	}

}
