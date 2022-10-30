using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void GoToSearchScene()
    {
        SceneManager.LoadScene("SearchScene");
    }

    public void GoToCreateScene()
    {
        SceneManager.LoadScene("CreateScene");
    }

    public void GoToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
