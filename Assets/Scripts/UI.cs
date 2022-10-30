using System.IO;
using UnityEngine;

public abstract class UI : MonoBehaviour
{
    protected string path;
    protected string lang;
    
    protected T SetLabels<T>()
    {
        T data = JsonUtility.FromJson<T>(File.ReadAllText(path));
        return data;
    }
}
