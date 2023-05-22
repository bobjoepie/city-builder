using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildPreviewer : MonoBehaviour, IInputController
{
    public static BuildPreviewer Instance;
    private InputManager inputManager;

    private Camera mainCamera;
    public bool isAttachedToMouse;
    public Material previewMaterial;

    private Rigidbody rb;
    private Transform displayObject;

    private BuildPreviewer()
    {
        Instance = this;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
            rb.MovePosition(gridPos);
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
        displayObject = Instantiate(buildingPreview, transform);
        displayObject.GetComponent<MeshRenderer>().material = previewMaterial;
        displayObject.GetComponent<Collider>().isTrigger = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<BuildingController>(out var building))
        {
            displayObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<BuildingController>(out var _))
        {
            return;
        }

        if (CanBuild())
        {
            displayObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    public bool CanBuild()
    {
        var extents = displayObject.GetComponent<Collider>().bounds.extents;
        var overlaps = Physics.OverlapBox(transform.position, extents);
        foreach (var overlap in overlaps)
        {
            if (overlap.transform != transform.GetChild(0) && overlap.gameObject.TryGetComponent<BuildingController>(out var building))
            {
                return false;
            }
        }
        return true;
    }
}
