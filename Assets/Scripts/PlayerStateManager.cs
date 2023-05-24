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

    private Action modalAcceptAction;
    private Action modalDeclineAction;

    private List<ToolbarAction> toolbarActions = new List<ToolbarAction>();

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
        if (HandleModalActions()) return;
        if (HandleSelectionActions()) return;
        if (HandlePendingActions()) return;
        if (HandleRepeatingActions()) return;
        if (HandleToolbarHotkeys()) return;
    }

    private bool HandleToolbarHotkeys()
    {
        if (toolbarActions.Count == 0) return false;
        foreach (var toolbarAction in toolbarActions)
        {
            if (inputManager.PollKeyDown(this, toolbarAction.hotkey))
            {
                toolbarAction.action.Invoke();
                return true;
            }
        }
        return false;
    }

    private bool HandleSelectionActions()
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

            return true;
        }
        else if (inputManager.PollKeyDown(this, KeyAction.CancelSelection))
        {
            if (selectedEntity != null)
            {
                HandleDeselection();
            }

            return true;
        }

        return false;
    }

    private bool HandleModalActions()
    {
        if (modalAcceptAction == null) return false;
        if (inputManager.PollKeyDown(this, KeyAction.ConfirmHotkey))
        {
            AcceptModalConfirmationWithHotkey();
            return true;
        }
        else if (inputManager.PollKeyDown(this, KeyAction.CancelHotkey))
        {
            DeclineModalConfirmationWithHotkey();
            return true;
        }

        return false;
    }

    private bool HandleRepeatingActions()
    {
        if (repeatingAction == null) return false;
        if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.ConfirmClick) ||
            inputManager.PollKeyDown(this, KeyAction.ConfirmHotkey))
        {
            CompleteRepeatingAction();
            return true;
        }
        else if (inputManager.PollKeyDown(this, KeyAction.CancelClick) ||
                 inputManager.PollKeyUp(this, KeyAction.RepeatModifier) ||
                 inputManager.PollKeyUp(this, KeyAction.ConfirmClick) ||
                 inputManager.PollKeyDown(this, KeyAction.CancelClick))
        {
            CancelRepeatingAction();
            return true;
        }

        return false;
    }

    private bool HandlePendingActions()
    {
        if (pendingAction == null) return false;
        if (inputManager.PollKeyUpIgnoreUI(this, KeyAction.ConfirmClick) ||
            inputManager.PollKeyDown(this, KeyAction.ConfirmHotkey))
        {
            if (inputManager.PollKey(this, KeyAction.RepeatModifier))
            {
                CompleteAndRepeatPendingAction();
            }
            else
            {
                CompletePendingAction();
            }

            return true;
        }
        else if (inputManager.PollKeyDown(this, KeyAction.CancelClick) ||
                 inputManager.PollKeyUp(this, KeyAction.ConfirmClick) ||
                 inputManager.PollKeyDown(this, KeyAction.CancelHotkey))
        {
            CancelPendingAction();
            return true;
        }

        return false;
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
        inputManager.Unregister(this, DefaultActionMaps.SelectionActions);
        if (selectedEntity == null) return;
        selectedEntity.Deselect();
        selectedEntity = null;
        UIDocManager.Instance.ClearEntityHUD();
        UIDocManager.Instance.ClearToolbarButtons();
    }

    private void HandleBuildingSelection(BuildingController building)
    {
        if (selectedEntity == building) return;
        if (selectedEntity != null)
        {
            HandleDeselection();
        }

        selectedEntity = building;
        selectedEntity.Select();
        UIDocManager.Instance.SetEntityHUD(building);

        SetToolbarButtons(building);
        inputManager.Register(this, DefaultActionMaps.SelectionActions);
    }

    public void SetToolbarButtons(EntityController entity)
    {
        toolbarActions.Clear();

        var deleteAction = new ToolbarAction();
        deleteAction.entity = entity;
        deleteAction.icon = deleteIcon;
        deleteAction.action = () =>
        {
            if (deleteAction.isConfirmationPrompted)
            {
                SetModalConfirmation(() =>
                    {
                        HandleDeselection();
                        entity.DestroySelf();
                    }, () =>
                    {
                        SetToolbarButtons(entity);
                    },
                    "Confirm?",
                    "Are you sure you want to destroy this building? Resources will not be refunded.");
            }
            else
            {
                HandleDeselection();
                entity.DestroySelf();
            }
            
        };
        deleteAction.hotkey = KeyAction.DeleteSelected;
        deleteAction.isConfirmationPrompted = true;
        toolbarActions.Add(deleteAction);

        var upgradeAction = new ToolbarAction();
        upgradeAction.entity = entity;
        upgradeAction.icon = upgradeIcon;
        upgradeAction.action = () =>
        {
            if (upgradeAction.isConfirmationPrompted)
            {
                SetModalConfirmation(() =>
                    {
                        entity.Upgrade();
                        SetToolbarButtons(entity);
                    }, () =>
                    {
                        SetToolbarButtons(entity);
                    },
                    "Confirm?",
                    "Are you sure you want to upgrade this building?");
            }
            else
            {
                entity.Upgrade();
            }
        };
        upgradeAction.hotkey = KeyAction.UpgradeSelected;
        upgradeAction.isConfirmationPrompted = true;
        toolbarActions.Add(upgradeAction);

        UIDocManager.Instance.SetToolbarButtons(toolbarActions);
    }

    public void SetModalConfirmation(Action acceptAction, Action declineAction = null, string title = "No Name", string description = "No Desc")
    {
        inputManager.HoldActionMap(this);
        UIDocManager.Instance.SetConfirmationModalButtons(() =>
        {
            AcceptModalConfirmation(acceptAction);
        }, () =>
        {
            if (declineAction != null) declineAction.Invoke();
            DeclineModalConfirmation();
        }, title, description);
        inputManager.Register(this, DefaultActionMaps.ModalHotkeys);
    }

    public void AcceptModalConfirmation(Action action)
    {
        inputManager.Unregister(this, DefaultActionMaps.ModalHotkeys);
        inputManager.ReleaseActionMap(this);
        
        action.Invoke();
        modalAcceptAction = null;
        modalDeclineAction = null;
    }

    public void DeclineModalConfirmation()
    {
        inputManager.ReleaseActionMap(this);
        inputManager.Unregister(this, DefaultActionMaps.ModalHotkeys);
        modalAcceptAction = null;
        modalDeclineAction = null;
    }

    public void CancelModalConfirmation()
    {
        inputManager.ReleaseActionMap(this);
        inputManager.Unregister(this, DefaultActionMaps.ModalHotkeys);
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

    public void AcceptModalConfirmationWithHotkey()
    {
        modalAcceptAction.Invoke();
    }

    public void DeclineModalConfirmationWithHotkey()
    {
        modalDeclineAction.Invoke();
    }

    public void SetModalConfirmationActionsForHotkey(Action acceptAction, Action declineAction)
    {
        modalAcceptAction = acceptAction;
        modalDeclineAction = declineAction;
    }
}

public class ToolbarAction
{
    public Action action;
    public Texture2D icon;
    public EntityController entity;
    public KeyAction hotkey;
    public bool isConfirmationPrompted;
}