using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using Column = Game.Models.Column;
using Position = Game.Models.Position;

namespace Game
{
    public class BoardPresenter : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private CardSprites _cardSprites;
        [SerializeField] private Material _cardShader;
        [SerializeField] private Transform _redPinPrefab;
        [SerializeField] private Transform _yellowPinPrefab;
        [SerializeField] private Transform _pinStartTRS;
        [SerializeField] private Transform _pinEndRotationScale;
        [SerializeField] private PinGrid _pinGrid;

        public event Func<Position, Awaitable> OnPositionClicked;

        private readonly Dictionary<Position, Button> _buttons = new();

        private void OnEnable()
        {
            VisualElement root = _uiDocument.rootVisualElement;

            List<VisualElement> visualRows = root.Query<VisualElement>("Row").ToList();

            Row[] rows = BoardLayout.AllRows();

            var rowPairs = visualRows.Zip(rows, (visualRow, row) => new { visualRow, row });

            foreach (var rowPair in rowPairs)
            {
                Row row = rowPair.row;

                List<VisualElement> slots = rowPair.visualRow.Query<VisualElement>("CardSlot").ToList();

                Column[] columns = BoardLayout.AllColumns();

                foreach (var slotPair in slots.Zip(columns, (visualSlot, column) => new { visualSlot, column }))
                {
                    Column column = slotPair.column;

                    Position position = new(row, column);

                    Button slot = slotPair.visualSlot.Q<Button>("Slot");

                    Card card = BoardLayout.Get(position);

                    slot.style.backgroundImage = new StyleBackground(_cardSprites.Get(card));

                    Label label = slot.Q<Label>();

                    label.text = card.Rank.AsSingleDigit();


                    slot.clicked += () => OnPositionClicked?.Invoke(position);

                    _buttons[position] = slot;
                }
            }
        }

        public void Shake(Position position)
        {
            Button slot = _buttons[position];
            ShakeElement(slot);
        }

        private void ShakeElement(VisualElement element)
        {
            // 1st arg: Center offset, 2nd arg: Max displacement radius, 3rd arg: Duration
            LMotion.Shake.Create(Vector3.zero, new Vector3(6f, 6f, 0f), 0.15f)
                .WithFrequency(10) // High frequency = very fast shake
                .WithDampingRatio(1f) // Smoothly dampens out to zero
                .Bind(offset => { element.style.translate = new Translate(offset.x, offset.y, 0f); })
                .AddTo(gameObject);
        }

        public void Pop(Position position)
        {
            Button slot = _buttons[position];
            slot.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));

            Vector2 normalScale = Vector2.one;
            Vector2 stretchedScale = new Vector2(1.2f, 1.2f);

            slot.style.scale = new Scale(normalScale);

            LMotion.Create(normalScale, stretchedScale, 0.1f)
                .WithEase(Ease.OutQuad)
                .WithOnComplete(() =>
                {
                    LMotion.Create(stretchedScale, normalScale, 0.2f)
                        .WithEase(Ease.OutElastic)
                        .Bind(val => slot.style.scale = new Scale(val));
                })
                .Bind(val => slot.style.scale = new Scale(val));
        }

        public async Awaitable Pin(Position position, Team team)
        {
            var prefab = team == Team.Red ? _redPinPrefab : _yellowPinPrefab;
            var pin = Instantiate(prefab, _pinStartTRS.position, _pinStartTRS.rotation);
            pin.localScale = _pinStartTRS.localScale;

            var endPosition = _pinGrid.Get(position);

            const float duration = 1f;

            LMotion.Create(pin.position, endPosition, duration)
                .WithEase(Ease.InOutCubic)
                .BindToPosition(pin);

            LMotion.Create(pin.rotation, _pinEndRotationScale.rotation, duration)
                .WithEase(Ease.InOutCubic)
                .BindToRotation(pin);

            LMotion.Create(pin.localScale, _pinEndRotationScale.localScale, duration)
                .WithEase(Ease.InOutCubic)
                .BindToLocalScale(pin);

            await Awaitable.WaitForSecondsAsync(duration);
        }
    }
}