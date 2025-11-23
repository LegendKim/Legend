using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum eMonsterType
{
    None = 0,
    Bat,
    Chicken,
    RedCyclopes,
    Bomb,
    Turtle,
    Lich,
    Chest,
    Golem,
    Metalon,
    Dragon,
}

public class cMonster : MonoBehaviour
{
    public enum eMonsterState
    {
        IDLE,
        MOVE,
        ATTACK,
        GET_HIT,
        DIE
    }

    protected SkinnedMeshRenderer m_skinnedMeshRenderer;
    public float m_fStrongValue { get; set; }
    public Animator m_animator { get; set; }
    public eMonsterState m_eState { get; set; }
    public Transform m_tTarget { get; set; }
    public Vector3 m_vDirection;
    public float m_fSpeed { get; set; }
    public int m_nMaxHp { get; set; }
    public int m_nHp;
    public int m_nDamage { get; set; }
    public int m_nExp { get; set; }
    public bool m_isDie { get; set; }
    public float m_fDieTime { get; set; }
    public float m_fMaxDieTime { get; set; }
    public float m_fGetHitTime { get; set; }
    public float m_fFireStateTime { get; set; }
    public int m_nFireCount;

    public Quaternion m_oldQuaternion;
    float m_fAStarTime;
    float m_fMaxAStarTime;
    public List<Vector3> m_vecRoute;
    int m_nStartIndex;

    public Vector3 m_OldtargetBlockPos;

    cPlayer m_Player;
    public bool m_isFireState { get; set; }
    public GameObject m_TargetZone;
    protected eMonsterType type = eMonsterType.None;

	protected virtual void Awake()
    {
        m_fMaxDieTime = 1.0f;
        m_fStrongValue = 1.0f;
        m_isFireState = false;
        m_fFireStateTime = 0.0f;
        m_animator = this.GetComponent<Animator>();
        m_oldQuaternion = this.transform.rotation;
    }

    public virtual void Start()
    {
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
        m_vecRoute.Clear();
        m_vecRoute = new List<Vector3>();
       
        m_nStartIndex = 0;
        m_fAStarTime = 0.0f;
        m_fMaxAStarTime = Random.Range(0.0f, 1.0f);

        m_nFireCount = 0;
        m_isFireState = false;
        m_skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        m_tTarget = m_Player.gameObject.transform;
        m_OldtargetBlockPos = new Vector3(0, 0, 0);

        SetMonsterData();
    }

    protected virtual void FixedUpdate()
    {
        transform.LookAt(m_tTarget.position);
        m_vDirection = (m_tTarget.position - this.transform.position).normalized;
    }

    protected virtual void Update()
    {
        if(m_isFireState)
        {

            m_fFireStateTime += Time.deltaTime;
            
            if (m_fFireStateTime > 1.0f)
            {
                m_nFireCount++;
                FireShotHit();
                m_fFireStateTime = 0.0f;
            }
            if(m_nFireCount >=3)
            {
                m_isFireState = false;
            }
        }

        switch (m_eState)
        {
            case eMonsterState.IDLE:
            case eMonsterState.MOVE:
            case eMonsterState.ATTACK:
                m_skinnedMeshRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;

            case eMonsterState.GET_HIT:
                m_fGetHitTime += Time.deltaTime;
                m_skinnedMeshRenderer.material.color = new Color(1.0f, m_fGetHitTime * 2.0f, m_fGetHitTime * 2.0f, 1.0f);
                if (m_fGetHitTime > 0.5f)
                {
                    m_fGetHitTime = 0.0f;
                    m_eState = eMonsterState.IDLE;
                }
                break;
            case eMonsterState.DIE:
                m_fDieTime += Time.deltaTime;

                if (m_fDieTime >= m_fMaxDieTime)
                {
                    MonsterDie();
                }
                
                break;
        }
    }
    void SetMonsterData()
    {
        cMonsterData data = cActorManager.GetInstance.m_monsterDataDic[type];
        m_nMaxHp = data.m_nHp;
        m_nHp = m_nMaxHp;
        m_nExp = data.m_nExp;
        m_nDamage = data.m_nDamage;
        m_fSpeed = data.m_fSpeed;
    }
    protected virtual void MonsterDieStand()
    {
        m_animator.SetTrigger("Die");
        this.m_nHp = 0;
        m_eState = eMonsterState.DIE;
    }

