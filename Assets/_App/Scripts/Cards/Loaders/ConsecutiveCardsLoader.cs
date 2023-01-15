using System.Collections.Generic;
using System.Threading;
using Cards.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cards.Loaders
{
    [CreateAssetMenu(
        fileName = "New Consecutive",
        menuName = "Cards Loaders/Consecutive")]
    public class ConsecutiveCardsLoader : CardsLoader
    {
        public override async UniTask LoadCards(
            List<Card> cards,
            CancellationToken cancellationToken)
        {
            foreach (var card in cards)
            {
                await LoadHeroImageSprite(card, cancellationToken);
                await card.ShowFront(cancellationToken);
            }

            CompleteLoading();
        }
    }
}