using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦車モデルに砲台アクションを実装
/// マウスの現在位置に砲台を向ける
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class TurretController : MonoBehaviour
    {

        [Header("[ 砲台の回転アクションを実装 ]")]
        [Space(8)]
        [SerializeField]
        [Tooltip("砲台部分")]
        private Transform mTurretTrans;

        private Plane mPlane; // マウス位置を取得するために使用する仮の地面オブジェクト
        private float mDistance; // カメラからマウス位置に向かってRayを飛ばした際にmPlaneと接触するまでの距離

        void Awake()
        {
            // カメラ~マウス座標間のRayが接触する仮地面を予め生成しておく
            mPlane = new Plane();
            mPlane.SetNormalAndPosition(Vector3.up, Vector3.zero);
        }

        //---------------------------------------------
        // Called by TankModel#Update or FixedUpdate //
        //----------------------------------------------------------------------------------------

        private Vector3 mTurretRotation = Vector3.zero;

        /// <summary>
        /// 砲台をマウスポインタの座標に向ける場合のRotation値(Vector3)を計算する
        /// </summary>
        public void CalRotation()
        {
            // カメラからマウス位置に対してRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (mPlane.Raycast(ray, out mDistance))
            {
                // Rayと仮地面との接点を取得
                Vector3 mTargetPos = ray.GetPoint(mDistance);

                // 対象座標 - 観測座標 = 観測座標から対象座標へ向かうベクトル
                Vector3 direction = mTargetPos - mTurretTrans.position;

                // 砲台の正面が対象を向くための回転角度を計算
                Quaternion quaternion = Quaternion.FromToRotation(transform.forward, direction);

                // QuaternionをVector3に変換し、ここで角度を制限する
                mTurretRotation = quaternion.eulerAngles;
                mTurretRotation.x = 0.0f;
                mTurretRotation.z = 0.0f;
                // Y軸の値について、インスペクタ上のRotationでは-180~180の表記だが、コード上では0~360で計算されている
                if (mTurretRotation.y < 30.0f)
                {
                    mTurretRotation.y = 30.0f;
                }
                else if (mTurretRotation.y > 270.0f)
                {
                    mTurretRotation.y = 30.0f;
                }
                else if (mTurretRotation.y > 150.0f)
                {
                    mTurretRotation.y = 150.0f;
                }
            }
        }

        /// <summary>
        /// 砲台を与えられた対象に向ける場合のRotation値(Vector3)を計算する
        /// </summary>
        /// <param name="target"></param>
        public void CalRotation(Transform target)
        {
            // 対象座標 - 観測座標 = 観測座標から対象座標へ向かうベクトル
            Vector3 direction = target.position - mTurretTrans.position;

            // 砲台の正面が対象を向くための回転角度を計算
            Quaternion quaternion = Quaternion.FromToRotation(transform.forward, direction);

            // QuaternionをVector3に変換し、ここでx,z角度を制限する(yは必ず範囲内になるため制限の必要がない)
            mTurretRotation = quaternion.eulerAngles;
            mTurretRotation.x = 0.0f;
            mTurretRotation.z = 0.0f;
        }

        /// <summary>
        /// 計算されたmNextRotationをTransform.rotationに適用する
        /// </summary>
        public void ApplyRotation()
        {
            mTurretTrans.rotation = Quaternion.Euler(mTurretRotation);
        }

        //-----------------------------------------------
        // LookAtで回転を行う場合は角度の制限ができない？ //
        //----------------------------------------------------------------------------------------
        /*
        Vector3 mTargetPos;

        private void GetTargetPos2()
        {
            // カメラからマウス位置に対してRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(mPlane.Raycast(ray, out mDistance)){
                // Rayと仮地面との交点を砲台を向ける目標地点として保持する
                // その際に目標地点のy座標を砲台と同一にすることで、LookAt時に砲台のx角度が動かないように制限する
                mTargetPos = ray.GetPoint(mDistance);
                mTargetPos.y = mTurretTrans.position.y;
            }
        }

        private void Rotate2()
        {
            mTurretTrans.LookAt(mTargetPos);
            // ↓このタイミングではLookAt後の角度が取得できない？
            Vector3 tempAngles = mTurretTrans.localEulerAngles;
            float tempY = tempAngles.y;
            Debug.Log("y="+tempY);
            if(tempY > 270.0f)
            {
                tempY = 0.0f;
            }else if(tempY > 180.0f)
            {
                tempY = 180.0f;
            }
            mCurrentRotation = new Vector3(tempAngles.x, tempY, tempAngles.z);
        }
        */

    }
}