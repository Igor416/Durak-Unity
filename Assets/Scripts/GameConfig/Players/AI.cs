using System.Collections.Generic;
using UnityEngine;

public abstract class AI : Player
{
    protected List<Card> knownCards = new List<Card>();
    protected List<Move> currentMoves = new List<Move>();
    protected Dictionary<int, Move> movePerSlot = new Dictionary<int, Move>();

    public void GetPickUpPermission()
    {
        MakeMoves();
        if (currentMoves.Count == 0)
        {
            GameController.OnAllowedPickUp(this);
        }
        else
        {
            foreach (Move currentMove in currentMoves)
            {
                SendMove(currentMove);
            }
            GameController.OnAllowedPickUp(this);
        }
    }

    public void MakeFirstMove()
    {
        MakeMoves();
        List<Move> moves = new List<Move>(currentMoves);
        foreach (Move move in moves)
        {
            SendMove(move);
        }
    }

    public override void AcceptGame()
    {
        GameController.OnGameAccepted(this);
    }

    public override void ReceiveMove(Move move)
    {
        if (!surrended)
        {
            RequestMove();
        }
    }

    public void RequestMove()
    {
        MakeMoves();
        if (currentMoves.Count == 0 || Round.GetMovesToBeat() > GetDefendingMovesCount())
        {
            if (Round.IsDefender(this))
            {
                surrended = true;
                GameController.OnRequestedPickUp(this);
            }
            else if (Round.AllMovesBeaten())
            {
                GameController.OnAllowedDraw(this);
            }
            return;
        }


        for (int i = 0; i < currentMoves.Count; i++)
        {
            if (currentMoves[i].action == Action.Reverse)
            {
                if (i == 0)
                {
                    SendMove(currentMoves[i]);
                    return;
                }
            }
            else
            {
                SendMove(currentMoves[i]);
            }
        }
    }

    protected void AddMove(Move move)
    {
        currentMoves.Add(move);
        movePerSlot[move.slot] = move;
    }

    protected void RemoveMove(int slot)
    {
        List<Move> newList = new List<Move>();
        foreach (Move move in currentMoves)
        {
            if (move.slot != slot)
            {
                newList.Add(move);
            }
        }
        currentMoves.Clear();
        foreach (Move move in newList)
        {
            currentMoves.Add(move);
        }
    }

    protected abstract void MakeMoves();

    private int GetDefendingMovesCount()
    {
        List<Card> hand = new List<Card>(field.hand);
        foreach (Move move in currentMoves)
        {
            if (hand.Contains(move.card))
            {
                hand.Remove(move.card);
            }
        }
        return field.hand.Count - hand.Count;
    }
}
