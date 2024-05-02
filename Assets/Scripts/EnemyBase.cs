using System;
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
    protected int mHitPoints = 0;
    public int HitPoints
    {
        get { return mHitPoints; }
        set { mHitPoints = value; }
    }

    [Header("Movement")]
    protected float mSpeed = 10;
    public float Speed
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }
    public ContactFilter2D mContactFilter;

    [Header("UI")]
    [SerializeField]
    private TMP_Text mUIHitPoints = null;

    [Header("WeaponPositions")]
    [SerializeField]
    List<GameObject> mWeaponPositions = new List<GameObject>();

    protected GameObject mTarget = null;
    public GameObject Target
    {
        get { return mTarget; }
        set { mTarget = value; }
    }

    protected Rigidbody2D mRigidBody = null;

    private List<Behaviour> mBehaviours = new List<Behaviour>();

    public void AddBehavior(Behaviour behaviour)
    {
        if (behaviour !=  null)
        {
            mBehaviours.Add(behaviour);
        }
    }

    private void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    private void LateUpdate()
    {
        if (mEnemy)
        {
            Vector3 target = FindClosestTarget();
            if (target != null)
            {
                LookAt2D(target);

                //float moveSpeed = 10.0f;
                //if (Physics2D.OverlapCircle(transform.position, 0.35f, mContactFilter, new List<Collider2D>()) > 0)
                //{
                //    moveSpeed = mCollisionSpeed;
                //}
                //else
                //{
                //    moveSpeed = mSpeed;
                //}

                //// Move towards the player
                ////transform.position = Vector3.MoveTowards(transform.position, target, mSpeed * Time.deltaTime);
                Vector3 dir = target - transform.position;

                //mRigidBody.MovePosition(transform.position + (dir * mSpeed * Time.fixedDeltaTime));

                //mRigidBody.AddForce(dir * mSpeed, ForceMode2D.Force);
                //mRigidBody.velocity = Vector3.ClampMagnitude(mRigidBody.velocity, mSpeed);
                if (mBehaviours.Count > 0)
                {
                    Vector2 force = Vector2.zero;
                    foreach (Behaviour behaviour in mBehaviours)
                    {
                        force += behaviour.BehaviorUpdate(this);
                    }

                    mRigidBody.AddForce(force * Time.fixedDeltaTime);
                    mRigidBody.velocity = Vector2.ClampMagnitude(mRigidBody.velocity, mSpeed);
                }
            }
            else
            {
                LookAt2D(GameManager.Instance.GetClosestEnemy(transform.position));
            }
        }
    }

    public virtual void Init(int hitPoints, GameObject target, bool isEnemy, float speed)
    {
        mEnemy = isEnemy;

        mHitPoints = hitPoints;

        // Set hit points ui
        mUIHitPoints.text = mHitPoints.ToString();

        mTarget = target;

        mSpeed = speed;
    }

    private void LookAt2D(Vector3 target)
    {
        Vector3 look = transform.InverseTransformPoint(target);

        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90.0f;

        transform.Rotate(0, 0, angle);
    }

    public virtual void Tick() { }

    protected virtual void Damage(int damage)
    {
        mHitPoints -= damage;
        if (mHitPoints <= 0)
        {
            Dead();
        }

        mUIHitPoints.text = mHitPoints.ToString();
    }

    protected virtual void Dead()
    {
        // Death stuff
        // Make sure it cleans any references to itself
        Destroy(this.gameObject);
    }

    protected Vector3 FindClosestTarget()
    {
        Vector3 closest = Vector3.zero;
        float smallestDistance = float.MaxValue;
        if (mEnemy)
        {
            List<GameObject> players = new List<GameObject>();
            GameObject.FindGameObjectsWithTag("Player", players);
            if (players.Count > 0)
            {
                foreach (GameObject p in players)
                {
                    float currentDistance = Vector3.Distance(transform.position, p.transform.position);
                    if (smallestDistance > currentDistance)
                    {
                        smallestDistance = currentDistance;
                        closest = p.transform.position;
                    }
                }
            }
        }
        return closest;
    }

    public Vector2 GetPos()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public Vector2 GetVel()
    {
        return mRigidBody.velocity;
    }

    private void OnDrawGizmos()
    {
        if (mRigidBody != null)
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(transform.position, transform.position + new Vector3(mRigidBody.velocity.x, mRigidBody.velocity.y, 0).normalized * 2);
        }
    }
}
