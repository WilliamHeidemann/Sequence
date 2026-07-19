using Game.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class OpponentHandAnimator : MonoBehaviour
    {
        [SerializeField] private UIDocument _cardPrefab;
        [SerializeField] private DiscardPile _discardPile;
        [SerializeField] private CardSprites _cardSprites;

        public async Awaitable AnimatePlay(Card card)
        {
            Quaternion startingRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f)));
        
            UIDocument document = Instantiate(_cardPrefab, transform.position, startingRotation);

            document.rootVisualElement.Q<VisualElement>("Card").style.backgroundImage =
                new StyleBackground(_cardSprites.Get(card));

            await _discardPile.Discard(document.transform);
        }
    }
}