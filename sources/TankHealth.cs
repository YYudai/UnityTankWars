using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦車モデルにHPの概念を実装
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class TankHealth : MonoBehaviour
    {
        private enum STATE
        {
            DEFAULT = 0,
            DAMAGE,
            HEAL
        }
        private STATE mState = STATE.DEFAULT;

        //---------------
        // ダメージ処理 //
        //----------------------------------------------------------------------------------------

        [Header("[ HPの概念を実装 ]")]
        [Space(8)]
        [SerializeField]
        [Tooltip("HPの初期値")]
        private int mHp = 100;
        private int mDamage;

        /// <summary>
        /// 戦車に被弾ダメージを与える
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>HPが0になった場合はtrueを返す</returns>
        public bool AddDamage(int damage)
        {
            mDamage += damage;
            if (mDamage > mHp) mDamage = mHp;
            CalHealthBarRate();

            if (mDamage >= mHp) return true;
            return false; 
        }

        //----------
        // HPバー //
        //----------------------------------------------------------------------------------------

        private readonly float UNIT_FILL_AMOUNT = 1.0f;

        [SerializeField]
        private Image mHealthBar;
        private float mCurrentFillAmount = 1.0f; // 現在描画されているHPバーのfillAmount
        private float mPreFillAmount; // 増減アクションの終了条件となるHPバーのfillAmount

        /// <summary>
        /// 残りHPに合わせてHPバーの描画割合を決定し、適切なmStateを設定する
        /// </summary>
        private void CalHealthBarRate()
        {
            float currentHp = (float)(mHp - mDamage);
            mPreFillAmount = Mathf.Clamp(currentHp / mHp, 0.0f, 1.0f);
            if (mPreFillAmount < mCurrentFillAmount)
            {
                mState = STATE.DAMAGE;
            }else if(mPreFillAmount > mCurrentFillAmount)
            {
                mState = STATE.HEAL;
            }else
            {
                mState = STATE.DEFAULT;
            }
        }

        public void RenewHealthBar()
        {
            switch (mState)
            {
                case STATE.DAMAGE:
                    mCurrentFillAmount -= UNIT_FILL_AMOUNT * Time.deltaTime;
                    if (mCurrentFillAmount < mPreFillAmount)
                    {
                        mCurrentFillAmount = mPreFillAmount;
                        mHealthBar.fillAmount = mCurrentFillAmount;
                        mState = STATE.DEFAULT;
                        return;
                    }
                    mHealthBar.fillAmount = mCurrentFillAmount;
                    break;
            }
        }

    }
}
