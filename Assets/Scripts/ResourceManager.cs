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
        UIDocManager.Instance.SetBuildMenu(buildingPrefabs, BuildPendingHandler);
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

    private bool Build(BuildingController building)
    {
        if (!BuildPreviewer.Instance.CanBuild())
        {
            BuildPreviewer.Instance.StopPreview();
            BuildPreview(building);
            return false;
        }

        BuildPreviewer.Instance.StopPreview();
        var buildTransform = BuildPreviewer.Instance.transform;
        Instantiate(building, buildTransform.position, buildTransform.rotation);
        return true;
    }

    private void CancelBuild()
    {
        BuildPreviewer.Instance.StopPreview();
    }
}
