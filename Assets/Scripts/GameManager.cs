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

    public void ResetGame()
    {
        // Reset tick manager
        if (TickManager.Instance != null)
            TickManager.Instance.ResetTicks();
        
        // Destroy all cars
        Car[] allCars = FindObjectsOfType<Car>();
        foreach (Car car in allCars)
            Destroy(car.gameObject);
        
        // Clear grid
        if (GridManager.Instance != null)
        {
            GridManager.Instance.occupiedSquares.Clear();
        }
        
        // Reset spawner state
        FrogSpawner spawner = FindObjectOfType<FrogSpawner>();
        if (spawner != null)
            spawner.Reset();
        
        Resume();
    }
}