using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SoldierTesting
{
    [Test]
    [TestCase(10, true, 10)]
    [TestCase(28, true, 100)]
    [TestCase(1, false, 1)]
    public void ASoldierInitTesting(int setHealth, bool isEnemy, int setSpeed)
    {
        GameObject mGameObject = new GameObject();
        Soldier mSoldier = mGameObject.AddComponent<Soldier>();
        Assert.IsNotNull(mSoldier, "Failed to create soldier");


        mSoldier.Init(setHealth, isEnemy, setSpeed);
        Assert.AreEqual(setHealth, mSoldier.HitPoints, "Failed to set health");
        Assert.AreEqual(isEnemy, mSoldier.IsEnemy, "Failed to set enemy");
        Assert.AreEqual(setSpeed, mSoldier.Speed, "Failed to set speed");
    }

    [Test]
    [TestCase(10)]
    [TestCase(-50)]
    public void BAttacked_Test(int attackAmount)
    {
        GameObject mGameObject = new GameObject();
        EnemyBase enemy = mGameObject.AddComponent<EnemyBase>();
        enemy.Init(100, true, 5.0f);

        enemy.Attacked(attackAmount);

        if (attackAmount > 0)
        {
            Assert.AreEqual(100 - attackAmount, enemy.HitPoints, "Failed to reduce hitpoint by accacking amount");
        }
        else
        {
            Assert.AreEqual(100, enemy.HitPoints, "Failed to filter incorrect attack amounts");
        }
    }

    //[UnityTest]
    //public IEnumerator CMovement_Test()
    //{
    //    GameObject mGameObject = new GameObject();
    //    EnemyBase mEnemy = mGameObject.AddComponent<EnemyBase>();
    //    mEnemy.Init(100, true, 5.0f);

    //    GameObject targetObject = new GameObject();
    //    targetObject.transform.position = new Vector3(10, 0, 0); // Set target position
        

    //    float initialDistance = Vector3.Distance(mGameObject.transform.position, targetObject.transform.position);
    //    yield return null; // Wait for one frame to update movement

    //    float updatedDistance = Vector3.Distance(mGameObject.transform.position, targetObject.transform.position);

    //    Assert.Less(updatedDistance, initialDistance); // Check if the enemy moved closer to the target
    //}
}

/*
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyBaseTests
{

    [UnityTest]
    public IEnumerator Movement_Test()
    {
        GameObject enemyObject = new GameObject();
        EnemyBase enemy = enemyObject.AddComponent<EnemyBase>();
        enemy.Init(100, true, 5.0f);

        GameObject targetObject = new GameObject();
        targetObject.transform.position = new Vector3(10, 0, 0); // Set target position

        enemy.Tick(); // Call Tick to initialize

        float initialDistance = Vector3.Distance(enemyObject.transform.position, targetObject.transform.position);
        yield return null; // Wait for one frame to update movement

        float updatedDistance = Vector3.Distance(enemyObject.transform.position, targetObject.transform.position);

        Assert.Less(updatedDistance, initialDistance); // Check if the enemy moved closer to the target
    }
}

*/
