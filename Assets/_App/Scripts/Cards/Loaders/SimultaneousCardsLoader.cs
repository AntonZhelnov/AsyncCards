using System.Collections.Generic;
using System.Threading;
using Cards.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cards.Loaders
{
    [CreateAssetMenu(
        fileName = "New Simultaneous",
        menuName = "Cards Loaders/Simultaneous")]
    public class SimultaneousCardsLoader : CardsLoader
    {
        private readonly List<UniTask> _tasks = new();


        public override async UniTask LoadCards(
            List<Card> cards,
            CancellationToken cancellationToken)
        {
            _tasks.Clear();

            foreach (var card in cards)
                _tasks.Add(LoadHeroImageSprite(card, cancellationToken));

            await UniTask.WhenAll(_tasks);

            foreach (var card in cards)
                card.ShowFront(cancellationToken);

            CompleteLoading();
        }
    }
}