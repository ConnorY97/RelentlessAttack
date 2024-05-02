using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Separation : Behaviour
{
    public Separation(float weight, float distance) : base(weight) { m_neighbourDistance = distance; }

    private float m_neighbourDistance = 10.0f;
    private List<EnemyBase> m_neighbourhood = new List<EnemyBase>();
    public override Vector2 BehaviorUpdate(EnemyBase agent)
    {
        Vector2 separationForce = Vector2.zero;
        int closeNeighbourCount = 0;

        foreach (EnemyBase neighbour in m_neighbourhood)
        {
            // Skip itself
            if (neighbour == agent)
                continue;

            if (Vector2.Distance(agent.GetPos(), neighbour.GetPos()) < m_neighbourDistance)
            {
                separationForce += (agent.GetPos() - neighbour.GetPos());//.normalized;
                closeNeighbourCount++;
            }
            else if (Vector2.Distance(agent.GetPos(), neighbour.GetPos()) == 0)
            {
                separationForce += new Vector2(0.5f, 0.5f);
                closeNeighbourCount++;
            }
        }

        // Need to make sure that we are not deviding by 0 if there are no close neighbours
        if (separationForce != Vector2.zero)
        {
            // Average separation force
            separationForce /= closeNeighbourCount;
            Vector2 force = (separationForce - agent.GetVel()) * GetWeight();
            //Debug.Log(agent.GetName() + "'s separation force is " + force);
            return force;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void SetNeighbourhood(List<EnemyBase> neighbourhood) { m_neighbourhood = neighbourhood; }

    public void UpdateDistance(float distance) { m_neighbourDistance = distance; }
}
