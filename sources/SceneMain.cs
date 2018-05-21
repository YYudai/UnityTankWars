using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jp.Yzroid.CsgTankWars
{
    public class SceneMain : MonoBehaviour
    {

        private GameController mGame;
        [SerializeField]
        [Tooltip("戦車マネージャー")]
        private TankManager mTank;
        [SerializeField]
        [Tooltip("UIマネージャー")]
        private UiManager mUi;

        // ゲームコントローラーの参照を保持し、ゲームシステムを初期化する
        void Awake()
        {
            mGame = GameController.Instance;
            mGame.InitGame();
        }

        //--------
        // 状態 //
        //----------------------------------------------------------------------------------------

        private enum STATE
        {
            WAIT_ENTER_KEY = 0,
            COUNT_DOWN,
            PLAY,
            RESULT
        };
        private STATE mState = STATE.WAIT_ENTER_KEY;

        //--------
        // 更新 //
        //----------------------------------------------------------------------------------------

        void Update()
        {
            CheckSystemInput();
            switch (mState)
            {
                case STATE.WAIT_ENTER_KEY: // enterが押されたらカウントダウン開始
                    mUi.UpdateCenterMsg(); // 中央メッセージの点滅アニメーション
                    if (WaitEnter())
                    {
                        mUi.HideCenterMsg();
                        mState = STATE.COUNT_DOWN;
                    }
                    break;
                case STATE.COUNT_DOWN: // カウントダウン
                    if (mUi.UpdateCountDown())
                    {
                        mTank.RegisterTanks();
                        mTank.OnActiveAllTanks();
                        mState = STATE.PLAY;
                    }
                    break;
                case STATE.PLAY: // ゲームプレイ中
                    CheckResult();
                    break;
            }
        }

        //-------------------
        // ゲームクリア判定 //
        //----------------------------------------------------------------------------------------

        private void CheckResult()
        {
            switch (mTank.CheckResult())
            {
                case TankManager.RESULT_WIN:
                    mUi.ShowWin();
                    break;
                case TankManager.RESULT_LOSE:
                    mUi.ShowLose();
                    break;
                case TankManager.RESULT_DRAW:
                    mUi.ShowDraw();
                    break;
                default:
                    return;
            }
            mTank.StopAllTanks();
            mState = STATE.RESULT;
        }

        //--------
        // 入力 //
        //----------------------------------------------------------------------------------------

        private void CheckSystemInput()
        {
            // リスタート
            if (Input.GetKeyDown(KeyCode.Backspace)) mGame.OnRestartButton();
        }

        private bool WaitEnter()
        {
            if (Input.GetKeyDown(KeyCode.Return)) return true;
            return false;
        }

    }
}
