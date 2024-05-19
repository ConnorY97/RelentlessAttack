using Codice.Client.Commands;
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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (mCanAttack)
        {
            // Do attack here
            if (mAttackTime < Time.time && mTarget != null)
            {
                // Create a bullet
                GameObject inst = Instantiate(mBulletPrefab, mPistol.transform, true);

                inst.transform.position = mPistol.transform.position;
                Bullet bullet = inst.GetComponent<Bullet>();

                if (bullet == null)
                {
                    Destroy(inst);
                    mAttackTime = Time.time + mAttackDelay;
                    return;
                }

                bullet.Init(mTarget, mAttackingSpeed, 3);
                mAttackTime = Time.time + mAttackDelay;
            }
        }
        else
        {
            base.Move();
        }
    }
}
