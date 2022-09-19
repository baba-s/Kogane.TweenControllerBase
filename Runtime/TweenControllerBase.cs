using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Kogane
{
    /// <summary>
    /// Tween の準備・再生・完了を管理するコンポーネントの基底クラス
    /// </summary>
    public abstract class TweenControllerBase : MonoBehaviour
    {
        //================================================================================
        // 変数
        //================================================================================
        private Tween m_tween;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// 準備します
        /// </summary>
        [Button( enabledMode: EButtonEnableMode.Playmode )]
        public void Ready()
        {
            // Tween を再生して即座に一時停止することで
            // Tween の開始前の状態を反映します
            m_tween?.Complete();
            m_tween = PlayCore().Pause();
        }

        /// <summary>
        /// 再生します
        /// </summary>
        [Button( enabledMode: EButtonEnableMode.Playmode )]
        public void Play()
        {
            PlayAsync().Forget();
        }

        /// <summary>
        /// 再生します
        /// </summary>
        public async UniTask PlayAsync()
        {
            // すでに Ready で Tween を作成している場合は
            // 一時停止を再開します
            if ( m_tween != null )
            {
                _ = m_tween.Play();
            }
            // Ready していない場合は新規で Tween を作成して開始します
            else
            {
                m_tween = PlayCore();
            }

            // 演出再生中にシーンを抜けても例外が発生しないように
            // 自分自身を紐付けておきます
            var cancellationToken = gameObject.GetCancellationTokenOnDestroy();

            await m_tween
                    .SetLink( gameObject )
                    .ToUniTask( TweenCancelBehaviour.KillAndCancelAwait, cancellationToken )
                ;

            // PlayAsync を再度できるように
            // 再生が完了した Tween の参照を破棄します
            m_tween = null;
        }

        /// <summary>
        /// 完了します
        /// </summary>
        [Button( enabledMode: EButtonEnableMode.Playmode )]
        public void Complete()
        {
            // Tween が完了した時の状態を反映します
            m_tween ??= PlayCore();
            m_tween.Complete();
            m_tween = null;
        }

        /// <summary>
        /// 派生クラスで Tween を作成して返す処理を記述します
        /// </summary>
        protected abstract Tween PlayCore();
    }
}