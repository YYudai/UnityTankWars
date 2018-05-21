using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトにライフタイムを実装
/// エフェクトなど、一定時間経過で自動的に削除されるようなオブジェクトに使用する
/// </summary>
namespace Jp.Yzroid.CsgTankWars
{
    public class DestroyTimer : MonoBehaviour
    {

        [Header("[ オブジェクトにライフタイムを実装 ]")]
        [Space(8)]
        [SerializeField]
        private float lifeTime = 2.0f;

        void Awake()
        {
            Destroy(this.gameObject, lifeTime);
        }

    }
}
