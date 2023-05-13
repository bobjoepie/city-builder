using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour, IInputController
{
    public InputManager input;
    public float cameraSpeed = 10.0f;
    public float rotateSpeed = 120.0f;
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.Instance;
        input.Register(this, DefaultActionMaps.CameraActions);

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        var horizontal = 0f;
        var vertical = 0f;

        if (input.PollKey(this, KeyAction.Left))
        {
            horizontal -= 1f;
        }
        if (input.PollKey(this, KeyAction.Right))
        {
            horizontal += 1f;
        }
        if (input.PollKey(this, KeyAction.Up))
        {
            vertical += 1f;
        }
        if (input.PollKey(this, KeyAction.Down))
        {
            vertical -= 1f;
        }

        if (horizontal == 0f && vertical == 0f) return;

        var movement = new Vector3(horizontal, 0, vertical).normalized;
        var camRot = transform.rotation.eulerAngles.y;
        var rot = Quaternion.Euler(0, camRot, 0);
        var isoMatrix = Matrix4x4.Rotate(rot);
        var res = isoMatrix.MultiplyPoint3x4(movement);

        if (input.PollKey(this, KeyAction.SpeedUpCamera))
        {
            res *= 3f;
        }

        transform.position += new Vector3(res.x * cameraSpeed * Time.deltaTime, 0, res.z * cameraSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        var horizontal = 0f;
        if (input.PollKey(this, KeyAction.RotateLeft))
        {
            horizontal += 1f;
        }
        if (input.PollKey(this, KeyAction.RotateRight))
        {
            horizontal -= 1f;
        }

        if (input.PollKeyDown(this, KeyAction.ResetZoom))
        {
            transform.rotation = Quaternion.identity;
        }

        if (horizontal == 0f) return;

        transform.Rotate(0, horizontal * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleZoom()
    {
        var vertical = 0f;
        var mouseWheel = Input.mouseScrollDelta.y;
        if (input.PollKeyDown(this, KeyAction.ZoomIn) || mouseWheel > 0)
        {
            vertical += 1f;
        }
        if (input.PollKeyDown(this, KeyAction.ZoomOut) || mouseWheel < 0)
        {
            vertical -= 1f;
        }

        if (input.PollKeyDown(this, KeyAction.ResetZoom))
        {
            mainCamera.transform.localPosition = new Vector3(0, 0, 0);
        }

        if (vertical == 0f) return;

        mainCamera.transform.localPosition += new Vector3(0, 0, vertical);
    }
}
