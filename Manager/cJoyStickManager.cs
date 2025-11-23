using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cJoyStickManager : cSingleTon<cJoyStickManager>
{
    public GameObject smallStick;
    public GameObject bGStick;
    Vector3 m_InitPosition;
    Vector3 stickFirstPosition;
    public Vector3 joyVec;
    float stickRadius;
    public Camera m_UICamera;
    cPlayer m_Player;
    private bool m_isMove;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
        stickRadius = (bGStick.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2) * 0.005f;

        Vector3 pos = new Vector3(530,350,0);
        pos.z = 100.0f;
        pos = m_UICamera.ScreenToWorldPoint(pos);

        m_InitPosition = pos;
        stickFirstPosition = bGStick.transform.position;
        m_isMove = false;
    }


    public void PointDown()
    {
        Vector3 screenPoint = new Vector3(0,0,0);

        screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f);
        bGStick.transform.position = m_UICamera.ScreenToWorldPoint(screenPoint);
        smallStick.transform.position = m_UICamera.ScreenToWorldPoint(screenPoint);
        stickFirstPosition = m_UICamera.ScreenToWorldPoint(screenPoint);
        m_isMove = true;
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        Vector3 DragPosition = pointerEventData.position;
        DragPosition.z = 100.0f;
        DragPosition = m_UICamera.ScreenToWorldPoint(DragPosition);

        joyVec = (DragPosition - stickFirstPosition).normalized;

        float stickDistance = Vector3.Distance(DragPosition, stickFirstPosition);

        if(stickDistance < stickRadius)
        {
            smallStick.transform.position = stickFirstPosition + joyVec * stickDistance;
        }
        else
        {
            smallStick.transform.position = stickFirstPosition + joyVec * stickRadius;
        }

        if ((!cUIManager.GetInstance.m_isPauseGame || !cUIManager.GetInstance.m_isPlayerLevelUp)  && 
            !m_Player.m_isDie)
        {
            if (!m_Player.m_isKeyDown)
            {
                if (stickDistance > 1.0f)
                {
                    stickDistance = 1.0f;
                }

                m_Player.m_fSpeed = stickDistance * 3.0f;

                if (m_Player.m_fSpeed > 2.0f)
                {
                    m_Player.m_ePlayerState = ePlayerState.RUN;
                }
                else
                {
                    m_Player.m_ePlayerState = ePlayerState.WALK;
                }

                Vector3 vec = new Vector3(0,0,0);
                vec.x = joyVec.x;
                vec.y = 0;
                vec.z = joyVec.y;

                m_Player.m_vecDir = vec;
                m_Player.transform.eulerAngles = new Vector3(0, Mathf.Atan2(joyVec.x, joyVec.y) * Mathf.Rad2Deg, 0);
            }
        }
    }

    public void Drop()
    {
        if(!m_Player.m_isDie)
        {
            m_Player.m_ePlayerState = ePlayerState.IDLE;
        }
       
        joyVec = Vector3.zero;
        bGStick.transform.position = m_InitPosition;
        smallStick.transform.position = m_InitPosition;
        m_isMove = false;
    }

    void Update()
    {
        if(cUIManager.GetInstance.m_isPauseGame || cUIManager.GetInstance.m_isPlayerLevelUp)
        {
            bGStick.SetActive(false);
            smallStick.SetActive(false);
        }
        else
        {
            bGStick.SetActive(true);
            smallStick.SetActive(true);
            if (m_isMove && !m_Player.m_isDie)
            {
                m_Player.transform.Translate(Vector3.forward * m_Player.m_fSpeed * Time.deltaTime);
            }
        }
    }
}
