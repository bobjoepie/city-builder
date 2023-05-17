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

    public VisualElement buildModal;

    public new class UxmlFactory : UxmlFactory<HUD_Overlay, UxmlTraits> { }

    public HUD_Overlay()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        InitBlockers();
        InitEntityInfo();
        InitBuildMenu();

        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void InitBuildMenu()
    {
        buildModal = this.Q("build-modal");

        var buildMenuButton = this.Q<Button>("build-button");
        buildMenuButton.clickable.clicked += () => { TogglePanel(PanelType.BuildMenu); };
        //var buildButtons = buildMenu.Children().Where(c => c is Button).Cast<Button>().ToList();
        //for (int i = 0; i < buildButtons.Count; i++)
        //{
        //    int ii = i;
        //    buildButtons[ii].clickable.clicked += () => { Debug.Log(ii); };
        //}
        buildModal.style.visibility = Visibility.Hidden;
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
                case PanelType.BuildMenu:
                    buildModal.style.visibility = Visibility.Visible;
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
                case PanelType.BuildMenu:
                    buildModal.style.visibility = Visibility.Hidden;
                    break;
            }

        }).Invoke();
    }

    public void TogglePanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.Button:
                //buttonPanel.style.visibility = Visibility.Hidden;
                break;
            case PanelType.Card:
                //cardPanel.style.visibility = Visibility.Hidden;
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

    public void SetBuildMenu(List<string> buildings)
    {
        ClearBuildMenu();
        var buildButtons = buildModal.Q("build-menu").Children().Where(c => c is Button).Cast<Button>().ToList();
        for (int i = 0; i < buildings.Count; i++)
        {
            int ii = i;
            buildButtons[ii].clickable = new Clickable(() => { });
            buildButtons[ii].clickable.clicked += () => { Debug.Log($"Built {buildings[ii]}"); };
            buildButtons[ii].text = buildings[ii];
        }
    }

    public void ClearBuildMenu()
    {
        var buildButtons = buildModal.Q("build-menu").Children().Where(c => c is Button).Cast<Button>().ToList();
        foreach (var button in buildButtons)
        {
            button.clickable = null;
            button.text = string.Empty;
        }
    }
}

public enum PanelType
{
    Button,
    Card,
    EntityInfo,
    EntityThumbnail,
    BuildMenu
}