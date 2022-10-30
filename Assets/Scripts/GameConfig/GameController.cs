using System.Collections.Generic;
using UnityEngine;

public delegate void ActionEvent(Player player);
public delegate void GameEvent();
public delegate void MoveEvent(Move move);

public class GameController : MonoBehaviour
{
    public GameUI UI;
    public Deck deck;
    public CardField cardField;
    public User user;

    public static ActionEvent GameRequested;
    public static ActionEvent GameAccepted;
    public static void OnGameRequested(Player player)
    {
        GameRequested?.Invoke(player);
    }
    public static void OnGameAccepted(Player player)
    {
        GameAccepted?.Invoke(player);
    }

    public static event GameEvent SetUp;
    public static void OnSetUp()
    {
        SetUp?.Invoke();
    }

    public static event GameEvent MoveDemanded;
    public static event MoveEvent MoveReceived;
    public static void OnMoveDemanded()
    {
        MoveDemanded?.Invoke();
    }
    public static void OnMoveReceived(Move move)
    {
        MoveReceived?.Invoke(move);
    }

    public static event ActionEvent RequestedPickUp;
    public static event ActionEvent AllowedPickUp;
    public static event ActionEvent RequestedDraw;
    public static event ActionEvent AllowedDraw;
    public static void OnRequestedPickUp(Player player)
    {
        RequestedPickUp?.Invoke(player);
    }
    public static void OnAllowedPickUp(Player player)
    {
        AllowedPickUp?.Invoke(player);
    }
    public static void OnRequestedDraw(Player player)
    {
        RequestedDraw?.Invoke(player);
    }
    public static void OnAllowedDraw(Player player)
    {
        AllowedDraw?.Invoke(player);
    }

    public static event GameEvent RoundEnded;
    public static void OnRoundEnded()
    {
        if (Round.ended && CardField.OnMoveQueueStateRequested())
        {
            RoundEnded?.Invoke();
        }
    }

    private readonly Dictionary<Player, bool> acceptedGame = new Dictionary<Player, bool>();
    private (string evt, Dictionary<Player, bool> players) allowedEndRound = ("Draw", new Dictionary<Player, bool>());

    void Awake()
    {
        GameRequested = RequestGame;
        GameAccepted = AcceptGame;

        SetUp = () => UI.UpdateUI("Start");

        MoveDemanded += DemandMove;
        MoveReceived += ReceiveMove;

        RequestedPickUp += RequestPickUp;
        AllowedPickUp += AllowPickUp;
        RequestedDraw += RequestDraw;
        AllowedDraw += AllowDraw;

        RoundEnded += () => EndRound(allowedEndRound.evt);
        RoundEnded += ClearAllowedEndRound;
        RoundEnded += GiveCards;
        RoundEnded += () => cardField.occupiedSlots = new bool[cardField.slotsNum];
        RoundEnded += () => Round.Next();
        RoundEnded += SetUI;
    }

    void Start()
    {
        Game.AddEnemy(user);
        Game.admin = user;
    }

    private void RequestGame(Player admin)
    {
        foreach (Player player in Game.players)
        {
            acceptedGame[player] = false;
        }

        acceptedGame[admin] = true;

        foreach (Player player in Game.players)
        {
            if (player == admin)
            {
                UI.UpdateUILate("RequestDraw");
            }
            else if (player == user)
            {
                UI.UpdateUI("Request");
            }
            else
            {
                player.AcceptGame();
            }
        }
    }

    private void AcceptGame(Player player)
    {
        acceptedGame[player] = true;

        bool allAccepted = true;
        for (int i = 0; i < Game.players.Count; i++)
        {
            if (!acceptedGame[Game.players[i]])
            {
                allAccepted = false;
                break;
            }
        }

        if (allAccepted)
        {
            SetUp += SetUI;
            SetUp += ClearAllowedEndRound;
            Game.Start();
        }
    }

    private void SetUI()
    {
        if (Round.IsDefender(user))
        {
            UI.UpdateUILate("RequestPickUp");
        }
        else
        {
            UI.UpdateUILate("AllowDraw");
        }
    }

    private void ClearAllowedEndRound()
    {
        foreach (Player player in Round.players)
        {
            allowedEndRound.players[player] = false;
            if (player != user && Round.ended)
            {
                ((EnemyField)player.field).ClearState();
            }
        }
        allowedEndRound.evt = "Draw";
    }

    private void DemandMove()
    {
        if (Round.attacker is AI AI)
        {
            AI.MakeFirstMove();
        }
    }

