using UnityEngine;

public class MenuUI : MonoBehaviour
{
    void Start()
    {
        GameManager.OnPause += Show;
        GameManager.OnResume += Hide;
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        GameManager.OnPause -= Show;
        GameManager.OnResume -= Hide;
    }
}
