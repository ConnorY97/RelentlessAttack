using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : EnemyBase
{
    [Header("WeaponPositions")]
    [SerializeField]
    private GameObject mWeapon = null;
    [SerializeField]
    private GameObject mSpearHead = null;
    [SerializeField]
    private GameObject mSeathedPosition = null;
    [SerializeField]
    private float mAttackSpeed = 10.0f;

    public override void Init(int hitPoints, bool isEnemy, float speed)
    {
        base.Init(hitPoints, isEnemy, speed);
    }

    public override void Tick()
    {
        base.Tick();

        if (mCanAttack && mTarget != null)
        {
            mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mTarget.transform.position, mAttackSpeed * Time.deltaTime);

            if (Vector3.Distance(mSpearHead.transform.position, mTarget.transform.position) < 0.25f)
            {
                mTarget.GetComponent<EnemyBase>().Attacked(1);
                mWeapon.transform.position = mSeathedPosition.transform.position;
            }
        }
        else
        {
            mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mSeathedPosition.transform.position, mAttackSpeed * Time.deltaTime);
        }
    }

    public void Attack(GameObject targetToAttack)
    {
        EnemyBase attackedTarget = targetToAttack.GetComponent<EnemyBase>();

        if (attackedTarget != null && attackedTarget == mTarget)
        {
            attackedTarget.Attacked(1);
        }
    }
}
