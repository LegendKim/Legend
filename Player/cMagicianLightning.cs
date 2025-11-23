using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMagicianLightning : MonoBehaviour
{
    private Vector3 m_vTargetPosition;

    private float m_fTime;

    private float m_fAttackTime;

    public int m_nDamage;

    // Start is called before the first frame update
    void Start()
    {
        m_fTime = 0.0f;
        m_fAttackTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_fTime += Time.deltaTime;

        if(m_fTime > 4.0f)
        {
            m_fTime = 0.0f;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }

        if (MonsterFind())
        {
            NearMonsterFind();
            m_fAttackTime += Time.deltaTime;
           
            if(m_fAttackTime > 0.6f)
            {
                m_fAttackTime = 0.0f;
                ShotBullet();
            }
        }
    }

    public void ShotBullet()
    {
        Vector3 pos = this.transform.position;
        Vector3 dir = (m_vTargetPosition - this.transform.position).normalized;
        pos.y = 1.0f;
        cLightningBullet bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.LightningBullet]).GetComponent<cLightningBullet>();
        bullet.m_nDamage = this.m_nDamage;
        bullet.Initialization(pos, dir, false, false, false);
        bullet.gameObject.SetActive(true);
    }

    public bool MonsterFind()
    {
        List<cMonster> monsterList = cActorManager.GetInstance.m_cMonsterList;

        if (monsterList != null)
        {
            for (int i = 0; i < monsterList.Count; ++i)
            {
                if (monsterList[i].gameObject.activeInHierarchy &&
                    monsterList[i].m_eState != cMonster.eMonsterState.DIE)
                {
                    return true;
                }
            }
        }

        return false;
    }


    public void NearMonsterFind()
    {
        List<cMonster> monsterList = cActorManager.GetInstance.m_cMonsterList;

        float fMinDistance = 50000.0f;

        Vector3 targetPos = new Vector3(0, 0, 0);

        cMonster target = null;

        for (int i = 0; i < monsterList.Count; ++i)
        {
            if (monsterList[i].gameObject.activeInHierarchy && monsterList[i].m_eState != cMonster.eMonsterState.DIE)
            {
                float distance = (this.transform.position - monsterList[i].transform.position).magnitude;

                if (fMinDistance > distance)
                {
                    fMinDistance = distance;
                    target = monsterList[i];
                    targetPos = monsterList[i].transform.position;
                }
                else
                {
                    monsterList[i].m_TargetZone.SetActive(false);
                }
            }

        }

        target.m_TargetZone.SetActive(true);

        targetPos.y = 0.5f;

        m_vTargetPosition = targetPos;

    }
}
