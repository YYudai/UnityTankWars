using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦車モデルに弾発射を実装
/// マウス左クリックで弾を飛ばす
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class FireController : MonoBehaviour
    {
        [Header("[ 弾の発射アクションを実装 ]")]
        [Space(8)]
        [SerializeField]
        [Tooltip("弾オブジェクトを一か所にまとめるための空オブジェクト")]
        private Transform mTransBulletGroup; // 弾オブジェクトのルートとなる
        [SerializeField]
        [Tooltip("弾の発射地点")]
        private Transform mTransFirePoint; // 発射時の弾とエフェクトについて、位置と方向を決定する
        [SerializeField]
        [Tooltip("発射エフェクト")]
        private GameObject mMuzzleFirePrefab;
        [SerializeField]
        [Tooltip("弾")]
        private GameObject mBulletPrefab;


        //------------------------------
        // Called by TankModel#Update //
        //----------------------------------------------------------------------------------------

        public void CheckInput()
        {
            if (Input.GetMouseButtonDown(0)) Fire();
        }

        //-----------
        // 弾の速度 //
        //----------------------------------------------------------------------------------------

        // 速度
        private const float MAX_BULLET_SPEED = 100.0f, MIN_BULLET_SPEED = 30.0f;
        private readonly float UNIT_BULLET_SPEED = 2.0f;
        private float mBulletSpeed = 40.0f; // 初期値

        public void UpBulletSpeed()
        {
            mBulletSpeed += UNIT_BULLET_SPEED;
            Mathf.Clamp(mBulletSpeed, MIN_BULLET_SPEED, MAX_BULLET_SPEED);
        }

        public void DownBulletSpeed()
        {
            mBulletSpeed -= UNIT_BULLET_SPEED;
            Mathf.Clamp(mBulletSpeed, MIN_BULLET_SPEED, MAX_BULLET_SPEED);
        }

        //-----------
        // 弾の威力 //
        //----------------------------------------------------------------------------------------

        private int mBulletAttack = 10;

        //-------------
        // 弾数の制限 // ボンバーマンの爆弾のように、一度に出せる弾には限界がある
        //----------------------------------------------------------------------------------------

        private readonly int MAX_BULLET_COUNT = 5; // 弾数のパワーアップ限界値
        private int mBulletCount = 3; // 発射できる弾数

        public void UpBulletCount()
        {
            mBulletCount++;
        }

        //--------
        // 発射 //
        //----------------------------------------------------------------------------------------

        private readonly float BULLET_OFFSET = 1.0f;
        private List<BulletModel> mBulletList = new List<BulletModel>(); // 生成された弾オブジェクトをプーリング

        public void Fire()
        {
            // 弾数が残っているならば発射処理
            if (mBulletCount > 0)
            {
                // 所持弾数をデクリメント
                mBulletCount--;

                // 発射エフェクト生成
                CreateMuzzleFire();

                // 弾生成
                CreateBullet();
            }
        }

        /// <summary>
        /// 発射時のエフェクト生成
        /// </summary>
        private void CreateMuzzleFire()
        {
            GameObject muzzleFire = Instantiate(mMuzzleFirePrefab, mTransFirePoint.position, mTransFirePoint.rotation) as GameObject;
            muzzleFire.transform.parent = mTransFirePoint;
        }

        /// <summary>
        /// 弾オブジェクトの生成
        /// </summary>
        private void CreateBullet()
        {
            // 休眠状態の弾オブジェクトがある場合はそれを再利用する
            foreach(BulletModel model in mBulletList)
            {
                if (model.IsSleep)
                {
                    model.Respawn(mBulletAttack, mBulletSpeed, mTransFirePoint.position + mTransFirePoint.forward * BULLET_OFFSET, mTransFirePoint.rotation);
                    return;
                }
            }

            // 再利用できるオブジェクトが無かった場合は新しく生成してリストに格納する
            GameObject bulletGo = Instantiate(mBulletPrefab, mTransFirePoint.position + mTransFirePoint.forward * BULLET_OFFSET, mTransFirePoint.rotation) as GameObject;
            bulletGo.transform.parent = mTransBulletGroup;
            BulletModel bulletModel = bulletGo.GetComponent<BulletModel>();
            bulletModel.SetDelegate(this);
            bulletModel.Spawn(mBulletAttack, mBulletSpeed);
            mBulletList.Add(bulletModel);
        }

        //--------
        // 停止 //
        //----------------------------------------------------------------------------------------

        /// <summary>
        /// 休眠状態でない全ての弾オブジェクトを休眠状態にする
        /// </summary>
        public void SleepAllBullets()
        {
            foreach(BulletModel model in mBulletList)
            {
                if (!model.IsSleep) model.Sleep();
            }
        }
        //---------------------
        // デリゲート受け取り //
        //----------------------------------------------------------------------------------------

        /// <summary>
        /// 発射した弾が休眠状態になった際に、残弾数を1つ回復する
        /// called by BulletModel
        /// </summary>
        public void OnSleepBullet()
        {
            mBulletCount++;
        }

    }
}
