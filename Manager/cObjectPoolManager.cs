using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class cObjectPoolManager : cSingleTon<cObjectPoolManager>
{
	[System.Serializable]
	public class ObjectPrefabPair
	{
		public Type key;
		public GameObject value;
	}
	public enum Type
	{
		// 몬스터
		RedCyclopes,
		Chicken,
		Bat,
		Turtle,
		Bomb,
		Lich,
		Chest,
		Golem,
		MetalonRed,
		MetalonGreen,
		MetalonPurple,
		Dragon,

		// 총알
		BombFire,
		LichBullet,
		LichExplosion,
		GolemBullet,
		GolemBulletExplosion,
		GolemRock,
		GolemRockExplosion,
		GolemExplosion,
		MetalonBullet,
		MetalonExplosion,
		DragonBreath,
		DragonBreathExplosion,
		DragonFireBall,
		DragonFireExplosion,
		DragonPortal,
		DragonMeteor,
		DragonMeteorExplosion,
		DragonMeteorAttackZone,
		DragonShadowBullet,
		DragonShadowExplosion,

		// 캐릭터
		AssassinBlink,
		MagicianLightning,
		prefabFireBall,
		prefabExplosion,
		LightningBullet,
		LightningExplosion,
		FireShotEffect,

		// 맵
		ForestGround1,
		ForestGround2,
		ForestBridge,
		Castle,
		CastleBack,
		CastleWall,
		ForestRiver,
		Wall,
		Trap,
		DesertGround1,
		DesertGround2,
		DesertBridge,
		DesertRiver,
		DungeonGround1,
		DungeonGround2,
		DungeonBridge,
		DungeonRiver,

		// 기타
		AttackZone,
		MonsterHpBar,
		DamageText,
		MonsterDeath,
		ExpOrb,
		ExpOrbExplosion,
		HealOrb,
		HealExplosion,
	}

	public GameObject m_AllPoolObjects;
	public List<ObjectPrefabPair> m_PrefabList;
	public Dictionary<Type, List<GameObject>> m_ObjectDic;

	Dictionary<char, int> blockCountDic = new Dictionary<char, int>();
	Dictionary<char, int> monsterCountDic = new Dictionary<char, int>();
	int maxMonsterCount = 0;

	protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        LoadMapList();
        
        m_AllPoolObjects = new GameObject("AllPoolObjects");
		m_ObjectDic = new Dictionary<Type, List<GameObject>>();

		ObjectCreate(Type.RedCyclopes, monsterCountDic['R']);
        ObjectCreate(Type.Chicken, monsterCountDic['C']);
        ObjectCreate(Type.Bat, monsterCountDic['B']);
        ObjectCreate(Type.Turtle, monsterCountDic['T']);
        ObjectCreate(Type.Bomb, monsterCountDic['R']);
        ObjectCreate(Type.BombFire, 10);
        ObjectCreate(Type.Lich, monsterCountDic['H']);
        ObjectCreate(Type.LichBullet, 30);
        ObjectCreate(Type.LichExplosion, 30);
        ObjectCreate(Type.Chest, monsterCountDic['E']);
        ObjectCreate(Type.Golem, monsterCountDic['G']);
        ObjectCreate(Type.GolemBullet, 10);
        ObjectCreate(Type.GolemBulletExplosion, 10);
        ObjectCreate(Type.GolemRock, 8);
        ObjectCreate(Type.GolemRockExplosion, 8);
        ObjectCreate(Type.GolemExplosion, 3);
        ObjectCreate(Type.MetalonRed, monsterCountDic['L']);
        ObjectCreate(Type.MetalonGreen, monsterCountDic['K']);
        ObjectCreate(Type.MetalonPurple, monsterCountDic['J']);
        ObjectCreate(Type.MetalonBullet, 10);
        ObjectCreate(Type.MetalonExplosion, 10);
        ObjectCreate(Type.Dragon, monsterCountDic['D']);
        ObjectCreate(Type.DragonBreath, 5);
        ObjectCreate(Type.DragonBreathExplosion, 10);
        ObjectCreate(Type.DragonFireBall, 10);
        ObjectCreate(Type.DragonFireExplosion, 10);
        ObjectCreate(Type.DragonPortal, 10);
        ObjectCreate(Type.DragonMeteor, 2);
        ObjectCreate(Type.DragonMeteorExplosion, 2);
        ObjectCreate(Type.DragonShadowBullet, 50);
        ObjectCreate(Type.DragonShadowExplosion, 50);
        ObjectCreate(Type.DragonMeteorAttackZone, 3);

        ObjectCreate(Type.AttackZone, 10);
        ObjectCreate(Type.MonsterHpBar, maxMonsterCount);
        ObjectCreate(Type.ExpOrb, 150);
        ObjectCreate(Type.ExpOrbExplosion, 150);
        ObjectCreate(Type.MonsterDeath, maxMonsterCount);
        ObjectCreate(Type.HealOrb, 3);
        ObjectCreate(Type.HealExplosion, 3);
        ObjectCreate(Type.DamageText, 120);
		ObjectCreate(Type.FireShotEffect, 20);

        ObjectCreate(Type.ForestGround1, blockCountDic['g']);
        ObjectCreate(Type.DesertGround1, blockCountDic['g']);
        ObjectCreate(Type.DungeonGround1, blockCountDic['g']);
        ObjectCreate(Type.ForestGround2, blockCountDic['G']);
        ObjectCreate(Type.DesertGround2, blockCountDic['G']);
        ObjectCreate(Type.DungeonGround2, blockCountDic['G']);
        ObjectCreate(Type.ForestBridge, blockCountDic['B']);
        ObjectCreate(Type.DesertBridge, blockCountDic['B']);
        ObjectCreate(Type.DungeonBridge, blockCountDic['B']);
        ObjectCreate(Type.Castle, blockCountDic['C']);
        ObjectCreate(Type.CastleBack, blockCountDic['c']);
        ObjectCreate(Type.Wall, blockCountDic['w']);
        ObjectCreate(Type.CastleWall, blockCountDic['W']);
        ObjectCreate(Type.ForestRiver, blockCountDic['R']);
        ObjectCreate(Type.DesertRiver, blockCountDic['R']);
        ObjectCreate(Type.DungeonRiver, blockCountDic['R']);
        ObjectCreate(Type.Trap, blockCountDic['T']);

		cCharacterSelectInfo characterInfo = GameObject.Find("CharacterSelectInfo").GetComponent<cCharacterSelectInfo>();
		string fireBallPath = "";
		string explosionPath = "";
		switch(characterInfo.m_eCharacterInfo)
		{
		case cCharacterSelectInfo.eCharacterInfo.ASSASSIN: // 선택한 캐릭터가 도적일 시
			fireBallPath = "MyGame/Prefab/BulletPrefab/AssassinBullet";
			explosionPath = "MyGame/Prefab/BulletPrefab/AssassinExplosion";
			ObjectCreate(Type.AssassinBlink, 5);
			break;
		case cCharacterSelectInfo.eCharacterInfo.WARRIOR: // 선택한 캐릭터가 전사일 시
			fireBallPath = "MyGame/Prefab/BulletPrefab/WarriorBullet";
			explosionPath = "MyGame/Prefab/BulletPrefab/WarriorExplosion";
			break;
		case cCharacterSelectInfo.eCharacterInfo.MAGICIAN: // 선택한 캐릭터가 마법사일 시
			fireBallPath = "MyGame/Prefab/BulletPrefab/MagicianBullet";
			explosionPath = "MyGame/Prefab/BulletPrefab/MagicianExplosion";

			ObjectCreate(Type.MagicianLightning, 5);
			ObjectCreate(Type.LightningBullet, 20);
			ObjectCreate(Type.LightningExplosion, 20);
			break;
		}

		ObjectCreate(Type.prefabFireBall, 200, fireBallPath);
		ObjectCreate(Type.prefabExplosion, 200, explosionPath);

	}
	void LoadMapList()
    {
        blockCountDic = new Dictionary<char, int>();
        monsterCountDic = new Dictionary<char, int>();
        
        int mapNum = 0;
        while(true)
        {
            string strMap = "MyGame/Table/Map/map" + mapNum;
            string strMonster = "MyGame/Table/Map/map" + mapNum + "A";
            bool isSetMap = SetCountDic(strMap, ref blockCountDic);
            bool isSetMonster = SetCountDic(strMonster, ref monsterCountDic, new List<char> { 'P', 'X' });
            
            if(!isSetMap || !isSetMonster)
            	break;
            
            mapNum++;
        }
        
        maxMonsterCount = monsterCountDic.Max(x => x.Value);
    }
    private bool SetCountDic(string str, ref Dictionary<char, int> refCountDic, List<char> passCharList = null)
    {
        TextAsset asset = Resources.Load<TextAsset>(str);
        if(asset == null)
        	return false;
        
        StringReader streader = new StringReader(asset.text);
        string line;
        Dictionary<char, int> countDic = new Dictionary<char, int>();
        while((line = streader.ReadLine()) != null)
        {
            for(int i = 0; i < line.Length; ++i)
            {
                if(passCharList != null && passCharList.Contains(line[i]))
                    continue;
                
                if(countDic.ContainsKey(line[i]))
                    countDic[line[i]] += 1;
                else
                    countDic.Add(line[i], 1);
            }
        }
        
        foreach(var dic in countDic)
        {
            if(refCountDic.ContainsKey(dic.Key))
            {
                if(refCountDic[dic.Key] < dic.Value)
                   refCountDic[dic.Key] = dic.Value;
            }
            else
                refCountDic.Add(dic.Key, dic.Value);
        }
        return true;
    }

    public GameObject GetObject(List<GameObject> gList)
    {
        for (int i = 0; i < gList.Count; ++i)
        {
            //활성화가 안되어있을시  = 비활성화시에만..
            if (gList[i].activeInHierarchy == false)
            {
                //가져온다..
                return gList[i];
            }
        }
        return null;
    }



    public void ObjectCreate(Type type, int count, string path = "")
    {
        List<GameObject> gList = new List<GameObject>();
        ObjectPrefabPair pair = m_PrefabList.Find(x => x.key == type);
        if(pair == null)
           return;

		GameObject gObject = null;
		if(path == "")
			gObject = pair.value;
		else
			gObject = Resources.Load<GameObject>(path);

		for (int i=0; i < count; ++i)
        {
            GameObject obj = Instantiate(gObject);
            obj.SetActive(false);
            obj.transform.SetParent(m_AllPoolObjects.transform);
            gList.Add(obj);
        }

        m_ObjectDic.Add(type, gList);
    }

    public void SetActiveFalse(GameObject obj)
    {
        obj.transform.SetParent(m_AllPoolObjects.transform);
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
    }
}