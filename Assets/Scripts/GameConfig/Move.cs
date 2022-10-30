public class Move
{
    public Card card;
    public int slot;
    public Action action;
    public Player player;
    public bool beaten;

    public Move(Card card, int slot, Action action, Player player)
    {
        this.card = card;
        this.slot = slot;
        this.action = action;
        this.player = player;
        beaten = false;
    }

    public override int GetHashCode()
    {
        return card.GetHashCode() * 100 + slot * 10 + (int)action;
    }

    public override bool Equals(object obj)
    {
        if (obj is Move move)
        {
            if (ReferenceEquals(this, move))
            {
                return true;
            }

            if (GetType() != move.GetType())
            {
                return false;
            }
            return card == move.card && action == move.action && action == move.action;
        }
        return false;
    }

    public static bool operator ==(Move move1, Move move2)
    {
        if (move1 is null)
        {
            if (move2 is null)
            {
                return true;
            }
            return false;
        }
        return move1.Equals(move2);
    }

    public static bool operator !=(Move move1, Move move2)
    {
        return !(move1 == move2);
    }

    public override string ToString()
    {
        return card + " " + slot + " " + action + " " + player.field.profile.Name.text;
    }
}
