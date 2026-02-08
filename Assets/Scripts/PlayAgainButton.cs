using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgainButton : MonoBehaviour
{
    public void OnClick()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetGame();
    }
}
