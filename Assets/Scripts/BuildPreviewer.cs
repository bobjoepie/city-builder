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
        CheckMousePosition();
        CheckInputs();
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
        CheckMousePosition();
        var bounds = buildingPreview.GetComponent<MeshRenderer>().bounds.size;
        var offsetPos = new Vector3(Mathf.FloorToInt(bounds.x) % 2 != 0 ? 0.5f : 0, 0, Mathf.FloorToInt(bounds.z) % 2 != 0 ? 0.5f : 0);
        displayObject = Instantiate(buildingPreview, transform.position + offsetPos, transform.rotation, transform);
        displayObject.GetComponent<MeshRenderer>().material = previewMaterial;
        displayObject.GetComponent<MeshRenderer>().material.color = HasRoomToBuild() ? Color.green : Color.red;
        displayObject.GetComponent<Collider>().isTrigger = true;
    }

    public void StopPreview()
    {
        GameObject[] allChildren = new GameObject[transform.childCount];

        var i = 0;
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }
        
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<BuildAreaCollisionController>(out var building))
        {
            displayObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<BuildAreaCollisionController>(out var _))
        {
            return;
        }

        if (HasRoomToBuild())
        {
            displayObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    public bool HasRoomToBuild()
    {
        if (displayObject == null || !displayObject.GetChild(0).TryGetComponent<Collider>(out var collider)) return false;
        var extents = displayObject.GetChild(0).GetComponent<Collider>().bounds.extents;
        var bounds = displayObject.GetComponent<MeshRenderer>().bounds.size;
        var offsetPos = new Vector3(Mathf.FloorToInt(bounds.x) % 2 != 0 ? 0.5f : 0, 0, Mathf.FloorToInt(bounds.z) % 2 != 0 ? 0.5f : 0);
        var overlaps = Physics.OverlapBox(transform.position + offsetPos, extents);
        foreach (var overlap in overlaps)
        {
            if (!overlap.transform.IsChildOf(transform) &&
                !overlap.transform.IsChildOf(transform.GetChild(0)) &&
                overlap.gameObject.TryGetComponent<BuildAreaCollisionController>(out var building))
            {
                return false;
            }
        }
        return true;
    }
}
