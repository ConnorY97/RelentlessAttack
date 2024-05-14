using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyBaseTesting
{
    [Test]
    [TestCase(10, true, 10)]
    [TestCase(28, true, 100)]
    [TestCase(1, false, 1)]
    public void AInit_Test(int setHealth, bool isEnemy, int setSpeed)
    {
        GameObject mGameObject = new GameObject();
        EnemyBase mEnemy = mGameObject.AddComponent<EnemyBase>();
        Assert.IsNotNull(mEnemy, "Failed to create soldier");

        mEnemy.Init(setHealth, isEnemy, setSpeed);
        Assert.AreEqual(setHealth, mEnemy.HitPoints, "Failed to set health");
        Assert.AreEqual(isEnemy, mEnemy.IsEnemy, "Failed to set enemy");
        Assert.AreEqual(setSpeed, mEnemy.Speed, "Failed to set speed");
    }

    [Test]
    [TestCase(10)]
    [TestCase(-50)]
    public void BAttacked_Test(int attackAmount)
    {
        GameObject mGameObject = new GameObject();
        EnemyBase mEnemy = mGameObject.AddComponent<EnemyBase>();
        mEnemy.Init(100, true, 5.0f);

        mEnemy.Attacked(attackAmount);

        if (attackAmount > 0)
        {
            Assert.AreEqual(100 - attackAmount, mEnemy.HitPoints, "Failed to reduce hitpoint by accacking amount");
        }
        else
        {
            Assert.AreEqual(100, mEnemy.HitPoints, "Failed to filter incorrect attack amounts");
        }
    }
}
