using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾モデル
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class BulletModel : MonoBehaviour
    {
        private Transform mTrans;
        private Rigidbody mRigid;

        void Awake()
        {
            mTrans = GetComponent<Transform>();
            mRigid = GetComponent<Rigidbody>();
        }

        // 攻撃力
        private int mAttackValue;

        //------------------------
        // 初期化・休眠・再活性化 //
        //----------------------------------------------------------------------------------------

        // 休眠状態
        private bool mIsSleep;
        public bool IsSleep { get { return mIsSleep; } }

        /// <summary>
        /// 生成時の初期化
        /// </summary>
        /// <param name="attackValue">攻撃力</param>
        /// <param name="bulletSpeed">弾の速さ</param>
        public void Spawn(int attackValue, float bulletSpeed)
        {
            mIsSleep = false;
            mAttackValue = attackValue;
            mRigid.velocity = mTrans.forward * bulletSpeed;
        }

        /// <summary>
        /// 休眠状態から再活性化の初期化
        /// </summary>
        /// <param name="attackValue">攻撃力</param>
        /// <param name="bulletSpeed">弾の速さ</param>
        /// <param name="position">リスポーン位置</param>
        /// <param name="rotation">リスポーン角度</param>
        public void Respawn(int attackValue, float bulletSpeed, Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            mTrans.SetPositionAndRotation(position, rotation);
            mIsSleep = false;
            mAttackValue = attackValue;
            mRigid.velocity = mTrans.forward * bulletSpeed;
        }

        public void Sleep()
        {
            mIsSleep = true;
            mRigid.velocity = Vector3.zero;
            gameObject.SetActive(false);

            // FireControllerに通知
            dOnSleep();
        }

        //----------------
        // Collider判定 //
        //----------------------------------------------------------------------------------------

        [SerializeField]
        [Tooltip("爆発エフェクト小")]
        private GameObject mExplosionSmallPrefab;
        [SerializeField]
        [Tooltip("爆発エフェクト大")]
        private GameObject mExplosionLargePrefab;

        /// <summary>
        /// 弾が領域外に出た際の処理
        /// このオブジェクトを休眠状態に遷移する。
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit(Collider collider)
        {
            if(collider.tag == GameController.TAG_DESTROY_AREA)
            {
                Sleep();
            }
        }

        /// <summary>
        /// 弾が戦車に当たった際の処理
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter(Collider collider)
        {
            if(collider.tag == GameController.TAG_TANK)
            {
                // 被弾した戦車にダメージを与える
                bool IsDead = collider.transform.GetComponent<TankHealth>().AddDamage(mAttackValue);

                if (!IsDead)
                {
                    // おおまかな接触点を取得（OnCollisionEnterとは異なり、triggerでは正確な座標を取得できない：でも着弾点としては十分？）
                    Vector3 hitPos = collider.ClosestPointOnBounds(mTrans.position);

                    // 接触した場所に爆発エフェクトを生成
                    Instantiate(mExplosionSmallPrefab, hitPos, Quaternion.identity);
                }else
                {
                    // HPが無くなった場合は戦車中央のポイントに大きな爆発エフェクトを生成
                    Vector3 hitPos = collider.transform.position;
                    Instantiate(mExplosionLargePrefab, hitPos, Quaternion.identity);

                    // 対象を死亡状態へ
                    collider.transform.GetComponent<TankModel>().IsDead = true;
                }

                // この弾を休眠状態へ
                Sleep();
            }
        }

        //-------------
        // デリゲート //
        //----------------------------------------------------------------------------------------

        private delegate void OnSleep();
        private OnSleep dOnSleep;

        public void SetDelegate(FireController fireController)
        {
            dOnSleep = fireController.OnSleepBullet;
            //dOnSleep = new OnSleep(fireController.OnSleepBullet);
        }

    }
}
