using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public Camera cameraObject;
    
    public float zoomSensitivity = 1f;
    public float panSensitivity = 0.25f;
    public float closestZoom;
    public float farthestZoom = 30;
    public float minX = 0;
    public float maxX = 100;
    public float upperY = 0;
    public float lowerY = 100;
    public float minClickableX;
    public float panSensitivityFactor = 0.0075f;
    public Vector3 lastCursorPosition;

    private void Start()
    {
        cameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();
        closestZoom = 2;
        farthestZoom = 30;
        minX = 0;
        upperY = 0;
        panSensitivity = cameraObject.orthographicSize / 100;
        zoomSensitivity = 1;
        minClickableX = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(2) || Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x >= minClickableX) cameraObject.transform.position -= (Input.mousePosition - lastCursorPosition) * panSensitivity;
        }
        var pos = cameraObject.transform.position;
        pos.x = Math.Min(maxX, Math.Max(minX, pos.x));
        pos.y = Math.Min(upperY,Math.Max(lowerY, pos.y));
        cameraObject.transform.position = pos;
        
        var scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue < 0) cameraObject.orthographicSize = Math.Min(farthestZoom, cameraObject.orthographicSize + zoomSensitivity);
        
        
        else if (scrollValue > 0) cameraObject.orthographicSize = Math.Max(closestZoom, cameraObject.orthographicSize - zoomSensitivity);
        panSensitivity = cameraObject.orthographicSize * panSensitivityFactor;
        lastCursorPosition = Input.mousePosition;
    }
}
