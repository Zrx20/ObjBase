

 //*	功能说明：游戏逻辑Obj管理器，对游戏中所有Obj提供创建，移除和管理
 //*	修改记录：
 //*	 修改list结构为Dictionary结构，

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Games.GlobeDefine;
using Games.LogicObj;
//using GCGame.Table;
//using Games.AI_Logic;
//using GCGame;
using System;
//using Games.Animation_Modle;
//using Module.Log;
//using Games.FakeObject;
//using Games.ObjAnimModule;
//using Games.Item;
//using Games.Scene;

//初始化一个Obj所需数据
public class Obj_Init_Data
{
	public float m_fX;           //坐标X
	public float m_fY;           //坐标Y
	public float m_fZ;           //坐标Z
	public float m_fDir;           //朝向
	public int m_RoleBaseID;   //在RoleBase表中的ID
	public int m_CharModelID;  //在CharModel表中ID（NPC可以从RoleBase表读取，但变换外形后会导致和RoleBase中的CharModelID不一致） 
	public int m_ClientInitID; //在ClientInitTable中的ID
	public int m_ServerID;     //服务器ID
	public UInt64 m_Guid;         //Obj在服务器的GUID
	public int m_Force;        //势力
	public string m_StrName;      //Obj的名字
	public int m_nProfession;  //职业 （创建玩家用的）
	public string m_strTitleName; //称号名
	public int m_CurTitleID;   //称号ID
	public bool m_isInMainPlayerPKList;//是否在主角反击列表中 只对非主角玩家有用
	public bool m_IsDie;        //是否死亡
	public int m_PkModel;        //PK模式
	public int m_OwnerObjId;   //主人objid
	public int m_MountID;  // 坐骑ID
	public float m_MoveSpeed;  // 移动速度
	public int m_WeaponDataID;     // 当前武器
	public int m_ModelVisualID;    // 当前模型外观ID
	public int m_WeaponEffectGem;  // 武器特效宝石
	public int m_StealthLev;//隐身级别
	public bool m_bNpcBornCreate;//是否是刚刚刷出来的NPC
	public int m_nOtherVipCost;//vip info
	public UInt64 m_GuildGuid;         //帮会GUID
	public int m_FellowQuality;     //伙伴品质（创建伙伴用）
	public int m_nOtherCombatValue;//战力
	public int m_BindParent;//绑定父节点
	public List<int> m_BindChildren;//绑定子节点
	public bool m_bIsWildEnemyForMainPlayer; //是否与主角敌对

	public int m_nGuildBusinessState;       //帮会跑商状态
	public Obj_Init_Data()
	{
		m_BindChildren = new List<int>();
	}
	public void CleanUp()
	{
		m_fX = 0.0f;
		m_fY = 0.0f;
		m_fZ = 0.0f;
		//m_RoleBaseID = GlobeVar.INVALID_ID;
		//m_CharModelID = GlobeVar.INVALID_ID;
		//m_ServerID = GlobeVar.INVALID_ID;
		//m_Guid = GlobeVar.INVALID_GUID;
		//m_Force = GlobeVar.INVALID_ID;
		//m_ClientInitID = GlobeVar.INVALID_ID;
		m_StrName = "";
		m_nProfession = -1; //职业 （创建玩家用的）
		m_isInMainPlayerPKList = false;
		m_IsDie = false;
		m_PkModel = -1;
		m_fDir = 0.0f;
		m_OwnerObjId = -1;
		m_MountID = -1;
		m_MoveSpeed = 0.0f;
		//m_ModelVisualID = GlobeVar.INVALID_ID;
		//m_WeaponDataID = GlobeVar.INVALID_ID;
		//m_WeaponEffectGem = GlobeVar.INVALID_ID;
		m_strTitleName = "";
		//m_CurTitleID = GlobeVar.INVALID_ID;
		m_StealthLev = 0;
		m_bNpcBornCreate = false;
		m_nOtherVipCost = -1;
		//m_GuildGuid = GlobeVar.INVALID_GUID;
		m_FellowQuality = 0;
		m_nOtherCombatValue = 0;
		m_BindParent = -1;
		m_BindChildren.Clear();
		m_bIsWildEnemyForMainPlayer = false;

		m_nGuildBusinessState = -1;
	}
}


