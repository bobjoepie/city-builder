using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour, IInputController
{
    public static PlayerStateManager Instance;
    public EntityController selectedEntity;
    public Material selectedMaterial;
    public Texture2D deleteIcon;
    public Texture2D upgradeIcon;

    private InputManager inputManager;
    private Camera mainCamera;

    private Func<bool, bool> pendingAction;
    private Func<bool, bool> repeatingAction;
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

        else if (pendingAction != null)
        {
            if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.Confirm))
            {
                if (inputManager.PollKey(this, KeyAction.RepeatModifier))
                {
                    CompleteAndRepeatPendingAction();
                }
                else
                {
                    CompletePendingAction();
                }
            }
            else if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.Cancel) ||
                     inputManager.PollKeyUp(this, KeyAction.Confirm))
            {
                CancelPendingAction();
            }
        }

        else if (repeatingAction != null)
        {
            if (inputManager.PollKeyUp(this, KeyAction.RepeatModifier))
            {
                CancelRepeatingAction();
            }
            else if (inputManager.PollKeyUp(this, KeyAction.Confirm))
            {
                CompleteRepeatingAction();
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
        UIDocManager.Instance.ClearEntityHUD();
        UIDocManager.Instance.ClearToolbarButtons();
    }

    private void HandleBuildingSelection(BuildingController building)
    {
        if (selectedEntity == building) return;
        if (selectedEntity != null) selectedEntity.Deselect();

        selectedEntity = building;
        selectedEntity.Select();
        UIDocManager.Instance.SetEntityHUD(building);

        var toolbarActions = new List<ToolbarAction>();

        var deleteAction = new ToolbarAction();
        deleteAction.entity = building;
        deleteAction.icon = deleteIcon;
        deleteAction.action = () =>
        {
            SetConfirmation(() =>
            {
                HandleDeselection();
                building.DestroySelf();
            },
            "Confirm?",
            "Are you sure you want to destroy this building? Resources will not be refunded.");
        };
        toolbarActions.Add(deleteAction);

        var upgradeAction = new ToolbarAction();
        upgradeAction.entity = building;
        upgradeAction.icon = upgradeIcon;
        upgradeAction.action = () =>
        {
            SetConfirmation(() =>
            {
                building.Upgrade();
            },
            "Confirm?",
            "Are you sure you want to upgrade this building?");
        };
        toolbarActions.Add(upgradeAction);

        UIDocManager.Instance.SetToolbarButtons(toolbarActions);
    }

    public void SetConfirmation(Action acceptAction, string title, string description)
    {
        inputManager.HoldActionMap(this);
        UIDocManager.Instance.SetConfirmationModalButtons(() => AcceptConfirmation(acceptAction), DeclineConfirmation, title, description);
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

    public void StartPendingAction(Func<bool, bool> action, Action cancellationAction)
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

        var result = pendingAction.Invoke(false);
        if (!result) return;
        pendingAction = null;
        cancelAction = null;

        inputManager.ReleaseActionMap(this);
        inputManager.Unregister(this, DefaultActionMaps.ConfirmationActions);
    }

    public void CancelPendingAction()
    {
        cancelAction.Invoke();
        pendingAction = null;
        cancelAction = null;

        inputManager.ReleaseActionMap(this);
        inputManager.Unregister(this, DefaultActionMaps.ConfirmationActions);
    }

    public void CompleteAndRepeatPendingAction()
    {
        if (pendingAction == null)
        {
            CancelPendingAction();
            return;
        }

        repeatingAction = pendingAction;
        pendingAction = null;

        repeatingAction.Invoke(true);
    }

    public void CancelRepeatingAction()
    {
        cancelAction.Invoke();
        repeatingAction = null;
        cancelAction = null;

        inputManager.ReleaseActionMap(this);
        inputManager.Unregister(this, DefaultActionMaps.ConfirmationActions);
    }

    public void CompleteRepeatingAction()
    {
        repeatingAction.Invoke(true);
    }
}
