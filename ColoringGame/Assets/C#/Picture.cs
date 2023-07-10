using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
<<<<<<< HEAD
    public string category;
<<<<<<< HEAD
    public EAcess acess;
=======
    public Enums.ECategory category;
    public Enums.EAcess acess;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
=======
    public Enums.EAcess acess;
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)

    public string Name;

    public List<Texture2D> Layers = new List<Texture2D>();
    public Texture2D MainImage;

    public float imagescaleMultyplyer;
}