public class ObjManager : Singleton<ObjManager>
{
	public delegate void DelAsycModelOver(object param1, object param2);
	//当前客户端所有非玩家Obj池，使用Obj（场景中）名字（唯一）索引，对应Obj的数据
	private Dictionary<string, Obj> m_ObjPools;
	public Dictionary<string, Obj> ObjPools
	{
		get { return m_ObjPools; }
		set { m_ObjPools = value; }
	}

	

	private Obj_MainPlayer m_MainPlayer;                //当前客户端主角色
	public Obj_MainPlayer MainPlayer { get { return m_MainPlayer; } }

	//当前客户端所有非Obj的GameObj池，使用 自定义 名字（唯一）索引，对应Obj的数据
	private Dictionary<string, GameObject> m_OtherGameObjPools;
	public Dictionary<string, GameObject> OtherGameObjPools
	{
		get { return m_OtherGameObjPools; }
		set { OtherGameObjPools = value; }
	}

	//优化Android下多人同时在线问题 
	private Dictionary<string, GameObject> m_NPCGameObjectList;
	private Dictionary<string, float> m_DeleteNPCList;
	private bool m_IsUseAndroid = true;
	public Dictionary<string, Obj_Character> m_MonsterList;
	public ObjManager()
	{
		m_ObjPools = new Dictionary<string, Obj>();  //动态的，注意使用上限
		m_ObjPools.Clear();

	

		m_MainPlayer = null;

		m_OtherGameObjPools = new Dictionary<string, GameObject>();  //动态的，注意使用上限
		m_OtherGameObjPools.Clear();
		m_MonsterList = new Dictionary<string, Obj_Character>();
		m_MonsterList.Clear();
	}

	private void AddPoolObj(string name, Obj obj)
	{
		try
		{

			//暂时的，服务器传过来的item id有重复的。解决这个问题  by dsy;
			if (m_ObjPools.ContainsKey(name) == false)
				m_ObjPools.Add(name, obj);
		}
		catch (System.Exception ex)
		{
			//LogModule.ErrorLog("ObjManager AddPoolObj(" + name + "," + obj.ServerID.ToString() + " Error: " + ex.Message);
		}
	}

	//初始化Obj管理器
	public bool Init()
	{
		return true;
	}

	public void OnEnterScene()
	{
#if UNITY_ANDROID
		m_IsUseAndroid = true;
		m_ShowingNPCList.Clear();
#endif

		DeleteNPCPool();
		m_ObjPools.Clear();
		m_OtherGameObjPools.Clear();
		
	}

	public void CleanSceneObj()
	{
		foreach (KeyValuePair<string, GameObject> otherGameObj in m_OtherGameObjPools)
		{
			//ResourceManager.DestroyResource(otherGameObj.Value);
		}

		m_OtherGameObjPools.Clear();

		List<GameObject> removeObjList = new List<GameObject>();
		foreach (KeyValuePair<string, Obj> gameObj in m_ObjPools)
		{
			removeObjList.Add(gameObj.Value.gameObject);
		}

		for (int i = 0; i < removeObjList.Count; ++i)
		{
			ReomoveObjInScene(removeObjList[i]);
		}

		m_MainPlayer = null;
		m_ObjPools.Clear();
		

	}

	public GameObject CreateGameObjectByResource(Obj_Init_Data initData)
	{

		return null;
	}
	// 建立剧情obj
	public void CreateModelStoryObj(int nCharModelID, string strName, DelAsycModelOver delAsycStroyModel)
	{
		
	}

	private void AsycCreateModelStoryOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	public void DeleteNPCPool()
	{
		if (m_NPCGameObjectList != null)
		{
			m_NPCGameObjectList.Clear();
			m_NPCGameObjectList = null;
		}

		if (m_DeleteNPCList != null)
		{
			m_DeleteNPCList.Clear();
			m_DeleteNPCList = null;
		}
	}

