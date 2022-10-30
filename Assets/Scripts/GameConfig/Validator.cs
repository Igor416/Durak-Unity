using System.Collections.Generic;
using UnityEngine;

public static class Validator
{
    public static List<Move> GetMoves(Player player, Card card)
    {
        List<Move> moves = new List<Move>();
        if (Round.IsDefender(player))
        {
            foreach (Move move in Round.moves)
            {
                if (!move.beaten)
                {
                    if (Round.MayReverse() && move.card.value == card.value)
                    {
                        moves.Add(new Move(card, -1, Action.Reverse, player));
                        if (card.trump)
                        {
                            moves.Add(new Move(card, move.slot, Action.Beat, player));
                        }
                    }

                    else if (move.card.suit == card.suit)
                    {
                        if (move.card.value < card.value)
                        {
                            moves.Add(new Move(card, move.slot, Action.Beat, player));
                        }
                    }
                    else if (card.trump)
                    {
                        moves.Add(new Move(card, move.slot, Action.Beat, player));
                    }
                }
            }
        }
        else
        {
            if (Round.moves.Count == 0)
            {
                moves.Add(new Move(card, -1, Action.Attack, player));
            }
            else
            {
                foreach (Move move in Round.moves)
                {
                    if (move.card.value == card.value)
                    {
                        moves.Add(new Move(card, -1, Action.Attack, player));
                        break;
                    }
                }
            }
        }
        return moves;
    }
}
