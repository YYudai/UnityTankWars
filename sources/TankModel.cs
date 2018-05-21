using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ChobiAssets.KTP;

/// <summary>
/// 戦車モデル本体
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class TankModel : MonoBehaviour
    {
        
        private TankMovement mMovementScript;
        private TurretController mTurretScript;
        private FireController mFireScript;
        private TankHealth mHealthScript;
        private EnemyAi mAi;

        void Start()
        {
            mMovementScript = GetComponent<TankMovement>();
            mTurretScript = GetComponent<TurretController>();
            mFireScript = GetComponent<FireController>();
            mHealthScript = GetComponent<TankHealth>();

            // 敵戦車はAIコンポーネントを取得し、初期化する
            if (!mIsPlayer)
            {
                mAi = GetComponent<EnemyAi>();
                mAi.Init(GetComponent<Transform>(), mMovementScript, mTurretScript, mFireScript);
            }
        }

        //----------
        // フラグ //
        //----------------------------------------------------------------------------------------

        [SerializeField]
        private bool mIsPlayer;
        public bool IsPlayer
        {
            get { return mIsPlayer; }
        }

        // 操作有効フラグ
        private bool mIsActive;
        public bool IsActive
        {
            get { return mIsActive; }
        }

        public void OnActive()
        {
            mIsActive = true;
        }

        // 死亡フラグ
        private bool mIsDead;
        public bool IsDead
        {
            get { return mIsDead; }
            set { mIsDead = value; }
        }

        //------------
        // 完全停止 //
        //----------------------------------------------------------------------------------------

        public void Stop()
        {
            // 操作不可状態に
            mIsActive = false;

            // 移動力をリセット
            mMovementScript.ResetVelocity();

            // 発射されている弾を全て休眠状態に
            mFireScript.SleepAllBullets();
        }

        //----------------------
        // 更新（入力チェック） //
        //----------------------------------------------------------------------------------------

        void Update()
        {
            if (mIsActive && mIsPlayer) // アクティブかつプレイヤーの場合は操作を受け付け
            {
                // 移動入力の受付
                mMovementScript.CheckInput();

                // 砲台向きを計算
                mTurretScript.CalRotation();

                // 発射
                mFireScript.CheckInput();
            }
            else if(mIsActive && !mIsPlayer) // アクティブかつ敵の場合はAI行動を決定＆実行
            {
                // 移動目標を決定
                mAi.DecideMovement();

                // 移動完了の判定
                mAi.CheckMovement();

                // 砲台向きを計算
                mAi.CalTurretRotation();

                // 発射
                mAi.Fire();
            }

            // HP描画はアクティブに影響を受けないで更新
            mHealthScript.RenewHealthBar();
        }

        void FixedUpdate()
        {
            //if (!mIsActive) return; これだと結果表示の際に移動力を0に設定してもTankMovementのRigidbody#velocityが0で固定されない（？？？）

            // 移動
            mMovementScript.ApplyVelocity();

            // 砲台
            mTurretScript.ApplyRotation();
        }

    }
}
