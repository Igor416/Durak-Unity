using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Player : MonoBehaviour
{
    public string PhotoPath;
    public bool surrended;
    public bool attacking;
    public Field field;

    public static event MoveEvent MoveReceived;
    public static void OnMoveReceived(Move move)
    {
        MoveReceived(move);
    }

    protected virtual void Awake()
    {
        field = GetComponent<Field>();

        MoveReceived += ReceiveMove;
    }

    public abstract void AcceptGame();

    public abstract void ReceiveMove(Move move);

    protected virtual void SendMove(Move move)
    {
        if (move.action == Action.Beat)
        {
            GameController.OnRequestedDraw(this);
        }
        GameController.OnMoveReceived(move);
    }
}
