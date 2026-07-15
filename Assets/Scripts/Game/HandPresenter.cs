using System;
using Game.Models;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class HandPresenter : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        [SerializeField] private CardSprites _cardSprites;

        [CreateProperty] public Sprite First => _cardSprites.Get(_hand.GetCards()[0]);
        [CreateProperty] public Sprite Second => _cardSprites.Get(_hand.GetCards()[1]);
        [CreateProperty] public Sprite Third => _cardSprites.Get(_hand.GetCards()[2]);
        [CreateProperty] public Sprite Fourth => _cardSprites.Get(_hand.GetCards()[3]);
        [CreateProperty] public Sprite Fifth => _cardSprites.Get(_hand.GetCards()[4]);
        [CreateProperty] public Sprite Sixth => _cardSprites.Get(_hand.GetCards()[5]);
        [CreateProperty] public Sprite Seventh => _cardSprites.Get(_hand.GetCards()[6]);
        
        private Hand _hand;

        public void Bind(Hand hand)
        {
            _hand = hand;
            
            var root = _uiDocument.rootVisualElement;

            var slots = root.Query<VisualElement>("Slot").ToList();

            string[] spriteNames =
            {
                nameof(First), nameof(Second), nameof(Third), 
                nameof(Fourth), nameof(Fifth), nameof(Sixth),
                nameof(Seventh)
            };

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSource = this,
                    dataSourcePath = new PropertyPath(spriteNames[i]),
                });
            }
        }
    }
}