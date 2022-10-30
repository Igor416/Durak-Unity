using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserField : Field
{
    public override Player GetPlayer(string type = "", string level = "")
    {
        return GetComponent<User>();
    }

    public override void SortHand()
    {
        bool itemMoved;
        do
        {
            itemMoved = false;
            for (int i = 0; i < hand.Count - 1; i++)
            {
                if (hand[i] > hand[i + 1])
                {
                    Card lowerValue = hand[i + 1];
                    hand[i + 1] = hand[i];
                    hand[i] = lowerValue;
                    itemMoved = true;
                }
            }
        } while (itemMoved);

        foreach (Card card in hand)
        {
            card.transform.SetAsLastSibling();
        }
        base.SortHand();
    }
}
