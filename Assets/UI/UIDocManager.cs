using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDocManager : MonoBehaviour
{
    public static UIDocManager Instance { get; private set; }
    private UIDocument document;
    private static VisualElement root;
    private HUD_Overlay hudOverlay;
    private List<VisualElement> rayCastBlockers = new List<VisualElement>();

    public UIDocManager()
    {
        Instance = this;
    }

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        hudOverlay = root.Q<HUD_Overlay>();
        hudOverlay.InitHUD();
    }

    public void AddRaycastBlocker(VisualElement element)
    {
        rayCastBlockers.Add(element);
    }

    public void RemoveRaycastBlocker(VisualElement element)
    {
        if (rayCastBlockers.Contains(element))
        {
            rayCastBlockers.Remove(element);
        }
    }

    public bool IsHoveringOverUI()
    {
        foreach (var blocker in rayCastBlockers)
        {
            if (IsMouseOverBlocker(blocker))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsMouseOverBlocker(VisualElement element)
    {
        var mousePosition = Input.mousePosition;
        var scaledMousePosition = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        scaledMousePosition.y = 1 - scaledMousePosition.y;

        var mousePosPanel = scaledMousePosition * root.panel.visualTree.layout.size;
        var blockingArea = element.layout;

        if (mousePosPanel.x <= blockingArea.xMax &&
            mousePosPanel.x >= blockingArea.xMin &&
            mousePosPanel.y <= blockingArea.yMax &&
            mousePosPanel.y >= blockingArea.yMin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetEntityHUD(EntityController entity)
    {
        hudOverlay.SetEntityInfo(entity);
        hudOverlay.ShowPanel(PanelType.EntityThumbnail);
        hudOverlay.ShowPanel(PanelType.EntityInfo);
    }

    public void ClearEntityHUD()
    {
        hudOverlay.HidePanel(PanelType.EntityThumbnail);
        hudOverlay.HidePanel(PanelType.EntityInfo);
        hudOverlay.SetEntityInfo(null);
    }

    public void SetBuildMenu(List<BuildingController> buildings, Action<BuildingController> buildAction)
    {
        hudOverlay.SetBuildMenu(buildings, buildAction);
        //hudOverlay.ShowPanel(PanelType.BuildMenu);
    }

    public void ClearBuildMenu()
    {
        hudOverlay.ClearBuildMenu();
    }

    public void SetConfirmationModalButtons(Action acceptAction, Action declineAction, string title, string description)
    {
        hudOverlay.SetConfirmationModalInfo(acceptAction, declineAction, title, description);
        hudOverlay.ShowPanel(PanelType.ConfirmationModal);
    }

    public void SetToolbarButtons(List<ToolbarAction> actions)
    {
        hudOverlay.SetToolbarButtons(actions);
    }

    public void ClearToolbarButtons()
    {
        hudOverlay.ClearToolbarButtons();
    }

    public void SetResourceHUD(List<ResourceInfo> resources)
    {
        hudOverlay.SetResourceHUD(resources);
    }
}
