using System.Collections.Generic;
using UnityEngine;

public delegate void CardClickEvent(Card card);
public delegate void SlotClickEvent(int slotId);

public class User : Player
{
    public static event CardClickEvent CardChoosed;
    public static event SlotClickEvent SlotChoosed;
    public static void OnCardChoosed(Card card)
    {
        CardChoosed(card);
    }
    public static void OnSlotChoosed(int slotId)
    {
        SlotChoosed(slotId);
    }

    private Card activeCard;

    protected override void Awake()
    {
        base.Awake();

        CardChoosed += SetActiveCard;
        SlotChoosed += SetInSlot;
    }

    public override void AcceptGame()
    {

    }

    public override void ReceiveMove(Move move)
    {
        //start timer
    }

    private void SetActiveCard(Card card)
    {
        List<Move> moves = Validator.GetMoves(this, card);
        bool contains = moves.Count != 0;
        
        if (!contains)
        {
            card.PlayAnimation("ToRed");
            return;
        }

        if (activeCard != card)
        {
            if (moves.Count > 1 || !Round.IsDefender(this) || (Round.MayReverse() && card.value == Round.moves[0].card.value))
            {
                if (activeCard != null)
                {
                    activeCard.PlayAnimation("StretchDown");
                }
                activeCard = card;
                card.PlayAnimation("StretchUp");
            }
            else
            {
                if (activeCard != null)
                {
                    activeCard.PlayAnimation("StretchDown");
                }
                SendMove(moves[0]);
            }
        }
    }

    private void SetInSlot(int slotId)
    {
        if (activeCard != null)
        {
            if (!CardField.OnMoveSlotCheckRequested(slotId))
            {
                Action action = Round.IsDefender(this) ? Action.Reverse : Action.Attack;
                SendMove(new Move(activeCard, slotId, action, this));
                activeCard = null;
            }
            else if (Round.IsDefender(this))
            {
                foreach (Move move in Round.moves)
                {
                    if (move.slot == slotId && !move.beaten)
                    {
                        SendMove(new Move(activeCard, slotId, Action.Beat, this));
                        break;
                    }
                }
                activeCard = null;
            }
        }
    }
}
