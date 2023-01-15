using System;
using System.Collections.Generic;
using System.Threading;
using Cards.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Cards.Loaders
{
    public abstract class CardsLoader : ScriptableObject
    {
        [SerializeField] private string _label;

        private Settings _settings;
        private SignalBus _signalBus;


        [Inject]
        public void Construct(
            Settings settings,
            SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        public string Label => _label;


#pragma warning disable CS1998
        public virtual async UniTask LoadCards(
#pragma warning restore CS1998
            List<Card> cards,
            CancellationToken cancellationToken)
        {
        }

        protected void CompleteLoading()
        {
            _signalBus.Fire<LoadingCompleteSignal>();
        }

        protected async UniTask LoadHeroImageSprite(
            Card card,
            CancellationToken cancellationToken,
            Action<Card> completed = null)
        {
            var request = UnityWebRequestTexture.GetTexture(_settings.URL);

            var operation = await request.SendWebRequest()
                .WithCancellation(cancellationToken);

            if (operation.result is UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(operation);
                card.HeroImageSpriteTexture = texture;
                completed?.Invoke(card);
            }
        }

        [Serializable]
        public class Settings
        {
            public string URL;
        }
    }
}