using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameManagerTesting
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ASoldierSceneSetup()
    {
        string mTargetScene = "Soldier";

        GameManager mGameManager = new GameManager();

        Assert.IsFalse(mGameManager != null, "Failed to create mGameManager");

        SceneManager.LoadScene(mTargetScene);

        Debug.Log($"Loaded {mTargetScene}");

        yield return new WaitForSeconds(2);

        Assert.IsTrue(mGameManager.SetUpGameScene(), "Failed to set up the scene correctly");
    }

    [UnityTest]
    public IEnumerator ASniperSceneSetUp()
    {
        string mTargetScene = "Sniper";

        GameManager mGameManager = new GameManager();

        Assert.IsFalse(mGameManager != null, "Failed to create mGameManager");

        SceneManager.LoadScene(mTargetScene);

        Debug.Log($"Loaded {mTargetScene}");

        yield return new WaitForSeconds(2);

        Assert.IsTrue(mGameManager.SetUpGameScene(), "Failed to set up the scene correctly");
    }
}