	/// <summary>
	/// 返回当前NPC数量
	/// </summary>
	/// <returns></returns>
	public int GetNPCNum()
	{
		if (m_NPCGameObjectList != null)
		{
			return m_NPCGameObjectList.Count;
		}
		return 0;
	}
	/// <summary>
	/// 返回真正显示的npc个数 
	/// </summary>
	/// <returns></returns>
	public int GetShowNPCNum()
	{
		if (this.m_IsUseAndroid)
		{
			if (m_NPCGameObjectList != null)
			{
				int count = m_NPCGameObjectList.Count;

				foreach (KeyValuePair<string, GameObject> npc in m_NPCGameObjectList)
				{
					Obj_Character character = npc.Value.GetComponent<Obj_Character>();
					//if (npc.Value.activeSelf == false || character.IsDie() || character.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_SNARE)
					//{
					//	count--;
					//}
				}
				return count;
			}
		}
		else
		{

			if (m_ObjPools == null) return 0;
			return m_ObjPools.Count - 1;
		}
		return 0;
	}
	/// <summary>
	/// 获取当前其他玩家数量
	/// </summary>
	/// <returns></returns>
	public int GetOtherPlayerNum()
	{
		int iTag = 0;
		if (m_ObjPools != null)
		{
			foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
			{
				//if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
				//{
				//	iTag++;
				//}
			}
		}

		return iTag;
	}

	//创建NPC
	public void CreateNPC(Obj_Init_Data initData)
	{
		

	}

	private void AsycCreateNPCOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{

		
	}


	

	private void AsycSnareObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	

	//创建其他玩家
	public void CreateOtherPlayer(Obj_Init_Data initData)
	{
		
	}

	private void AsycCreateOtherPlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	//创建其他玩家
	public void CreateZombieUser(Obj_Init_Data initData)
	{
	}

	private void AsycCreateZombiePlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	//创建伙伴
	public void CreateFellow(Obj_Init_Data initData)
	{
	}

	private void AsycCreateFellowOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	//创建主角
	private bool m_bBeginAsycCreateMainPlayer = false;      //是否开始创建主角标记位，由于主角会在Update中创建，所以改为异步之后需要判断此标记位
	public void CreateMainPlayer()
	{

		
	}


	public event System.Action MainPlayerOnLoad;
	//异步加载主角OK
	private void AsycCreateMainPlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{


		
	}

	

	public void OnCreateDropItem(GameObject resObj, object param)
	{
		
	}

	
	private void AsycCreateCollectItemOver(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
	{
		
	}
	
	private void AsycCreateJuqingItemOver(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
	{
		
	}


	public void AsycReloadModelOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	public void AsycReloadMountModelOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	public void AsycLoadFakeObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	//查看他人属性专用使用，将WeaponID 作为上边的函数参数实在改动太大，所以加一个新的
	public void OtherView_AsycLoadFakeObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}


	public void AsycLoadFitOnObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}

	public void AsycLoadRoleViewFitOnObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
	}


	public void AsycReloadWeaponOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{

	}


	public void OtherView_AsycReloadWeaponOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
	}


#if UNITY_ANDROID
	
	private Dictionary<string, Obj_Init_Data> m_ShowingNPCList = new Dictionary<string, Obj_Init_Data>();
	private void ShowingNPCList(Obj_Init_Data initData)
	{
		if (m_ShowingNPCList == null)
		{
			m_ShowingNPCList = new Dictionary<string, Obj_Init_Data>();
		}
		
		if (!m_ShowingNPCList.ContainsKey(initData.m_ServerID.ToString()))
		{
			m_ShowingNPCList.Add(initData.m_ServerID.ToString(), initData);
		}
	}
	
	public void ShowNPC()
	{
		if (m_ShowingNPCList == null) return;
		if (m_ShowingNPCList.Count <= 0) return;
		
		int iShowTag = 1;
		int iTag = 0;
		string[] strNPCName = new string[iShowTag];
		foreach (KeyValuePair<string, Obj_Init_Data> keyValuePair in m_ShowingNPCList)
		{
			if (keyValuePair.Value != null)
			{
				CreateNPC(keyValuePair.Value);
			}
			strNPCName.SetValue(keyValuePair.Key, iTag);
			
			iTag++;
			if (iTag >= iShowTag) break;
		}
		
		for (int i = 0; i < strNPCName.Length; i++)
		{
			if (string.IsNullOrEmpty(strNPCName[i])) continue;
			
			if (m_ShowingNPCList.ContainsKey(strNPCName[i]))
			{
				m_ShowingNPCList[strNPCName[i]] = null;
				m_ShowingNPCList.Remove(strNPCName[i]);
			}
		}
		
		strNPCName = null;
	}
	
