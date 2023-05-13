using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour, IInputController
{
    public EntityController selectedEntity;
    public Material selectedMaterial;

    private InputManager inputManager;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.Register(this, DefaultActionMaps.MouseKeyActions);
        inputManager.Register(this, DefaultActionMaps.MovementKeyActions);
        inputManager.Register(this, DefaultActionMaps.CameraActions);
        inputManager.Register(this, DefaultActionMaps.NumberKeyActions);

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    private void CheckInputs()
    {
        if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.LeftClick))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity))
            {
                HandleSelection(hit);
            }
            else
            {
                HandleDeselection();
            }
        }
    }

    private void HandleSelection(RaycastHit hit)
    {
        if (hit.transform.root.TryGetComponent<BuildingController>(out var building))
        {
            HandleBuildingSelection(building);
        }
        else
        {
            HandleDeselection();
        }
    }

    private void HandleDeselection()
    {
        if (selectedEntity == null) return;
        selectedEntity.Deselect();
        selectedEntity = null;
        UIDocManager.Instance.HideEntityHUD();
    }

    private void HandleBuildingSelection(BuildingController building)
    {
        if (selectedEntity == building) return;
        if (selectedEntity != null) selectedEntity.Deselect();

        selectedEntity = building;
        selectedEntity.Select();
        UIDocManager.Instance.ShowEntityHUD(building);
    }
}
