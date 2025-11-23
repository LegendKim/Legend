using UnityEngine;
using System.Collections;

public abstract class cBaseSkill : MonoBehaviour
{
    public enum eCollisionWallType
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }


    public Vector3 m_vecDirection;
    public int m_nDamage;
    public float m_fSpeed        { get; set; }
    public bool m_isWallReflect{ get; set; }
    public bool m_isPass;
    public bool m_isFireShot;
    public eCollisionWallType m_eCollisionWallType;
    public float m_fMaxTime;

    public int m_nCollisionCount;

    public float m_fTime = 0;
    public Rigidbody m_oRigidbody3d;
    protected SphereCollider m_sphereCollider;
    protected cObjectPoolManager.Type m_explosionType;
	protected float m_fMaxExplosionTime = 0.5f;

	protected virtual void Awake()
    {
        m_fMaxTime = 3.0f;
        m_oRigidbody3d = this.GetComponent<Rigidbody>();
        m_sphereCollider = this.GetComponent<SphereCollider>();
    }

    protected virtual void OnEnable()
    {
        m_sphereCollider.isTrigger = false;
        Vector3 vec = this.transform.position + m_vecDirection;
        this.transform.LookAt(vec);
        m_nCollisionCount = 0;
        m_vecDirection = m_vecDirection.normalized;
        m_oRigidbody3d.velocity = Vector3.zero;
        m_oRigidbody3d.AddForce(m_vecDirection * m_fSpeed, ForceMode.Impulse);
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void Update()
    {
        //더해주고.
        m_fTime += Time.deltaTime;
        if (m_fTime > m_fMaxTime)
        {
            m_fTime = 0;
            m_isPass = false;
            m_isWallReflect = false;
            m_sphereCollider.isTrigger = false;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }

    public virtual void Initialization(Vector3 startVector, Vector3 direction, bool isReflect, bool isPass, bool isFireShot)
    {
        transform.position = startVector;
        m_vecDirection = direction;
        m_isWallReflect = isReflect;
        m_isPass = isPass;
        m_isFireShot = isFireShot;
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MapWall"))
        {
            if (m_isWallReflect)
            {
                m_nCollisionCount++;
                if (m_nCollisionCount > 2)
                {
                    CollisionWall();
                }
                else
                {
                    WallReflect(collision);
                }
            }
            else
            {
                CollisionWall();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("MapWall"))
        {
            m_sphereCollider.isTrigger = false;
        }
    }

    private void WallReflect(Collision collision)
    {
        // 입사벡터를 알아본다. (충돌할때 충돌한 물체의 입사 벡터 노말값)
        Vector3 incomingVector = m_vecDirection.normalized;
        //충돌한 면의 법선 벡터를 구해낸다.
        Vector3 normalVector = collision.contacts[0].normal;
         Vector3 reflectVector = Vector3.Reflect(incomingVector, normalVector); //반사각

        m_vecDirection = reflectVector.normalized;

        Vector3 vec = this.transform.position + m_vecDirection;
        this.transform.LookAt(vec);
        m_oRigidbody3d.velocity = Vector3.zero;
        m_oRigidbody3d.AddForce(m_vecDirection * m_fSpeed, ForceMode.Impulse);
    }
    public virtual void CollisionWall()
    {
        SetExplosionEffect();

        m_isPass = false;
        m_isWallReflect = false;

        m_fTime = 0.0f;
        m_oRigidbody3d.velocity = Vector3.zero;

        cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
    }


    public virtual void CollisionMonster(Collision collision)
    {
        SetExplosionEffect();

        if(!m_isPass)
        {
            m_isPass = false;
            m_isWallReflect = false;
            m_sphereCollider.isTrigger = false;
            m_fTime = 0.0f;
            m_oRigidbody3d.velocity = Vector3.zero;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }

    void SetExplosionEffect()
    {
        cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[m_explosionType]).GetComponent<cExplosionEffect>();
        effect.m_fMaxTime = m_fMaxExplosionTime;
        effect.InitPosition(this.transform.position);
        effect.gameObject.SetActive(true);
    }
}