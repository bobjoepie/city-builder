using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDocManager : MonoBehaviour
{
    public static UIDocManager Instance { get; private set; }
    public UIDocument document;
    private static VisualElement root;
    public HUD_Overlay hudOverlay;
    private List<VisualElement> rayCastBlockers = new List<VisualElement>();

    public UIDocManager()
    {
        Instance = this;
    }

    private void Awake()
    {
        root = document.rootVisualElement;
        hudOverlay = root.Q<HUD_Overlay>();
    }

    private void Start()
    {
        hudOverlay.HidePanel(PanelType.Button);
    }

    public void AddRaycastBlocker(VisualElement element)
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            rayCastBlockers.Add(element);

        }).Invoke();
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
}
