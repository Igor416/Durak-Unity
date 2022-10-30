using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public static List<Player> players = new List<Player>();
    public static Player admin;
    public static int cards = 24;
    public static bool mayThrowIn = true;
    public static bool drawedOnce;
    public static bool final;
    public static bool ended;
    public static Dictionary<Player, int> winners = new Dictionary<Player, int>();
    public static (int value, Suit suit) leastTrump;
    public static List<(int value, Suit suit)> deck = new List<(int value, Suit suit)>();

    public static void AddEnemy(Player enemy)
    {
        players.Add(enemy);
        winners[enemy] = 0;

        GameController.OnSetUp();
    }

    public static void Start()
    {
        Round.players = new List<Player>(players);
        foreach (Player player in players)
        {
            winners[player] = 0;
        }

        int i = 15 - (cards / 4);

        for (; i < 15; i++)
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                deck.Add((i, suit));
            }
        }

        drawedOnce = false;
        ended = false;
        final = false;
        CreateDeck();
        Deck.OnInited();
    }

    public static void AddWinner(Player winner)
    {
        if (winners[winner] != 0)
        {
            return;
        }
        Debug.Log("Winner: " + winner.name);
        int id = 0;
        foreach (Player player in winners.Keys)
        {
            if (winners[player] > id)
            {
                id = winners[player];
            }
        }
        id++;
        winners[winner] = id;
        Round.winners.Add(winner);

        Debug.Log("Count: " + Round.players.Count + " " + (Round.winners.Count + 1));
        if (Round.players.Count == Round.winners.Count + 1)
        {
            ended = true;
            int looserId = -Round.players.IndexOf(winner) + 1; //transforms 0 to 1 and 1 to zero
            winners[Round.players[looserId]] = -1;
            Debug.Log("Losed: " + Round.players[looserId].field.name);
        }
    }

    public static bool IsAdmin(Player player)
    {
        return player == admin;
    }

    private static void CreateDeck()
    {
        Suit trump;
        int playersNum = players.Count;
        deck = Shuffle(deck);

        if (playersNum * 6 == cards)
        {
            trump = deck[cards - 1].suit;
            leastTrump = (15 - (cards / 4), trump);
            return;
        }
        else
        {
            trump = deck[playersNum * 6].suit;
        }
        leastTrump = (14, trump);

        bool leastTrumpChanged = false;

        for (int i = 0; i < playersNum * 6; i++)
        {
            if (deck[i].suit == trump)
            {
                if (deck[i].value < leastTrump.value)
                {
                    leastTrump = deck[i];
                }
                leastTrumpChanged = true;
            }
        }

        if (!leastTrumpChanged)
        {
            CreateDeck();
        }
    }

    private static List<(int, Suit)> Shuffle(List<(int, Suit)> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(0, deck.Count);
            (int, Suit) temp = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = temp;
        }
        return deck;
    }
}
