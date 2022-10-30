using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void AnimationEvent();

public class Deck : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject SuitPrefab;
    public List<Sprite> FaceSprites;
    public List<Sprite> SuitSprites;

    public EnemiesField enemiesField;
    public UserField userField;
    public Animator animator;

    public List<Player> playersOrder = new List<Player>();

    public static GameEvent Inited;
    public static void OnInited()
    {
        Inited?.Invoke();
    }

    public static event AnimationEvent HandedOut;
    public static void OnHandedOut()
    {
        HandedOut?.Invoke();
    }

    public static event AnimationEvent HidedFields;
    public static void OnHidedFields()
    {
        HidedFields?.Invoke();
    }

    public static event AnimationEvent ShowedLeastTrump;
    public static void OnShowedLeastTrump()
    {
        ShowedLeastTrump?.Invoke();
    }

    private int enemyId = 0;
    private int cardId = 0;
    private int enemiesNum;
    private int trumpId;
    private bool trumpDisplayed;
    private Card leastTrump;
    private bool handedout;
    private readonly Queue<Card> deck = new Queue<Card>();

    private readonly List<Player> enemies = new List<Player>();

    void Awake()
    {
        Inited = Init;
        HandedOut = HandOut;
        HidedFields += HideFields;
        ShowedLeastTrump += ShowLeastTrump;
    }

    public void Init()
    {
        enemiesNum = Game.players.Count - 1;

        trumpId = (enemiesNum + 1) * 6;

        foreach (Player player in Game.players)
        {
            if (player.field.name == "Enemy")
            {
                enemies.Add(player);
            }
        }

        foreach (var cardInfo in Game.deck)
        {
            GameObject cardObj = Instantiate(CardPrefab, transform);

            Card card = cardObj.GetComponent<Card>();
            card.suit = cardInfo.suit;
            card.value = cardInfo.value;
            card.trump = Game.leastTrump.suit == card.suit;

            foreach (Sprite sprite in FaceSprites)
            {
                if (sprite.name == card.ToString())
                {
                    card.Sprite = sprite;
                    break;
                }
            }

            if (cardInfo.Equals(Game.leastTrump))
            {
                leastTrump = card;
            }

            deck.Enqueue(card);
        }

        for (int i = 1; i < Game.players.Count; i++)
        {
            playersOrder.Add(Game.players[i]);
        }
        playersOrder.Add(Game.players[0]);

        enemiesField.AddEnemyButton.SetActive(false);
        animator.Play("Appearing");
    }

    public void HideFields()
    {
        animator.Play("HidingDeck");

        foreach (Player player in Game.players)
        {
            player.field.SortHand();
        }
    }

    public void ShowLeastTrump()
    {
        if (!userField.hand.Contains(leastTrump))
        {
            leastTrump.PlayAnimation("ShowLeastTrump");
        }
        userField.SortHand();
    }

    private void HandOut()
    {
        if (cardId < trumpId) //the function is called by animation event - recursion
        {
            int id = cardId % (enemiesNum + 1);
            /*
            if (id == enemiesNum)
            {
                field = userField; // user is last to handout
                name = "User";
            }
            else
            {
                field = enemies[id].field;
                name = "Enemy" + (enemiesNum == 1 ? "" : enemiesNum + "-" + (enemies.IndexOf(enemies[id]) + 1));
            }
            */
            SendCard(id, id == enemiesNum);
        }
        else
        {
            if (trumpId != Game.cards)
            {
                //all cards are handled, it's time for trump
                Card trump = deck.Dequeue();
                trump.PlayAnimation("OpenTrump");
                trump.transform.SetAsFirstSibling();
                deck.Enqueue(trump);
                //We take current top card, flip it and put it at the bottom of the deck
            }
            else
            {
                Game.final = true;
                HideFields();
                DisplayTrump();
            }

            Round.Init(userField.GetPlayer(), enemies[0]);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].field.hand.Contains(leastTrump))
                {
                    if (i == enemies.Count - 1)
                    {
                        Round.Init(enemies[i], userField.GetPlayer());
                    }
                    else
                    {
                        Round.Init(enemies[i], enemies[i + 1]);
                    }
                }
            }
            Round.CreateOrder();

            cardId = 0;
            enemyId = 0;
            HandedOut = HandOutLate;
            GameController.OnSetUp();
            //for later handouts
        }
    }

    private void HandOutLate()
    {
        if (!Game.final)
        {
            if (playersOrder[enemyId].field.hand.Count < 6)
            {
                handedout = true;
                /*
                if (playersOrder[enemyId] is User)
                {
                    name = "User";
                }
                else
                {
                    name = "Enemy" + (enemiesNum == 1 ? "" : enemiesNum + "-" + (enemies.IndexOf(playersOrder[enemyId]) + 1));
                }
                */
                SendCard(enemyId, playersOrder[enemyId] is User);
            }
            else
            {
                cardId = 0;
                if (enemyId != playersOrder.Count - 1)
                {
                    enemyId++;
                    HandOutLate();
                }
                else
                {
                    enemyId = 0;
                }

                if (!handedout)
                {
                    GameController.OnMoveDemanded();
                }
                handedout = false;
            }
        }
        else
        {
            if (!trumpDisplayed)
            {
                DisplayTrump();
            }
        }
    }

    private void SendCard(int id, bool condition, bool toSort = false)
    {
        Card card = deck.Dequeue();
        Player player = playersOrder[id];
        string animationName;

        if (condition)
        {
            animationName = "HandOutUser";
        }
        else
        {
            animationName = "HandOutEnemy" + (enemiesNum == 1 ? "" : enemiesNum + "-" + (id + 1));
        }

        if (toSort)
        {
            animationName = "Late" + animationName;
        }

        if (deck.Count == 0)
        {
            animationName = "HandOut" + player.field.name + "Trump";
            Game.final = true;
        }
        player.field.AddCard(card, player.field.name == "User");
        card.Parent = player.field.Hand;
        card.ToSort = toSort;
        card.Speed = enemiesNum / 10f + 0.9f;
        card.PlayAnimation(animationName);
        cardId++;
    }

    private void DisplayTrump()
    {
        SuitPrefab.SetActive(true);
        string suitName = leastTrump.suit.ToString().ToLower();
        foreach (Sprite suit in SuitSprites)
        {
            if (suit.name == suitName)
            {
                SuitPrefab.GetComponent<Image>().sprite = suit;
                break;
            }
        }
        animator.Play("DisplayTrump");
        trumpDisplayed = true;
    }
}
