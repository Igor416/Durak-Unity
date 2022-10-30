using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Profile : MonoBehaviour
{
    public Image Photo;
    public TextMeshProUGUI Name;

    public void SetPhoto()
    {
        //open photo from directory storage
    }

    public void SetName(string name)
    {
        Name.text = name;
    }
}
