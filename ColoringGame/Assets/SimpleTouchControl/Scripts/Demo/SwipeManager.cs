// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTouchControl{
    public class SwipeManager : MonoBehaviour {

        public Transform player;
        private float moveSpeed = 0.1f;

    	// Use this for initialization
    	void Start () {
            InputManager.Instance.onSwipeLeft += moveLeft;
            InputManager.Instance.onSwipeRight += moveRight;
            InputManager.Instance.onSwipeDown += moveDown;
            InputManager.Instance.onSwipeUp += moveUp;
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
    	}

        void moveLeft(float delta){
            player.Translate(new Vector3(-moveSpeed*delta, 0, 0));
        }

        void moveRight(float delta){
            player.Translate(new Vector3(moveSpeed*delta, 0, 0));
        }

        void moveUp(float delta){
            player.Translate(new Vector3(0, moveSpeed*delta, 0));
        }

        void moveDown(float delta){
            player.Translate(new Vector3(0, -moveSpeed*delta, 0));
        }
    }
}
