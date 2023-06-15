using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public Enums.ECategory category;
    public Enums.EAcess acess;

    public string Name;

    public List<Sprite> StandartLayers;
    public List<Sprite> Layers;
    public Sprite MainImage;
}