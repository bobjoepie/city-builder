using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPreviewer : MonoBehaviour, IInputController
{
    public static BuildPreviewer Instance;
    private InputManager inputManager;

    private Camera mainCamera;
    public bool isAttachedToMouse;

    private BuildPreviewer()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        inputManager = InputManager.Instance;
        inputManager.Register(this, KeyAction.Rotate);
    }

    private void Update()
    {
        if (isAttachedToMouse)
        {
            CheckMousePosition();
            CheckInputs();
        }
    }

    private void CheckMousePosition()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity,
                layerMask: LayerUtility.Only(new String[] { "Grid" })))
        {
            var gridPos = new Vector3(Mathf.FloorToInt(hit.point.x + 0.5f), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z + 0.5f));
            transform.position = gridPos;

        }
    }
    private void CheckInputs()
    {
        if (inputManager.PollKeyDown(this, KeyAction.Rotate))
        {
            if (transform.rotation.eulerAngles.y == 0)
            {
                transform.Rotate(new Vector3(0, -90, 0));
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public void StartPreview(Transform buildingPreview)
    {
        isAttachedToMouse = true;
        Instantiate(buildingPreview, transform);
    }

    public void StopPreview()
    {
        isAttachedToMouse = false;
        GameObject[] allChildren = new GameObject[transform.childCount];

        var i = 0;
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }
        
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
