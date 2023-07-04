using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public string category;
    public Enums.EAcess acess;

    public string Name;

    public List<Texture2D> Layers = new List<Texture2D>();
    public Texture2D MainImage;

    public float imagescaleMultyplyer;
}