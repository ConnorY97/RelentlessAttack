using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected bool mEnemy = true;
    public bool Enemy
    {
        get { return mEnemy; }
        set { mEnemy = value; }
    }

    [Header("Health")]
    [SerializeField]
    protected int mHitPoints = 0;
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
    protected Vector3 mPreviousLookAt = Vector3.zero;

    [Header("UI")]
    [SerializeField]
    private TMP_Text mUIHitPoints = null;

    protected CharacterController mCharacterController = null;

    [Header("Attacking Vars")]
    [SerializeField]
    protected float mAttackDistance = 1.0f;
    public float AttackDistance
    {
        get { return mAttackDistance; }
        set { mAttackDistance = value; }
    }
    [SerializeField]
    protected bool mCanAttack = false;

    protected GameObject mTarget = null;

    [Header("Enemy Design")]
    protected Material mMaterial = null;

    private void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
        mPreviousLookAt = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        Tick();

        mTarget = FindClosestTarget(out mCanAttack);
        if (mTarget != null)
        {
            SmoothLookAt2D(mTarget.transform.position);

            // Move towards the target
            Vector3 move = (mTarget.transform.position - transform.position).normalized * mSpeed;
            mCharacterController.Move(move * Time.deltaTime);
        }
    }

    public virtual void Init(int hitPoints, bool isEnemy, float speed)
    {
        mEnemy = isEnemy;

        mHitPoints = hitPoints;

        // Set hit points ui

        SetUIText(mHitPoints.ToString());

        mSpeed = speed;

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            if (mEnemy)
            {
                mat.color = Color.red;
            }
            else
            {
                mat.color = Color.green;
            }
        }
    }

    private void SmoothLookAt2D(Vector3 target)
    {
        Vector3 look = target - transform.position;
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate towards the desired rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * mTurnSpeed);
    }

    public virtual void Tick() { }
    protected GameObject FindClosestTarget(out bool canAttack)
    {
        GameObject closest = null;
        canAttack = false;
        float smallestDistance = float.MaxValue;
        if (mEnemy)
        {
            List<GameObject> players = new List<GameObject>();
            GameObject.FindGameObjectsWithTag("Player", players);
            if (players.Count > 0)
            {
                foreach (GameObject player in players)
                {
                    float currentDistance = Vector3.Distance(transform.position, player.transform.position);
                    if (currentDistance < smallestDistance)
                    {
                        smallestDistance = currentDistance;
                        closest = player;
                    }

                    if (currentDistance < mAttackDistance)
                    {
                        canAttack = true;
                    }
                }
                return closest;
            }
            else
            {
                return null;
            }
        }
        else
        {
            List<GameObject> enemies = new List<GameObject>();
            GameObject.FindGameObjectsWithTag("Enemy", enemies);
            if (enemies.Count > 0)
            {
                foreach (GameObject enemy in enemies)
                {
                    float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (currentDistance < smallestDistance)
                    {
                        smallestDistance = currentDistance;
                        closest = enemy;
                    }

                    if (currentDistance < mAttackDistance)
                    {
                        canAttack = true;
                    }
                }
                return closest;
            }
            return null;
        }
    }

    public void Attacked(int damageDelt)
    {
        mHitPoints -= damageDelt;

        if (mHitPoints <= 0)
        {
            if (mEnemy)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.RemoveDeadEnity(this, true);
            }
            else
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.RemoveDeadEnity(this, false);
            }

            Destroy(gameObject);

            if (GameManager.Instance != null)
                GameManager.Instance.IncrementScore(1);
        }
        else
        {
            if (mUIHitPoints != null)
            {
                SetUIText(mHitPoints.ToString());
            }
        }
    }

    private void SetUIText(string text)
    {
        if (mUIHitPoints != null)
        {
            mUIHitPoints.text = text;
        }
    }
}
