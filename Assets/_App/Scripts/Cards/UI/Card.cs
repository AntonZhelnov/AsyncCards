using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Cards.UI
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image _heroImage;
        [SerializeField] private CanvasGroup _front;
        [SerializeField] private CanvasGroup _back;

        private int _flipDurationMs;
        private float _halfFlipDuration;
        private bool _isShowingFront;
        private float _quarterFlipDuration;
        private Settings _settings;


        [Inject]
        public void Construct(Settings settings)
        {
            _settings = settings;
        }

        public Texture2D HeroImageSpriteTexture
        {
            set
            {
                DestroyHeroImageSpriteAndTexture();
                _heroImage.sprite = CreateSprite(value);
            }
        }

        private void Start()
        {
            _halfFlipDuration = _settings.FlipDuration / 2f;
            _quarterFlipDuration = _halfFlipDuration / 2f;
        }

        public async UniTask ShowBack()
        {
            if (_isShowingFront)
            {
                _isShowingFront = false;

                await AnimateFlip(
                    _quarterFlipDuration,
                    false);
            }
        }

        public async UniTask ShowFront(CancellationToken cancellationToken)
        {
            if (_isShowingFront is false)
            {
                _isShowingFront = true;

                await AnimateFlip(
                    _halfFlipDuration,
                    true,
                    cancellationToken);
            }
        }

        private async UniTask AnimateFlip(
            float halfFlipDuration,
            bool toFront,
            CancellationToken cancellationToken = default)
        {
            var flipSequence = DOTween.Sequence();
            flipSequence
                .Append(transform.DOScaleX(0f, halfFlipDuration))
                .Join(transform.DOScaleY(1.2f, halfFlipDuration))
                .AppendCallback(() =>
                {
                    _front.alpha = toFront ? 1f : 0f;
                    _back.alpha = toFront ? 0f : 1f;
                })
                .Append(transform.DOScaleX(1f, halfFlipDuration))
                .Join(transform.DOScaleY(1f, halfFlipDuration))
                .SetLink(gameObject)
                .Play();

            var isCanceled = await UniTask.Delay(
                    TimeSpan.FromSeconds(_settings.FlipDuration),
                    DelayType.Realtime,
                    PlayerLoopTiming.Update,
                    cancellationToken)
                .SuppressCancellationThrow();

            if (isCanceled)
                flipSequence.Kill();
        }

        private static Sprite CreateSprite(Texture2D value)
        {
            var sprite = Sprite.Create(
                value,
                new Rect(0, 0, value.width, value.height),
                new Vector2(0.5f, 0.5f));
            return sprite;
        }

        private void DestroyHeroImageSpriteAndTexture()
        {
            if (_heroImage.sprite)
            {
                if (_heroImage.sprite.texture)
                    Destroy(_heroImage.sprite.texture);
                Destroy(_heroImage.sprite);
            }
        }

        [Serializable]
        public class Settings
        {
            public float FlipDuration = .5f;
        }
    }
}