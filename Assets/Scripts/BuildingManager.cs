using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public List<BuildingSO> buildingData = new List<BuildingSO>();

    private BuildingManager()
    {
        Instance = this;
    }

    private void Start()
    {
        UIDocManager.Instance.SetBuildMenu(buildingData, BuildPendingHandler);
    }

    private void BuildPendingHandler(BuildingSO buildingObject)
    {
        PlayerStateManager.Instance.SetModalConfirmation(() =>
            {
                BuildPreview(buildingObject);
                UIDocManager.Instance.SetBuildMenu(buildingData, BuildPendingHandler);
            }, () =>
            {
                UIDocManager.Instance.SetBuildMenu(buildingData, BuildPendingHandler);
            },
            "Confirm?",
            "Are you sure you want to purchase this building?");
    }

    private void BuildPreview(BuildingSO buildingObject)
    {
        BuildPreviewer.Instance.StartPreview(buildingObject.prefab.transform);
        PlayerStateManager.Instance.StartPendingAction((continueIteration) => Build(buildingObject, continueIteration), CancelBuild);
    }

    private bool Build(BuildingSO buildingObject, bool continueIteration = false)
    {
        if (!BuildPreviewer.Instance.HasRoomToBuild() || !EconomyManager.Instance.TryPurchase(buildingObject))
        {
            BuildPreviewer.Instance.StopPreview();
            BuildPreview(buildingObject);
            return false;
        }

        if (!continueIteration)
        {
            BuildPreviewer.Instance.StopPreview();
        }
        
        var buildTransform = BuildPreviewer.Instance.transform;
        var bounds = buildingObject.prefab.GetComponent<MeshRenderer>().bounds.size;
        var offsetPos = new Vector3(Mathf.FloorToInt(bounds.x) % 2 != 0 ? 0.5f : 0, 0, Mathf.FloorToInt(bounds.z) % 2 != 0 ? 0.5f : 0);
        var buildingGameObject = Instantiate(buildingObject.prefab, buildTransform.position + offsetPos, buildTransform.rotation);
        var building = buildingGameObject.GetOrAddComponent<BuildingController>();
        var resourceGenerator = building.GetOrAddComponent<ResourceGenerator>();
        buildingObject.ConvertData(building);
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