using UnityEngine;

public class Slot : MonoBehaviour
{
    private int id;

    void Awake()
    {
        id = transform.GetSiblingIndex();
    }

    public void Choose()
    {
        User.OnSlotChoosed(id);
    }
}
