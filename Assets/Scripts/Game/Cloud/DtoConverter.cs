using System;
using System.Linq;
using Game.Models;
using Game.Models.Players;

namespace Game
{
    public class DtoConverter
    {
        public static GameStateData Convert(
            Unity.Services.CloudCode.GeneratedBindings.Game.Models.Players.GameStateData dto)
        {
            return new GameStateData
            {
                Deck = dto.Deck.Select(Convert).ToArray(),
                Moves = dto.Moves.Select(Convert).ToArray(),
                RedHand = dto.RedHand.Select(Convert).ToArray(),
                YellowHand = dto.YellowHand.Select(Convert).ToArray(),
            };
        }

        private static Move Convert(Unity.Services.CloudCode.GeneratedBindings.Game.Models.Players.Move dto)
        {
            return new Move
            {
                Position = Convert(dto.Position),
                Card = Convert(dto.Card),
                Team = Convert(dto.Team),
            };
        }

        private static Card Convert(Unity.Services.CloudCode.GeneratedBindings.Game.Models.Card dto)
        {
            Rank rank = dto.Rank switch
            {
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Ace => Rank.Ace,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Two => Rank.Two,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Three => Rank.Three,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Four => Rank.Four,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Five => Rank.Five,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Six => Rank.Six,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Seven => Rank.Seven,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Eight => Rank.Eight,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Nine => Rank.Nine,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Ten => Rank.Ten,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Jack => Rank.Jack,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.Queen => Rank.Queen,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Rank.King => Rank.King,
                _ => throw new ArgumentOutOfRangeException()
            };

            Symbol symbol = dto.Symbol switch
            {
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Symbol.Sun => Symbol.Sun,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Symbol.Moon => Symbol.Moon,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new Card(symbol, rank);
        }

        private static Position Convert(Unity.Services.CloudCode.GeneratedBindings.Game.Models.Position dto)
        {
            Row row = dto.Row switch
            {
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.One => Row.One,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.Two => Row.Two,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.Three => Row.Three,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.Four => Row.Four,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.Five => Row.Five,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Row.Six => Row.Six,
                _ => throw new ArgumentOutOfRangeException()
            };

            Column column = dto.Column switch
            {
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.One => Column.One,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Two => Column.Two,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Three => Column.Three,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Four => Column.Four,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Five => Column.Five,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Six => Column.Six,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Seven => Column.Seven,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Column.Eight => Column.Eight,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new Position(row, column);
        }

        private static Team Convert(Unity.Services.CloudCode.GeneratedBindings.Game.Models.Team dto)
        {
            return dto switch
            {
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Team.Red => Team.Red,
                Unity.Services.CloudCode.GeneratedBindings.Game.Models.Team.Yellow => Team.Yellow,
                _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, null)
            };
        }
    }
}