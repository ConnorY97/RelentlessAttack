using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerType
{
    SOLDIER,
    NONE
}


public class GameManager : MonoBehaviour
{
    [Header("Grid vars")]
    public bool mDrawGizmos = false;
    public CellsDebugger mCellsReference;// = null;

    [Header("Prefab vars")]
    public Soldier mSoldierPrefab = null;
    public GameObject mPlayerSpawn = null;
    public Sniper mSniperPrefab = null;

    [Header("Entity Tracking")]
    private List<EnemyBase> mEnemyBaseList = new List<EnemyBase>();
    public List<EnemyBase> EnemyList
    {
        get { return mEnemyBaseList; }
    }
    private List<EnemyBase> mPlayerList = new List<EnemyBase>();
    public List<EnemyBase> PlayerList
    {
        get { return mPlayerList; }
    }
    private Soldier mPlayer = null;

    [Header("Enemy Vars")]
    public float mSpeed = 5;

    [Header("UI")]
    public Button mSoldierSpawnButton = null;
    public TMP_Text mUIScoreValue = null;
    public TMP_Text mUIGameOver = null;

    private int mScore = 5;
    public int Score
    {
        get { return mScore; }
    }

    private string mCurrentSceneName = "";
    public string CurrentSceneName
    {
        get { return mCurrentSceneName; }
    }

    private bool mInGame = false;
    public bool InGame
    {
        get { return mInGame; }
    }

    // Singleton Functions
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (mInGame)
        {
            // Checking game state
            if (mScore < 5 && mPlayerList.Count == 0)
            {
                // Player lost
                mUIGameOver.text = "You Lost!";
                mUIGameOver.gameObject.SetActive(true);
                StartCoroutine(LoadMainMenuWithDelay(3));
                mInGame = false;
            }
            else if (mEnemyBaseList.Count == 0)
            {
                // Player won
                mUIGameOver.text = "You Won!";
                mUIGameOver.gameObject.SetActive(true);
                StartCoroutine(LoadMainMenuWithDelay(3));
                mInGame = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mDrawGizmos && mCellsReference.mCells != null)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < mCellsReference.mCol; i++)
            {
                for (int j = 0; j < mCellsReference.mRow; j++)
                {
                    Gizmos.DrawCube(mCellsReference.mCells[i][j], new Vector3(mCellsReference.mCellSize, mCellsReference.mCellSize, mCellsReference.mCellSize));
                }
            }
        }
    }

    public GameObject GetPlayer() { return mPlayer.gameObject; }

    public void SpawnSoldier()
    {
        // Create the player
        Soldier instance = Instantiate(mSoldierPrefab, mPlayerSpawn.transform.position, Quaternion.identity);
        instance.Init(10, false, mSpeed);
        instance.tag = "Player";

        // Update the score when you purchase a soldier
        mScore -= 5;
        mUIScoreValue.text = mScore.ToString();

        // Check if you have enough to spawn another soldier
        if (mScore < 5)
        {
            mSoldierSpawnButton.interactable = false;
        }

        mPlayerList.Add(instance);
    }

    public void SpawnSniper()
    {
        // Create the player
        Sniper instance = Instantiate(mSniperPrefab, mPlayerSpawn.transform.position, Quaternion.identity);
        instance.Init(10, false, mSpeed);
        instance.tag = "Player";

        // Update the score when you purchase a soldier
        mScore -= 5;
        mUIScoreValue.text = mScore.ToString();

        // Check if you have enough to spawn another soldier
        if (mScore < 5)
        {
            mSoldierSpawnButton.interactable = false;
        }

        mPlayerList.Add(instance);
    }

    public void IncrementScore(int increment)
    {
        mScore += increment;

        mUIScoreValue.text = mScore.ToString();

        if (mScore >= 5)
        {
            mSoldierSpawnButton.interactable = true;
        }
    }

    public void RemoveDeadEnity(EnemyBase deadEntity, bool isEnemy)
    {
        if (isEnemy)
        {
            mEnemyBaseList.Remove(deadEntity);
        }
        else
        {
            mPlayerList.Remove(deadEntity);
        }
    }

    private IEnumerator LoadMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    public bool SetUpGameScene()
    {
        bool successfullySetUp = true;
        mPlayerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (mPlayerSpawn == null)
        {
            Debug.Log("Failed to find player spawn");
            successfullySetUp = false;
        }

        mSoldierSpawnButton = GameObject.FindGameObjectWithTag("SoldierSpawnButton").GetComponent<Button>();

        if (mSoldierSpawnButton == null)
        {
            Debug.Log("Failed to find soldier spawn button");
            successfullySetUp = false;
        }

        mUIScoreValue = GameObject.FindGameObjectWithTag("ScoreValue").GetComponent<TMP_Text>();

        if (mUIScoreValue == null)
        {
            Debug.Log("Failed to find score ui");
            successfullySetUp = false;
        }

        mUIGameOver = GameObject.FindGameObjectWithTag("GameOverMsg").GetComponent<TMP_Text>();

        if (mUIGameOver == null)
        {
            Debug.Log("Failed to find game over ui");
            successfullySetUp = false;
        }

        mUIGameOver.gameObject.SetActive(false);

        return successfullySetUp;
    }

    private void SpawnEnemies(EnemyBase prefab, string enemyType)
    {
        if (!SetUpGameScene())
            return;

        if (mCellsReference.mCells == null)
        {
            mCellsReference.init();
        }

        // Total amount of soldier we can spawn
        int totalEnemySpawn = mCellsReference.mCol * mCellsReference.mRow;
        int currentRow = 0;
        int currentCol = 0;
        // Create soldiers
        for (int i = 0; i < totalEnemySpawn; i++)
        {
            EnemyBase instance = null;
            instance = Instantiate(prefab, transform.position, new Quaternion(0, 0, -90, 0));
            instance.name = $"{enemyType} {i}";

            instance.Init(1, true, mSpeed / 2);

            instance.tag = "Enemy";

            if (i != 0 && i % mCellsReference.mCol == 0)
            {
                currentRow++;
                currentCol = 0;
            }

            instance.transform.position = mCellsReference.mCells[currentCol][currentRow];

            currentCol++;

            mEnemyBaseList.Add(instance);

            mInGame = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Soldier":
                SpawnEnemies(mSoldierPrefab, "Soldier");
                break;
            case "Sniper":
                SpawnEnemies(mSniperPrefab, "Sniper");
                break;
            default:
                break;
        }
    }
}
