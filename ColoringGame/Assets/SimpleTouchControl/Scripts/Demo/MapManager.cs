// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTouchControl {
    public class MapManager : MonoBehaviour {

        public Camera camera;
        [Tooltip("The rate of change of the field of view in perspective mode")]
        public float perspectiveZoomSpeed = 0.1f;        // The rate of change of the field of view in perspective mode.
        [Tooltip("The rate of change of the field of view in orthographic mode")]
        public float orthoZoomSpeed = 0.03f;        // The rate of change of the orthographic size in orthographic mode.
        [Tooltip("The drag map speed")]
        public float dragSpeed = 0.01f;

        private const float constmaxX = 3.0f;
        private const float constmaxY = 1.7f;
        private const float minCamSize = 1.0f;
        private const float maxCamSize = 2.7f;

        private float maxX = 3.0f;
        private float maxY = 1.7f;

        private float currentCamSize;

    	// Use this for initialization
    	void Start () {
            #if UNITY_EDITOR
            dragSpeed = 0.001f;
            orthoZoomSpeed = 0.1f;
            #endif

            if (camera == null)
                camera = Camera.main;
            currentCamSize = camera.orthographicSize;
            InputManager.Instance.onTouchMoved += moveCamera;
            InputManager.Instance.onPinch += pinchAndZoomCamera;
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
    	}

        void moveCamera(Vector2 touchDeltaPosition){
            // Move object across XY plane
            camera.transform.Translate(-touchDeltaPosition.x * dragSpeed,
                -touchDeltaPosition.y * dragSpeed, 0);

            // clipping 
            calculateMovedRange();
        }

        void pinchAndZoomCamera(float deltaMagnitudeDiff){
            if (camera.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minCamSize, maxCamSize);

                currentCamSize = camera.orthographicSize;
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);

                currentCamSize = camera.fieldOfView;
            }

            // calculate movment range based on zoom mode 
            calculateMovedRange();
        }

        void calculateMovedRange(){
            maxX = func(currentCamSize, maxCamSize, minCamSize, 0, constmaxX);
            maxY = func(currentCamSize, maxCamSize, minCamSize, 0, constmaxY);
            //Debug.Log("Cam :  " + maxX + " " + maxY);
            // clipping 
            float x = Mathf.Clamp(camera.transform.position.x, -maxX, maxX);
            float y = Mathf.Clamp(camera.transform.position.y, -maxY, maxY);
            camera.transform.position = new Vector3(x, y, 0);
        }

        float func(float currentA, float minA, float maxA, float minB, float maxB){
            return Mathf.Clamp((currentA - minA) / (maxA - minA) * (maxB - minB) + minB, minB, maxB);
        }
    }
}
