using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }
    public List<BuildingController> buildings = new List<BuildingController>();

    public bool autoTickEnabled = true;
    public float autoTickFrequency = 5f;

    private CancellationTokenSource cancellationToken;

    private EconomyManager()
    {
        Instance = this;
    }

    private void Awake()
    {
        if (autoTickEnabled)
        {
            StartAutoTick();
        }
    }

    private void OnDisable()
    {
        CancelAutoTick();
    }

    public void Tick()
    {
        Debug.Log("Tick");
    }

    private async UniTask AutoTick()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (autoTickEnabled)
            {
                Tick();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(autoTickFrequency));
        }
    }

    public void StartAutoTick()
    {
        if (cancellationToken != null) return;
        cancellationToken = new CancellationTokenSource();
        AutoTick().Forget();
    }

    public void CancelAutoTick()
    {
        cancellationToken.Cancel();
        cancellationToken.Dispose();
        autoTickEnabled = false;
    }

    public void Register(EntityController entity)
    {
        switch (entity)
        {
            case BuildingController bc:
                buildings.Add(bc);
                break;
        }
    }

    public void Unregister(EntityController entity)
    {
        switch (entity)
        {
            case BuildingController bc:
                buildings.Remove(bc);
                break;
        }
    }
}
