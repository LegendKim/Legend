using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class cMapManager : cSingleTon<cMapManager>
{
    public enum eMapState
    {
        NORMAL,
        ALL_MONSTER_DIE,
        CLEAR
    }

    public enum eMapTheme
    {
        FOREST,
        DESERT,
        SNOW,
        DUNGEON
    }

    public int m_nMapNum;
    public float m_MaxWidth { get; set; }
    public float m_MaxHeight { get; set; }
    public List<cBlock> m_blockList;
    public eMapTheme m_eMapTheme { get; set; }
    public eMapState m_eMapState;
    public float m_fTime;
    public int m_nMapRow;
    public int m_nAllBlockNum;

    protected override void Awake()
    {
        base.Awake();
        m_eMapState = eMapState.NORMAL;
        cObjectPoolManager.GetInstance.Initialize();
        LoadMap(0);
    }

    protected void Update()
    {
        CheckAllMonsterDie();
    }
    void CheckAllMonsterDie()
    {
        if(m_eMapState == eMapState.ALL_MONSTER_DIE)
        {
            m_fTime += Time.deltaTime;
            
            if(m_fTime > 1.0f)
            {
                m_fTime = 0.0f;
                OpenCastleDoor();
            }
        }
    }


	public void AllMonsterDie()
    {
        m_eMapState = eMapState.ALL_MONSTER_DIE;
        if (m_nMapNum % 10 == 9 || m_nMapNum % 10 == 0)
        {
            cSoundManager.GetInstance.StopBGM();
        }
    }

    public void OpenCastleDoor()
    {
        m_eMapState = eMapState.CLEAR;
        cCastle door = GameObject.Find("ForestCastle(Clone)").GetComponent<cCastle>();
        door.OpenDoor();
    }

    public void NextStageSetting()
    {
        m_eMapState = eMapState.NORMAL;
        cUIManager.GetInstance.m_StageNumLabel.SetActive(true);
        NextMapCreate();
    }


    void NextMapCreate()
    {
        ReturnMapBlock();
        LoadMap(m_nMapNum + 1);
	}

	private void LoadMap(int mapNum)
    {
        m_nMapNum = mapNum;
        ChangeTheme();
        PlayMapBGM(m_nMapNum);
        if (m_blockList != null)
            m_blockList.Clear();
    
        m_blockList = new List<cBlock>();
        m_nAllBlockNum = 0;
    
        int row = 0;
        int col = 0;
		string strFinal = "MyGame/Table/Map/map" + m_nMapNum;

		TextAsset asset = Resources.Load<TextAsset>(strFinal);
        if (asset == null)
        {
            Debug.LogError($"Map file not found: {strFinal}");
            return;
        }

        StringReader streader = new StringReader(asset.text);
        string line;
    
        while ((line = streader.ReadLine()) != null)
        {
            col = 0;
            for (int i = 0; i < line.Length; ++i)
            {
                CreateMapBlock(line[i], row, col);
                col++;
                m_nAllBlockNum++;
            }
            row++;
        }

        m_nMapRow = row;
        m_MaxWidth = col;
        m_MaxHeight = row;

        cAStarManager.GetInstance.AStarSetting();
    }

	private void PlayMapBGM(int mapNum)
    {
        var sound = cSoundManager.GetInstance;
    
        if (mapNum == 1)
            sound.PlayBGM(sound.m_ForestBGM);
        else if (mapNum == 10)
            sound.PlayBGM(sound.m_ForestBossBGM);
        else if (mapNum == 11)
            sound.PlayBGM(sound.m_DesertBGM);
        else if (mapNum == 20)
            sound.PlayBGM(sound.m_DesertBossBGM);
        else if (mapNum == 21)
            sound.PlayBGM(sound.m_DungeonBGM);
        else if (mapNum == 30)
            sound.PlayBGM(sound.m_DungeonBossBGM);
    }

    void CreateMapBlock(char c, int row, int col)
    {
		cObjectPoolManager pool = cObjectPoolManager.GetInstance;

		switch(m_eMapTheme)
        {
            case eMapTheme.FOREST:
                CreateBlock(c, row, col, pool.m_ObjectDic[cObjectPoolManager.Type.ForestGround1], pool.m_ObjectDic[cObjectPoolManager.Type.ForestGround2], 
                pool.m_ObjectDic[cObjectPoolManager.Type.ForestRiver], pool.m_ObjectDic[cObjectPoolManager.Type.ForestBridge]);
                break;
            case eMapTheme.DESERT:
                CreateBlock(c, row, col, pool.m_ObjectDic[cObjectPoolManager.Type.DesertGround1], pool.m_ObjectDic[cObjectPoolManager.Type.DesertGround2], 
                pool.m_ObjectDic[cObjectPoolManager.Type.DesertRiver], pool.m_ObjectDic[cObjectPoolManager.Type.DesertBridge]);
                break;
            case eMapTheme.DUNGEON:
                CreateBlock(c, row, col, pool.m_ObjectDic[cObjectPoolManager.Type.DungeonGround1], pool.m_ObjectDic[cObjectPoolManager.Type.DungeonGround2], 
                pool.m_ObjectDic[cObjectPoolManager.Type.DungeonRiver], pool.m_ObjectDic[cObjectPoolManager.Type.DungeonBridge]);
                break;
        }
    }

    public void ReturnMapBlock()
    {
        for (int i = 0; i < m_blockList.Count; ++i)
        {
            m_blockList[i].gameObject.SetActive(false);
        }
    }

    public void CreateBlock(char c, int row, int col, List<GameObject> ground1List, List<GameObject> ground2List,
    	List<GameObject> riverList, List<GameObject> bridgeList)
    {
        Vector3 position = new Vector3(0, 0, 0);
        
        GameObject block = null;
        cObjectPoolManager pool = cObjectPoolManager.GetInstance;

        switch(c)
        {
        case 'g':
            block = pool.GetObject(ground1List);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.GROUND;
            
            position = new Vector3(col, 0, row);
            break;
        case 'G':
            block = pool.GetObject(ground2List);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.GROUND;
            position = new Vector3(col, 0, row);
            break;
        case 'R':
            block = pool.GetObject(riverList);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.RIVER;
            position = new Vector3(col, 0, row);
            break;
        case 'B':
            block = pool.GetObject(bridgeList);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.BRIDGE;
            position = new Vector3(col, block.gameObject.transform.localScale.y, row);
            break;
        case 'w':
            block = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Wall]);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.WALL;
            position = new Vector3(col, block.gameObject.transform.localScale.y / 2.0f, row);
            break;
        case 'W':
            block = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.CastleWall]);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.WALL;
            position = new Vector3(col, block.gameObject.transform.localScale.y / 2.0f, row);
            break;
        case 'C':
            block = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Castle]);
            position = new Vector3(col, block.gameObject.transform.localScale.y, row);
            break;
        case 'c':
            block = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.CastleBack]);
            position = new Vector3(col, block.gameObject.transform.localScale.y, row);
            break;
        case 'T':
            block = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.Trap]);
            block.GetComponent<cBlock>().m_eBlockType = cBlock.eBlockType.TRAP;
            position = new Vector3(col, block.gameObject.transform.localScale.y, row);
            break;
        }
        
        if(block != null)
        {
        	block.GetComponent<cBlock>().Initialization(position);
        	block.GetComponent<cBlock>().m_nCol = col;
        	block.GetComponent<cBlock>().m_nRow = row;
        	block.GetComponent<cBlock>().m_blockNum = m_nAllBlockNum;
        	block.gameObject.SetActive(true);
        	m_blockList.Add(block.GetComponent<cBlock>());
        }
    }

    public void ChangeTheme()
    {
        if(m_nMapNum <= 10)
        {
            m_eMapTheme = eMapTheme.FOREST;
        }
        else if(m_nMapNum > 10 && m_nMapNum <= 20)
        {
            m_eMapTheme = eMapTheme.DESERT;
        }
        else if(m_nMapNum > 20)
        {
            m_eMapTheme = eMapTheme.DUNGEON;
        }
    }
}