using System.Collections.Generic;
using System.Threading;
using Cards.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cards.Loaders
{
    [CreateAssetMenu(
        fileName = "New Immediate",
        menuName = "Cards Loaders/Immediate")]
    public class ImmediateCardsLoader : CardsLoader
    {
        private readonly List<UniTask> _tasks = new();


        public override async UniTask LoadCards(
            List<Card> cards,
            CancellationToken cancellationToken)
        {
            _tasks.Clear();

            foreach (var card in cards)
            {
                void Completed(Card loadedCard)
                {
                    ShowCardFront(loadedCard, cancellationToken);
                }

                var task = LoadHeroImageSprite(
                    card,
                    cancellationToken,
                    Completed
                );

                _tasks.Add(task);
            }

            await UniTask.WhenAll(_tasks);
            CompleteLoading();
        }

        private static void ShowCardFront(
            Card card,
            CancellationToken cancellationToken)
        {
            card.ShowFront(cancellationToken);
        }
    }
}