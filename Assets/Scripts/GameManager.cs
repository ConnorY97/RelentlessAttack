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
    
    [Header("Enemy Tracking")]
    private List<EnemyBase> mEnemyBaseList = new List<EnemyBase>();
    private Soldier mPlayer = null;
    
    [Header("Enemy Vars")]
    public float mSpeed = 5;

    [Header("UI")]
    public Button mSoldierSpawnButton = null;
    public TMP_Text mUIScoreValue = null;

    private int mScore = 0;


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
            Soldier instance = Instantiate(mSoldierPrefab, transform.position, Quaternion.identity);
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
        mPlayer = Instantiate(mSoldierPrefab, mPlayerSpawn.transform.position, Quaternion.identity);
        mPlayer.Init(10, false, mSpeed);
        mPlayer.tag = "Player";
    }

    public void IncrementScore(int increment)
    {
        mScore += increment;

        mUIScoreValue.text = mScore.ToString();
    }
}