#endif
	public event System.Action<Obj_Init_Data> CreateCharacterCallBack;
	//根据类型创建非主角玩家
	public void NewCharacterObj(GameDefine_Globe.OBJ_TYPE type, Obj_Init_Data initData)
	{
		switch (type)
		{
			case GameDefine_Globe.OBJ_TYPE.OBJ_NPC:
				{
#if UNITY_ANDROID
			ShowingNPCList(initData);
#else
					CreateNPC(initData);
#endif
				}
				break;
			case GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW:
				{
					CreateFellow(initData);
				}
				break;
			case GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER:
				{
					CreateOtherPlayer(initData);
				}
				break;
			case GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER:
				{
					CreateZombieUser(initData);
				}
				break; ;
			default:
				break;
		}
	}


	
	//根据ServerID来查找Obj
	public Obj FindObjInScene(int nServerID)
	{
		return null;
	}

	//根据ServerID来查找Obj_Character
	public Obj_Character FindObjCharacterInScene(int nServerID)
	{
		if (nServerID < 0)
		{
			return null;
		}

		if (m_ObjPools.ContainsKey(nServerID.ToString()))
		{

			return m_ObjPools[nServerID.ToString()] as Obj_Character;
		}

		return null;
	}
	public List<Obj_Character> FindCharacterByName(string name)
	{
		List<Obj_Character> lstchar = new List<Obj_Character>();
		foreach (KeyValuePair<string, Obj> objitem in m_ObjPools)
		{
			if (objitem.Value is Obj_Character)
			{
				Obj_Character character = objitem.Value as Obj_Character;
				//if (character.BaseAttr.RoleName == name)
				//{
				//	lstchar.Add(character);
				//}
			}
		}
		return lstchar;
	}
	//根据Obj_Character的BaseAttr中的名字来查找NPC
	//遍历，不推荐反复使用
	public Obj_Character FindObjCharacterInSceneByName(string szBaseAttrName, bool bIsAlive = true)
	{
		Obj_Character objTarget = null;
		float minDistance = 8f;

		foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
		{
			Obj_Character objChar = objPair.Value as Obj_Character;
			//if (objChar && objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC && objChar.BaseAttr.RoleName == szBaseAttrName)
			//{
			//	//是否要寻找非死亡目标
			//	if (bIsAlive && objChar.IsDie())
			//	{
			//		continue;
			//	}

			//	//CharacterDefine.REPUTATION_TYPE nType = Reputation.GetObjReputionType(objChar);
			//	//if (nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_INVALID
			//	//	|| nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND)
			//	//{
			//	//	return objChar;
			//	//}
			//	//else
			//	//{
			//	//	Vector3 UserPos = Singleton<ObjManager>.GetInstance().MainPlayer.Position;
			//	//	float distance = Mathf.Abs(Vector3.Distance(UserPos, objChar.Position));
			//	//	if (distance - minDistance <= 0)
			//	//	{
			//	//		minDistance = distance;
			//	//		objTarget = objChar;
			//	//	}
			//	//}
			//}
		}

		return objTarget;
	}

	//查找某个玩家
	public Obj_OtherPlayer FindOtherPlayerInScene(UInt64 guid)
	{
		//if (guid == GlobeVar.INVALID_GUID)
		//{
		//	return null;
		//}

		foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
		{
			//if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
			//{
			//	Obj_OtherPlayer objPlayer = objPair.Value as Obj_OtherPlayer;
			//	if (null != objPlayer && objPlayer.GUID == guid)
			//	{
			//		return objPlayer;
			//	}
			//}
		}

		return null;
	}

	//判断场景中某个Obj是否存在
	public bool IsObjExist(int nServerID)
	{
		return m_ObjPools.ContainsKey(nServerID.ToString());
	}

	int[] GuildBossId = new int[3] { 50001, 50002, 50003 };
	public bool IsGuildBoss(int nDataID)
	{
		for (int i = 0; i < GuildBossId.Length; i++)
		{
			if (GuildBossId[i] == nDataID)
				return true;
		}
		return false;
	}

	//根据ServerID删除场景中的Obj
	public bool RemoveObj(int nServerID)
	{
		Obj obj = FindObjInScene(nServerID);
		if (null != obj)
		{
			ReomoveObjInScene(obj.gameObject);
			return true;
		}

		return false;
	}


	/// <summary>
	/// 删除NPC缓存中对象
	/// </summary>
	/// <param name="isInitiative">是否主动调用删除</param>
	public void DeleteNPCGameObject()
	{
		if (!m_IsUseAndroid) return;

		if (m_DeleteNPCList == null) return;

		List<string> temp = new List<string>();
		int deleteTag = 0;
		foreach (KeyValuePair<string, float> deleteNpc in m_DeleteNPCList)
		{
			if (deleteTag >= 8) break;

			if (null != m_NPCGameObjectList && m_NPCGameObjectList.ContainsKey(deleteNpc.Key))
			{
				ReomoveObjInScene(m_NPCGameObjectList[deleteNpc.Key], true);
				m_NPCGameObjectList[deleteNpc.Key] = null;
				m_NPCGameObjectList.Remove(deleteNpc.Key);
			}

			temp.Add(deleteNpc.Key);
			deleteTag++;

		}

		for (int i = 0; i < temp.Count; i++)
		{
			m_DeleteNPCList.Remove(temp[i]);
		}

		temp.Clear();
		temp = null;
	}

	//删除场景中的Obj
	public bool ReomoveObjInScene(GameObject removeObject, bool isDelete = false)
	{
		if (null == removeObject)
		{
			return false;
		}

		Obj tempObj = removeObject.GetComponent<Obj>();
		if (tempObj)
		{
			if (m_IsUseAndroid && !isDelete)
			{
				//Obj_NPC npc = removeObject.GetComponent<Obj_NPC>();
				//if (npc != null)
				//{
				//	if (null != m_NPCGameObjectList && m_NPCGameObjectList.ContainsKey(removeObject.name))
				//	{
				//		m_NPCGameObjectList[removeObject.name].SetActive(false);

				//		if (m_DeleteNPCList == null)
				//		{
				//			m_DeleteNPCList = new Dictionary<string, float>();
				//		}

				//		if (!m_DeleteNPCList.ContainsKey(removeObject.name))
				//		{
				//			m_DeleteNPCList.Add(removeObject.name, Time.time);
				//		}

				//		npc.StopNPCEffect();

				//		npc = null;
				//		removeObject = null;
				//		return true;
				//	}
				//}
			}

#if UNITY_ANDROID
			//删除延迟缓存中的NPC列表信息
			if (m_ShowingNPCList != null)
			{
				if (m_ShowingNPCList.ContainsKey(tempObj.ServerID.ToString()))
				{
					m_ShowingNPCList.Remove(tempObj.ServerID.ToString());
				}
			}
#endif

			m_ObjPools.Remove(tempObj.ServerID.ToString());
			if (m_MonsterList.ContainsKey(tempObj.ServerID.ToString()))
				m_MonsterList.Remove(tempObj.ServerID.ToString());
		

			UpdateHidePlayers();

			//删除名字版
			Obj_Character tempObjCharacter = tempObj as Obj_Character;
			//if (tempObjCharacter)
			//{
			//	ResourceManager.UnLoadHeadInfoPrefab(tempObjCharacter.HeadInfoBoard);
			//}
			//ResourceManager.DestroyResource(ref removeObject);
			return true;
		}

		return false;
	}

	//同步场景中的Obj位置
	public void SyncObjectPosition(int nServerId, int nPosX, int nPosZ)
	{
		//如果是自己则不进行同步
		if (Singleton<ObjManager>.GetInstance().MainPlayer.ServerID == nServerId)
		{
			return;
		}
		float fPosX = ((float)nPosX) / 100;
		float fPosZ = ((float)nPosZ) / 100;
		Obj_Character obj = FindObjCharacterInScene(nServerId);
		if (null != obj)
		{
			Vector3 pos = new Vector3(fPosX, obj.gameObject.transform.position.y, fPosZ);
			//            if (GameManager.gameManager.ActiveScene.IsT4MScene())
			//            {
			//				pos.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(pos);
			//            }
			//            else if (null != Terrain.activeTerrain)
			//            {
			//				pos.y = Terrain.activeTerrain.SampleHeight(pos);
			//            }

			//校验，如果发现距离相差太远，则直接拉过去
			//if (Vector3.Distance(pos, obj.Position) > 5.0f || obj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
			//{
			//	obj.Position = pos;
			//}
			//else
			//{
			//	//obj.MoveTo(pos, null, 0.5f);
			//}
		}
	}


	private void AddPoolOtherGameObj(string name, GameObject gObj)
	{
		try
		{
			m_OtherGameObjPools.Add(name, gObj);
		}
		catch (System.Exception ex)
		{
			//LogModule.ErrorLog("ObjManager AddPoolOtherGameObj(" + name + "," + "Error: " + ex.Message);
		}
	}

	//根据Gameobject的Name在非Obj池查找
	public GameObject FindOtherGameObj(string strName)
	{
		if (m_OtherGameObjPools.ContainsKey(strName))
		{
			return m_OtherGameObjPools[strName];
		}

		return null;
	}

	//删除场景中非obj池 中的GameObject
	public bool RemoveOtherGameObj(string strName)
	{
		GameObject removeObject = FindOtherGameObj(strName);
		if (null == removeObject)
		{
			return false;
		}

		if (m_OtherGameObjPools.Remove(removeObject.name))
		{
			//ResourceManager.DestroyResource(ref removeObject);
			return true;
		}
		return false;
	}

	// 根据屏蔽规则判断隐藏的玩家是否可以显示
	public void UpdateHidePlayers()
	{
		//if (m_ObjOtherPlayerShowList.Count > PlayerPreferenceData.SystemShowOtherPlayerCount)
		//{
		//	// 当前显示的已经超过预期，将多余的隐藏
		//	int canHideCount = m_ObjOtherPlayerShowList.Count - PlayerPreferenceData.SystemShowOtherPlayerCount;

		//	canHideCount = canHideCount > m_ObjOtherPlayerShowList.Count ? m_ObjOtherPlayerShowList.Count : canHideCount;

		//	for (int i = 0; i < canHideCount; i++)
		//	{
		//		// 排在显示队列末尾的拥有比较高的显示优先级
		//		Obj_HidePlayerData curObjData = m_ObjOtherPlayerShowList[m_ObjOtherPlayerShowList.Count - 1];
		//		m_ObjOtherPlayerShowList.RemoveAt(m_ObjOtherPlayerShowList.Count - 1);

		//		if (m_ObjPools.ContainsKey(curObjData.m_serverID))
		//		{
		//			Obj_OtherPlayer curChar = m_ObjPools[curObjData.m_serverID] as Obj_OtherPlayer;
		//			m_ObjOtherPlayerHideList.Add(curObjData);
		//			if (null != curChar)
		//			{
		//				curChar.SetVisible(false);
		//			}

		//		}
		//	}
		//}
		//else
		//{
		//	// 当前显示不足预期，放开显示
		//	//int canShowCount = PlayerPreferenceData.SystemShowOtherPlayerCount - m_ObjOtherPlayerShowList.Count;

		//	//canShowCount = canShowCount > m_ObjOtherPlayerHideList.Count ? m_ObjOtherPlayerHideList.Count : canShowCount;
		//	//for (int i = 0; i < canShowCount; i++)
		//	//{
		//	//	Obj_HidePlayerData curObjData = m_ObjOtherPlayerHideList[0];
		//	//	m_ObjOtherPlayerHideList.RemoveAt(0);

		//	//	if (m_ObjPools.ContainsKey(curObjData.m_serverID))
		//	//	{
		//	//		Obj_OtherPlayer curChar = m_ObjPools[curObjData.m_serverID] as Obj_OtherPlayer;
		//	//		m_ObjOtherPlayerShowList.Add(curObjData);
		//	//		if (null != curChar)
		//	//		{
		//	//			curChar.SetVisible(true);
		//	//			//#if UNITY_ANDROID
		//	//			//判断是否已经下载模型。如果没加载模型，加载模型
		//	//			Transform childTrans = curChar.transform.Find("Model");
		//	//			if (childTrans == null)
		//	//			{
		//	//				ReloadModel(curChar.gameObject, curObjData.ResPath, AsycCreateOtherPlayerOver, curObjData.InitData);
		//	//			}
		//	//			else
		//	//			{
		//	//				curChar.RideOrUnMount(curChar.MountID);
		//	//			}
		//	//			childTrans = null;
		//	//			//#endif
		//	//		}

		//	//	}
		//	//}

		//}
	}

	public void TestOtherPlayerVisible(Obj_OtherPlayer curPlayer)
	{
		//m_ObjOtherPlayerShowList.Add(new Obj_HidePlayerData(curPlayer.ServerID.ToString(), curPlayer.GetVisibleValue()));
		UpdateHidePlayers();
	}


	public static void AddOutLineMaterial(GameObject parentObj)
	{
		//if (PlayerPreferenceData.SystemWallVisionEnable == false)
		//{
		//	return;
		//}
		foreach (SkinnedMeshRenderer curMeshRender in parentObj.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			Material[] newMaterialArray = new Material[curMeshRender.materials.Length + 1];
			for (int i = 0; i < curMeshRender.materials.Length; i++)
			{
				if (curMeshRender.materials[i].name.Contains("PlayerXplay"))
				{
					return;
				}
				else
				{
					newMaterialArray[i] = curMeshRender.materials[i];
				}
			}

			UnityEngine.Object playerXPlayObj = Resources.Load("Material/PlayerXplay");
			if (null != playerXPlayObj)
			{
				newMaterialArray[curMeshRender.materials.Length] = GameObject.Instantiate(playerXPlayObj) as Material;
			}
			//			UnityEngine.Object OutLineObj = Resources.Load("Material/OutLine");
			//			if (null != playerXPlayObj)
			//			{
			//				newMaterialArray[curMeshRender.materials.Length+1] = GameObject.Instantiate(OutLineObj) as Material;
			//				newMaterialArray[curMeshRender.materials.Length+1].mainTexture=curMeshRender.materials[curMeshRender.materials.Length-1].mainTexture;
			//			}

			curMeshRender.materials = newMaterialArray;

		}
	}

	public static void RemoveOutLineMaerial(GameObject parentObj)
	{
		foreach (SkinnedMeshRenderer curMeshRender in parentObj.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			int newMaterialArrayCount = 0;
			for (int i = 0; i < curMeshRender.materials.Length; i++)
			{
				if (curMeshRender.materials[i].name.Contains("PlayerXplay"))
				{
					newMaterialArrayCount++;
				}
			}

			if (newMaterialArrayCount > 0)
			{
				Material[] newMaterialArray = new Material[newMaterialArrayCount];
				int curMaterialIndex = 0;
				for (int i = 0; i < curMeshRender.materials.Length; i++)
				{
					if (curMaterialIndex >= newMaterialArrayCount)
					{
						break;
					}
					if (!curMeshRender.materials[i].name.Contains("PlayerXplay"))
					{
						newMaterialArray[curMaterialIndex] = curMeshRender.materials[i];
						curMaterialIndex++;
					}
				}

				curMeshRender.materials = newMaterialArray;
			}
		}
	}
}
