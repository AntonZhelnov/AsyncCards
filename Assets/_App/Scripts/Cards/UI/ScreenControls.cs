using System.Collections.Generic;
using Cards.Loaders;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Cards.UI
{
    public class ScreenControls : MonoBehaviour
    {
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_Dropdown _loadingModeDropdownMenu;

        private List<CardsLoader> _cardsLoaders;
        private int _loadingModeId;
        private SignalBus _signalBus;


        [Inject]
        public void Construct(
            SignalBus signalBus,
            List<CardsLoader> cardsLoaders)
        {
            _signalBus = signalBus;
            _cardsLoaders = cardsLoaders;
        }

        private void Start()
        {
            foreach (var cardsLoader in _cardsLoaders)
                _loadingModeDropdownMenu.options.Add(new TMP_Dropdown.OptionData(cardsLoader.Label));

            _loadButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _signalBus.Fire(new LoadCommand(_loadingModeId));
                    MakeLoadingControlsAvailable(false);
                }).AddTo(this);

            _cancelButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _signalBus.Fire<CancelLoadingCommand>();
                    MakeLoadingControlsAvailable(true);
                }).AddTo(this);

            _loadingModeDropdownMenu.onValueChanged.AsObservable()
                .Subscribe(SelectLoadingMode).AddTo(this);

            _signalBus.Subscribe<LoadingCompleteSignal>(() => MakeLoadingControlsAvailable(true));
        }

        private void MakeLoadingControlsAvailable(bool available)
        {
            _loadingModeDropdownMenu.interactable = available;
            _loadButton.interactable = available;
            _cancelButton.interactable = !available;
        }

        private void SelectLoadingMode(int id)
        {
            _loadingModeId = id;
        }
    }
}