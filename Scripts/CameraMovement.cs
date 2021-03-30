using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraMovement : MonoBehaviour
{

    private Camera cam;
    private float targetZoom;
    public float panSpedd = 10f;
    public float panBorderThickness = 10f;
    public float scrollSpeeed = 10f;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    public Tilemap map;
    
    private float halfWidth;

    private void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        halfWidth = targetZoom * cam.aspect;
        
        bottomLeftLimit = map.localBounds.min + new Vector3(halfWidth,targetZoom);
        topRightLimit = map.localBounds.max + new Vector3(-halfWidth, -targetZoom);
    }


    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w") || Input.mousePosition.y >=Screen.height - panBorderThickness)
        {
            pos.y += panSpedd * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <=panBorderThickness)
        {
            pos.y -= panSpedd * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >=Screen.width - panBorderThickness)
        {
            pos.x += panSpedd * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <=panBorderThickness)
        {
            pos.x -= panSpedd * Time.deltaTime;
        }
        
        
        pos.x = Mathf.Clamp(pos.x, bottomLeftLimit.x, topRightLimit.x);
        pos.y = Mathf.Clamp(pos.y, bottomLeftLimit.y, topRightLimit.y);
        
        transform.position = pos;
        
        cameraZoom();

    }

    private void cameraZoom()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * 10f;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * scrollSpeeed);
        targetZoom = Mathf.Clamp(targetZoom, 5f, 14.9f); 
    }
}
