using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    public Color _color;

    public void SetColor()
    {
        GameObject.FindGameObjectWithTag("Table").GetComponent<DrowingScene>().Set_nowColor(_color);
    }
}
