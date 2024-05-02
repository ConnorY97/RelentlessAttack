using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Behaviour
{
    private float m_weight;

    public Behaviour(float weight)
    {
        m_weight = weight;
    }

    public virtual Vector2 BehaviorUpdate(EnemyBase enemy) { return Vector2.zero; }

    public float GetWeight() { return m_weight; }

    public void UpdateWeight(float weight) { m_weight = weight; }
}
