using Codice.CM.Common.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject mTarget = null;

    private Rigidbody2D mRigidBody = null;

    private Vector3 mDirection = Vector3.zero;

    private float mSpeed = 0;

    private float mLifeTime = 3;

    private float mTimer = 0;

    public void Init(GameObject target, float speed, float lifeTime)
    {
        mTarget = target;

        mRigidBody = GetComponent<Rigidbody2D>();

        if (mRigidBody == null)
        {
            // If we fail to get the rigidbody just get rid of the bullet
            Destroy(gameObject);
        }

        // Add force towards the taret and just send it
        mDirection = mTarget.transform.position - transform.position;

        mSpeed = speed;

        mLifeTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        mRigidBody.AddForce(mDirection * mSpeed);

        mTimer += Time.deltaTime;

        // Kill the bullet after the life time.
        if (mTimer > mLifeTime)
        {
            Destroy(gameObject);
        }

        if (mTarget != null)
        {
            if (Vector3.Distance(transform.position, mTarget.transform.position) < 0.25f)
            {
                mTarget.GetComponent<EnemyBase>().Attacked(1);

                Destroy(gameObject);
            }
        }
    }
}
