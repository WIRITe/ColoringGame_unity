// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTouchControl{
    public class ThrowManager : MonoBehaviour {

        public GameObject prefabBall;
        public GameObject prefabTrail;
        public float ForceThrow;
        private GameObject _currentBall;
        private GameObject _currentTrail;
        private Vector3 _startPos;

    	// Use this for initialization
    	void Start () {
            InputManager.Instance.onSwipeUp += (float force)=>{
                newBall();
                throwBall(force);
            };
            InputManager.Instance.onTouchMoved += drawParticleEffect; 

            _currentTrail = (GameObject)Instantiate(prefabTrail);
            _startPos = new Vector3(0, 0, -5);
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
    	}

        void newBall(){
            _currentBall = (GameObject)Instantiate(prefabBall);
            _currentBall.transform.position = _startPos;
            _currentBall.GetComponent<Rigidbody>().isKinematic = true;
        }

        void throwBall(float force){
            Vector3 endTouchPosition = InputManager.Instance.GetCurrentTouchPosition();
            Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(endTouchPosition.x, endTouchPosition.y, 10)); // target to a plance distance 10meter away 
            Vector3 dir = target - Camera.main.transform.position;
            Rigidbody body = _currentBall.GetComponent<Rigidbody>();
            body.isKinematic = false;
//            Debug.Log("Dir " + dir + " || Force " + force);
            body.AddForce(dir * ForceThrow * force);
        }

        void drawParticleEffect(Vector2 deltaMove){
            Vector3 endTouchPosition = InputManager.Instance.GetCurrentTouchPosition();
            Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(endTouchPosition.x, endTouchPosition.y, 10)); // target to a plance distance 10meter away 
            _currentTrail.transform.position = target;

        }
    }
}
