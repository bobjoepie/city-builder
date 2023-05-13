using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD_Overlay : VisualElement
{
    public new class UxmlFactory : UxmlFactory<HUD_Overlay, UxmlTraits> { }

    public HUD_Overlay()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        var blockers = this.Query(className: "blocker").ToList();
        foreach (var blocker in blockers)
        {
            UIDocManager.Instance.AddRaycastBlocker(blocker);
        }
        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void Show()
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            this.style.visibility = Visibility.Visible;

        }).Invoke();
    }

    public void Hide()
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            this.style.visibility = Visibility.Hidden;

        }).Invoke();
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

    public void ShowPanel(PanelType panel)
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            switch (panel)
            {
                case PanelType.Button:
                    //buttonPanel.style.visibility = Visibility.Visible;
                    break;
                case PanelType.Card:
                    //cardPanel.style.visibility = Visibility.Visible;
                    break;
            }

        }).Invoke();
    }

    public void HidePanel(PanelType panel)
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            switch (panel)
            {
                case PanelType.Button:
                    //buttonPanel.style.visibility = Visibility.Hidden;
                    break;
                case PanelType.Card:
                    //cardPanel.style.visibility = Visibility.Hidden;
                    break;
            }

        }).Invoke();
    }
}

public enum PanelType
{
    Button,
    Card
}