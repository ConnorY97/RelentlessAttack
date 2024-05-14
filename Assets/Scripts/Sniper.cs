using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : EnemyBase
{
    [Header("Pistol")]
    [SerializeField]
    private GameObject mPistol = null;
    [SerializeField]
    private GameObject mBulletPrefab = null;

    private GameObject mBullet = null;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (mCanAttack)
        {
            // Do attack here
            if (mAttackTime < Time.time && mBullet == null)
            {
                // Create a bullet
                mBullet = Instantiate(mBulletPrefab, mPistol.transform);

                mAttackTime = Time.time + mAttackDelay;
            }
        }
        else
        {
            base.Move();
        }

        // If a bullet exists, move it towards the player
        if (mBullet && mTarget != null)
        {
            mBullet.transform.position = Vector3.MoveTowards(mBullet.transform.position, mTarget.transform.position, mAttackingSpeed * Time.deltaTime);

            if (Vector3.Distance(mBullet.transform.position, mTarget.transform.position) < 0.25f)
            {
                mTarget.GetComponent<EnemyBase>().Attacked(1);

                Destroy(mBullet);
            }
        }
    }
}
