using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDragonShadowBullet : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.DragonShadowExplosion;
        m_fMaxExplosionTime = 1f;
    }

    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 30;
        m_fSpeed = 8.0f;

        base.OnEnable();
    }

    protected override void Update()
    {
        if (this.transform.position.y < 0.5f && m_nCollisionCount < 1)
        {
            CollisionFloor();
            m_nCollisionCount++;
        }
        base.Update();
    }

    void CollisionFloor()
    {
        Vector3 dir = new Vector3(0, 0, 1);
        ShotBullet(dir);

        dir = Quaternion.Euler(0, 90, 0) * dir;
        ShotBullet(dir);

        dir = new Vector3(0, 0, 1);
        dir = Quaternion.Euler(0, 180, 0) * dir;
        ShotBullet(dir);

        dir = new Vector3(0, 0, 1);
        dir = Quaternion.Euler(0, 270, 0) * dir;
        ShotBullet(dir);

        cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.DragonShadowExplosion]).GetComponent<cExplosionEffect>();
        effect.m_fMaxTime = 1.0f;
        effect.InitPosition(this.transform.position);
        effect.gameObject.SetActive(true);

        m_isPass = false;
        m_isWallReflect = false;

        m_fTime = 0.0f;
        m_oRigidbody3d.velocity = Vector3.zero;
        cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);

    }

    void ShotBullet(Vector3 dir)
    {
        cDragonShadowBullet bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.DragonShadowBullet]).GetComponent<cDragonShadowBullet>();
        Vector3 pos = this.transform.position;

        pos.y += 0.5f;
        bullet.Initialization(pos, dir, false, false, false);

        bullet.gameObject.SetActive(true);

    }
}
