// /*****************************************************************
//  * Copyright (C) 2017 Ngan Do - dttngan91@gmail
//  *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleTouchControl{
    public class GenericTouchManager : MonoBehaviour {

        public Animator singleTouchAnim;
        public Animator longTouchAnim;
        public Animator nTouchAnim;
        public Text counNTouch;

        const string _touchConst = "isTouch";
        const string _otherTouchConst = "isOtherTouch";

    	// Use this for initialization
    	void Start () {
           

            // register events 
            InputManager.Instance.onSingleTouch += onSingleTouch;
            InputManager.Instance.onLongTouch += onLongTouch;
            InputManager.Instance.onNTouch += onNTouch;
    	}
    	
    	// Update is called once per frame
    	void Update () {
            
    	}

        void onSingleTouch(){
            singleTouchAnim.SetTrigger(_touchConst);
            longTouchAnim.SetTrigger(_otherTouchConst);
            nTouchAnim.SetTrigger(_otherTouchConst);
        }

        void onLongTouch(){
            singleTouchAnim.SetTrigger(_otherTouchConst);
            longTouchAnim.SetTrigger(_touchConst);
            nTouchAnim.SetTrigger(_otherTouchConst);
        }

        void onNTouch(int num){
            singleTouchAnim.SetTrigger(_otherTouchConst);
            longTouchAnim.SetTrigger(_otherTouchConst);
            nTouchAnim.SetTrigger(_touchConst);

            counNTouch.text = num + " consecutive touch";
        }
    }
}