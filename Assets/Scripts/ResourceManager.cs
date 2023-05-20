using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public List<string> buildings = new List<string>();
    public List<BuildingController> buildingPrefabs = new List<BuildingController>();

    private ResourceManager()
    {
        Instance = this;
    }

    private void Start()
    {
        UIDocManager.Instance.ShowBuildHUD(buildingPrefabs, BuildPendingHandler);
    }

    private void BuildPendingHandler(BuildingController building)
    {
        PlayerStateManager.Instance.SetConfirmation(() => BuildPreview(building));
    }

    private void BuildPreview(BuildingController building)
    {
        BuildPreviewer.Instance.StartPreview(building.transform);
        PlayerStateManager.Instance.StartPendingAction(() => Build(building), CancelBuild);
    }

    private void Build(BuildingController building)
    {
        Debug.Log($"Built {building.displayName}");
        BuildPreviewer.Instance.StopPreview();
        var buildTransform = BuildPreviewer.Instance.transform;
        Instantiate(building, buildTransform.position, buildTransform.rotation);
    }

    private void CancelBuild()
    {
        BuildPreviewer.Instance.StopPreview();
    }
}
