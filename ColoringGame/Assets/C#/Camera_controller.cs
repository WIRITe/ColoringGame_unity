using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleTouchControl;

/// <summary> A modular and easily customisable Unity MonoBehaviour for handling swipe and pinch motions on mobile. </summary>
public class Camera_controller : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.1f;
    public float orthoZoomSpeed = 0.03f;
    public float dragSpeed = 0.01f;

    private const float constmaxX = 3.0f;
    private const float constmaxY = 1.7f;
    private const float minCamSize = 1.0f;
    private const float maxCamSize = 2.7f;

    private float maxX = 3.0f;
    private float maxY = 1.7f;

    private float currentCamSize;

    public bool isDragging_started = false;

    public Slider _slider;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        InputManager.Instance.onTouchMoved += moveCamera;
        InputManager.Instance.onSingleTouch += start_dragging;
        InputManager.Instance.onTouchEnded += end_draggind;
    }

    public void start_dragging()
    {
        isDragging_started = true;
    }

    public void end_draggind()
    {
        isDragging_started = false;
    }

    public void zoom_camera_handler()
    {
        mainCamera.orthographicSize = _slider.value;
    }

    void moveCamera(Vector2 touchDeltaPosition)
    {
        if (isDragging_started)
        {
            if (DrowingScene.coloring)
            {
                mainCamera.transform.Translate(-touchDeltaPosition.x * dragSpeed, -touchDeltaPosition.y * dragSpeed, 0);

                calculateMovedRange();
            }
        }
    }

    void calculateMovedRange()
    {
        maxX = func(currentCamSize, maxCamSize, minCamSize, 0, constmaxX);
        maxY = func(currentCamSize, maxCamSize, minCamSize, 0, constmaxY);

        float x = Mathf.Clamp(mainCamera.transform.position.x, -maxX, maxX);
        float y = Mathf.Clamp(mainCamera.transform.position.y, -maxY, maxY);
        mainCamera.transform.position = new Vector3(x, y, -10);
    }

    float func(float currentA, float minA, float maxA, float minB, float maxB)
    {
        return Mathf.Clamp((currentA - minA) / (maxA - minA) * (maxB - minB) + minB, minB, maxB);
    }
}