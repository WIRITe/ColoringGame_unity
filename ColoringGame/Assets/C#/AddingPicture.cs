using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AddingPicture : MonoBehaviour
{
    [MenuItem("GameObject/Picture")]
    public static void spawnNewPicture()
    {
        GameObject _obj = new GameObject("new picture");
        _obj.AddComponent<Picture>();
    }
}
