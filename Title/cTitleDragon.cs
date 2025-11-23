using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cTitleDragon : MonoBehaviour
{
    public bool m_isBackFly;
    public float m_fTime;
    public float m_fMaxTime;

    public AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = this.GetComponent<AudioSource>();
        this.transform.position = new Vector3(-60, 80, 124);
        this.transform.rotation = Quaternion.Euler(38, 112, -69);
        m_fTime = 0.0f;
        m_fMaxTime = 10.0f;
        m_isBackFly = true;
    }

    private void FixedUpdate()
    {
        m_fTime += Time.deltaTime;

        if (m_fTime > m_fMaxTime)
        {
            m_fTime = 0.0f;
            if (m_isBackFly)
            {
                m_fMaxTime = 5.0f;
                m_isBackFly = false;
                m_AudioSource.gameObject.SetActive(true);
                m_AudioSource.PlayOneShot(m_AudioSource.clip);
                this.transform.position = new Vector3(-12, -12, 10);
                this.transform.rotation = Quaternion.Euler(-44, 81, -66);
            }
            else
            {
                m_fMaxTime = 9.0f;
                m_isBackFly = true;
                this.transform.position = new Vector3(-60, 80, 124);
                this.transform.rotation = Quaternion.Euler(38, 112, -69);

            }
        }

        if (m_isBackFly)
        {
            this.transform.Translate(Vector3.forward * 0.8f, Space.Self);
        }
        else
        {
            this.transform.Translate(Vector3.forward * 1.2f, Space.Self);
        }
    }
}
