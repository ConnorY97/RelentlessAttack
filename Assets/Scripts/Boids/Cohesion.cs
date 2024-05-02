using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Behaviour
{
    public Cohesion(float weight, float distance) : base(weight) { m_neighbourDistance = distance; }

    private float m_neighbourDistance = 10.0f;
    private List<EnemyBase> m_neighbourhood = new List<EnemyBase>();

    public override Vector2 BehaviorUpdate(EnemyBase agent)
    {
        Vector2 cohesionForce = Vector2.zero;
        int closeNeighbourCount = 0;

        foreach (EnemyBase neighbour in m_neighbourhood)
        {
            if (neighbour == agent)
                continue;

            if (Vector2.Distance(agent.GetPos(), neighbour.GetPos()) > m_neighbourDistance)
            {
                cohesionForce.x += agent.GetPos().x;
                cohesionForce.y += agent.GetPos().y;
                closeNeighbourCount++;
            }

            if (cohesionForce != Vector2.zero)
            {
                cohesionForce /= closeNeighbourCount;
                Vector2 force = (cohesionForce - agent.GetVel()) * GetWeight();
                return force;
            }
            else
            {
                return Vector2.zero;
            }
        }
        return Vector2.zero;
    }

    public void SetNeighbourhood(List<EnemyBase> neighbourhood) { m_neighbourhood = neighbourhood; }

    public void UpdateDistance(float distance) { m_neighbourDistance = distance; }
}
