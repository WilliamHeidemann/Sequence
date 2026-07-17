using System.Collections.Generic;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UtilityToolkit.Editor;

namespace Game
{
    public class CardAligner : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private float _spacing;
        [SerializeField] private float _curve;
        [SerializeField] private float _angle;

        [SerializeField] private Transform[] _previewCards;

        [SerializeField] private List<Transform> _cards = new();

        private void Start()
        {
            _previewCards.ToList().ForEach(card => card.gameObject.SetActive(false));
        }

        public void AddCard(Transform card)
        {
            _cards.Add(card);
            AlignCards(_cards);
        }

        private void AlignCards(List<Transform> cards)
        {
            // 0 -> []
            // 1 -> [0]
            // 2 -> [-.25, .25]
            // 3 -> [-.5, 0, .5]
            // 4 -> [-.75, -.25, .25, .75]
            // 5 -> [-1, -.5, 0, .5, 1]

            float totalWidth = (cards.Count - 1) * _spacing;
            float startX = -totalWidth / 2;
            float endX = totalWidth / 2;

            for (int i = 0; i < cards.Count; i++)
            {
                float t = i / (cards.Count - 1f + 0.001f);
                float x = Mathf.Lerp(startX, endX, t);
                float y = Mathf.Sin(t * Mathf.PI) * _curve;
                Vector3 startPosition = cards[i].position;
                Vector3 targetPosition = _origin.position + new Vector3(x, y, -i * 0.01f);

                LMotion.Create(startPosition, targetPosition, .2f)
                    .WithEase(Ease.InOutCubic)
                    .BindToPosition(cards[i]);

                float angle = Mathf.Lerp(-_angle, _angle, t);
                Quaternion startRotation = cards[i].rotation;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                
                LMotion.Create(startRotation, targetRotation, .2f)
                    .WithEase(Ease.InOutCubic)
                    .BindToRotation(cards[i]);
                
                LMotion.Create(cards[i].localScale, Vector3.one, .2f)
                    .WithEase(Ease.InOutCubic)
                    .BindToLocalScale(cards[i]);
            }
        }

        [Button]
        private void AlignEditMode()
        {
            AlignCards(_previewCards.ToList());
        }
    }
}