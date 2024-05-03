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


    // Update is called once per frame
    void Update()
    {
        Tick();

        if (mEnemy)
        {
            Vector3 target = FindClosestTarget();
            if (GameManager.Instance.GetPlayer() != null)
            {
                LookAt2D(target);

                // Check if we are colliding with other enemy soldier
                //  If so slow movement
                var colliders = Physics.OverlapSphere(transform.position, 0.35f);

                if (colliders.Length > 0)
                {
                    foreach (var collider in colliders)
                    {
                        if (collider.tag == "Enemy")
                        {
                            // Reduce movement speed
                            mSpeed *= 0.1f;
                        }
                    }
                }


                // Move towards the player
                transform.position = Vector3.MoveTowards(transform.position, target, mSpeed * Time.deltaTime);
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
                    if (currentDistance < smallestDistance)
                    {
                        smallestDistance = currentDistance;
                        closest = p.transform.position;
                    }
                }
            }
        }
        return closest;
    }
}
