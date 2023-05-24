using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    public int goldGenerationRate;
    public int stoneGenerationRate;
    public int woodGenerationRate;
    public int ironGenerationRate;

    public int populationGenerationAmount;
    public int populationConsumptionAmount;
    
    void Start()
    {
        EconomyManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        EconomyManager.Instance.Unregister(this);
    }
}
