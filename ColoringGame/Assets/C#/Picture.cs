using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public Enums.ECategory category;
    public Enums.EAcess acess;

    public string Name;

    public List<string> StandartLayers = new List<string>();
    public List<string> Layers = new List<string>();
    public string MainImage;
}