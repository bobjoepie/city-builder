using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD_Overlay : VisualElement
{
    public VisualElement entityThumbnail;
    public VisualElement entityInfo;
    public Label entityName;
    public Label entityDescription;

    public new class UxmlFactory : UxmlFactory<HUD_Overlay, UxmlTraits> { }

    public HUD_Overlay()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        InitBlockers();
        InitEntityInfo();

        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
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

    public void ShowPanel(PanelType panelType)
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            switch (panelType)
            {
                case PanelType.Button:
                    //buttonPanel.style.visibility = Visibility.Visible;
                    break;
                case PanelType.Card:
                    //cardPanel.style.visibility = Visibility.Visible;
                    break;
                case PanelType.EntityInfo:
                    entityInfo.style.visibility = Visibility.Visible;
                    break;
                case PanelType.EntityThumbnail:
                    entityThumbnail.style.visibility = Visibility.Visible;
                    break;
            }

        }).Invoke();
    }

    public void HidePanel(PanelType panelType)
    {
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            switch (panelType)
            {
                case PanelType.Button:
                    //buttonPanel.style.visibility = Visibility.Hidden;
                    break;
                case PanelType.Card:
                    //cardPanel.style.visibility = Visibility.Hidden;
                    break;
                case PanelType.EntityInfo:
                    entityInfo.style.visibility = Visibility.Hidden;
                    break;
                case PanelType.EntityThumbnail:
                    entityThumbnail.style.visibility = Visibility.Hidden;
                    break;
            }

        }).Invoke();
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
}

public enum PanelType
{
    Button,
    Card,
    EntityInfo,
    EntityThumbnail
}