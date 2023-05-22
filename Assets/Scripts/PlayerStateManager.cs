using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour, IInputController
{
    public static PlayerStateManager Instance;
    public EntityController selectedEntity;
    public Material selectedMaterial;

    private InputManager inputManager;
    private Camera mainCamera;

    private Func<bool> pendingAction;
    private Action cancelAction;

    private PlayerStateManager()
    {
        Instance = this;
    }

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
        if (inputManager.PollKeyDownIgnoreUI(this, KeyAction.LeftClick))
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

        if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.Confirm))
        {
            CompletePendingAction();
        }
        else if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.Cancel) ||
                 inputManager.PollKeyUp(this, KeyAction.Confirm))
        {
            CancelPendingAction();
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

    public void SetConfirmation(Action acceptAction)
    {
        inputManager.HoldActionMap(this);
        UIDocManager.Instance.SetConfirmationModalButtons(() => AcceptConfirmation(acceptAction), DeclineConfirmation);
    }

    public void AcceptConfirmation(Action action)
    {
        inputManager.ReleaseActionMap(this);
        action.Invoke();
    }

    public void DeclineConfirmation()
    {
        inputManager.ReleaseActionMap(this);
    }

    public void CancelConfirmation()
    {
        inputManager.ReleaseActionMap(this);
    }

    public void StartPendingAction(Func<bool> action, Action cancellationAction)
    {
        pendingAction = action;
        cancelAction = cancellationAction;
        inputManager.HoldActionMap(this);
        inputManager.Register(this, DefaultActionMaps.ConfirmationActions);
    }

    public void CompletePendingAction()
    {
        if (pendingAction == null)
        {
            CancelPendingAction();
            return;
        }

        inputManager.ReleaseActionMap(this);
        var result = pendingAction.Invoke();
        if (!result) return;

        pendingAction = null;
        cancelAction = null;
        inputManager.Unregister(this, DefaultActionMaps.ConfirmationActions);
    }

    public void CancelPendingAction()
    {
        inputManager.ReleaseActionMap(this);
        cancelAction.Invoke();
        pendingAction = null;
        cancelAction = null;
        inputManager.Unregister(this, DefaultActionMaps.ConfirmationActions);
    }
}
