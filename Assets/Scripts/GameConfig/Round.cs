using System.Collections.Generic;
using UnityEngine;

public static class Round
{
    public static Player attacker;
    public static Player defender;
    public static List<Player> players = new List<Player>();
    public static List<Player> playersOrder = new List<Player>();
    public static List<Player> winners = new List<Player>();
    public static List<Move> moves = new List<Move>();
    public static bool ended;
    public static string evt;

    public static void Init(Player attacker, Player defender)
    {
        Round.attacker = attacker;
        Round.defender = defender;
    }

    public static void Next()
    {
        foreach (Player winner in winners)
        {
            players.Remove(winner);
        }
        if (Game.ended)
        {
            Debug.Log("FUCK YEAH!");
            foreach (Card card in players[0].field.hand)
            {
                card.Destroy();
            }
            GameController.OnSetUp();
            return;
        }
        defender.surrended = false;
        if (evt == "PickUp")
        {
            TranslateAttacker();
        }
        evt = "";
        moves.Clear();
        winners.Clear();
        TranslateAttacker();
        CreateOrder();
        ended = false;

        if (Game.final)
        {
            Debug.Log("Demanded");
            Debug.Log(defender);
            Debug.Log(attacker);
            GameController.OnMoveDemanded();
        }
    }

    public static void CreateOrder()
    {
        playersOrder.Clear();
        int j, i = players.IndexOf(attacker);
        for (; i >= 0; i--)
        {
            playersOrder.Add(players[i]);
        }

        i = players.IndexOf(defender);
        j = players.Count - 1;
        for (; j > i; j--)
        {
            playersOrder.Add(players[j]);
        }

        //defender isn't in order, because he is always last
    }

    public static bool AllMovesBeaten()
    {
        foreach (Move move in moves)
        {
            if (!move.beaten)
            {
                return false;
            }
        }
        return true;
    }

    public static int GetAttackMoves()
    {
        int count = 0;
        foreach (Move move in moves)
        {
            if (move.action == Action.Attack)
            {
                count++;
            }
        }
        return count;
    }

    public static int GetMovesToBeat()
    {
        int i = 0;
        foreach (Move move in moves)
        {
            if (!move.beaten)
            {
                i++;
            }
        }
        return i;
    }

    public static bool MayReverse()
    {
        foreach (Move move in moves)
        {
            if (move.action == Action.Beat)
            {
                return false;
            }
        }
        if (Game.final)
        {
            TranslateAttacker();
            if (defender.field.hand.Count < moves.Count + 1)
            {
                TranslateAttackerBack();
                return false;
            }
            TranslateAttackerBack();
        }
        return true;
    }

    public static bool MayAttack(int movesDone)
    {
        return movesDone <= defender.field.hand.Count;
    }

    public static void ChangeAttacker()
    {
        if (players.IndexOf(attacker) == players.Count - 1)
        {
            attacker = players[0];
        }
        else
        {
            attacker = players[players.IndexOf(attacker) + 1];
        }
    }

    public static void TranslateAttacker()
    {
        if (ended)
        {
            foreach (Player winner in winners)
            {
                Debug.Log(winner, defender);
            }
            if (winners.Contains(defender))
            {
                Debug.Log("Defender: " + defender.field);
                Player newAttacker = playersOrder[1];

                for (int i = 1; i < playersOrder.Count; i++)
                {
                    if (!winners.Contains(playersOrder[i]))
                    {
                        newAttacker = playersOrder[i];
                        break;
                    }
                }

                foreach (Player player in winners)
                {
                    players.Remove(player);
                }

                attacker = newAttacker;

                if (players.IndexOf(attacker) == players.Count - 1)
                {
                    defender = players[0];
                }
                else
                {
                    defender = players[players.IndexOf(attacker) + 1];
                }
            }
        }
        else
        {
            attacker = defender;
            if (players.IndexOf(attacker) == players.Count - 1)
            {
                defender = players[0];
            }
            else
            {
                defender = players[players.IndexOf(attacker) + 1];
            }

            foreach (Player player in winners)
            {
                players.Remove(player);
            }
        }
    }

    public static void TranslateAttackerBack()
    {
        defender = attacker;
        if (players.IndexOf(defender) == 0)
        {
            attacker = players[players.Count - 1];
        }
        else
        {
            attacker = players[players.IndexOf(defender) - 1];
        }
    }

    public static void AddMove(Move newMove)
    {
        if (newMove.action == Action.Beat)
        {
            newMove.beaten = true;
            foreach (Move move in moves)
            {
                if (move.slot == newMove.slot)
                {
                    move.beaten = true;
                }
            }
        }
        else if (newMove.action == Action.Reverse)
        {
            newMove.action = Action.Attack;
            foreach (Move move in moves)
            {
                move.action = Action.Attack;
            }
        }
        moves.Add(newMove);
    }

    public static bool IsAttacker(Player player)
    {
        return player == attacker;
    }

    public static bool IsDefender(Player player)
    {
        return player == defender;
    }
}
