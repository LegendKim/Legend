using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMetalon : cMonster
{
    public enum eBossPattern
    {
        IDLE,
        WALK,
        RUN,
        SPELL_FRONT,
        SPELL_UP
    }

    private float m_fActionTime;
    private float m_fPatternTime;
    private float m_fMaxPatternTime;
    public eBossPattern m_eBossPattern;
    public GameObject m_MagicZone;
    public Transform m_attackTransform;

    public Vector3 m_SpellTargetPosition;

    public override void Start()
    {
        type = eMonsterType.Metalon;
        base.Start();

        m_fActionTime = 0.0f;
        m_fPatternTime = 0.0f;
        m_fMaxPatternTime = Random.Range(1.5f, 2.5f);
    }

    protected override void FixedUpdate()
    {
        if ((this.transform.position.x > 0.0f && this.transform.position.x < cMapManager.GetInstance.m_MaxWidth - 0.5f) &&
            (this.transform.position.z > 0.5f && this.transform.position.z < cMapManager.GetInstance.m_MaxHeight - 1.5f))
        {
            // Do Nothing
        }
        else
        {

            MapOutPosition();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (m_eBossPattern == eBossPattern.IDLE)
        {
            m_fPatternTime += Time.deltaTime;
        }
        



        if (m_fPatternTime >= m_fMaxPatternTime)
        {
            m_fPatternTime = 0.0f;
            m_fMaxPatternTime = Random.Range(1.5f, 2.5f);
            int randNum = Random.Range(1, 5);
            m_eBossPattern = (eBossPattern)randNum;
            BossAnimation();

        }
        BossPatternUpdate();
    }

    void BossPatternUpdate()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.WALK:
                Walk();
                break;
            case eBossPattern.RUN:
                Run();
                break;
            case eBossPattern.SPELL_FRONT:
                SpellFront();
                break;
            case eBossPattern.SPELL_UP:
                SpellUp();
                break;
        }
    }


    void BossAnimation()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.WALK:
                m_MagicZone.SetActive(false);
                m_animator.SetBool("Walk", true);
                break;
            case eBossPattern.RUN:
                Vector3 vec = new Vector3(0, 0, 0);
                vec.x = Random.Range(0, 3.14f);
                vec.z = Random.Range(0, 3.14f);
                m_vDirection = vec;

                vec = this.transform.position + m_vDirection;
                this.transform.LookAt(vec);
                m_MagicZone.SetActive(false);
                m_animator.SetBool("Run", true);
                break;
            case eBossPattern.SPELL_FRONT:
                Vector3 pos = m_tTarget.position;
                pos.y += 0.5f;
                m_SpellTargetPosition = pos;
                transform.LookAt(m_tTarget.position);
                m_animator.SetTrigger("SpellFront");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.SPELL_UP:
                transform.LookAt(m_tTarget.position);
                SpellZone();
                m_animator.SetTrigger("SpellUp");
                m_MagicZone.SetActive(true);
                break;
        }
    }

    void Idle()
    {
        transform.LookAt(m_tTarget.position);
    }



    void Walk()
    {
        transform.LookAt(m_tTarget.position);
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 2.0f)
        {
            m_fActionTime = 0.0f;
            m_eBossPattern = eBossPattern.IDLE;
            m_animator.SetBool("Walk", false);
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, m_tTarget.position, Time.deltaTime * 0.4f);
        }
    }

    void Run()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 4.0f)
        {
            m_fActionTime = 0.0f;
            m_eBossPattern = eBossPattern.IDLE;
            m_animator.SetBool("Run", false);
        }
        else
        {
            this.transform.position += m_vDirection.normalized * m_fSpeed * Time.deltaTime;
        }
    }

    void SpellZone()
    {
        Vector3 pos = m_tTarget.position;
        pos.y += 0.5f;

        cExplosionEffect attackZone =
    cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.AttackZone])
    .GetComponent<cExplosionEffect>();
        attackZone.InitPosition(pos);
        attackZone.gameObject.SetActive(true);
        m_SpellTargetPosition = pos;
    }

    void JumpAttack()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 2.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }

        this.transform.position = Vector3.Lerp(this.transform.position, m_SpellTargetPosition, Time.deltaTime);
    }

    void SpellFront()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 0.7f)
        {
            Vector3 pos = m_attackTransform.position;
            Vector3 dir = (m_SpellTargetPosition - m_attackTransform.position).normalized;
            ShotBullet(pos, dir);
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }
    }


    void SpellUp()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 1.0f)
        {
            Vector3 pos = m_SpellTargetPosition;
            pos.y += 10.0f;
            Vector3 dir = new Vector3(0, -1, 0);
            ShotBullet(pos, dir);
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }
    }

    void ShotBullet(Vector3 pos, Vector3 dir)
    {
        cMetalonBullet metalonBullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.MetalonBullet]).GetComponent<cMetalonBullet>();
        metalonBullet.Initialization(pos, dir, false, false, false);
        metalonBullet.gameObject.SetActive(true);
    }




    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.layer == LayerMask.NameToLayer("MapWall"))
        {
            WallReflect(collision);
        }
    }

    private void MapOutPosition()
    {
        List<cBlock> blockList = cMapManager.GetInstance.m_blockList;

        float minDistance = 99999999.9f;
        int blockNum = 0;
        for (int i = 0; i < blockList.Count; ++i)
        {
            if (blockList[i].m_eBlockType == cBlock.eBlockType.GROUND)
            {
                Vector3 vec = blockList[i].transform.position - this.transform.position;

                if (minDistance > vec.magnitude)
                {
                    minDistance = vec.magnitude;
                    blockNum = i;
                }

            }
        }

        Vector3 pos = blockList[blockNum].transform.position;
        pos.y = 0.5f;

        this.transform.position = pos;

    }

    private void WallReflect(Collision collision)
    {
        Vector3 incomingVector = m_vDirection.normalized;
        //충돌한 면의 법선 벡터를 구해낸다.
        Vector3 normalVector = collision.contacts[0].normal;
        Vector3 reflectVector = Vector3.Reflect(incomingVector, normalVector); //반사각

        m_vDirection = reflectVector.normalized;

        Vector3 vec = this.transform.position + m_vDirection;
        this.transform.LookAt(vec);
    }
}
