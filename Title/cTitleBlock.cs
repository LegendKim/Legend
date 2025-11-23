using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cTitleBlock : MonoBehaviour
{
    private bool m_isFloat;
    private float m_fTime;
    private float m_fMaxTime;

    // Start is called before the first frame update
    void Start()
    {
        m_fTime = 0.0f;
        m_fMaxTime = Random.Range(0.5f, 1.5f);
        m_isFloat = true;
    }

    private void FixedUpdate()
    {
        m_fTime += Time.deltaTime;

        if (m_fTime > m_fMaxTime)
        {
            m_fTime = 0.0f;
            m_fMaxTime = Random.Range(0.5f, 1.5f);
            if (m_isFloat)
            {
                m_isFloat = false;
            }
            else
            {
                m_isFloat = true;
            }
        }

        if (m_isFloat)
        {
            this.transform.Translate(0, 0.005f, 0);
        }
        else
        {
            this.transform.Translate(0, -0.005f, 0);
        }

    }
}
