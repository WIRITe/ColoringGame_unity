using Mycom.Tracker.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTrackerScript : MonoBehaviour
{
    public string SDK_KEY_IOS = "SDK_KEY_IOS";
    public string SDK_KEY_ANDROID = "SDK_KEY_ANDROID";

    public void Awake()
    {
        #if !UNITY_IOS && !UNITY_ANDROID
        Debug.Log("dffdsdfcsdzvxvcd b");
            return;
        #endif
            var myTrackerConfig = MyTracker.MyTrackerConfig;

        #if UNITY_IOS
            MyTracker.Init(SDK_KEY_IOS);
        #elif UNITY_ANDROID
            MyTracker.Init(SDK_KEY_ANDROID);
        #endif
    }
}
