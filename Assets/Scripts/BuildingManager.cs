using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public List<BuildingController> buildingPrefabs = new List<BuildingController>();

    private BuildingManager()
    {
        Instance = this;
    }

    private void Start()
    {
        UIDocManager.Instance.SetBuildMenu(buildingPrefabs, BuildPendingHandler);
    }

    private void BuildPendingHandler(BuildingController buildingPrefab)
    {
        PlayerStateManager.Instance.SetModalConfirmation(() =>
            {
                BuildPreview(buildingPrefab);
                UIDocManager.Instance.SetBuildMenu(buildingPrefabs, BuildPendingHandler);
            }, () =>
            {
                UIDocManager.Instance.SetBuildMenu(buildingPrefabs, BuildPendingHandler);
            },
            "Confirm?",
            "Are you sure you want to purchase this building?");
    }

    private void BuildPreview(BuildingController buildingPrefab)
    {
        BuildPreviewer.Instance.StartPreview(buildingPrefab.transform);
        PlayerStateManager.Instance.StartPendingAction((continueIteration) => Build(buildingPrefab, continueIteration), CancelBuild);
    }

    private bool Build(BuildingController buildingPrefab, bool continueIteration = false)
    {
        if (!BuildPreviewer.Instance.CanBuild())
        {
            BuildPreviewer.Instance.StopPreview();
            BuildPreview(buildingPrefab);
            return false;
        }

        if (!continueIteration)
        {
            BuildPreviewer.Instance.StopPreview();
        }
        
        var buildTransform = BuildPreviewer.Instance.transform;
        var building = Instantiate(buildingPrefab, buildTransform.position, buildTransform.rotation);
        var resourceGenerator = building.GetOrAddComponent<ResourceGenerator>();
        resourceGenerator.goldGenerationRate = 10;
        resourceGenerator.stoneGenerationRate = 1;
        resourceGenerator.woodGenerationRate = 1;
        resourceGenerator.ironGenerationRate = 1;
        resourceGenerator.populationConsumptionAmount = 1;
        resourceGenerator.populationGenerationAmount = 4;
        return true;
    }

    private void CancelBuild()
    {
        BuildPreviewer.Instance.StopPreview();
    }

    public bool CanAfford()
    {
        return true;
    }
}