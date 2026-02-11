using System;
using UnityEngine;

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

    public void AdvanceTick()
    {
        _tickCounter++;

        if (OnTick != null)
        {
            OnTick.Invoke();
        }
        
        if (_tickCounter >= maxTicks)
        {
        }
    }

    public void ResetTicks()
    {
        _tickCounter = 0;
    }
}
