using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDragonPortal : cExplosionEffect
{
    public Vector3 m_shotDirection;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        m_fTime += Time.deltaTime;
        if (m_fTime > m_fMaxTime)
        {
            CreateBullet();

            m_fTime = 0;
            this.transform.rotation = m_oldQuaternion;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }
    
    void CreateBullet()
    {
        Vector3 pos = this.transform.position;
        pos.y = 1.0f;

        cLichBullet bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.LichBullet]).
            GetComponent<cLichBullet>();
        bullet.Initialization(pos, m_shotDirection, false, false, false);
        bullet.gameObject.SetActive(true);

        Vector3 dir = m_shotDirection;
        dir = Quaternion.Euler(0, -45, 0) * dir;
        bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.LichBullet]).GetComponent<cLichBullet>();
        bullet.Initialization(pos, dir, false, false, false);
        bullet.gameObject.SetActive(true);

        dir = m_shotDirection;
        dir = Quaternion.Euler(0, 45, 0) * dir;
        bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.LichBullet]).GetComponent<cLichBullet>();
        bullet.Initialization(pos, dir, false, false, false);
        bullet.gameObject.SetActive(true);
    }
}
