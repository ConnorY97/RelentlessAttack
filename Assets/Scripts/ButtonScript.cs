using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void LoadSoldierScene()
    {
        SceneManager.LoadScene("Soldier");
    }
    public void LoadSniperScene()
    {
        SceneManager.LoadScene("Sniper");
    }

    public void SpawnSoldier()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpawnSoldier();
        }
    }
}
