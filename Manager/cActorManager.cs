using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class cMonsterData
{
    public enum Type
    {
		Type,
        HP,
        Damage,
        Exp,
        Speed
    }

    public eMonsterType m_type;
    public int m_nHp;
    public int m_nDamage;
    public int m_nExp;
    public float m_fSpeed;
}

public class cActorManager : cSingleTon<cActorManager>
{
    public cPlayer m_Player;

    public List<cMonster> m_cMonsterList;

    public List<GameObject> m_MonsterHpList;

    public float m_fGamePassedTime;

    public cHPImage m_BossHUD;

    public bool m_isBossStage;

    public Camera m_HpCamera;

    public Dictionary<eMonsterType, cMonsterData> m_monsterDataDic;

    protected override void Awake()
    {
        base.Awake();
        LoadMonsterData();
	}

	void Start()
    {
        // 플레이어와 몬스터들 파일 읽어서 위치 조정
        cCharacterSelectInfo characterInfo = GameObject.Find("CharacterSelectInfo").GetComponent<cCharacterSelectInfo>();
        GameObject obj = null;

        switch (characterInfo.m_eCharacterInfo)
        {
            case cCharacterSelectInfo.eCharacterInfo.ASSASSIN:
                obj = GameObject.Instantiate(Resources.Load<GameObject>("MyGame/Prefab/CharacterPrefab/Assassin"));
                obj.SetActive(true);
                break;
            case cCharacterSelectInfo.eCharacterInfo.WARRIOR:
                obj = GameObject.Instantiate(Resources.Load<GameObject>("MyGame/Prefab/CharacterPrefab/Warrior"));
                obj.SetActive(true);
                break;
            case cCharacterSelectInfo.eCharacterInfo.MAGICIAN:
                obj = GameObject.Instantiate(Resources.Load<GameObject>("MyGame/Prefab/CharacterPrefab/Magician"));
                obj.SetActive(true);
                break;

        }

        m_Player = obj.GetComponent<cPlayer>();
        ActorPositionSetting();
    }

    void Update()
    {
        if (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.NORMAL)
        {
            if (IsAllMonsterDie())
            {
                cMapManager.GetInstance.AllMonsterDie();
            }
            m_fGamePassedTime += Time.deltaTime;
        }

        if(!m_isBossStage)
        {
            for (int i = 0; i < m_cMonsterList.Count; ++i)
            {
                if (m_cMonsterList[i].gameObject.activeInHierarchy)
                {
                    m_MonsterHpList[i].SetActive(true);
                }
                else
                {
                    m_MonsterHpList[i].SetActive(false);
                }
            }

            for (int i = 0; i < m_MonsterHpList.Count; ++i)
            {
                Vector3 monPos = Camera.main.WorldToScreenPoint(m_cMonsterList[i].transform.position);
                monPos.z = 100.0f;
                Vector3 pos = m_HpCamera.ScreenToWorldPoint(monPos);
                pos.y += 8.0f;
                m_MonsterHpList[i].transform.position = pos;
            }
        }
        else
        {
            for (int i = 0; i < m_cMonsterList.Count; ++i)
            {
                m_MonsterHpList[i].SetActive(false);
            }
        }

        if(m_Player.m_nExp >= m_Player.m_nMaxExp)
        {

            PlayerLevelUp();
        }

    }

    private void LateUpdate()
    {
        if(!m_isBossStage)
        {
            for (int i = 0; i < m_MonsterHpList.Count; ++i)
            {
                cHPImage hpBarImage = m_MonsterHpList[i].GetComponentInChildren<cHPImage>();
                hpBarImage.SettingFillImage((float)m_cMonsterList[i].m_nHp, (float)m_cMonsterList[i].m_nMaxHp);
            }
        }
        else
        {
            int allMaxHp = 0;
            int allHp = 0;
            for (int i = 0; i < m_cMonsterList.Count; ++i)
            {
                allHp += m_cMonsterList[i].m_nHp;
                allMaxHp += m_cMonsterList[i].m_nMaxHp; 
            }
            m_BossHUD.SettingFillImage((float)allHp, (float)allMaxHp);
        }
    }
    void LoadMonsterData()
    {
        m_monsterDataDic = new Dictionary<eMonsterType, cMonsterData>();
        string strFinal = "MyGame/Table/Monster/MonsterData";
        
        TextAsset asset = Resources.Load<TextAsset>(strFinal);
        if(asset == null)
        {
        	Debug.LogError($"file not found: {strFinal}");
        	return;
        }
        
        StringReader streader = new StringReader(asset.text);
        string line;
        
        while((line = streader.ReadLine()) != null)
        {
            string[] datas = line.Split(',');
            
            cMonsterData data = new cMonsterData();
            data.m_type = (eMonsterType)int.Parse(datas[(int)cMonsterData.Type.Type]);
            data.m_nHp = int.Parse(datas[(int)cMonsterData.Type.HP]);
            data.m_nDamage = int.Parse(datas[(int)cMonsterData.Type.Damage]);
            data.m_nExp = int.Parse(datas[(int)cMonsterData.Type.Exp]);
            data.m_fSpeed = float.Parse(datas[(int)cMonsterData.Type.Speed]);
            m_monsterDataDic.Add(data.m_type, data);
        }
    }

