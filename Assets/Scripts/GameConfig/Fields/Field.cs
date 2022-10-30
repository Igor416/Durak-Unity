using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Field : MonoBehaviour
{
    public int id = 0;
    public new string name;
    public Profile profile;
    public Transform Hand;

    public List<Card> hand = new List<Card>();

    public abstract Player GetPlayer(string type, string level = "");

    public void AddCard(Card card, bool interactable = false)
    {
        if (interactable)
        {
            card.Interactable = true;
            card.FacePrefab.SetActive(true);
        }
        hand.Add(card);
    }

    public void RemoveCard(Card card)
    {
        card.Interactable = false;
        card.FacePrefab.SetActive(true);
        hand.Remove(card);
    }

    public virtual void SortHand()
    {
        StartCoroutine(ArrangeHand());
    }

    protected virtual IEnumerator ArrangeHand(float maxAngle = 20f, float k = 1f, bool change = true)
    {
        yield return new WaitForSeconds(0.1f);
        const float maxLength = 430f;
        const float maxCardWidth = 212f; //max angle is 160, so we find out width with this angle
        const float distDispersion = 0.75f;
        const float angleDispersion = 2f;
        const float cardWidth = 167f;

        int count = hand.Count;
        int part = Mathf.FloorToInt(count / 2);

        float n = 0f;
        float margin = cardWidth * part * (1 - distDispersion) + maxCardWidth / 2;

        if (change && margin > maxLength / 2)
        {
            k = maxLength / margin / 2.25f;
        }

        float dist, angle;

        for (int j, i = 0; i < part; i++)
        {
            j = count - i - 1;

            angle = maxAngle - (angleDispersion * i * maxAngle / count);
            dist = k * cardWidth * (part - i) * (1 - distDispersion);
            if (i == part - 1 && count % 2 == 0)
            {
                dist /= 2;
            }

            hand[i].Rotate(Quaternion.Euler(0, 0, angle));
            hand[i].Move(new Vector3(dist - n, 0, 0));

            hand[j].Rotate(Quaternion.Euler(0, 0, -angle));
            hand[j].Move(new Vector3(-dist - n, 0, 0));

            yield return new WaitForSeconds(0.05f);
        }

        if (count % 2 == 1)
        {
            hand[part].Rotate(Quaternion.Euler(0, 0, 0));
            hand[part].Move(new Vector3(-n, 0, 0));
        }
    }

    public override string ToString()
    {
        string str = "";

        foreach (Card card in hand)
        {
            str += card.ToString() + " ";
        }
        return str;
    }
}
