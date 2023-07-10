using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorToRandom : MonoBehaviour
{
    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        Color _color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 255);
        sprite.color = _color;
    }
}