    void PlayerLevelUp()
    {
        m_Player.m_nLevel++;
        m_Player.m_nExp -= m_Player.m_nMaxExp;
        m_Player.m_nMaxExp = (int)(m_Player.m_nMaxExp * 1.3f);

        cUIManager.GetInstance.CreateAbilitySelectPopup();
    }

    void SettingActor(char c, int row, int col)
    {
        Vector3 position = new Vector3(col, 0.5f, row);

        GameObject monster = null;
        GameObject monsterHp = null;
		cObjectPoolManager pool = cObjectPoolManager.GetInstance;

		switch (c)
        {
            case 'P':
                m_Player.gameObject.transform.position = position;
                break;
            case 'R':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.RedCyclopes]);
                break;
            case 'T':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Turtle]);
                break;
            case 'C':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Chicken]);
                break;
            case 'M':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Bomb]);
                break;
            case 'B':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Bat]);
                break;
            case 'H':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Lich]);
                break;
            case 'E':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Chest]);
                position = new Vector3(col, 1.0f, row);
                break;
            case 'G':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Golem]);
                break;
            case 'J':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.MetalonPurple]);
                break;
            case 'K':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.MetalonGreen]);
                break;
            case 'L':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.MetalonRed]);
                break;
            case 'D':
                monster = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Dragon]);
                break;
        }

        if (monster != null)
        {
            monster.GetComponent<cMonster>().Start();
            monster.GetComponent<cMonster>().InitPosition(position);
            monster.SetActive(true);
            m_cMonsterList.Add(monster.GetComponent<cMonster>());

            monsterHp = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.MonsterHpBar]);
            monsterHp.SetActive(true);
            monsterHp.transform.parent = this.transform;

            m_MonsterHpList.Add(monsterHp);
        }
    }

    public void ActorPositionSetting()
    {
        if (m_cMonsterList != null)
        {
            ReturnMonster();
            m_cMonsterList.Clear();
            m_MonsterHpList.Clear();
        }
        else
        {
            m_cMonsterList = new List<cMonster>();
            m_MonsterHpList = new List<GameObject>();
        }

        int col = 0;
        int row = 0;

        string strStart1 = "MyGame/Table/Map/map";
        string strEnd1 = "A";
        string strFinal = strStart1 + cMapManager.GetInstance.m_nMapNum + strEnd1;
        TextAsset asset = Resources.Load<TextAsset>(strFinal);

        StringReader sr = new StringReader(asset.text);
        string strLine = asset.text;
        StringReader streader = new StringReader(strLine);

        while ((strLine = streader.ReadLine()) != null)
        {
            col = 0;

            int num = strLine.Length;

            for (int i = 0; i < strLine.Length; ++i)
            {
                char c = strLine[i];
                SettingActor(strLine[i], row, col);
                col++;
            }
            row++;
        }

        if (cMapManager.GetInstance.m_nMapNum % 10 == 0)
        {
            m_isBossStage = true;
            m_BossHUD.gameObject.SetActive(true);
        }
        else
        {
            m_isBossStage = false;
            m_BossHUD.gameObject.SetActive(false);
        }
    }


    private bool IsAllMonsterDie()
    {

        for(int i=0; i < m_cMonsterList.Count; ++i)
        {
            if(m_cMonsterList[i].gameObject.activeInHierarchy)
            {
                return false;
            }

        }

        m_BossHUD.gameObject.SetActive(false);
        return true;
    }

    public void ReturnMonster()
    {
        for(int i = 0; i < m_MonsterHpList.Count; ++i)
        {
            cObjectPoolManager.GetInstance.SetActiveFalse(m_MonsterHpList[i]);
        }
    }

}