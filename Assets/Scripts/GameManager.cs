using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action OnPause;
    public static event Action OnResume;
    public static event Action OnWin;
    
    public GameObject menuPanel;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        if (menuPanel != null)
            menuPanel.SetActive(true);
        OnPause?.Invoke();
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        if (menuPanel != null)
            menuPanel.SetActive(false);
        OnResume?.Invoke();
    }

    public bool IsPaused() => isPaused;

    public void Win()
    {
        OnWin?.Invoke();
        Pause();
    }

    public void CheckWinCondition()
    {
        Frog[] frogs = FindObjectsOfType<Frog>();
        if (frogs.Length < 2)
            return;

        bool allFrogsWon = true;
        for (int i = 0; i < frogs.Length; i++)
        {
            if (!frogs[i].IsOnWinningSquare())
            {
                allFrogsWon = false;
                break;
            }
        }

        if (allFrogsWon)
            Win();
    }

    public void ResetGame()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.ResetTicks();
        
        Car[] allCars = FindObjectsOfType<Car>();
        foreach (Car car in allCars)
            Destroy(car.gameObject);
        
        Frog[] allFrogs = FindObjectsOfType<Frog>();
        foreach (Frog frog in allFrogs)
            frog.ResetToStartPosition();
        
        if (GridManager.Instance != null)
        {
            GridManager.Instance.occupiedSquares.Clear();
        }
        
        FrogSpawner spawner = FindObjectOfType<FrogSpawner>();
        if (spawner != null)
            spawner.Reset();
        
        Resume();
    }
}