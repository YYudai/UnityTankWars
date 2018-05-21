using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全ての戦車モデルを一元管理する
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class TankManager : MonoBehaviour
    {
        private List<TankModel> mTankList = new List<TankModel>();

        /// <summary>
        /// Tankタグのついたオブジェクトを戦車としてリスト保持する
        /// </summary>
        public void RegisterTanks()
        {
            GameObject[] tank = GameObject.FindGameObjectsWithTag(GameController.TAG_TANK);
            foreach (GameObject model in tank)
            {
                mTankList.Add(model.GetComponent<TankModel>());
            }
        }

        /// <summary>
        /// 全ての戦車を操作可能にする
        /// ゲーム開始時に一度だけ呼び出される
        /// </summary>
        public void OnActiveAllTanks()
        {
            foreach(TankModel model in mTankList)
            {
                model.OnActive();
            }
        }

        /// <summary>
        /// 全ての戦車を停止する
        /// 勝敗が決まった際に一度だけ呼び出される
        /// </summary>
        public void StopAllTanks()
        {
            foreach (TankModel model in mTankList)
            {
                model.Stop();
            }
        }

        //--------
        // 判定 //
        //----------------------------------------------------------------------------------------

        public const int RESULT_WIN = 1, RESULT_LOSE = 2, RESULT_DRAW = 3;

        /// <summary>
        /// ゲームの終了判定
        /// </summary>
        /// <returns>WIN = 1, LOSE = 2, DRAW = 3, OTHERS = -1</returns>
        public int CheckResult()
        {
            // 個別の死亡判定
            bool playerDead = false;
            bool enemyDead = false;
            foreach(TankModel model in mTankList)
            {
                if (model.IsDead)
                {
                    if (model.IsPlayer)
                    {
                        playerDead = true;
                    }else
                    {
                        enemyDead = true;
                    }
                }
            }

            // 結果判定
            if (playerDead)
            {
                if (enemyDead)
                {
                    return RESULT_DRAW;
                }else
                {
                    return RESULT_LOSE;
                }
            }else if (enemyDead)
            {
                return RESULT_WIN;
            }
            return -1;
        }

    }
}
