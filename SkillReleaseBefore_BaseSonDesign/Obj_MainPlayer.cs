
 //	功能说明：游戏主角Obj逻辑类


using System.Runtime.Serialization.Formatters;
//using Games.AI_Logic;
//using Games.ImpactModle;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Games.GlobeDefine;
//using Games.Events;
//using Games.Scene;
//using GCGame;

//using GCGame.Table;
//using Games.SkillModle;
//using Games.Animation_Modle;
//using Games.Item;
//using Module.Log;
using System;
//using Games.Fellow;

namespace Games.LogicObj
{
    //主玩家脚本--继承自其它玩家脚本
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        // 加载模型相关
        private static int m_originalModelID = -1;
        public static int OriginalModelID { set { m_originalModelID = value; } get { return m_originalModelID; } }
        private static int m_changeModelID = -1;
        public static int ChangeModelID { set { m_changeModelID = value; } get { return m_changeModelID; } }

        public Obj_MainPlayer()
        {
            //m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER;
            //m_BaseAttr = new BaseAttr();

        }

        public override bool Init(Obj_Init_Data initData)
        {
            //主角进行Init的时候调用一次Unload方法
            Resources.UnloadUnusedAssets();
            LastHeartBeatTime = -1;
            return true;
        }
        void Awake()
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }
        }

        //ThirdPersonController m_Thirdcontroller = null;
        //public ThirdPersonController Thirdcontroller
        //{
        //    get { return m_Thirdcontroller; }
        //}

        //public override int Profession
        //{
        //    //get { return GameManager.gameManager.PlayerDataPool.Profession; }
        //    //set { GameManager.gameManager.PlayerDataPool.Profession = value; }
        //}
        //public override System.UInt64 GuildGUID
        //{
        //    //get { return GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid; }
        //    //set { GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid = value; }
        //}
        //public System.UInt64 LoverGUID
        //{
        //    //get { return GameManager.gameManager.PlayerDataPool.LoverGUID; }
        //    //set { GameManager.gameManager.PlayerDataPool.LoverGUID = value; }
        //}
        private UInt64 m_CurUseMountItemGuid;

        public float LastHeartBeatTime = -1;
        void Start()
        {
           
        }

        //开始每秒一次的循环


        //开始每分钟一次循环
        IEnumerator UpdatePerMinute()
        {
            while (true)
            {
                yield return new WaitForSeconds(60);

              
            }
        }

        //更新Obj_MainPlayer逻辑数据

        float updateSecondStep = 0;
        void UpdateSecond()
        {
           
        }
        void Update()
        {
           
            
          

        }

        void FixedUpdate()
        {
           
        }

        //更新玩家脚本
        void UpdateStep()
        {
            

        }

        public static float m_fTimeSecond = Time.realtimeSinceStartup;
        void UpdateReliveEntryTime()
        {
            float ftimeSec = Time.realtimeSinceStartup;
            int nTimeData = (int)(ftimeSec - m_fTimeSecond);
            if (nTimeData > 0)
            {
                if (ReliveEntryTime > 0)
                {
                    ReliveEntryTime = ReliveEntryTime - nTimeData;
                    if (ReliveEntryTime < 0)
                    {
                        ReliveEntryTime = 0;
                    }
                }
                m_fTimeSecond = ftimeSec;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //向服务器同步相关
        //////////////////////////////////////////////////////////////////////////
        //同步位置信息间隔
        const float m_fSyncPosTimeInterval = 0.2f;
        float m_fLastSyncPosTime = 0.0f;
        Vector3 m_fLastSyncPos = new Vector3(0.0f, 0.0f, 0.0f);
        public UnityEngine.Vector3 LastSyncPos
        {
            get { return m_fLastSyncPos; }
            set { m_fLastSyncPos = value; }
        }
        //同步位置信息给Server
        void SyncPosToServer()
        {
           
        }

      

        void InitNameBoard()
        {
            

        }

        void OnLoadNameBoard(GameObject objNameBoard)
        {
            
        }
        public override void UpdateVipInfo()
        {
            base.UpdateVipInfo();
            OnVipCostChange();
        }

        public void ShowPlayerTitleInvestitive()
        {
        }

        public void UpadatePlayerGBState()
        {
            
        }

        //玩家登陆接口
        public void OnPlayerLogin()
        {
        }

        //切换场景调用接口
        public void OnPlayerEnterScene()
        {
            

        }
 
        public void ChangeHeadPic()
        {
           
        }

        public void OnPlayerLeaveScene()
        {
        }



        void UpdateCameraController()
        {
        }
        //////////////////////////////////////////////////////////////////////////
        /// 移动和动画相关
        //////////////////////////////////////////////////////////////////////////
       

        //////////////////////////////////////////////////////////////////////////
        //目标选择逻辑
        //////////////////////////////////////////////////////////////////////////
        private Obj_Character m_selectTarget = null;      //选择的目标
        public Obj_Character SelectTarget
        {
            get { return m_selectTarget; }
            set { m_selectTarget = value; }
        }
        private bool m_onSelectForClick = false;//标记从点击选择的目标
        public bool OnSelectForClick
        {
            get { return m_onSelectForClick; }
            set { m_onSelectForClick = value; }
        }

        public void OnSelectTargetForClick(GameObject targetObj, bool isMoveAgainSelect = true)
        {
            m_onSelectForClick = true;
            OnSelectTarget(targetObj, isMoveAgainSelect);
        }

        public void OnSelectTarget(GameObject targetObj, bool isMoveAgainSelect = true)
        {
           
        }

        public void UpdateSelectTarget()
        {
            
        }

        //////////////////////////////////////////////////////////////////////////
        // 坐骑相关

        public override void RideOrUnMount(int nMountID)
        {
            base.RideOrUnMount(nMountID);

        }

        //////////////////////////////////////////////////////////////////////////
        // 伙伴相关
        //////////////////////////////////////////////////////////////////////////
        //当前召出伙伴服务器objid
        private int m_nCurFellowObjId = -1;
        public int CurFellowObjId
        {
            get { return m_nCurFellowObjId; }
            set { m_nCurFellowObjId = value; }
        }
        //private float m_fAutoHPDrugSecond = -16.0f;
        //private float m_fBuyHPDrugSecond = Time.realtimeSinceStartup;
        //private float m_fAutoMPDrugSecond = -16.0f;
        //private float m_fBuyMPDrugSecond = Time.realtimeSinceStartup;
        public void AutoUseHPMPDrug()
        {
            
        }

        public bool isUp = false;

      
        public void OnVipCostChange()
        {
           
        }

        private int m_lastLevel = -1;
      

      

        public void OnOffLineExpChange()
        {
        }

       
        public void AskCombatValue(bool bPowerRemind)
        {
            
        }

        //死亡相关
        private int m_nReliveEntryTime = 0;//记录复活剩余秒
        public int ReliveEntryTime
        {
            get { return m_nReliveEntryTime; }
            set { m_nReliveEntryTime = value; }
        }
        public override bool OnCorpse()
        {
            base.OnCorpse();
           ;
            return true;
        }


        //Obj死亡时候调用

        public override bool OnDie()
        {
            if (IsDie())
            {
                return false;
            }
            base.OnDie();
            
            return true;
        }
        public override bool OnRelife()
        {
            base.OnRelife();
            
            return true;
        }
        public override void OptChangPKModle()
        {
           
        }

      

        private bool m_bIsInModelStory = false;
        public bool IsInModelStory
        {
            get { return m_bIsInModelStory; }
            set { m_bIsInModelStory = value; }
        }

        private bool m_bIsNoMove = false;
        public bool IsNoMove
        {
            get { return m_bIsNoMove; }
            set { m_bIsNoMove = value; }
        }

        public void SendNoticMsg(bool IsFilterRepeat, string strMsg, params object[] args)
        {
            
        }

        //玩家是否接受外部移动指令
        public bool IsCanOperate_Move()
        {
            
            return true;
        }

  

        public override void EndQingGong()
        {
            base.EndQingGong();
            ProcessQingGongOver();
            ReqShowFellow();
        }

        //玩家轻功开始之后，强制更新一下轻功点给服务器
        public void SycQingGongPos(Vector3 pos)
        {
            
        }

     
        void ProcessQingGongStart()
        {
            
        }
        void ProcessQingGongOver()
        {
           
        }

        //切磋
        public UInt64 DuelTargetGuid { set; get; }
        //
        public void ReqDuel(UInt64 targetGuid)
        {
           
        }

        public void DuelWithMe(UInt64 targetGuid, string name)
        {
            
        }

        public void AgreeDuelWithOther() { DecideDuelWithOrNot(1); }
        public void RefuseDuelWithOther() { DecideDuelWithOrNot(0); }

        public void DecideDuelWithOrNot(int agree)
        {
            
        }

        //----
        private int m_SpcialAnimationID = -1;
      
        public void OnStartPlayStory(int storyID)
        {
            switch (storyID)
            {
                case 16:
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(170, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 17:
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(171, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(172, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 18: // 45 ,45
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(173, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 19: // 36
                    m_SpcialAnimationID = 2;
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(174, "JXZPlaySPAni1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 20: // 37
                    m_SpcialAnimationID = 2;
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(175, "JXZPlaySPAni2", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
            }
        }

        public void RemoveAllSpicalClient()
        {
            RemoveSpicalClient(1);
            RemoveSpicalClient(2);
        }

        public void OnPlayStoryOver(int storyID)
        {
           

        }

        private void RemoveSpicalClient(int idx)
        {
           
        }

        public void ProcessQingGongCharAsycLoadOver(object param1, object param2)
        {
          
        }

        public void ProcessQingGongBossAsycLoadOver(object param1, object param2)
        {
           
        }

        public void ProcessCharFastMoveToMainPlayerAsycLoadOver(object param1, object param2)
        {
           

        }
    

        public void ProcessCharPlayAnimationAsycLoadOver(object param1, object param2)
        {
           
        }



        public void InitYanMenGuanWaiVisual()
        {
           
        }

        private int m_nCopySceneId = -1;
        private int m_nCopySceneSingle = -1;
        private int m_nCopySceneDifficult = -1;
        public void SendOpenScene(int nSceneId, int nSingle, int nDifficult)
        {
            
        }
        public void OnOpenCopySceneOK()
        {
          
        }
        public void OnOpenCopySceneNO()
        {

        }

        // 随玩家等级开放按钮
        void LevelUpButtonActive()
        {
           
        }

        void InitLevelButtonActive()
        {
            
        }

       
        void StaminaTimerFunc()
        {
            
        }

        public void ReqHideFellow()
        {
            
        }

        public void ReqShowFellow()             //请求显示伙伴
        {
           
        }

   
      

        public bool IsGBCanAccept()
        {
            
            return false;
        }

       
        
        void OnUseMountItemOk()
        {
        }

       
      
      
        //宝石孔是否满足级别需求
        public bool CheckLevelForGemSlot(int slotindex)
        {
           
            return true;
        }

        //此装备位是否已有相同属性宝石
        public bool IsSameGemForEquipSlot(int gemId, int equipSlot)
        {
            

            return false;
        }
        public void InitCangJingGeInfo()
        {
        }


      
        public int GetTotalEquipCombatValue()
        {
            int totalCombatValue = 0;
           
            return totalCombatValue;
        }

        public int GetTotalGemCombatValue()
        {
            int totalCombatValue = 0;
          
            return totalCombatValue;
        }

        public int GetTotalFellowCombatValue()
        {
            int totalCombatValue = 0;
          
            return totalCombatValue;
        }

        public void updateWeaponPoint()//GameDefine_Globe.OBJ_ANIMSTATE ObjState
        {

            
        }

       

        public void changeWeaponPath(string weaponUrl, int tempIndex)
        {
            
        }

        private int breakStandTime = 50;
        private void updateBreakForActStand()
        {
            
        }
        public bool isHideWeapon = false;
        //public bool isMissionCollect = false;
        public void HideOrShowWeanpon()
        {
        }

    }

}