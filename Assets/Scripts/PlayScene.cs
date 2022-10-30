using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayScene : MonoBehaviour
{
    public static bool ranking = true;
    public static int rank = 7;

    public Toggle Ranking;

    public void SetRanking()
    {
        ranking = Ranking.isOn;
    }

    public void PlayWithAI()
    {
        SceneManager.LoadScene("PVEConfigScene");
    }

    public void PlayWithUser()
    {
        SceneManager.LoadScene("PVPConfigScene");
    }

    public void PlayWithFriend()
    {
        SceneManager.LoadScene("PVFConfigScene");
    }
}
