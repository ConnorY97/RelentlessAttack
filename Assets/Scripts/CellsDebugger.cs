using UnityEngine;

[CreateAssetMenu(fileName = "CellsReference", menuName = "ScriptableObjects/SpawnCellsReferenceObject", order = 1)]
public class CellsDebugger : ScriptableObject
{
    public Vector2 mPos;
    public int mRow;
    public int mCol;
    public float mCellSize;
    public float mXSpacing;
    public float mYSpacking;
    public Vector3[][] mCells;

    public void init()
    {
        mCells = new Vector3[mCol][];
        for (int i = 0; i < mCol; i++)
        {
            mCells[i] = new Vector3[mRow];
            for (int j = 0; j < mRow; j++)
            {
                mCells[i][j] = new Vector3();
                mCells[i][j] = new Vector3(mPos.x + (i * (mCellSize + mXSpacing)), mPos.y - (j * (mCellSize + mYSpacking)), 0);
            }
        }
    }
}
