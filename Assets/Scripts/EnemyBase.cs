using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField]
    protected bool mIsEnemy = true;
    public bool IsEnemy
    {
        get { return mIsEnemy; }
        set { mIsEnemy = value; }
    }

    [Header("Health")]
    [SerializeField]
    protected int mHitPoints = 10;
    public int HitPoints
    {
        get { return mHitPoints; }
        set { mHitPoints = value; }
    }

    [Header("Movement")]
    [SerializeField]
    protected float mSpeed = 10.0f;
    public float Speed
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }
    [SerializeField]
    protected float mTurnSpeed = 10.0f;

    [Header("Attacking")]
    [SerializeField]
    protected float mAttackDistance = 1.0f;
    [SerializeField]
    protected float mAttackDelay = 1.0f;
    [SerializeField]
    protected float mEnemyDelayModifier = 1.5f;
    [SerializeField]
    protected float mAttackingSpeed = 5.0f;
    [SerializeField]
    protected float mEnemyAttackSpeedModifier = 0.5f;
    protected float mAttackTime = 0.0f;


    [Header("UI")]
    [SerializeField]
    private TMP_Text mUiHitPoints = null;

    [Header("Memeber Vars")]
    protected Material mMaterial = null;
    protected CharacterController mCharacterController = null;
    protected GameObject mTarget = null;
    protected bool mCanAttack = false;
    protected GameManager mGameManager;

    private void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
        mGameManager = GameManager.Instance;

        // Initialize attack time
        mAttackTime = Time.time + mAttackDelay;
    }

    protected virtual void Update()
    {
        mTarget = FindClosestTarget(mIsEnemy ? "Player" : "Enemy", out mCanAttack);
        if (mTarget != null)
        {
            transform.rotation = SmoothLookAt2D();
        }
    }

    protected virtual void Move()
    {
        if (mTarget != null)
        {
            mCharacterController.Move((mTarget.transform.position - transform.position).normalized * mSpeed * Time.deltaTime);
        }
    }

    public virtual void Init(int hitPoints, bool isEnemy, float speed)
    {
        mIsEnemy = isEnemy;
        mHitPoints = hitPoints;

        SetUIText(hitPoints.ToString());

        mSpeed = speed;

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend != null)
        {
            mMaterial = rend.material;
            if (mMaterial != null)
            {
                if (mMaterial != null)
                    mMaterial.color = mIsEnemy ? Color.red : Color.green;
            }
        }

        // Add modifers if it is an enemy
        if (mIsEnemy)
        {
            mAttackingSpeed *= mEnemyAttackSpeedModifier;

            mAttackDelay *= mEnemyDelayModifier;
        }
    }

    protected GameObject FindClosestTarget(string tag, out bool canAttack)
    {
        canAttack = false;
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float smallestDistance = Mathf.Infinity;

        foreach (GameObject potentialTarget in targets)
        {
            float currentDistance = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                closest = potentialTarget;
            }

            if (currentDistance < mAttackDistance)
                canAttack = true;
        }

        return closest;
    }

    protected Quaternion SmoothLookAt2D()
    {
        Vector3 look = mTarget.transform.position - transform.position;
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate towards the desired rotation
        return Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * mTurnSpeed);
    }

    public void Attacked(int damage)
    {
        if (damage >= 0)
        {
            mHitPoints -= damage;

            if (mHitPoints <= 0)
            {
                if (mGameManager != null)
                    mGameManager.RemoveDeadEnity(this, mIsEnemy);

                Destroy(gameObject);

                if (mIsEnemy)
                {
                    if (mGameManager != null)
                        mGameManager.IncrementScore(1);
                }
            }
            else if (mUiHitPoints != null)
            {
                SetUIText(mHitPoints.ToString());
            }
        }
    }

    private void SetUIText(string text)
    {
        if (mUiHitPoints != null)
            mUiHitPoints.text = text;
    }
}
