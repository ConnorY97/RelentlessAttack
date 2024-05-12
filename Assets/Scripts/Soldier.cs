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
    [SerializeField]
    private float mAttackDelay = 1.0f;
    public float mAttackTime = 0;
    public float time = 0;

    public override void Init(int hitPoints, bool isEnemy, float speed)
    {
        base.Init(hitPoints, isEnemy, speed);

        mAttackTime = Time.time + mAttackDelay;
    }

    public override void Tick()
    {
        time = Time.time;
        base.Tick();
        if (mCanAttack && mTarget != null && mAttackTime < Time.time)
        {
            mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mTarget.transform.position, mAttackSpeed * Time.deltaTime);

            if (Vector3.Distance(mSpearHead.transform.position, mTarget.transform.position) < 0.25f)
            {
                mTarget.GetComponent<EnemyBase>().Attacked(1);
                mWeapon.transform.position = mSeathedPosition.transform.position;

                // Reset Timer only after the attack has happened
                mAttackTime = Time.time + mAttackDelay;
            }
        }
        else
        {
            mWeapon.transform.position = Vector3.MoveTowards(mWeapon.transform.position, mSeathedPosition.transform.position, mAttackSpeed * Time.deltaTime);
        }
    }
}
