using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySO : ScriptableObject
{
    public GameObject prefab;
    public Texture2D thumbnail;
    public string displayName;
    public string description;
    public int level;

    public int goldCost;
    public int stoneCost;
    public int woodCost;
    public int ironCost;
    public int populationCost;

    public int goldGenerationRate;
    public int stoneGenerationRate;
    public int woodGenerationRate;
    public int ironGenerationRate;

    public int populationGenerationAmount;
    public int populationConsumptionAmount;

    public void ConvertData(EntityController entity)
    {
        entity.thumbnail = this.thumbnail;
        entity.displayName = this.displayName;
        entity.description = this.description;
        entity.level = this.level;

        entity.goldCost = this.goldCost;
        entity.stoneCost = this.stoneCost;
        entity.woodCost = this.woodCost;
        entity.ironCost = this.ironCost;
        entity.populationCost = this.populationCost;
        
        if (entity.TryGetComponent<ResourceGenerator>(out var resourceGenerator))
        {
            resourceGenerator.goldGenerationRate = this.goldGenerationRate;
            resourceGenerator.stoneGenerationRate = this.stoneGenerationRate;
            resourceGenerator.woodGenerationRate = this.woodGenerationRate;
            resourceGenerator.ironGenerationRate = this.ironGenerationRate;

            resourceGenerator.populationGenerationAmount = this.populationGenerationAmount;
            resourceGenerator.populationConsumptionAmount = this.populationConsumptionAmount;
        }
    }
}
