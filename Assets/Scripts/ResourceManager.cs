using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public List<BuildingController> buildingPrefabs = new List<BuildingController>();
    public List<ResourceInfo> resources = new List<ResourceInfo>();

    private ResourceManager()
    {
        Instance = this;
    }

    private void Start()
    {
        UIDocManager.Instance.SetBuildMenu(buildingPrefabs, BuildPendingHandler);
        UIDocManager.Instance.SetResourceHUD(resources);
    }

    private void BuildPendingHandler(BuildingController building)
    {
        PlayerStateManager.Instance.SetConfirmation(() =>
        {
            BuildPreview(building);
        },
        "Confirm?",
        "Are you sure you want to purchase this building?");
    }

    private void BuildPreview(BuildingController building)
    {
        BuildPreviewer.Instance.StartPreview(building.transform);
        PlayerStateManager.Instance.StartPendingAction((continueIteration) => Build(building, continueIteration), CancelBuild);
    }

    private bool Build(BuildingController building, bool continueIteration = false)
    {
        if (!BuildPreviewer.Instance.CanBuild())
        {
            BuildPreviewer.Instance.StopPreview();
            //PlayerStateManager.Instance.CancelPendingAction();
            BuildPreview(building);
            return false;
        }

        if (!continueIteration)
        {
            BuildPreviewer.Instance.StopPreview();
        }
        
        var buildTransform = BuildPreviewer.Instance.transform;
        Instantiate(building, buildTransform.position, buildTransform.rotation);
        return true;
    }

    private void CancelBuild()
    {
        BuildPreviewer.Instance.StopPreview();
    }
}

[Serializable]
public class ResourceInfo
{
    public string displayName;
    public Texture2D icon;
    public int curAmount;
    public int maxAmount;
}