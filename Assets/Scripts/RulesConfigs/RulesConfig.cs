using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RulesConfig : MonoBehaviour
{
    protected bool ranking;
    protected int rank;
    protected int min;
    protected bool reverse;
    protected int cards;

    public Toggle Timer;
    public GameObject TimerButtons;
    public Toggle Reverse;
    public Toggle Cards36;
    public Toggle Cards52;

    protected virtual void Start()
    {
        ranking = PlayScene.ranking;
        rank = PlayScene.rank;

        min = 0;
        reverse = true;
        cards = 36;
    }

    public void ToggleButtons()
    {
        TimerButtons.SetActive(Timer.isOn);
        min = Timer.isOn ? 5 : 0;
    }

    public void Set1m()
    {
        min = 1;
    }

    public void Set2m()
    {
        min = 2;
    }

    public void Set3m()
    {
        min = 3;
    }

    public void Set5m()
    {
        min = 5;
    }

    public void SetReverse()
    {
        reverse = Reverse.isOn;
    }

    public void SetCards36()
    {
        bool val = Cards36.isOn;
        cards = val ? 36 : 52;
        Cards52.isOn = !val;
    }

    public void SetCards52()
    {
        bool val = Cards52.isOn;
        cards = val ? 52 : 26;
        Cards36.isOn = !val;
    }

    public virtual void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
}
