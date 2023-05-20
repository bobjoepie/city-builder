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

    private void Start()
    {

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

    public void ShowEntityHUD(EntityController entity)
    {
        hudOverlay.ShowPanel(PanelType.EntityThumbnail);
        hudOverlay.SetEntityThumbnail(entity.thumbnail);

        hudOverlay.ShowPanel(PanelType.EntityInfo);
        hudOverlay.SetEntityName(entity.displayName);
        hudOverlay.SetEntityDescription(entity.description);
    }

    public void HideEntityHUD()
    {
        hudOverlay.HidePanel(PanelType.EntityThumbnail);
        hudOverlay.SetEntityThumbnail(null);

        hudOverlay.HidePanel(PanelType.EntityInfo);
        hudOverlay.SetEntityName(string.Empty);
        hudOverlay.SetEntityDescription(string.Empty);
    }

    public void ShowBuildHUD(List<BuildingController> buildings, Action<BuildingController> buildAction)
    {
        hudOverlay.SetBuildMenu(buildings, buildAction);
        //hudOverlay.ShowPanel(PanelType.BuildMenu);
    }

    public void SetConfirmationModalButtons(Action acceptAction, Action declineAction)
    {
        hudOverlay.SetConfirmationModalButtons(acceptAction, declineAction);
        hudOverlay.ShowPanel(PanelType.ConfirmationModal);
    }
}
