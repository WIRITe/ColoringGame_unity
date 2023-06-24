using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public string Name;

    public Enums.ECategory category;
    public Enums.EAcess acess;

    public Texture2D MainImage;
    public List<Texture2D> Layers = new List<Texture2D>();
}