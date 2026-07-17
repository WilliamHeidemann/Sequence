using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Game
{
    public class CardDrawAnimator : MonoBehaviour
    {
        [SerializeField] private UIDocument _drawPileDocument;
        [SerializeField] private VisualTreeAsset _cardBack;
                
        private VisualElement _drawPile;
        
        private void OnEnable()
        {
            _drawPile = _drawPileDocument.rootVisualElement.Q<VisualElement>("Container");
            DrawCard();
        }

        public void DrawCard()
        {
            TemplateContainer card = _cardBack.Instantiate();
            card.style.flexGrow = 1f;
            _drawPile.Add(card);
        }
    }
}
