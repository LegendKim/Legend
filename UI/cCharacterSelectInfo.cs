using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cCharacterSelectInfo : MonoBehaviour
{
    public enum eCharacterInfo
    {
        ASSASSIN,
        WARRIOR,
        MAGICIAN
    }

    public eCharacterInfo m_eCharacterInfo;

    private void Awake()
    {
       
        DontDestroyOnLoad(this.gameObject);
    }


}
