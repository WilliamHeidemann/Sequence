using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using LitMotion;
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
        [SerializeField] private Game _game;

        public Action<Card> OnCardClicked;

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

                List<Button> slots = rowPair.visualRow.Query<Button>("Slot").ToList();

                Column[] columns = BoardLayout.AllColumns();

                foreach (var slotPair in slots.Zip(columns, (visualSlot, column) => new { visualSlot, column }))
                {
                    Column column = slotPair.column;

                    Button slot = slotPair.visualSlot;

                    Card card = BoardLayout.Get(row, column);

                    Sprite sprite = _cardSprites.Get(card);

                    Material material = new(_cardShader);

                    material.SetTexture("_MainTex", sprite.texture);

                    slot.style.unityMaterial = material;

                    slot.clicked += () => OnCardClicked?.Invoke(card);

                    Position position = new(rowPair.row, slotPair.column);

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
                .Bind(offset =>
                {
                    element.style.translate = new Translate(offset.x, offset.y, 0f);
                })
                .AddTo(gameObject);
        }

        public void Mark(Position position, Team team)
        {
            _buttons[position].AddToClassList(team == Team.Red ? "red" : "yellow");
        }

        public void ClearMarks()
        {
            foreach (Button button in _buttons.Values)
            {
                button.RemoveFromClassList("red");
                button.RemoveFromClassList("yellow");
            }
        }
    }
}