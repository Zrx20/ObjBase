/********************************************************************************
 *	文件名：	Obj_OtherPlayer.cs
 *	全路径：	\Script\Obj\Obj_OtherPlayer.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏其他玩家Obj逻辑类
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
//using Games.GlobeDefine;
//using GCGame;
//using GCGame.Table;
//using Games.Animation_Modle;
//using Games.ObjAnimModule;
using System;
//using Module.Log;
//using Games.Item;
//using Games.Scene;
using System.Collections.Generic;
//using Games.Events;
//using Games.ChatHistory;

namespace Games.LogicObj
{
    //其它玩家脚本--继承自角色脚本
    public class Obj_OtherPlayer : Obj_Character
    {
        public Obj_OtherPlayer()
        {
        }

        public override void OnReloadModle()
        {
            base.OnReloadModle();
        
        }

        public void UpdateGBStateEffect(int nState)
        {
            
            switch (nState)
            {
                case 0:
                    base.StopEffect(0xef, true);
                    base.PlayEffect(0xee, null, null);
                    break;

                case 1:
                    base.StopEffect(0xee, true);
                    base.PlayEffect(0xef, null, null);
                    break;

                default:
                    base.StopEffect(0xee, true);
                    base.StopEffect(0xef, true);
                    break;
            }
        }

        private int m_fellowID;
        public int FellowID
        {
            get { return m_fellowID; }
            set { m_fellowID = value; }
        }

        private bool m_bIsWildEnemyForMainPlayer = false;
        public bool IsWildEnemyForMainPlayer
        {
            get { return m_bIsWildEnemyForMainPlayer; }
            set { m_bIsWildEnemyForMainPlayer = value; }
        }
        public override bool Init(Obj_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }

          

            return true;
        }

        //进入可视区域
        void OnBecameVisible()
        {
           
        }

        //离开可视区域
        void OnBecameInvisible()
        {
            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = false;

            //隐藏名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(false);
            }

            // 隐藏模型
            if (null != ModelNode)
            {
                ModelNode.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            

        }

        void InitNameBoard()
        {
           
        }

        void OnLoadNameBoard(GameObject objNameBoard)
        {
           
        }

        public void UpdateGBNameBoard()
        {
           
        }

        public virtual void UpdateCombatValue()
        {
           
        }
        public virtual void UpdateVipInfo()
        {
           
        }



        private string m_strTitleInvestitive;
        public string StrTitleInvestitive
        {
            get { return m_strTitleInvestitive; }
            set { m_strTitleInvestitive = value; }
        }

        private int m_CurTitleID;
        public int CurTitleID
        {
            get { return m_CurTitleID; }
            set { m_CurTitleID = value; }
        }

        public void ShowTitleInvestitive()
        {
            
        }

        //职业
        private int m_nProfession = -1;
        public virtual int Profession
        {
            get { return m_nProfession; }
            set { m_nProfession = value; }
        }
        //VIP
        private int m_nVipCost = -1;
        public virtual int VipCost
        {
            get { return m_nVipCost; }
            set { m_nVipCost = value; UpdateVipInfo(); }
        }


        private int m_nOtherCombatValue = -1;
        public virtual int OtherCombatValue
        {
            get { return m_nOtherCombatValue; }
            set { m_nOtherCombatValue = value; }
        }
        //PK模式
        private int m_nPkModle = -1;
        public virtual int PkModle
        {
            get { return m_nPkModle; }
            set { m_nPkModle = value; }
        }
        //是否在主角的反击列表中
        private bool m_bIsInMainPlayerPKList = false;
        public bool IsInMainPlayerPKList
        {
            get { return m_bIsInMainPlayerPKList; }
            set { m_bIsInMainPlayerPKList = value; }
        }

        public override Color GetNameBoardColor()
        {
            return Color.white;
        }

        public virtual void OptChangPKModle()
        {
            SetNameBoardColor();
        }

     
        //坐骑
        private int m_MountID = -1;
        virtual public int MountID
        {
            get { return m_MountID; }
            set
            {
                m_MountID = value;
                //RideOrUnMount(m_MountID);
            }
        }

        private bool m_bIsNeedUnMount = false;
        public bool IsNeedUnMount
        {
            get { return m_bIsNeedUnMount; }
            set { m_bIsNeedUnMount = value; }
        }
        public float GetMountNameBoardHeight()
        {
            if (m_MountID == -1)
            {
                return 0;
            }



            return -1;
        }

        // 上马下马 统一接口
        public virtual void RideOrUnMount(int nMountID)
        {
           

            if (nMountID >= 0)
            {
                RideMount(nMountID);
            }
            else
            {
               
                UnMount();
            }
        }
        // 骑马
        private void RideMount(int nMountID)
        {
           
        }

        // 下马
        private void UnMount()
        {
           
        }

     
        public override bool OnCorpse()
        {
            base.OnCorpse();
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

      
     

        public void ReloadWeaponEffectGem()
        {
            
        }

        // 放技能时换装需要等待
        private bool m_UpdateModelWait = false;
        private bool m_UpdateWeaponWait = false;
        private bool m_UpdateWeaponGemWait = false;

        public void UpdateVisualAfterSkill()
        {
           
        }

        //玩家轻功部分处理
        private bool m_bQingGongState = false;
        public bool QingGongState
        {
            get { return m_bQingGongState; }
            set { m_bQingGongState = value; }
        }

        private Vector3 m_QingGongSrc = Vector3.zero;
        private Vector3 m_QingGongDst = Vector3.zero;
      
     
        private float m_fQingGongMaxHeight = 0;
        private float m_fQingGongTime = 0;
        private float m_fQingGongBeginTime = 0;
      

        public virtual void EndQingGong()
        {
           
        }

        public virtual void UpdateQingGong()
        {
           
        }

     

        public virtual void SetVisible(bool bVisible)
        {
          
        }

        //==============
        public void updateWeaponPoint()//GameDefine_Globe.OBJ_ANIMSTATE ObjState
        {
        }

        private void changeWeaponPath(string weaponUrl, int tempIndex)
        {
            
        }
        //==========end=========
        private int breakStandTime = 50;
        private void updateBreakForActStand()
        {
           
        }

    }
}
