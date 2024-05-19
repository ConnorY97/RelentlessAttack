using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BulletTesting
{
    [UnityTest]
    public IEnumerator BulletTestingWithEnumeratorPasses()
    {
        GameObject mGameObject = new GameObject();
        Bullet mBullet = mGameObject.AddComponent<Bullet>();

        GameObject mTarget = new GameObject();
        mTarget.transform.position = new Vector3(10,10,10);

        mBullet.Init(mTarget, 10, 2);


        yield return new WaitForSeconds(2);

        Assert.IsTrue(mGameObject == null, "Failed to destory bullet after lifetime.");
    }
}
