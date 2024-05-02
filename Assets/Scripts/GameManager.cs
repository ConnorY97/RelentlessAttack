using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Grid vars")]
    public bool mDrawGizmos = false;
    public CellsDebugger mCellsReference;
    [Header("Prefab vars")]
    public Soldier mSoldierPrefab = null;
    public GameObject mPlayerSpawn = null;
    // Enemy tracker
    private List<EnemyBase> mEnemyBaseList = new List<EnemyBase>();
    private Soldier mPlayer = null;
    [Header("Enemy Vars")]
    public float mSpeed = 5;

    [Header("Boids Behavior")]
    public float mSeekWeight = 100.0f;
    public float mCohesionWeight = 100.0f;
    public float mAlignmentWeight = 100.0f;
    public float mSeparationWeight = 100.0f;
    public float mCohesionDistance = 100.0f;
    public float mNeighbourhoodDistance = 10.0f;
    public float mAlignmentBuffer = 0.75f;

    private Seek mSeekBehaviour = null;
    private Cohesion mCohesionBehavior = null;
    private Alignment mAlignmentBehavior = null;
    private Separation mSeparationBehavior = null;

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

        // Create the player
        mPlayer = Instantiate(mSoldierPrefab, mPlayerSpawn.transform.position, Quaternion.identity);
        mPlayer.Init(10, null, false, mSpeed);
        mPlayer.tag = "Player";

        // Boid behavior initialization
        mSeekBehaviour = new Seek(mSeekWeight);
        mSeekBehaviour.SetTarget(mPlayer.GetComponent<EnemyBase>());
        mCohesionBehavior = new Cohesion(mCohesionWeight, mCohesionDistance);
        mAlignmentBehavior = new Alignment(mAlignmentWeight, mNeighbourhoodDistance, mAlignmentBuffer);
        mSeparationBehavior = new Separation(mSeparationWeight, mNeighbourhoodDistance);

        // Total amount of soldier we can spawn
        int totalEnemySpawn = mCellsReference.mCol * mCellsReference.mRow;
        int currentRow = 0;
        int currentCol = 0;
        // Create soldiers
        for (int i = 0; i < totalEnemySpawn; i++)
        {
            // Do spawn stuff here
            Soldier instance = Instantiate(mSoldierPrefab, transform.position, Quaternion.identity);
            instance.Init(1, mPlayer.gameObject, true, mSpeed / 2);

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

        mAlignmentBehavior.SetNeighbourHood(mEnemyBaseList);
        mCohesionBehavior.SetNeighbourhood(mEnemyBaseList);
        mSeparationBehavior.SetNeighbourhood(mEnemyBaseList);
        foreach (EnemyBase enemy in mEnemyBaseList)
        {
            enemy.AddBehavior(mSeekBehaviour);
            enemy.AddBehavior(mCohesionBehavior);
            enemy.AddBehavior(mAlignmentBehavior);
            enemy.AddBehavior(mSeparationBehavior);
        }
    }

    private void Update()
    {
        mSeekBehaviour.UpdateWeight(mSeekWeight);
        mCohesionBehavior.UpdateWeight(mCohesionWeight);
        mAlignmentBehavior.UpdateWeight(mAlignmentWeight);
        mSeparationBehavior.UpdateWeight(mSeparationWeight);
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

}
