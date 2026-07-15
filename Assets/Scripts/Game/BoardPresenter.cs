using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
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

        public Action<Card> OnCardClicked;

        private Dictionary<Position, Button> _buttons = new();
        private Dictionary<Position, Card> _cards = new();

        private void OnEnable()
        {
            VisualElement root = _uiDocument.rootVisualElement;

            List<VisualElement> visualRows = root.Query<VisualElement>("Row").ToList();

            Row[] rows = BoardLayout.AllRows();

            foreach (var rowPair in visualRows.Zip(rows, (visualRow, row) => new { visualRow, row }))
            {
                List<Button> slots = rowPair.visualRow.Query<Button>("Slot").ToList();

                Column[] columns = BoardLayout.AllColumns();

                Row row = rowPair.row;
                
                foreach (var slotPair in slots.Zip(columns, (visualSlot, column) => new { visualSlot, column }))
                {
                    Button slot = slotPair.visualSlot;
                    
                    Column column = slotPair.column;

                    Card card = BoardLayout.Get(row, column);
                    
                    Sprite sprite = _cardSprites.Get(card);

                    slot.style.backgroundImage = new StyleBackground(sprite);

                    slot.clicked += () =>
                    {
                        Debug.Log(card);
                        OnCardClicked?.Invoke(card);
                    };

                    Position position = new(rowPair.row, slotPair.column);

                    _buttons[position] = slot;

                    _cards[position] = card;
                }
            }
        }

        public void Mark(Position position, Team team)
        {
            _buttons[position].AddToClassList(team == Team.Red ? "red" : "yellow");
        }
    }
}