using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cHealOrb : MonoBehaviour
{
    public cPlayer m_target;
    public Vector3 m_vecDirection { get; set; }
    protected CapsuleCollider m_capsuleCollider;
    private int m_nHp;
    public float m_fTime = 0;
    public float m_fSpeed { get; set; }

    void Start()
    {
        m_fSpeed = 10.0f;
        m_target = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
        m_nHp = m_target.m_nMaxHp / 2;
        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
    }

    protected void Update()
    {
        Vector3 pos = m_target.transform.position;
        pos.y = 1.0f;
        this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * m_fSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            cPlayer player = other.GetComponent<cPlayer>();
            player.m_nHp += this.m_nHp;
            if(player.m_nHp > player.m_nMaxHp)
            {
                player.m_nHp = player.m_nMaxHp;
            }

            Vector3 pos = this.transform.position;
            pos.y = 1.5f;

            cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.HealExplosion]).GetComponent<cExplosionEffect>();
            effect.m_fMaxTime = 2.0f;
            effect.InitPosition(pos);
            effect.gameObject.SetActive(true);

            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }

    }
}
