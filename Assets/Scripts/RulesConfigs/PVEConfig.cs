using UnityEngine;
using UnityEngine.UI;

public class PVEConfig : RulesConfig
{
    Level level;

    public Button PlayButton;

    protected override void Start()
    {
        base.Start();
    }

    public void SetEasy()
    {
        level = Level.Easy;
        PlayButton.interactable = true;
    }

    public void SetMedium()
    {
        level = Level.Medium;
        PlayButton.interactable = true;
    }

    public void SetHard()
    {
        level = Level.Hard;
        PlayButton.interactable = true;
    }

    public void SetInsane()
    {
        level = Level.Insane;
        PlayButton.interactable = true;
    }

    public override void Play()
    {
        Debug.Log(level);
        base.Play();
    }
}
