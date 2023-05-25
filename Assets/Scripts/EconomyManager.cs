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
        var resourceChangedHandler = new Action<ResourceInfo, int, int>((resource, oldAmount, newAmount) =>
        {
            //Debug.Log($"{resource.internalName} {(newAmount - oldAmount < 0 ? "" : "+")}{newAmount - oldAmount}");
        });

        resources.ForEach(r => r.OnCurAmountChanged += resourceChangedHandler);

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
                .ForEach(c =>
                {
                    if (c.CurAmount + resourceGenerator.goldGenerationRate <= c.maxAmount)
                    {
                        c.CurAmount += resourceGenerator.goldGenerationRate;
                    }
                    else
                    {
                        c.CurAmount += c.maxAmount - c.CurAmount;
                    }
                });

            resources.Where(c => c.internalName == "stone").ToList()
                .ForEach(c =>
                {
                    if (c.CurAmount + resourceGenerator.stoneGenerationRate <= c.maxAmount)
                    {
                        c.CurAmount += resourceGenerator.stoneGenerationRate;
                    }
                    else
                    {
                        c.CurAmount += c.maxAmount - c.CurAmount;
                    }
                });

            resources.Where(c => c.internalName == "wood").ToList()
                .ForEach(c =>
                {
                    if (c.CurAmount + resourceGenerator.woodGenerationRate <= c.maxAmount)
                    {
                        c.CurAmount += resourceGenerator.woodGenerationRate;
                    }
                    else
                    {
                        c.CurAmount += c.maxAmount - c.CurAmount;
                    }
                });

            resources.Where(c => c.internalName == "iron").ToList()
                .ForEach(c =>
                {
                    if (c.CurAmount + resourceGenerator.ironGenerationRate <= c.maxAmount)
                    {
                        c.CurAmount += resourceGenerator.ironGenerationRate;
                    }
                    else
                    {
                        c.CurAmount += c.maxAmount - c.CurAmount;
                    }
                });
        }
        UIDocManager.Instance.SetResourceHUD(resources);
    }

    public bool TryPurchase(BuildingSO buildingSO)
    {
        if (resources.First(c => c.internalName == "gold").CurAmount - buildingSO.goldCost < 0)
        {
            return false;
        }
        if (resources.First(c => c.internalName == "stone").CurAmount - buildingSO.stoneCost < 0)
        {
            return false;
        }
        if (resources.First(c => c.internalName == "wood").CurAmount - buildingSO.woodCost < 0)
        {
            return false;
        }
        if (resources.First(c => c.internalName == "iron").CurAmount - buildingSO.ironCost < 0)
        {
            return false;
        }
        if (resources.First(c => c.internalName == "population").CurAmount + buildingSO.populationCost > resources.First(c => c.internalName == "population").maxAmount)
        {
            return false;
        }

        resources.First(c => c.internalName == "gold").CurAmount -= buildingSO.goldCost;
        resources.First(c => c.internalName == "stone").CurAmount -= buildingSO.stoneCost;
        resources.First(c => c.internalName == "wood").CurAmount -= buildingSO.woodCost;
        resources.First(c => c.internalName == "iron").CurAmount -= buildingSO.ironCost;

        return true;
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
                        c.CurAmount += rg.populationConsumptionAmount;
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
                        c.CurAmount -= rg.populationConsumptionAmount;
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

    [SerializeField]
    private int _curAmount;
    public Action<ResourceInfo, int, int> OnCurAmountChanged;
    public int CurAmount
    {
        get => _curAmount;
        set
        {
            var oldVal = _curAmount;
            _curAmount = value;
            if (oldVal != _curAmount && OnCurAmountChanged != null)
                OnCurAmountChanged(this, oldVal, _curAmount);
        }
    }
    
    public int maxAmount;
}