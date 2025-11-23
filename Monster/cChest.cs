using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cChest : cMonster
{
    protected override void Awake()
    {
        base.Awake();
        m_fStrongValue = 1.0f;
    }

    public override void Start()
    {
        type = eMonsterType.Chest;
        base.Start();
    }

    protected override void FixedUpdate()
    {
        
    }
    protected override void MonsterDie()
    {
        m_fDieTime = 0.0f;
        m_eState = eMonsterState.IDLE;
        this.transform.rotation = m_oldQuaternion;
        InitHealOrb();

        cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.MonsterDeath]).GetComponent<cExplosionEffect>();
        effect.m_fMaxTime = 1.0f;

        Vector3 pos = this.transform.position;
        pos.y = 1.5f;

        effect.InitPosition(pos);
        effect.gameObject.SetActive(true);

        cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        
    }
    void InitHealOrb()
    {
        cHealOrb healOrb = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.HealOrb]).GetComponent<cHealOrb>();
        Vector3 pos = this.transform.position;
        pos.y = 1.2f;

        healOrb.transform.position = pos;
        healOrb.gameObject.SetActive(true);
    }
}
