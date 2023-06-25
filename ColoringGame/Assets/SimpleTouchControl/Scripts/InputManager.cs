// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleTouchControl{
    public class InputManager : SingletonBehavior<InputManager> {
        public bool IsVerboseLogging;

        private const float thresholdMovement = 100f;
        private const float thresholdSwipe = 300f;

        public Action onTouchBegan;
        public Action onTouchEnded;
        public Action<Vector2> onTouchMoved;
        public Action<List<Vector3>> onMultiTouchMoved;

        public Action onSingleTouch; 
        public Action onLongTouch;
        public Action onDoubleTouch; 
        public Action<int> onNTouch; 

        public Action<float> onSwipeLeft; 
        public Action<float> onSwipeRight; 
        public Action<float> onSwipeUp; 
        public Action<float> onSwipeDown;

        public Action<float> onPinch;
        public Action<float> onPinchRotate;

        Vector3 _currentTouchPosition;
        Vector3 _previousTouchPosition;
        Vector3 _startTouchPosition;
        List<Vector3> _currentMultiTouchPositions = new List<Vector3>();
        float _timeBeginTouch;
        float _deltaMagnitudeDiff;
        float _deltaAngleDiff;
        int _countConsecutiveTouch;
        float _timeIntervalTouch;

        public Vector3 GetCurrentTouchPosition(){
            return _currentTouchPosition;
        }

        public Vector3 GetDeltaTouchPosition(){
            return _currentTouchPosition - _previousTouchPosition;
        }

    	// Use this for initialization
    	void Start () {
    	}
    	
    	// Update is called once per frame
    	void Update () {
            if (_timeIntervalTouch > 0)
            {
                _timeIntervalTouch -= Time.deltaTime;
                if (_timeIntervalTouch < 0)
                    _timeIntervalTouch = -1;
            }

            //=========================
            if (checkTouchBegan())
            {
//                if(IsVerboseLogging)
//                    Debug.Log("Touch begin");
                _previousTouchPosition = _currentTouchPosition;                    
                _startTouchPosition = _currentTouchPosition = getInputPosition();

                _timeBeginTouch = Time.time;

                if (onTouchBegan != null)
                    onTouchBegan();
            }
            if (checkTouchMoved())
            {
//                if(IsVerboseLogging)
//                    Debug.Log("Touch moved");
                
                Vector3 savedPos = _previousTouchPosition = _currentTouchPosition; 
                _currentTouchPosition = getInputPosition();

                if (onTouchMoved != null)
                {
                    #if UNITY_EDITOR
                    Vector3 delta = _currentTouchPosition - savedPos;
                    onTouchMoved(new Vector2(delta.x, delta.y));
                    #else
                    onTouchMoved(Input.GetTouch(0).deltaPosition);
                    #endif
                }
            }

            if (checkTouchMovedMulti())
            {
                if (onMultiTouchMoved != null)
                    onMultiTouchMoved(_currentMultiTouchPositions);
            }
            if (checkTouchEnded())
            {
//                if(IsVerboseLogging)
//                    Debug.Log("Touch end");
                
                _currentTouchPosition = getInputPosition();

                if (onTouchEnded != null)
                    onTouchEnded();
            }
            if (checkPinch())
            {
                if(IsVerboseLogging)
                    Debug.Log("Pinch " + _deltaMagnitudeDiff);
                
                if (onPinch != null)
                    onPinch(_deltaMagnitudeDiff);
            }
            if (checkPinchRotate())
            {
                if(IsVerboseLogging)
                    Debug.Log("Pinch rotate " + _deltaAngleDiff);

                if (onPinchRotate != null)
                    onPinchRotate(_deltaAngleDiff);
            }
            if (checkSingleTouch())
            {
                bool isSingleTouch = true;
                _currentTouchPosition = getInputPosition();
                // check multiple consucutive touch 
                float deltaTime = Time.time - _timeIntervalTouch;
                if (deltaTime < 0.75f)
                {
                    // count logic 
                    _countConsecutiveTouch++;
                    isSingleTouch = false;

                    // verbose 
                    if(IsVerboseLogging)
                        Debug.Log(_countConsecutiveTouch + " consecutive touch || " + deltaTime);

                    // fire action 
                    if (_countConsecutiveTouch == 2)
                    {
                        if (onDoubleTouch != null)
                            onDoubleTouch();
                    }
                    if (onNTouch != null)
                        onNTouch(_countConsecutiveTouch);
                    
                }
                else
                {
                    _countConsecutiveTouch = 1;
                }

                _timeIntervalTouch = Time.time;
                if (isSingleTouch)
                {
                    if (IsVerboseLogging)
                        Debug.Log("Single touch");
                
                    if (onSingleTouch != null)
                        onSingleTouch();
                }
            }
            if (checkLongTouch())
            {
                if(IsVerboseLogging)
                    Debug.Log("Long touch");
                
                _currentTouchPosition = getInputPosition();
                if (onLongTouch != null)
                    onLongTouch();
            }
           

            {
                switch (checkSwipe())
                {
                    case 0:
                        // nothing 
                        break;
                    case 1:
                        float delta = Vector3.Distance(_startTouchPosition, _currentTouchPosition);
                        if(IsVerboseLogging)
                            Debug.Log("Swipe up speed " + delta);
                        
                        _currentTouchPosition = getInputPosition();
                        if (onSwipeUp != null)
                            onSwipeUp(delta);
                        break;
                    case 2:
                        delta = Vector3.Distance(_startTouchPosition, _currentTouchPosition);
                        if(IsVerboseLogging)
                            Debug.Log("Swipe down speed " + delta);
                        
                        _currentTouchPosition = getInputPosition();
                        if (onSwipeDown != null)
                            onSwipeDown(delta);
                        break;
                    case 3:
                        delta = Vector3.Distance(_startTouchPosition, _currentTouchPosition);
                        if(IsVerboseLogging)
                            Debug.Log("Swipe left speed " + delta);
                        
                        _currentTouchPosition = getInputPosition();
                        if (onSwipeLeft != null)
                            onSwipeLeft(delta);
                        break;
                    case 4:
                        delta = Vector3.Distance(_startTouchPosition, _currentTouchPosition);
                        if(IsVerboseLogging)
                            Debug.Log("Swipe right speed "+ delta);
                        
                        _currentTouchPosition = getInputPosition();
                        if (onSwipeRight != null)
                            onSwipeRight(delta);
                        break;
                }
            }
        }

        Vector3 getInputPosition(){
            #if UNITY_EDITOR
            return  Input.mousePosition;
            #else
            return Input.GetTouch(0).position;
            #endif
        }


        #region Condition 
        bool checkTouchBegan(){
            #if UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
            #else
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
            #endif
        }

        bool checkTouchEnded(){
            #if UNITY_EDITOR
            return Input.GetMouseButtonUp(0);
            #else
            return (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended);
            #endif
        }

        bool checkTouchMoved(){
            #if UNITY_EDITOR
            return Input.GetMouseButton(0)
                && Vector3.Distance(getInputPosition(), _startTouchPosition) > thresholdMovement;
            #else
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved;
            #endif
        }
 
        bool checkTouchMovedMulti(){
            #if UNITY_EDITOR
            return false;
            #else
            Touch[] myTouches = Input.touches;
            _currentMultiTouchPositions.Clear();
            for(int i = 0; i < Input.touchCount; i++)
            {
                if(myTouches[i].phase == TouchPhase.Moved){
                    Vector3 currentPos = myTouches[i].position;
                    _currentMultiTouchPositions.Add(currentPos);
                }
            }
            return _currentMultiTouchPositions.Count > 0;
            #endif
        }

        bool checkSingleTouch(){
            #if UNITY_EDITOR

            return Input.GetMouseButtonUp(0)
                && (Vector3.Distance(getInputPosition() , _startTouchPosition) < thresholdMovement
                        &&  (Time.time - _timeBeginTouch) < 0.1f);
            #else
            if(IsVerboseLogging){
                if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended){
            Debug.Log("Mouse: " + Vector3.Distance(getInputPosition() , _startTouchPosition));
                    Debug.Log("Time: " + (Time.time - _timeBeginTouch));
                }
            }
            return (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            && (Vector3.Distance(getInputPosition() , _startTouchPosition) < thresholdMovement
            && (Time.time - _timeBeginTouch) < 0.1f);
            #endif
        }

        bool checkLongTouch(){
            #if UNITY_EDITOR
            return Input.GetMouseButtonUp(0)
                && Vector3.Distance(getInputPosition() , _startTouchPosition) < thresholdMovement
                &&  (Time.time - _timeBeginTouch) > 0.5f;
            #else
            return (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            && Vector3.Distance(getInputPosition() , _startTouchPosition) < thresholdMovement
            && (Time.time - _timeBeginTouch) > 0.5f;
            #endif
        }

      
        bool checkPinch(){
            #if UNITY_EDITOR
            _deltaMagnitudeDiff = Input.GetAxis("Mouse ScrollWheel");
            return Input.GetAxis("Mouse ScrollWheel") != 0;
            #else
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                _deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                return true;
            }
            return false;
            #endif

        }

        bool checkPinchRotate(){
            #if UNITY_EDITOR
            _deltaAngleDiff = Input.GetAxis("Mouse X");
            return Input.GetAxis("Mouse X") != 0 && Input.GetKey(KeyCode.LeftControl);
            #else
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find angle 
                float anglePrev = Vector2.Angle(touchZeroPrevPos, touchOnePrevPos);
                float angle = Vector2.Angle(touchZero.position, touchOne.position);
                _deltaAngleDiff = angle - anglePrev;
                    
                Debug.Log("_deltaAngleDiff "+_deltaAngleDiff);
        
                return Mathf.Abs(_deltaAngleDiff) > 0.5f;
                    
             }
            return false;
            #endif

        }
            
        /*
         * return 0: no swipe
         * return 1: swipe up 
         * return 2: swipe down
         * return 3: swipe left 
         * return 4: swipe right 
        */
        int checkSwipe(){
            #if UNITY_EDITOR
            // keypad 
            if (Input.GetKey (KeyCode.UpArrow)) {
                return 1;
            } else if (Input.GetKey (KeyCode.DownArrow)) {
                return 2;
            } else if (Input.GetKey (KeyCode.LeftArrow)) {
                return 3;
            } else if (Input.GetKey (KeyCode.RightArrow)) {
               return 4;
            }

            // mouse 
            if(Input.GetMouseButtonUp(0)){
               
                float vert = Input.mousePosition.y - _startTouchPosition.y;
                float honz = Input.mousePosition.x - _startTouchPosition.x;
//                Debug.Log("Mouse " + vert + " " + honz);
                if(Mathf.Abs(vert) > thresholdSwipe || Mathf.Abs(honz) > thresholdSwipe) 
                {
                    if(Mathf.Abs(vert) > Mathf.Abs(honz))
                    {
                        if(vert > 0) // swipe up 
                        {
                            return 1;
                        }
                        else // swipe down
                        {
                            return 2;
                        }
                    }
                    else 
                    {
                        if(honz > 0) // swipe right
                        {
                            return 4;
                        }
                        else // swipe left
                        {
                            return 3;
                        }
                    }
                }
            }

            return 0;
            #else
            Touch[] myTouches = Input.touches;
            for(int i = 0; i < Input.touchCount; i++)
            {
            }
            if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended){
                float vert = Input.GetTouch(0).position.y - _startTouchPosition.y;
                float honz = Input.GetTouch(0).position.x - _startTouchPosition.x;
                if(Mathf.Abs(vert) > thresholdSwipe || Mathf.Abs(honz) > thresholdSwipe) 
                {
                    if(Mathf.Abs(vert) > Mathf.Abs(honz))
                    {
                        if(vert > 0) // swipe up 
                        {
                            return 1;
                        }
                        else // swipe down
                        {
                            return 2;
                        }
                    }
                    else 
                    {
                        if(honz > 0) // swipe right
                        {
                            return 4;
                        }
                        else // swipe left
                        {
                            return 3;
                        }
                    }
                }
            }
             
            return 0;
            #endif 
        }
        #endregion
    }
}