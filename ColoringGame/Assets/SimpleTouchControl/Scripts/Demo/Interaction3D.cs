// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTouchControl{
    public class Interaction3D : MonoBehaviour {

        public Transform cube;
        public float scaleSpeed = 0.01f;
        public float rotateSpeed;
        [Header("Mobile only")]
        public bool IsMultiSelection = false;

        private float _distanceCubeToCamera = 5;
        private Vector3 _orgScale;
        private List<Transform> _listCubes = new List<Transform>();


    	// Use this for initialization
    	void Start () {
            if (IsMultiSelection)
            {
                InputManager.Instance.onMultiTouchMoved += dragMultipleCubes;
            }
            else
            {
                InputManager.Instance.onTouchMoved += dragCube;
                InputManager.Instance.onPinch += scaleCube;
                InputManager.Instance.onPinchRotate += rotateCube;
            }
            #if UNITY_EDITOR
            scaleSpeed = 1;
            #endif
            _orgScale = cube.localScale;
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
    	}

        void dragMultipleCubes(List<Vector3> listPos){
            if (listPos.Count < _listCubes.Count)
            {
                // remove the last one 
                for(int i = _listCubes.Count-1; i >= listPos.Count; i--){
                    Destroy(_listCubes[i].gameObject);
                    _listCubes.RemoveAt(i);
                }
            }
            int countCube = _listCubes.Count;

            for(int i = 0; i< listPos.Count; i++)
            {
                if (i < countCube)
                {
                    moveCube(_listCubes[i], listPos[i]);
                }
                else
                {
                    GameObject go = GameObject.Instantiate(cube.gameObject);
                    _listCubes.Add(go.transform);
                    moveCube(go.transform, listPos[i]);
                }
            }

        }

        void dragCube(Vector2 deltaPosition){
            // ignore delta position because we dont use it anymore 
            moveCube(cube, InputManager.Instance.GetCurrentTouchPosition());
        }


        void moveCube(Transform cubeObj, Vector3 touchPosition){
            Vector3 worldPosition =  Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, _distanceCubeToCamera));
            cubeObj.position = new Vector3(worldPosition.x, worldPosition.y, cubeObj.position.z);
        }

        void scaleCube(float delta){
            float scale = Mathf.Clamp(cube.localScale.x - delta * scaleSpeed, 0.5f, 2f);
            cube.localScale = _orgScale * scale;
        }

        void rotateCube(float delta){
            cube.RotateAround(Vector3.up, delta * rotateSpeed);
        }
    }
}