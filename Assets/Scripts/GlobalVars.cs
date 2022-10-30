public enum Suit
{
    Diamonds,
    Hearts,
    Clubs,
    Spades
}

public enum Rank
{
    Jack = 11,
    Queen,
    King,
    Ace
}

public enum Level
{
    Easy,
    Medium,
    Hard,
    Insane
}

public enum Action
{
    Attack,
    Beat,
    Reverse
}

public struct Rating
{
    public int min;
    public int max;

    public Rating(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

public struct Friend
{
    public string name;
    public int rating;
    public bool online;

    public Friend(string name, int rating, bool online)
    {
        this.name = name;
        this.rating = rating;
        this.online = online;
    }
}

public struct Enemy
{
    public string name;
    public int rating;

    public Enemy(string name, int rating)
    {
        this.name = name;
        this.rating = rating;
    }
}