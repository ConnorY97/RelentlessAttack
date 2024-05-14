using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SoldierTesting
{
    [UnityTest]
    public IEnumerator ASoldierMovement_Test()
    {
        GameObject mGameObject = new GameObject();
        mGameObject.transform.position = new Vector3(0, 0, 0);
        Soldier mSoldier = mGameObject.AddComponent<Soldier>();
        mSoldier.Init(100, true, 5.0f);
        CharacterController mCharacterController = mGameObject.AddComponent<CharacterController>();

        GameObject targetObject = new GameObject();
        targetObject.transform.position = new Vector3(10, 0, 0); // Set target position
        targetObject.tag = "Player";
        yield return new WaitForSeconds(2.0f); // Wait for one frame to update movement

        float distance = Vector3.Distance(targetObject.transform.position, mGameObject.transform.position);

        Assert.IsTrue(distance < 0.05f, $"Failed to move towards the target in time, resulting distance: {distance}, cureent position: {mGameObject.transform.position}"); // Check if the enemy moved closer to the target
    }
}
