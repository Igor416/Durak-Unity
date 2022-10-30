using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PVFConfig : RulesConfig
{
    Friend friend;
    readonly List<GameObject> onlineList = new List<GameObject>();
    readonly List<GameObject> offlineList = new List<GameObject>();

    public GameObject FriendsContainer;
    public GameObject FriendList;
    public GameObject OnlineList;
    public GameObject OfflineList;
    public GameObject FriendPrefab;

    protected override void Start()
    {
        base.Start();
        
        RectTransform rect = FriendList.GetComponent<RectTransform>();
        if (275 + 50 * 11 > rect.rect.height)
        {
            rect.sizeDelta = new Vector2(rect.rect.width, 275 + 50 * 11);
            rect.offsetMax = new Vector2(0, rect.offsetMax.y);
        }

        GameObject friend;
        int offset;

        //change when server is done
        for (int i = 0; i < 11; i++)
        {
            Friend friendInfo = new Friend(i.ToString(), UnityEngine.Random.Range(1, 1000), Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
            if (friendInfo.online)
            {
                friend = Instantiate(FriendPrefab, OnlineList.transform);
            }
            else
            {
                friend = Instantiate(FriendPrefab, OfflineList.transform);
            }

            friend.GetComponent<Button>().onClick.AddListener(() => { this.friend = friendInfo; });
            friend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = friendInfo.name;
            friend.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = friendInfo.rating.ToString();

            if (friendInfo.online)
            {
                onlineList.Add(friend);
            }
            else
            {
                offlineList.Add(friend);
            }
        }

        for (int i = 0; i < onlineList.Count; i++)
        {
            offset = (i + 1) * 50;
            onlineList[i].transform.localPosition += new Vector3(0, -offset, 0);
        }

        OfflineList.transform.localPosition += new Vector3(0, -(onlineList.Count * 50), 0);

        for (int i = 0; i < offlineList.Count; i++)
        {
            offset = (i + 1) * 50;
            offlineList[i].transform.localPosition += new Vector3(0, -offset, 0);
        }
    }

    public void ToggleFriendsList()
    {
        FriendsContainer.SetActive(!FriendsContainer.activeSelf);
    }

    public override void Play()
    {
        Debug.Log(friend);
        base.Play();
    }
}
