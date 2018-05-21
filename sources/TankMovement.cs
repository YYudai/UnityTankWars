using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦車モデルに移動アクションを実装
/// W or ↑: 上移動
/// S or ↓: 下移動
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class TankMovement : MonoBehaviour
    {

        private Rigidbody mRigid;

        void Awake()
        {
            mRigid = GetComponent<Rigidbody>();
            mCurrentSpeed = SPEED_DEFAULT;
        }

        //---------------------------------------------
        // Called by TankModel#Update or FixedUpdate //
        //----------------------------------------------------------------------------------------

        /// <summary>
        /// 移動入力の受付
        /// </summary>
        public void CheckInput()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                SetVelocityUp();
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                SetVelocityDown();
            }
            else
            {
                ResetVelocity();
            }
        }

        /// <summary>
        /// 計算されたmVelocityをRigidbody.velocityに適用する
        /// </summary>
        public void ApplyVelocity()
        {
            mRigid.velocity = mVelocity;
        }

        //--------
        // 移動 //
        //----------------------------------------------------------------------------------------

        [Header("[ 移動アクションを実装 ]")]
        [Space(8)]
        [SerializeField]
        [Tooltip("上限速度")]
        private float SPEED_LIMIT = 40.0f;

        [SerializeField]
        [Tooltip("初期速度")]
        private float SPEED_DEFAULT = 20.0f;

        [SerializeField]
        [Tooltip("速度係数")]
        private float SPEED_COEFFICIENT = 4.0f;

        // 現在の速度
        private float mCurrentSpeed;
        private Vector3 mVelocity = Vector3.zero;

        /// <summary>
        /// 前進移動の設定
        /// </summary>
        public void SetVelocityUp()
        {
            mVelocity = new Vector3(0.0f, 0.0f, mCurrentSpeed);
        }

        /// <summary>
        /// 後退移動の設定
        /// </summary>
        public void SetVelocityDown()
        {
            mVelocity = new Vector3(0.0f, 0.0f, -mCurrentSpeed);
        }

        /// <summary>
        /// 移動力をリセット
        /// </summary>
        public void ResetVelocity()
        {
            mVelocity = Vector3.zero;
        }

        /// <summary>
        /// 速度アップ
        /// </summary>
        public void UpSpeed()
        {
            mCurrentSpeed += SPEED_COEFFICIENT;
            if (mCurrentSpeed > SPEED_LIMIT) mCurrentSpeed = SPEED_LIMIT;
            mVelocity = new Vector3(0.0f, 0.0f, mCurrentSpeed);
        }

        /// <summary>
        /// 速度ダウン
        /// </summary>
        public void DownSpeed()
        {
            mCurrentSpeed -= SPEED_COEFFICIENT;
            if (mCurrentSpeed < SPEED_DEFAULT) mCurrentSpeed = SPEED_DEFAULT;
            mVelocity = new Vector3(0.0f, 0.0f, mCurrentSpeed);
        }

    }
}
