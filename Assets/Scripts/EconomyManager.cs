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

    public List<ResourceInfo> resources = new List<ResourceInfo>();

    public List<BuildingController> buildings = new List<BuildingController>();
    public List<ResourceGenerator> resourceGenerators = new List<ResourceGenerator>();

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

    private void Start()
    {
        UIDocManager.Instance.SetResourceHUD(resources);
    }
    
    public void Tick()
    {
        ProcessResourceGenerators();
    }

    private void ProcessResourceGenerators()
    {
        foreach (var resourceGenerator in resourceGenerators)
        {
            resources.Where(c => c.internalName == "gold").ToList()
                .ForEach(c => c.curAmount += resourceGenerator.goldGenerationRate);

            resources.Where(c => c.internalName == "stone").ToList()
                .ForEach(c => c.curAmount += resourceGenerator.stoneGenerationRate);

            resources.Where(c => c.internalName == "wood").ToList()
                .ForEach(c => c.curAmount += resourceGenerator.woodGenerationRate);

            resources.Where(c => c.internalName == "iron").ToList()
                .ForEach(c => c.curAmount += resourceGenerator.ironGenerationRate);
        }
        UIDocManager.Instance.SetResourceHUD(resources);
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
    private void OnDisable()
    {
        CancelAutoTick();
    }

    public void Register<T>(T entity)
    {
        switch (entity)
        {
            case BuildingController bc:
                buildings.Add(bc);
                break;
            case ResourceGenerator rg:
                resourceGenerators.Add(rg);
                resources.Where(c => c.internalName == "population").ToList()
                    .ForEach(c =>
                    {
                        c.curAmount += rg.populationConsumptionAmount;
                        c.maxAmount += rg.populationGenerationAmount;
                    });
                UIDocManager.Instance.SetResourceHUD(resources);
                break;
        }
    }

    public void Unregister<T>(T entity)
    {
        switch (entity)
        {
            case BuildingController bc:
                buildings.Remove(bc);
                break;
            case ResourceGenerator rg:
                resourceGenerators.Remove(rg);
                resources.Where(c => c.internalName == "population").ToList()
                    .ForEach(c =>
                    {
                        c.curAmount -= rg.populationConsumptionAmount;
                        c.maxAmount -= rg.populationGenerationAmount;
                    });
                UIDocManager.Instance.SetResourceHUD(resources);
                break;
        }
    }
}


[Serializable]
public class ResourceInfo
{
    public string internalName;
    public string displayName;
    public Texture2D icon;
    public int curAmount;
    public int maxAmount;
}