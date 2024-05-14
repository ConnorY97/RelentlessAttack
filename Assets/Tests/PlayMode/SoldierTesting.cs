using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SoldierTesting
{

    [Test]
    [TestCase(10, true, 10, 5)]
    [TestCase(28, true, 100, 19)]
    [TestCase(1, false, 1, 1)]
    public void SoldierInitTesting(int setHealth, bool isEnemy, int setSpeed, int attackDamage)
    {
        GameObject gameObject = new GameObject();
        Soldier soldier = gameObject.AddComponent<Soldier>();
        Assert.IsNotNull(soldier, "Failed to create soldier");


        soldier.Init(setHealth, isEnemy, setSpeed);
        Assert.IsTrue(soldier.HitPoints == setHealth, "Failed to set health");
        Assert.IsTrue(soldier.IsEnemy == isEnemy, "Failed to set enemy");
        Assert.IsTrue(soldier.Speed == setSpeed, "Failed to set speed");

        soldier.Attacked(attackDamage);
        Assert.IsTrue(soldier.HitPoints == (setHealth - attackDamage), "Attacked Failed to deduct correct heatl");
    }
}
