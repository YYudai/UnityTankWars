using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内UIを管理・操作
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class UiManager : MonoBehaviour
    {

        //---------------------
        // センターのテキスト //
        //----------------------------------------------------------------------------------------

        [SerializeField]
        private Image mPanelCenterMsg;
        [SerializeField]
        private Text mTextCenterMsg;
        private readonly float VALUES_FLASH_CENTER_MSG_WAIT = 1.5f;
        private readonly float VALUES_FLASH_CENTER_MSG_ALPHA = 1.4f;
        private int mCenterMsgState;
        private float mCenterMsgWait;
        private float mCenterMsgAlpha = 1.0f;

        /// <summary>
        /// センターテキストの点滅アニメーション
        /// </summary>
        public void UpdateCenterMsg()
        {
            switch (mCenterMsgState)
            {
                case 0: // フェードアウト開始まで待機
                    mCenterMsgWait += Time.deltaTime;
                    if (mCenterMsgWait >= VALUES_FLASH_CENTER_MSG_WAIT) mCenterMsgState = 1;
                    break;
                case 1: // フェードアウト（完全には消さない）
                    mCenterMsgAlpha -= VALUES_FLASH_CENTER_MSG_ALPHA * Time.deltaTime;
                    if(mCenterMsgAlpha <= 0.1f)
                    {
                        mCenterMsgAlpha = 0.1f;
                        mTextCenterMsg.color = new Color(1.0f, 1.0f, 1.0f, mCenterMsgAlpha);
                        mCenterMsgState = 2;
                    }else
                    {
                        mTextCenterMsg.color = new Color(1.0f, 1.0f, 1.0f, mCenterMsgAlpha);
                    }
                    break;
                case 2: // 半透明状態からフェードイン
                    mCenterMsgAlpha += VALUES_FLASH_CENTER_MSG_ALPHA * Time.deltaTime;
                    if(mCenterMsgAlpha >= 1.0f)
                    {
                        mCenterMsgAlpha = 1.0f;
                        mTextCenterMsg.color = new Color(1.0f, 1.0f, 1.0f, mCenterMsgAlpha);
                        // 待機時間とstateをリセット
                        mCenterMsgWait = 0;
                        mCenterMsgState = 0;
                    }else
                    {
                        mTextCenterMsg.color = new Color(1.0f, 1.0f, 1.0f, mCenterMsgAlpha);
                    }
                    break;
            }
        }

        /// <summary>
        /// センターテキストを背景のパネルごと非表示にする
        /// </summary>
        public void HideCenterMsg()
        {
            mPanelCenterMsg.gameObject.SetActive(false);
        }

        //-----------------
        // ボトムテキスト //
        //----------------------------------------------------------------------------------------

        
        //-----------------
        // カウントダウン //
        //----------------------------------------------------------------------------------------

        [SerializeField]
        private Text mTextCountDown;
        private readonly string[] COUNT_ARRAY = { "2", "1", "FIRE!" };
        
        private int mCurrentIndex;
        private float mDurationTime;

        /// <summary>
        /// カウントダウンアクション
        /// </summary>
        /// <returns>アクション完了でtrueを返す</returns>
        public bool UpdateCountDown()
        {
            switch (mCurrentIndex)
            {
                case 0: // カウントダウンテキストを表示
                    mTextCountDown.gameObject.SetActive(true);
                    mDurationTime = 1.0f;
                    mCurrentIndex++;
                    break;
                case 1: // 2を表示
                    mDurationTime -= Time.deltaTime;
                    if (mDurationTime <= 0.0f)
                    {
                        mTextCountDown.text = COUNT_ARRAY[0];
                        mDurationTime += 1.0f;
                        mCurrentIndex++;
                    }
                    break;
                case 2: // 1を表示
                    mDurationTime -= Time.deltaTime;
                    if (mDurationTime <= 0.0f)
                    {
                        mTextCountDown.text = COUNT_ARRAY[1];
                        mDurationTime += 1.0f;
                        mCurrentIndex++;
                    }
                    break;
                case 3: // Fire!を表示
                    mDurationTime -= Time.deltaTime;
                    if (mDurationTime <= 0.0f)
                    {
                        mTextCountDown.text = COUNT_ARRAY[2];

                        // コルーチンを使った遅延処理でテキストを1秒後に非表示にする
                        StartCoroutine(HideCountDownText());
                        return true;
                    }
                    break;
            }
            return false;
        }

        private IEnumerator HideCountDownText()
        {
            yield return new WaitForSeconds(1.0f);
            mTextCountDown.gameObject.SetActive(false);
        }

        //------------
        // 結果表示 // カウントダウンのTextを流用して結果を表示
        //----------------------------------------------------------------------------------------

        private readonly string WIN = "WIN!", LOSE = "LOSE", DRAW = "DRAW";

        public void ShowWin()
        {
            mTextCountDown.text = WIN;
            mTextCountDown.gameObject.SetActive(true);
        }

        public void ShowLose()
        {
            mTextCountDown.text = LOSE;
            mTextCountDown.gameObject.SetActive(true);
        }

        public void ShowDraw()
        {
            mTextCountDown.text = DRAW;
            mTextCountDown.gameObject.SetActive(true);
        }

    }
}
