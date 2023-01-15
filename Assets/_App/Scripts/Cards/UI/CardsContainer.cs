using System.Collections.Generic;
using UnityEngine;

namespace Cards.UI
{
    public class CardsContainer : MonoBehaviour
    {
        [SerializeField] private List<Card> _cards;


        public List<Card> Cards => _cards;
    }
}