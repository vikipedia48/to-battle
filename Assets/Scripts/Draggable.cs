using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public Camera mainCamera;

    private bool isFollowingCursor;
    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (!isFollowingCursor) return;
        var newPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        gameObject.transform.position = newPos;
    }

    public void FollowCursor()
    {
        isFollowingCursor = true;
        
    }

    public void StopFollowingCursor()
    {
        isFollowingCursor = false;
    }

    
}
