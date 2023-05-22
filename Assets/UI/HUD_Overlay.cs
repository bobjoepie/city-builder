using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD_Overlay : VisualElement
{
    public VisualElement bottomToolbar;
    public VisualElement entityThumbnail;
    public VisualElement entityInfo;
    public Label entityName;
    public Label entityDescription;
    public List<Button> toolbarButtons;

    public VisualElement buildModal;

    public VisualElement confirmationModal;
    public Button confirmationAcceptButton;
    public Button confirmationDeclineButton;

    public new class UxmlFactory : UxmlFactory<HUD_Overlay, UxmlTraits> { }

    public HUD_Overlay()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void InitHUD()
    {
        //InitBlockers();
        InitBottomToolbar();
        InitEntityInfo();
        InitBuildMenu();
        InitConfirmationModal();
    }

    private void InitBottomToolbar()
    {
        bottomToolbar = this.Q("bottom-toolbar");
        toolbarButtons = bottomToolbar.Q("toolbar-buttons").Children().Select(c => c.Children().First()).Where(c => c is Button).Cast<Button>().ToList();
        ShowPanel(PanelType.BottomToolbar);
    }

    private void InitConfirmationModal()
    {
        confirmationModal = this.Q("confirmation-modal");
        confirmationAcceptButton = confirmationModal.Q<Button>("confirmation-accept-button");
        confirmationDeclineButton = confirmationModal.Q<Button>("confirmation-decline-button");

        HidePanel(PanelType.ConfirmationModal);
    }

    private void InitBuildMenu()
    {
        buildModal = this.Q("build-modal");

        var buildMenuButton = this.Q<Button>("build-button");
        buildMenuButton.clickable.clicked += () => { TogglePanel(PanelType.BuildMenu); };
        buildMenuButton.AllowUIButtonKeyModifiers();
        //var buildButtons = buildMenu.Children().Where(c => c is Button).Cast<Button>().ToList();
        //for (int i = 0; i < buildButtons.Count; i++)
        //{
        //    int ii = i;
        //    buildButtons[ii].clickable.clicked += () => { Debug.Log(ii); };
        //}
        HidePanel(PanelType.BuildMenu);
    }

    private void InitBlockers()
    {
        var blockers = this.Query(className: "blocker").ToList();
        foreach (var blocker in blockers)
        {
            UIDocManager.Instance.AddRaycastBlocker(blocker);
        }
    }

    private void InitEntityInfo()
    {
        entityThumbnail = this.Q("EntityThumbnail");
        entityInfo = this.Q("EntityInfo");
        entityName = this.Q<Label>("EntityName");
        entityDescription = this.Q<Label>("EntityDescription");

        SetEntityThumbnail(null);
        SetEntityName(string.Empty);
        SetEntityDescription(string.Empty);

        HidePanel(PanelType.EntityThumbnail);
        HidePanel(PanelType.EntityInfo);
    }

    public void Show()
    {
        this.style.visibility = Visibility.Visible;
    }

    public void Hide()
    {
        this.style.visibility = Visibility.Hidden;
    }

    public void ToggleView()
    {
        if (this.style.visibility == Visibility.Visible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void ShowPanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.BottomToolbar:
                bottomToolbar.style.visibility = Visibility.Visible;
                UIDocManager.Instance.AddRaycastBlocker(bottomToolbar);
                break;
            case PanelType.EntityInfo:
                entityInfo.style.visibility = Visibility.Visible;
                break;
            case PanelType.EntityThumbnail:
                entityThumbnail.style.visibility = Visibility.Visible;
                break;
            case PanelType.BuildMenu:
                buildModal.style.visibility = Visibility.Visible;
                UIDocManager.Instance.AddRaycastBlocker(buildModal);
                break;
            case PanelType.ConfirmationModal:
                confirmationModal.style.visibility = Visibility.Visible;
                UIDocManager.Instance.AddRaycastBlocker(confirmationModal);
                break;
        }
    }

    public void HidePanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.BottomToolbar:
                bottomToolbar.style.visibility = Visibility.Hidden;
                UIDocManager.Instance.RemoveRaycastBlocker(bottomToolbar);
                break;
            case PanelType.EntityInfo:
                entityInfo.style.visibility = Visibility.Hidden;
                break;
            case PanelType.EntityThumbnail:
                entityThumbnail.style.visibility = Visibility.Hidden;
                break;
            case PanelType.BuildMenu:
                buildModal.style.visibility = Visibility.Hidden;
                UIDocManager.Instance.RemoveRaycastBlocker(buildModal);
                break;
            case PanelType.ConfirmationModal:
                confirmationModal.style.visibility = Visibility.Hidden;
                UIDocManager.Instance.RemoveRaycastBlocker(confirmationModal);
                break;
        }
    }

    public void TogglePanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.BottomToolbar:
                if (IsPanelHidden(bottomToolbar))
                    ShowPanel(panelType);
                else
                    HidePanel(panelType);
                break;
            case PanelType.EntityInfo:
                if (IsPanelHidden(entityInfo)) 
                    ShowPanel(panelType);
                else 
                    HidePanel(panelType);
                break;
            case PanelType.EntityThumbnail:
                if (IsPanelHidden(entityThumbnail))
                    ShowPanel(panelType);
                else
                    HidePanel(panelType);
                break;
            case PanelType.BuildMenu:
                if (IsPanelHidden(buildModal))
                    ShowPanel(panelType);
                else
                    HidePanel(panelType);
                break;
            case PanelType.ConfirmationModal:
                if (IsPanelHidden(confirmationModal))
                    ShowPanel(panelType);
                else
                    HidePanel(panelType);
                break;
        }
    }

    public bool IsPanelHidden(VisualElement element)
    {
        return element.style.visibility == Visibility.Hidden;
    }

    public void SetEntityThumbnail(Texture2D image)
    {
        entityThumbnail.style.backgroundImage = image;
    }

    public void SetEntityName(string name)
    {
        entityName.text = name;
    }

    public void SetEntityDescription(string description)
    {
        entityDescription.text = description;
    }

    public void SetBuildMenu(List<BuildingController> buildings, Action<BuildingController> buildAction)
    {
        ClearBuildMenu();
        var buildButtons = buildModal.Q("build-menu").Children().Select(c => c.Children().First()).Where(c => c is Button).Cast<Button>().ToList();
        for (int i = 0; i < buildings.Count; i++)
        {
            if (i >= buildButtons.Count) break;
            int ii = i;
            buildButtons[ii].clickable = new Clickable(() => { });
            buildButtons[ii].clickable.clicked += () => buildAction.Invoke(buildings[ii]);
            buildButtons[ii].AllowUIButtonKeyModifiers();
            buildButtons[ii].text = buildings[ii].displayName;
        }
    }

    public void ClearBuildMenu()
    {
        var buildButtons = buildModal.Q("build-menu").Children().Select(c => c.Children().First()).Where(c => c is Button).Cast<Button>().ToList();
        foreach (var button in buildButtons)
        {
            button.clickable = new Clickable(() => { });
            button.text = string.Empty;
        }
    }

    public void SetConfirmationModalButtons(Action acceptAction, Action declineAction)
    {
        confirmationAcceptButton.clickable = new Clickable(() => { });
        confirmationAcceptButton.clickable.clicked += () =>
        {
            HidePanel(PanelType.ConfirmationModal);
            acceptAction.Invoke();
        };
        confirmationAcceptButton.AllowUIButtonKeyModifiers();

        confirmationDeclineButton.clickable = new Clickable(() => { });
        confirmationDeclineButton.clickable.clicked += () =>
        {
            HidePanel(PanelType.ConfirmationModal);
            declineAction.Invoke();
        };
        confirmationDeclineButton.AllowUIButtonKeyModifiers();
    }

    public void SetToolbarButtons(List<ToolbarAction> toolbarActions)
    {
        for (int i = 0; i < toolbarButtons.Count; i++)
        {
            int ii = i;
            toolbarButtons[ii].clickable = new Clickable(() => { });
            toolbarButtons[ii].style.backgroundImage = null;

            if (i >= toolbarActions.Count) continue;

            toolbarButtons[ii].clickable.clicked += () => toolbarActions[ii].action.Invoke();
            toolbarButtons[ii].AllowUIButtonKeyModifiers();
            toolbarButtons[ii].style.backgroundImage = toolbarActions[ii].icon;
        }
    }

    public void ClearToolbarButtons()
    {
        foreach (var button in toolbarButtons)
        {
            button.clickable = new Clickable(() => { });
            button.style.backgroundImage = null;
        }
    }
}

public enum PanelType
{
    EntityInfo,
    EntityThumbnail,
    BottomToolbar,
    BuildMenu,
    ConfirmationModal
}

public enum ToolButtonType
{
    DestroySelected
}

public class ToolbarAction
{
    public Action action;
    public Texture2D icon;
    public EntityController entity;
}