using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate void UIEvent();

public class GameUI : UI
{
    public Player user;
    public TextMeshProUGUI Resign;
    public TextMeshProUGUI Action;

    public static event UIEvent ButtonClicked;
    public static void OnButtonClicked()
    {
        ButtonClicked?.Invoke();
    }

    /*
    Initializers are made, so unity doesn't alert us that fields are never assigned;
    */ 

    [System.Serializable]
    private class JSONContainer
    {
        public string[] langs;
        public Labels[] labels;

        public JSONContainer(string[] langs, Labels[]  labels)
        {
            this.langs = langs;
            this.labels = labels;
        }
    }

    [System.Serializable]
    private class Labels
    {
        public string resign;
        public string[] start;
        public string[] action;

        public Labels(string resign, string[] start, string[] action)
        {
            this.resign = resign;
            this.start = start;
            this.action = action;
        }
    };

    private Labels labels;
    private readonly Dictionary<string, int> actions = new Dictionary<string, int>()
    {
        { "RequestDraw", 0 },
        { "AllowDraw", 1 },
        { "RequestPickUp", 2 },
        { "AllowPickUp", 3 },
    };

    void Awake()
    {
        path = "Assets/Scripts/GameConfig/Labels.json";
        lang = "ru";

        JSONContainer container = SetLabels<JSONContainer>();
        int i;
        for (i = 0; i < container.langs.Length; i++)
        {
            if (container.langs[i] == lang)
            {
                break;
            }
        }

        labels = container.labels[i];
        Resign.text = labels.resign;
    }

    public string GetState(string state)
    {
        return labels.action[actions[state]];
    }

    public void UpdateUI(string state)
    {
        if (state == "Start")
        {
            if (Game.IsAdmin(user))
            {
                Action.text = labels.start[0];
                ButtonClicked = () => GameController.OnGameRequested(user);
            }
            else
            {
                Action.text = "";
            }
            return;
        }
        if (state == "Request")
        {
            Action.text = labels.start[1];
            ButtonClicked = () => GameController.OnGameAccepted(user);
        }
    }

    public void UpdateUILate(string state)
    {
        Action.text = labels.action[actions[state]];

        switch (state)
        {
            case "RequestDraw": ButtonClicked = null; break;
            case "AllowDraw": ButtonClicked = () => GameController.OnAllowedDraw(user); break;
            case "RequestPickUp": ButtonClicked = () => GameController.OnRequestedPickUp(user); break;
            case "AllowPickUp": ButtonClicked = () => GameController.OnAllowedPickUp(user); break;
        }
    }
}
