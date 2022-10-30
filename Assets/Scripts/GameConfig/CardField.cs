using System.Collections.Generic;
using UnityEngine;

public delegate bool MoveSlotEvent(int slot);
public delegate bool MoveQueueEvent();

public class CardField : MonoBehaviour
{
    public bool[] occupiedSlots;

    public static event AnimationEvent CardHandled;
    public static void OnCardHandled()
    {
        CardHandled?.Invoke();
    }

    public static event MoveSlotEvent MoveSlotCheckRequested;
    public static bool OnMoveSlotCheckRequested(int slot)
    {
        return (bool)MoveSlotCheckRequested?.Invoke(slot);
    }

    public static event MoveQueueEvent MoveQueueStateRequested;
    public static bool OnMoveQueueStateRequested()
    {
        return (bool)MoveQueueStateRequested?.Invoke();
    }

    public static event AnimationEvent CardsReturned;
    public static void OnCardsReturned()
    {
        CardsReturned?.Invoke();
    }

    public int slotsNum;

    private readonly Queue<Move> moves = new Queue<Move>();
    private readonly Queue<Card> cards = new Queue<Card>();
    private Field surrender;

    void Awake()
    {
        occupiedSlots = new bool[slotsNum];

        CardHandled += HandleNextMove;
        MoveSlotCheckRequested += (slot) => occupiedSlots[slot];
        MoveQueueStateRequested += () => moves.Count == 0;
        CardsReturned += SortSurrenderHand;
    }

    public int GetUnoccupiedSlot()
    {
        int i = 0;
        for (; i < occupiedSlots.Length; i++)
        {
            if (!occupiedSlots[i])
            {
                break;
            }
        }
        return i;
    }

    public void SortSurrenderHand()
    {
        if (surrender != null)
        {
            cards.Dequeue();
            if (cards.Count == 0)
            {
                surrender.SortHand();
                surrender = null;
            }
        }
    }

    public void ReturnCards(Field field)
    {
        surrender = field;
        foreach (Transform slot in transform)
        {
            foreach (Transform cardObj in slot)
            {
                Card card = cardObj.GetComponent<Card>();
                field.AddCard(card, field.name == "User");
                cards.Enqueue(card);
                card.SetBlendedCallBack("Return" + field.name, field.Hand);
                FadeIn(card);
            }
        }
    }

    public void RemoveCards()
    {
        foreach (Transform slot in transform)
        {
            foreach (Transform cardObj in slot)
            {
                Card card = cardObj.GetComponent<Card>();

                card.SetBlendedCallBack();
                FadeIn(card);
            }
        }
    }

    public void AddCard(Move move)
    {
        occupiedSlots[move.slot] = true;
        if (moves.Count == 0)
        {
            SetInSlot(move);
        }
        moves.Enqueue(move);
    }

    private void SetInSlot(Move move)
    {
        Transform slot = transform.GetChild(move.slot).transform;

        MoveCard(move.card, move.player.field, slot);
    }

    private void HandleNextMove()
    {
        if (moves.Count != 0)
        {
            moves.Dequeue();
            if (moves.Count != 0)
            {
                SetInSlot(moves.Peek());
            }
        }
    }

    private void MoveCard(Card card, Field field, Transform slot)
    {
        Vector3 dist = new Vector3(Random.Range(-35f, 35f), Random.Range(-35f, 35f));
        Quaternion angle = Quaternion.Euler(0, 0, Random.Range(-40f, 40f));
        card.SetBlendedCallBack(dist, angle, slot);
        FadeIn(card, field.name == "User");
    }

    private void FadeIn(Card card, bool active = true)
    {
        card.PlayAnimation("FadeIn" + (active ? "User" : "Enemy"));
    }
}
