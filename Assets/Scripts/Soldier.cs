using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Soldier : EnemyBase
{
    [Header("Weapon Positions")]
    [SerializeField]
    private GameObject mWeapon = null;
    [SerializeField]
    private GameObject mSpearHead = null;
    [SerializeField]
    private GameObject mSeathedPosition = null;

    public override void Init(int hitPoints, bool isEnemy, float speed)
    {
        base.Init(hitPoints, isEnemy, speed);

        if (mIsEnemy)
        {
            // Halfing the attacking speed of the enemy
            mAttackingSpeed /= 2;
        }
    }

    protected override void Update()
    {
        base.Update();
        base.Move();

        if (mWeapon != null)
        {
            // Check if it can attack and has a target
            if (mCanAttack && mTarget != null && mAttackTime < Time.time)
            {
                // Move the weapon towards the target
                mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mTarget.transform.position, mAttackingSpeed * Time.deltaTime);

                // Check if the spear head has hit the target
                if (Vector3.Distance(mSpearHead.transform.position, mTarget.transform.position) < 0.25f)
                {
                    // Attack the target
                    mTarget.GetComponent<EnemyBase>().Attacked(1);

                    // Reset the weapon position
                    mWeapon.transform.position = mSeathedPosition.transform.position;

                    // Reset the attack timer
                    mAttackTime = Time.time + mAttackDelay;
                }
            }
            else
            {
                // Move the weapon back to its seathed position
                mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mSeathedPosition.transform.position, mAttackingSpeed * Time.deltaTime);
            }
        }
    }
}
