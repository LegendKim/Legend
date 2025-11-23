using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cExplosionEffect : MonoBehaviour
{
    public float m_fTime = 0;
    public float m_fMaxTime = 0;
    public Quaternion m_oldQuaternion;

    private void Awake()
    {
        m_fTime = 0;
        m_fMaxTime = 2.0f;
    }

    protected virtual void Start()
    {
        m_oldQuaternion = this.transform.rotation;
    }

    protected virtual void Update()
    {
        m_fTime += Time.deltaTime;
        if (m_fTime > m_fMaxTime)
        {
            m_fTime = 0;
            this.transform.rotation = m_oldQuaternion;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }

    public void InitPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }

    public void InitRotation(Quaternion q)
    {
        this.transform.rotation = q;
    }

    public void InitLocalPosition(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }

    public void InitParentPosition(Transform transform)
    {
        this.transform.SetParent(transform);
    }

}
