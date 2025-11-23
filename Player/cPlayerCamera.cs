using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPlayerCamera : MonoBehaviour
{
    public cPlayer m_cPlayer;
    public Transform m_me;
    public Vector3 m_offSet;
    public Vector3 m_direction;
    public bool m_isMove;

    private void Start()
    {
        m_offSet = new Vector3(0, 14.0f, -5f);
        m_isMove = true;
        m_cPlayer = GameObject.FindWithTag("Player")
            .GetComponent<cPlayer>();

        transform.position = Vector3.Lerp(m_cPlayer.transform.position + m_offSet, transform.position, 2.0f * Time.deltaTime);
    }


    void Update()
    {
        transform.position = Vector3.Lerp(m_cPlayer.transform.position + m_offSet, transform.position, 2.0f * Time.deltaTime);
        transform.LookAt(m_cPlayer.transform.position);
    }
}