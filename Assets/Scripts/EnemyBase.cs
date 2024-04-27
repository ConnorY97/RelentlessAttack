using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using Unity.Notifications.iOS;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected int mHitPoints = 0;
    public int HitPoints
    {
        get { return mHitPoints; }
        set { mHitPoints = value; }
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

        if (GameManager.Instance.GetPlayer() != null)
        {
            Vector3 look = transform.InverseTransformPoint(GameManager.Instance.GetPlayer().transform.position);

            float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90.0f;

            transform.Rotate(0,0,angle);
        }
    }

    public virtual void Init(int hitPoints, GameObject target)
    {
        mHitPoints = hitPoints;

        // Set hit points ui
        mUIHitPoints.text = mHitPoints.ToString();

        mTarget = target;
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

    public void Attack()
    {

    }


}