    private void ReceiveMove(Move move)
    {
        if (move.slot == -1)
        {
            move.slot = cardField.GetUnoccupiedSlot();
            if (move.slot == cardField.slotsNum)
            {
                return;
            }
        }
        if (move.action == Action.Attack)
        {
            if (Round.GetMovesToBeat() == Round.defender.field.hand.Count || (Round.GetAttackMoves() == 5 && !Game.drawedOnce))
            {
                if (allowedEndRound.evt == "Draw")
                {
                    if (Round.GetMovesToBeat() == 0)
                    {
                        OnAllowedDraw(move.player);
                    }
                }
                else
                {
                    foreach (Player player in Round.players)
                    {
                        if (!allowedEndRound.players[player])
                        {
                            OnAllowedPickUp(player);
                        }
                    }
                }
                return;
            }
            //create later self verification in AI
        }
        else if (move.action == Action.Reverse)
        {
            Round.TranslateAttacker();
            SetUI();
        }
        else
        {
            foreach (Player player in Round.players)
            {
                if (player != user && Round.ended)
                {
                    ((EnemyField)player.field).ClearState();
                }
            }
        }

        DisplayMove(move);
        SendMove(move);

        if (Game.final && move.player.field.hand.Count == 0)
        {
            Game.AddWinner(move.player);

            if (move.player == user)
            {
                UI.UpdateUILate("RequestDraw");
                if (allowedEndRound.evt == "Draw")
                {
                    OnAllowedDraw(user);
                }
                else
                {
                    OnAllowedPickUp(user);
                }
            }
        }
    }

    private void DisplayMove(Move move)
    {
        move.player.field.RemoveCard(move.card);
        cardField.AddCard(move);
        move.player.field.SortHand();
        Round.AddMove(move);
    }

    private void SendMove(Move move)
    {
        if (Round.IsDefender(move.player))
        {
            Round.attacker.ReceiveMove(move);
        }
        else if (Round.IsAttacker(move.player))
        {
            Round.defender.ReceiveMove(move);
        }
    }

    private void RequestDraw(Player player)
    {
        allowedEndRound.evt = "Draw";
        if (Round.moves.Count == 0)
        {
            return;
        }

        allowedEndRound.players[player] = true;
        if (player != user)
        {
            ((EnemyField)player.field).UpdateState(UI.GetState("RequestDraw"));
        }
        if (Round.IsAttacker(user))
        {
            UI.UpdateUILate("AllowDraw");
        }
    }

    private void AllowDraw(Player player)
    {
        allowedEndRound.players[player] = true;
        if (player != user)
        {
            ((EnemyField)player.field).UpdateState(UI.GetState("AllowDraw"));
        }

        Round.ChangeAttacker();
        if (Round.IsAttacker(Round.defender))
        {
            bool allAllowed = true;
            for (int i = 0; i < Round.players.Count; i++)
            {
                if (!allowedEndRound.players[Round.players[i]])
                {
                    allAllowed = false;
                    break;
                }
            }

            if (allAllowed)
            {
                Round.TranslateAttackerBack();
                Round.ended = true;
                OnRoundEnded();
                return;
            }
            else
            {
                Round.ChangeAttacker();
            }
        }

        if (Round.attacker is AI AI)
        {
            AI.RequestMove();
        }
        else if (Round.IsAttacker(user))
        {
            UI.UpdateUILate("AllowDraw");
        }
    }

    private void RequestPickUp(Player player)
    {
        ClearAllowedEndRound();
        if (Round.winners.Contains(user) || !Round.players.Contains(user))
        {
            Debug.Log("Doesn't contain user");
            OnAllowedPickUp(user);
        }
        allowedEndRound.evt = "PickUp";
        if (Round.moves.Count == 0)
        {
            return;
        }

        allowedEndRound.players[player] = true;
        if (player != user)
        {
            ((EnemyField)player.field).UpdateState(UI.GetState("RequestPickUp"));
        }
        if (Round.attacker is AI AI)
        {
            AI.GetPickUpPermission();
        }
        else if (Round.IsAttacker(user))
        {
            UI.UpdateUILate("AllowPickUp");
        }
    }

    private void AllowPickUp(Player player)
    {
        allowedEndRound.players[player] = true;
        if (player != user)
        {
            ((EnemyField)player.field).UpdateState(UI.GetState("AllowPickUp"));
        }

        bool iteratedOnce = false;
        Round.ChangeAttacker();
        while (allowedEndRound.players[Round.attacker])
        {
            Round.ChangeAttacker();
            if (Round.IsAttacker(Round.defender))
            {
                Debug.Log(iteratedOnce);
                if (iteratedOnce)
                {
                    Round.TranslateAttackerBack();
                    Round.ended = true;
                    OnRoundEnded();
                    return;
                }
                iteratedOnce = true;
            }
        }
        if (Round.attacker is AI AI)
        {
            AI.GetPickUpPermission();
        }
        else if (Round.IsAttacker(user))
        {
            UI.UpdateUILate("AllowPickUp");
        }
    }

    private void EndRound(string evt)
    {
        Debug.Log("Ending Round");
        Round.evt = evt;
        if (evt == "PickUp")
        {
            PickUp();
        }
        else
        {
            Draw();
        }
    }

    private void PickUp()
    {
        cardField.ReturnCards(Round.defender.field);
    }

    private void Draw()
    {
        cardField.RemoveCards();
        Game.drawedOnce = true;
    }

    private void GiveCards()
    {
        deck.playersOrder = new List<Player>(Round.playersOrder) { Round.defender };
        Deck.OnHandedOut();
    }
}