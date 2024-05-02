using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Behaviour
{
    private EnemyBase m_target;

    public Seek(float weight) : base(weight) { }

    public override Vector2 BehaviorUpdate(EnemyBase agent)
    {
        // Checking if the provided agent is valid
        if (agent == null)
            return Vector2.zero;
        //                      Dir                                 Dir                   Force
        return ((m_target.GetPos() - agent.GetPos()).normalized * GetWeight());// - agent.GetVel();

        /* Explanation of this functions V
        // Calculate the vector describing the direction to the target and normalize it
        // The direction is calulated by targets position subtracted from our own
        Vector2 dir = (m_target.GetPos() - agent.GetPos()).normalized;

        // Multiply the direction by the speed we want the agent to move
        dir *= GameManager.SPEED;

        // Subtract the agent’s current velocity from the result to get the force we need to apply
        Vector2 force = dir - agent.GetVel();

        // return the force
        return force;
        */
    }

    public void SetTarget(EnemyBase agent)
    {
        if (agent != null)
        {
            m_target = agent;
        }
        else
        {
            Debug.Log("Failed to set target for seek bahavior, check passed object");
        }
    }
}
