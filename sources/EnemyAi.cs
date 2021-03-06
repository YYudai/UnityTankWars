﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦車モデルに行動AIを実装
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class EnemyAi : MonoBehaviour
    {

        private Transform mTrans;
        private TankMovement mMoveScript;
        private TurretController mTurretScript;
        private FireController mFireScript;

        public void Init(Transform trans, TankMovement move, TurretController turret, FireController fire)
        {
            mTrans = trans;
            mMoveScript = move;
            mTurretScript = turret;
            mFireScript = fire;

            // 1発目の弾を発射するまでの待機時間を決めておく
            RandomSetWaitFireTime();
        }

        //--------
        // 移動 //
        //----------------------------------------------------------------------------------------

        private enum MOVE_STATE
        {
            DEFAULT = 0,
            UP = 1,
            DOWN = 2
        }
        private MOVE_STATE mMoveState;
        private float mGoalZ; // 目標とするz座標

        /// <summary>
        /// mMoveState==DEFAULTの場合に、目標とするz座標を計算する
        /// </summary>
        public void DecideMovement()
        {
            if(mMoveState == MOVE_STATE.DEFAULT)
            {
                // 現在のz座標を取得
                float currentZ = mTrans.position.z;

                // 移動可能なz軸の範囲から乱数取得
                float rand = Random.Range(-14.0f, 15.0f);

                // 取得した乱数と現在のz座標の距離を計算
                float difference = 0.0f;
                if(currentZ >= rand)
                {
                    difference = Mathf.Abs(currentZ - rand);
                    mMoveState = MOVE_STATE.DOWN;
                }else
                {
                    difference = Mathf.Abs(rand - currentZ);
                    mMoveState = MOVE_STATE.UP;
                }

                // 距離が1.0f以上あるならば目標の座標を決定し、そうでないならば次フレームまでその場で待機
                if(difference >= 1.0f)
                {
                    mGoalZ = rand;
                }else
                {
                    mMoveState = MOVE_STATE.DEFAULT;
                }

                // mMoveStateを条件にして、TankMovementの移動メソッドを呼び出す
                switch (mMoveState)
                {
                    case MOVE_STATE.DEFAULT:
                        mMoveScript.ResetVelocity();
                        break;
                    case MOVE_STATE.UP:
                        mMoveScript.SetVelocityUp();
                        break;
                    case MOVE_STATE.DOWN:
                        mMoveScript.SetVelocityDown();
                        break;
                }
            }
        }

        /// <summary>
        /// 移動の完了判定
        /// 移動完了の判定を行い、完了しているならばmMoveStateをDEFAULTに戻す
        /// </summary>
        public void CheckMovement()
        {
            float currentZ = mTrans.position.z;
            switch (mMoveState)
            {
                case MOVE_STATE.DEFAULT:
                    return;
                case MOVE_STATE.UP:
                    if (currentZ >= mGoalZ) mMoveState = MOVE_STATE.DEFAULT;
                    break;
                case MOVE_STATE.DOWN:
                    if (currentZ <= mGoalZ) mMoveState = MOVE_STATE.DEFAULT;
                    break;
            }
        }

        //--------
        // 砲台 //
        //----------------------------------------------------------------------------------------

        [SerializeField]
        [Tooltip("砲台を向ける対象のTransform")]
        private Transform mTargetTrans;

        /// <summary>
        /// 砲台をターゲットに向けるためのRotationを計算
        /// </summary>
        public void CalTurretRotation()
        {
            mTurretScript.CalRotation(mTargetTrans);
        }

        //--------
        // 発射 //
        //----------------------------------------------------------------------------------------

        private readonly float MAX_WAIT_FIRE = 1.0f, MIN_WAIT_FIRE = 0.2f;
        private float mWaitFireTime; // 弾を発射するまでの待機時間

        /// <summary>
        /// 次に弾を発射するまでの待機時間を乱数で決定する
        /// </summary>
        private void RandomSetWaitFireTime()
        {
            mWaitFireTime = Random.Range(MIN_WAIT_FIRE, MAX_WAIT_FIRE);
        }

        /// <summary>
        /// 待機が完了した場合は弾を発射し、次の発射までの待機時間を決定する
        /// </summary>
        public void Fire()
        {
            mWaitFireTime -= Time.deltaTime;
            if(mWaitFireTime <= 0.0f)
            {
                mFireScript.Fire();
                RandomSetWaitFireTime();
            }
        }

    }
}
