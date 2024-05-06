using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public CellsDebugger mCellsReference;
    
    [Header("Prefab vars")]
    public Soldier mSoldierPrefab = null;
    public GameObject mPlayerSpawn = null;
    
    [Header("Entity Tracking")]
    private List<EnemyBase> mEnemyBaseList = new List<EnemyBase>();
    private List<EnemyBase> mPlayerList = new List<EnemyBase>();
    private Soldier mPlayer = null;
    
    [Header("Enemy Vars")]
    public float mSpeed = 5;

    [Header("UI")]
    public Button mSoldierSpawnButton = null;
    public TMP_Text mUIScoreValue = null;
    public TMP_Text mGameOverText = null;

    private int mScore = 5;

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
    }

    // Start is called before the first frame update
    void Start()
    {
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
            // Do spawn stuff here
            Soldier instance = Instantiate(mSoldierPrefab, transform.position, new Quaternion(0,0,-90,0));
            instance.Init(1, true, mSpeed / 2);

            instance.name = $"Soldier {i}";
            instance.tag = "Enemy";

            if (i != 0 && i % mCellsReference.mCol == 0)
            {
                currentRow++;
                currentCol = 0;
            }

            instance.transform.position = mCellsReference.mCells[currentCol][currentRow];

            currentCol++;

            mEnemyBaseList.Add(instance);
        }
    }

    private void Update()
    {
        // Checking game state
        if (mScore < 5 && mPlayerList.Count == 0)
        {
            // Player lost
            mGameOverText.text = "You Lost!";
            mGameOverText.gameObject.SetActive(true);
        }
        else if (mEnemyBaseList.Count == 0)
        {
            // Player won
            mGameOverText.text = "You Won!";
            mGameOverText.gameObject.SetActive(true);
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

    public Vector3 GetClosestEnemy(Vector3 currentPosition)
    {
        float smallestDistance = float.MaxValue;
        Vector3 closestEnemy = Vector3.zero;

        foreach (var enemyBase in mEnemyBaseList)
        {
            float currentDistance = Vector3.Distance(currentPosition, enemyBase.transform.position);
            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                closestEnemy = enemyBase.transform.position;
            }
        }

        return closestEnemy;
    }

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
}
