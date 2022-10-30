using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public delegate void BlendEvent();

public class Card : MonoBehaviour
{
    public GameObject FacePrefab;

    public Image faceImage;
    public Button button;
    public RectTransform rect;
    public Animator animator;

    public int value;
    public Suit suit;
    public bool trump;

    public bool Interactable { set => button.enabled = value; }
    public bool Animator { set => animator.enabled = value; }
    public bool ToSort { set => toSort = value; }
    public float Speed { set => animator.speed = value; }
    public Sprite Sprite { set => faceImage.sprite = value; }
    public Transform Parent { set => parent = value; }

    public event BlendEvent Blended;
    public void OnBlended()
    {
        Blended?.Invoke();
        CardField.OnCardHandled();
    }

    private string returnState;
    private bool toSort;
    private Vector3 dist;
    private Quaternion angle;
    private Transform parent;

    void OnDestroy()
    {
        Animator = false;
    }

    public override int GetHashCode()
    {
        return (int)suit * 100 + value;
    }

    public override bool Equals(object obj)
    {
        if (obj is Card card)
        {
            if (ReferenceEquals(this, card))
            {
                return true;
            }

            if (GetType() != card.GetType())
            {
                return false;
            }
            return card.suit == suit && card.value == value;
        }
        return false;
    }

    public static bool operator >(Card card1, Card card2)
    {
        if (card1.trump)
        {
            if (card2.trump)
            {
                return card1.value > card2.value;
            }
            return true;
        }
        if (card2.trump)
        {
            return false;
        }

        if (card1.suit == card2.suit)
        {
            return card1.value > card2.value;
        }

        return (int) card1.suit > (int) card2.suit;
    }

    public static bool operator <(Card card1, Card card2)
    {
        return !(card1 > card2);
    }

    public static bool operator ==(Card card1, Card card2)
    {
        if (card1 is null)
        {
            if (card2 is null)
            {
                return true;
            }
            return false;
        }
        return card1.Equals(card2);
    }

    public static bool operator !=(Card card1, Card card2)
    {
        return !(card1 == card2);
    }

    public override string ToString()
    {
        string name;
        if (value > 10)
        {
            name = ((Rank)value).ToString().Substring(0, 1);
        }
        else
        {
            name = value.ToString();
        }
        return name + suit.ToString()[0];
    }

    public void PlayAnimation(string state)
    {
        animator.Play(state);
    }

    public void Rotate(Quaternion angle)
    {
        rect.rotation = angle;
    }

    public void Move(Vector3 dist)
    {
        rect.anchoredPosition = dist;
    }

    public void Choose()
    {
        User.OnCardChoosed(this);
    }

    public void HideFields()
    {
        Deck.OnHidedFields();
    }

    public void HandOutNextCard()
    {
        Deck.OnHandedOut();
    }

    public void ChangeParent()
    {
        transform.SetParent(parent, true);
        Speed = 1f;

        if (parent.CompareTag("Enemy Field"))
        {
            PlayAnimation("Center");
        }
        else
        {
            PlayAnimation("CenterUserCard");
        }
    }

    public void DemandMove()
    {
        GameController.OnMoveDemanded();
    }


    public void FromRed()
    {
        PlayAnimation("FromRed");
    }

    public IEnumerator Blend()
    {
        yield return new WaitForSeconds(0.1f);
        OnBlended();
    }

    public void SetBlendedCallBack()
    {
        Blended = Destroy;
    }

    public void SetBlendedCallBack(string state, Transform parent)
    {
        returnState = state;
        this.parent = parent;
        Blended = Return;
    }

    public void SetBlendedCallBack(Vector3 dist, Quaternion angle, Transform parent)
    {
        this.dist = dist;
        this.angle = angle;
        this.parent = parent;
        Blended = FadeOut;
    }

    public void Return()
    {
        transform.SetParent(parent);
        transform.SetAsLastSibling();
        Move(dist);
        PlayAnimation(returnState);
    }

    public void FadeOut()
    {
        transform.SetParent(parent);
        transform.SetAsLastSibling();
        Move(dist);
        Rotate(angle);
        PlayAnimation("FadeOut");
    }

    public void EndRound()
    {
        GameController.OnRoundEnded();
    }

    public void SortPlayerHand()
    {
        Player player;
        if (parent.CompareTag("Enemy Field"))
        {
            parent.parent.TryGetComponent(out player);
        }
        else
        {
            parent.TryGetComponent(out player);
        }

        if (player is Player)
        {
            if (Game.final)
            {
                player.field.SortHand();
                GameController.OnMoveDemanded();
                return;
            }
            if (toSort)
            {
                if (player.field.hand.Count == 6)
                {
                    player.field.SortHand();
                    if (Round.IsAttacker(player))
                    {
                        GameController.OnMoveDemanded();
                    }
                }
            }
        }
    }

    public void SortSurrenderHand()
    {
        CardField.OnCardsReturned();
    }

    public void Destroy()
    {
        Destroy(gameObject, 2f);
    }
}
