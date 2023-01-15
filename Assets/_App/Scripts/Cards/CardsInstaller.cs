using System.Collections.Generic;
using Cards.Loaders;
using Cards.UI;
using UnityEngine;
using Zenject;

namespace Cards
{
    [CreateAssetMenu(
        fileName = "New Cards",
        menuName = "Installers/Cards")]
    public class CardsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private List<CardsLoader> _cardsLoaders;
        [SerializeField] private CardsLoader.Settings _cardsLoaderSettings;
        [SerializeField] private Card.Settings _cardSettings;


        public override void InstallBindings()
        {
            InstallSignals();
            InstallCardsLoaders();

            Container.BindInstance(_cardSettings).WhenInjectedInto<Card>();

            Container.BindInterfacesTo<CardsLoadingReactor>().AsSingle().NonLazy();
        }

        private void InstallCardsLoaders()
        {
            Container.BindInstance(_cardsLoaderSettings).AsSingle();
            Container.BindInstance(_cardsLoaders).AsSingle();

            foreach (var cardsLoader in _cardsLoaders)
                Container.QueueForInject(cardsLoader);
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LoadCommand>();
            Container.DeclareSignal<CancelLoadingCommand>();
            Container.DeclareSignal<LoadingCompleteSignal>();
        }
    }
}