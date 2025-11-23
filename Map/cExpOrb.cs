using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cExpOrb : MonoBehaviour
{
    public Transform m_targetTransform;
    public Vector3 m_vecDirection { get; set; }
    protected CapsuleCollider m_capsuleCollider;
    private int m_nExp;
    public float m_fTime = 0;
    public float m_fSpeed { get; set; }
    private Vector3 m_destPosition;

    public cPlayer m_Player;

    private bool m_isInitPosition;

    private void Awake()
    {
       
    }

    void Start()
    {
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
        m_fSpeed = 15.0f;
        m_targetTransform = m_Player.transform;
        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
    }

    protected void Update()
    {
        if (!m_isInitPosition && (m_destPosition - this.transform.position).sqrMagnitude > 1.0f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, m_destPosition, Time.deltaTime * 3.0f);
        }
        else
        {
            m_isInitPosition = true;
            m_destPosition = this.transform.position;
        }

        if (m_isInitPosition && (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.ALL_MONSTER_DIE ||
        cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.CLEAR))
        {
            Vector3 targetPos = m_targetTransform.position;
            targetPos.y = 1.0f;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * 8.0f);

            if(Vector3.Distance(this.transform.position, targetPos) < 0.4f)
            {
                cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.ExpOrbExplosion]).GetComponent<cExplosionEffect>();
                effect.m_fMaxTime = 1.0f;

                Vector3 pos = this.transform.position;
                pos.y = 1.5f;

                effect.InitPosition(pos);
                effect.gameObject.SetActive(true);
                
                m_Player.m_nExp += m_nExp;
                cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
            }
        }
    }

    public void Initialization(Vector3 startVector, Vector3 destPosition)
    {
        m_isInitPosition = false;
        transform.position = startVector;
        m_destPosition = destPosition;
        m_vecDirection = (m_destPosition - this.transform.position).normalized;

    }

    public void InitExp(int exp)
    {
        m_nExp = exp;
    }
}
