using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Grid vars")]
    public bool mDrawGizmos = false;
    [Header("Prefab vars")]
    public Soldier mSoldierPrefab = null;
    public GameObject mPlayerSpawn = null;
    // Enemy tracker
    private List<EnemyBase> mEnemyBaseList = new List<EnemyBase>();
    [Header("Debugging")]
    public CellsDebugger mCellsReference;

    // Singleton Functions
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
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
        // Create soldiers
        for (int i = 0; i < mCellsReference.mCol; i++)
        {
            // Do spawn stuff here
            Soldier instance = Instantiate(mSoldierPrefab, transform.position, Quaternion.identity);
            instance.Init(1, null);

            instance.name = $"Soldier {i}";

            instance.transform.position = mCellsReference.mCells[i][0];

            mEnemyBaseList.Add(instance);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (mCellsReference.mCells != null)
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
        else
        {
            mCellsReference.init();
        }
    }

    public GameObject GetPlayer() { return mPlayerSpawn; }
}
