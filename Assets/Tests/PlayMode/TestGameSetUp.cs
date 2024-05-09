using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGameSetUp
{
    [UnityTest]
    public IEnumerator TestingEnemySetup()
    {
        var gameObject = new GameObject();
        EnemyBase testEnemmy = gameObject.AddComponent<EnemyBase>();
        testEnemmy.Init(10, true, 10);

        // Testing Init func
        // hitpoints
        Assert.AreEqual(10, testEnemmy.HitPoints);
        // set as enemy
        Assert.True(testEnemmy.Enemy);
        // speed
        Assert.AreEqual(10, testEnemmy.Speed);

        // Testing functions
        // Attacked
        testEnemmy.Attacked(5);
        Assert.AreEqual(5, testEnemmy.HitPoints);

        yield return null;
    }
}