    protected virtual void MonsterDie()
    {
        m_fDieTime = 0.0f;
        m_eState = eMonsterState.IDLE;
        this.transform.rotation = m_oldQuaternion;

        cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.MonsterDeath]).GetComponent<cExplosionEffect>();
        effect.m_fMaxTime = 1.0f;

        Vector3 pos = this.transform.position;
        pos.y = 1.5f;

        effect.InitPosition(pos);
        effect.gameObject.SetActive(true);

        InitExpOrb();
        cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            cBaseSkill skill = collision.gameObject.GetComponent<cBaseSkill>();

            if(skill.m_isPass)
            {
                skill.CollisionMonster(collision);
                collision.collider.isTrigger = true;
                skill.m_oRigidbody3d.velocity = Vector3.zero;
                skill.m_oRigidbody3d.AddForce(skill.m_vecDirection * 12, ForceMode.Impulse);
            }
            else
            {
                skill.CollisionMonster(collision);
            }

            if(m_eState != eMonsterState.DIE)
            {
                if (skill.m_isFireShot)
                {
                    m_nFireCount = 0;
                    m_isFireState = true;
                    FireEffect();
                }
                else
                {
                    m_isFireState = false;
                }

                CollisionPlayerBullet(collision);

                if (this.m_nHp <= 0)
                {
                    MonsterDieStand();
                }
                else
                {
                    m_eState = eMonsterState.GET_HIT;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            cBaseSkill skill = other.gameObject.GetComponent<cBaseSkill>();

            if (skill.m_isPass)
            {
                other.isTrigger = false;
            }
        }
    }

    public void MonsterAStarMove()
    {
        m_fAStarTime += Time.deltaTime;
        if (!IsMoveDirect())
        {
            if (m_fAStarTime >= m_fMaxAStarTime)
            {
                m_fAStarTime = 0.0f;
                m_fMaxAStarTime = Random.Range(0.5f, 1.0f);

                
                if(!m_Player.m_isBlocked)
                {
                    Vector3 targetBlockPos = cAStarManager.GetInstance.TargetBlockPos(m_tTarget.position);
                    Vector3 MonsterPos = cAStarManager.GetInstance.TargetBlockPos(this.transform.position);

                    if (m_OldtargetBlockPos != targetBlockPos &&
                        targetBlockPos != MonsterPos)
                    {
                        m_OldtargetBlockPos = targetBlockPos;
                        m_vecRoute.Clear();

                        cAStarManager.GetInstance.AStarRoute(this.transform.position, m_tTarget.position, ref m_vecRoute);

                        m_nStartIndex = 0;
                        if (m_vecRoute.Count > 1)
                        {
                            m_nStartIndex = 1;
                        }
                    }
                }
            }

            AStarMove(m_vecRoute);
        }
        else
        {
            m_vecRoute.Clear();
            this.transform.position += m_vDirection * m_fSpeed * Time.deltaTime;
        }
    }

    void AStarMove(List<Vector3> vecRoute)
    {
        if (vecRoute.Count > 0)
        {
            Vector3 dir = (vecRoute[m_nStartIndex] - this.transform.position).normalized;


            this.transform.LookAt(this.transform.position + dir);

            this.transform.position += dir * m_fSpeed * Time.deltaTime;

            if (Vector3.Distance(this.transform.position, vecRoute[m_nStartIndex]) < 0.2f)
            {
                m_nStartIndex++;
            }

        }
    }

    bool IsMoveDirect()
    {
        RaycastHit hit;
        this.transform.LookAt(m_tTarget);
        Vector3 pos = this.transform.position;
        pos.y += 0.5f;
        Vector3 targetPos = m_tTarget.transform.position;
        targetPos.y += 0.5f;
        Vector3 dir = (targetPos - pos);

        if (Physics.Raycast(pos, dir, out hit, 30.0F))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("MapWall") ||
                hit.collider.gameObject.layer == LayerMask.NameToLayer("MapObstacle"))
            {
                return false;
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return true;
    }

    public void InitPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }

    void FireShotHit()
    {
        int randDamage = Random.Range((int)(m_nMaxHp * 0.01f), (int)(m_nMaxHp * 0.02f));
        if(randDamage > 200)
        {
            randDamage = Random.Range(100, 200);
        }
        randDamage += Random.Range(2, 7);
        m_nHp -= randDamage;

        DamageTextSetting(randDamage, false, true);
    }

    void FireEffect()
    {
        if(transform.Find("FireShotEffect(Clone)") == null)
        {
            cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.FireShotEffect]).GetComponent<cExplosionEffect>();
            effect.m_fMaxTime = 3.0f;
            effect.InitParentPosition(this.transform);
            effect.InitLocalPosition(new Vector3(0, 1, 0));
            effect.gameObject.SetActive(true);
        }
    }

    protected virtual void CollisionPlayerBullet(Collision collision)
    {
        int damage = collision.gameObject.GetComponent<cBaseSkill>().m_nDamage;

        int randDamage = Random.Range((int)(damage - (damage * 0.1f)), (int)(damage + (damage * 0.1f)));

        int cri = Random.Range(1, 100);

        bool isCritical = false;

        if(cri < cActorManager.GetInstance.m_Player.m_nCritical)
        {
            isCritical = true;
        }

        if (isCritical)
        {
            randDamage *= 2;
        }

        m_nHp -= randDamage;
        DamageTextSetting(randDamage, isCritical, false);

    }

    protected void DamageTextSetting(int damage, bool critical, bool isFireShot)
    {
        GameObject damageObject = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.DamageText]);

        cDamage damageSet = damageObject.GetComponent<cDamage>();
        Vector3 randpos = this.transform.position;

        randpos.x = Random.Range(randpos.x - 1.0f, randpos.x + 1.0f);
        randpos.z = Random.Range(randpos.z - 1.0f, randpos.z + 1.0f);

        damageSet.transform.SetParent(cActorManager.GetInstance.gameObject.transform);
        damageSet.Init(randpos, damage, critical, isFireShot);
        damageSet.gameObject.SetActive(true);
    }


    protected virtual void AnimationSetting()
    {

    }

    public void InitExpOrb()
    {
        Vector3 startPosition = this.transform.position;
        startPosition.y = 1.5f;
        Vector3 destPosition = this.transform.position;

        for (int i = 0; i < 5; ++i)
        {
            destPosition.x = Random.Range(this.transform.position.x - 2.0f, this.transform.position.x + 2.0f);
            destPosition.z = Random.Range(this.transform.position.z - 2.0f, this.transform.position.z + 2.0f);

            cExpOrb orb = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.ExpOrb]).GetComponent<cExpOrb>();
            orb.Initialization(startPosition, destPosition);
            orb.InitExp((int)(m_nExp / 5.0f));
            orb.gameObject.SetActive(true);
        }
    }

    protected bool IsLineCross(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 v0 = new Vector3(p0.x, p0.z, 0);
        Vector3 v1 = new Vector3(p1.x, p1.z, 0);
        Vector3 v2 = new Vector3(p2.x, p2.z, 0);
        Vector3 v3 = new Vector3(p3.x, p3.z, 0);

        Vector3 v01 = v1 - v0;
        Vector3 v23 = v3 - v2;

        Vector3 v02 = v2 - v0;
        Vector3 v03 = v3 - v0;

        Vector3 v21 = v1 - v2;
        Vector3 v20 = v0 - v2;

        Vector3 vCross0102, vCross0103, vCross2321, vCross2320;

        vCross0102 = Vector3.Cross(v01, v02);
        vCross0103 = Vector3.Cross(v01, v03);

        vCross2321 = Vector3.Cross(v23, v21);
        vCross2320 = Vector3.Cross(v23, v20);

        if (vCross0102.z * vCross0103.z < 0 &&
            vCross2321.z * vCross2320.z < 0)
        {
            return true;
        }

        return false;
    }

    protected bool IsLineToRect(Vector3 v0, Vector3 v1, Rect rect)
    {

        Vector3 vLT = new Vector3(rect.xMin, 0, rect.yMin);
        Vector3 vRT = new Vector3(rect.xMax, 0, rect.yMin);
        Vector3 vLB = new Vector3(rect.xMin, 0, rect.yMax);
        Vector3 vRB = new Vector3(rect.xMax, 0, rect.yMax);

        if (IsLineCross(v0, v1, vRT, vLT))
        {
            return true;
        }
        if (IsLineCross(v0, v1, vLB, vRB))
        {
            return true;
        }
        if (IsLineCross(v0, v1, vLT, vLB))
        {
            return true;
        }
        if (IsLineCross(v0, v1, vRB, vRT))
        {
            return true;
        }

        return false;
    }
}
