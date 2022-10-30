using System.Collections;
using UnityEngine;
using TMPro;

public delegate void HandEvent();

public class EnemyField : Field
{
    public bool small = false;
    public TextMeshProUGUI State;
    public Animator stateAnimator;

    public static event HandEvent HandChanged;
    public static void OnHandChanged()
    {
        HandChanged?.Invoke();
    }

    private bool showLeastTrump = true;

    void Awake()
    {
        HandChanged = SortHand;
    }

    public override Player GetPlayer(string type, string level = "")
    {
        switch (type)
        {
            case "AI":
                switch (level)
                {
                    case "Easy": return gameObject.AddComponent<AIEasy>();
                    case "Medium": return gameObject.AddComponent<AIMedium>();
                    case "Hard": return gameObject.AddComponent<AIHard>();
                    case "Insane": return gameObject.AddComponent<AIInsane>();
                    default: return null;
                }
            case "Human": return gameObject.AddComponent<Human>();
            default: return null;
        }
    }

    public void UpdateState(string state)
    {
        State.text = state;
        if (state != "")
        {
            stateAnimator.Play("FadeOut");
        }
    }

    public void ClearState()
    {
        stateAnimator.Play("FadeIn");
    }

    public override void SortHand()
    {
        foreach (Card card in hand)
        {
            card.transform.SetAsLastSibling();
        }
        StartCoroutine(ArrangeHand(small ? 10 : 20, small ? 0.2f : 1, !small));
    }

    protected override IEnumerator ArrangeHand(float maxAngle, float k, bool change)
    {
        StartCoroutine(base.ArrangeHand(maxAngle, k, change));

        yield return new WaitForSeconds(0.05f);
        if (showLeastTrump)
        {
            showLeastTrump = false;
            Deck.OnShowedLeastTrump();
        }
    }
}
