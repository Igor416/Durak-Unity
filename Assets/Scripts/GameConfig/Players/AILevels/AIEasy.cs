using System.Collections.Generic;

public class AIEasy : AI
{
    protected override void MakeMoves()
    {
        currentMoves.Clear();
        movePerSlot.Clear();
        List<Move> validMoves;

        foreach (Card card in field.hand)
        {
            validMoves = Validator.GetMoves(this, card);
            foreach (Move move in validMoves)
            {
                if (movePerSlot.ContainsKey(move.slot))
                {
                    if (move.card < movePerSlot[move.slot].card)
                    {
                        RemoveMove(move.slot);
                        AddMove(move);
                    }
                    else if (move.card.value == movePerSlot[move.slot].card.value && !move.card.trump)
                    {
                        AddMove(move);
                    }
                }
                else
                {
                    AddMove(move);
                }
            }
        }
    }
}
