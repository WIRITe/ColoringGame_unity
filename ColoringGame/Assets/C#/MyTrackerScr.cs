using Mycom.Tracker.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTrackerScr : MonoBehaviour
{
    public string android_SDK_key;
    public string IOs_SDK_key;

    public void Awake()
    {
#if !UNITY_IOS && !UNITY_ANDROID
        return;
#endif

        // При необходимости настройте конфигурацию трекера
        var myTrackerConfig = MyTracker.MyTrackerConfig;
        var myTrackerParams = MyTracker.MyTrackerParams;

        // Инициализируйте трекер в зависимости от платформы
#if UNITY_IOS
        MyTracker.Init(IOs_SDK_key);
#elif UNITY_ANDROID
        MyTracker.Init(android_SDK_key);
#endif
    }
}