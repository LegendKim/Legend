using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class cBlock : MonoBehaviour
{
    public enum eBlockType
    {
        NONE,
        GROUND,
        RIVER,
        BRIDGE,
        WALL,
        TRAP
    };

    public enum  eBlockState
    {
        NONE,
		OPEN,
		CLOSE,
		WALL,
		ROUTE
    };
    

    public float m_fF;
    public float m_fG;
    public float m_fH;
    public int m_blockNum;

    public cBlock m_Parent;
    //public int  m_nParentNum;
    public eBlockType m_eBlockType;
    public int m_nRow;
    public int m_nCol;
    public eBlockState m_eBlockState;

    public int m_nDamage = 50;
    public Rect m_Rect;

    void Start()
    {
        m_eBlockState = eBlockState.NONE;
        m_Parent = null;
        m_fF = 0.0f;
        m_fG = 0.0f;
        m_fH = 0.0f;
    }


    public void Initialization(Vector3 startVector)
    {
        transform.position = startVector;
        if(m_eBlockType == eBlockType.WALL)
        {
            m_Rect = new Rect(startVector.x - 0.5f, startVector.z + 0.5f, 1.0f, 1.0f);
        }
       
    }
}


 




















