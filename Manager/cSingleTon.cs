using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class cSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;

    public static T GetInstance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }

            if (m_Instance == null)
            {
                GameObject obj = new GameObject();
                obj.AddComponent(typeof(T));
                obj.name = typeof(T).ToString();
            }

            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        m_Instance = typeof(T) as T;
    }
}
