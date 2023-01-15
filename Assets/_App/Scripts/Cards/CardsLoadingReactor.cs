using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cards.Loaders;
using Cards.UI;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Cards
{
    public class CardsLoadingReactor : IInitializable
    {
        private readonly CardsContainer _cardsContainer;
        private readonly List<CardsLoader> _cardsLoaders;
        private readonly SignalBus _signalBus;
        private readonly List<UniTask> _tasks = new();
        private CancellationTokenSource _cancellationTokenSource;


        public CardsLoadingReactor(
            SignalBus signalBus,
            List<CardsLoader> cardsLoaders,
            CardsContainer cardsContainer)
        {
            _signalBus = signalBus;
            _cardsLoaders = cardsLoaders;
            _cardsContainer = cardsContainer;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LoadCommand>(signal => LoadCards(signal.LoaderId));
            _signalBus.Subscribe<CancelLoadingCommand>(CancelLoading);
        }

        private void CancelLoading()
        {
            _cancellationTokenSource.Cancel();
            FLipAllCardsDown();
        }

        private async Task FLipAllCardsDown()
        {
            _tasks.Clear();

            foreach (var card in _cardsContainer.Cards)
                _tasks.Add(card.ShowBack());

            await UniTask.WhenAll(_tasks);
        }

        private async UniTask LoadCards(int loaderId)
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            await FLipAllCardsDown();

            var cardsLoader = _cardsLoaders[loaderId];
            await cardsLoader.LoadCards(
                    _cardsContainer.Cards,
                    _cancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
    }
}