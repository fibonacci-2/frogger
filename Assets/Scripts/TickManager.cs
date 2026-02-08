using System;
using UnityEngine;

// Central tick manager: advance the game's tick and notify subscribers.
public class TickManager : MonoBehaviour
{
    public static TickManager Instance { get; private set; }

    public event Action OnTick;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Advance the game by one tick and notify listeners.
    public void AdvanceTick()
    {
        OnTick?.Invoke();
    }
}
