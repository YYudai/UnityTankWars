using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スレッドセーフ
/// MyConstantsとして、タグ名やプログラム全体にわたる定数を定義
/// 複数シーンにまたがって呼び出されるマネージャーやオブジェクトを保持
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class GameController : MonoBehaviour
    {

        // 初期化タイミングでインスタンスを生成（スレッドセーフ）
        private static readonly GameController mInstance = new GameController();

        // コンストラクタをprivateにすることによって他クラスからnewできないようにする
        private GameController() { }

        // 他クラスからこのマネージャーを参照する
        public static GameController Instance
        {
            get
            {
                return mInstance;
            }
        }

        //--------
        // 定数 //
        //----------------------------------------------------------------------------------------

        // タグ
        public static string TAG_TANK = "Tank";
        public static string TAG_DESTROY_AREA = "DestroyArea";

        //--------
        // 設定 //
        //----------------------------------------------------------------------------------------

        // FixedUpdateの間隔
        private readonly float FIXED_TIME_STEP = 0.03f;

        /// <summary>
        /// ゲームシステムを初期化する
        /// </summary>
        public void InitGame()
        {
            // 更新間隔の設定
            Time.fixedDeltaTime = FIXED_TIME_STEP;

            // 当たり判定の設定
            SetLayerCollision();
        }

        //-----------------------------------------
        // レイヤーでコライダー毎の当たり判定を制御 //
        //----------------------------------------------------------------------------------------

        public static int LAYER_WHEEL = 9;
        public static int LAYER_SUSPENSIONS = 10;
        public static int LAYER_MAIN_BODY = 11;

        private void SetLayerCollision()
        {
            // 初期化：車輪とボディについて、全ての接触を有効にする
            for (int i = 0; i <= 11; i++)
            {
                Physics.IgnoreLayerCollision(LAYER_WHEEL, i, false);
                Physics.IgnoreLayerCollision(LAYER_MAIN_BODY, i, false);
            }

            // 車輪同士の接触は無効
            Physics.IgnoreLayerCollision(LAYER_WHEEL, LAYER_WHEEL, true);

            // 車輪と本体の接触は無効
            Physics.IgnoreLayerCollision(9, 11, true);

            // サスペンションと全ての物体の接触は無効
            for (int i = 0; i <= 11; i++)
            {
                Physics.IgnoreLayerCollision(10, i, true);
            }
        }

        //----------------------
        // システム系入力の処理 //
        //----------------------------------------------------------------------------------------

        /// <summary>
        /// リスタートの処理
        /// </summary>
        public void OnRestartButton()
        {
            // シーンの再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /*
        private bool mIsPause;
        public bool IsPause
        {
            get { return mIsPause; }
        }

        /// <summary>
        /// ポーズまたは再開の処理
        /// </summary>
        public void OnPauseButton()
        {
            mIsPause = !mIsPause;
            if (mIsPause)
            {
                Time.timeScale = 0.0f;
            }else
            {
                Time.timeScale = 1.0f;
            }
        }

        /// <summary>
        /// 終了の処理
        /// </summary>
        public void OnEndButton()
        {
            Application.Quit();
        }
        */

    }
}
