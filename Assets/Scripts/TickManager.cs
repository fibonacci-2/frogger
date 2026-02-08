using System;
using UnityEngine;

// Central tick manager: advance the game's tick and notify subscribers.
public class TickManager : MonoBehaviour
{
    public static TickManager Instance { get; private set; }

    public event Action OnTick;
    
    public int maxTicks = 100;
    private int _tickCounter = 0;
    public int tickCounter => _tickCounter;

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
        _tickCounter++;

        if (OnTick != null)
        {
            OnTick.Invoke();
        }
        
        if (_tickCounter >= maxTicks)
        {
            Debug.Log("Frog consumed all ticks! TickCounter reached " + _tickCounter);
        }
    }
}
