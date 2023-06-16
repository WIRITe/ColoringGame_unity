using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorsCanvas : MonoBehaviour
{
    public GameObject _buttonPrefab;

    public void setButtons(List<Color> _colors)
    {
        foreach (Color _color in _colors)
        {
            GameObject button_prefab = Instantiate(_buttonPrefab);
            button_prefab.transform.SetParent(gameObject.transform);

            button_prefab.transform.localScale = new Vector3(1, 1, 1);

            button_prefab.GetComponent<Image>().color = _color;
            button_prefab.GetComponent<ColorButton>()._color = _color;
        }
    }
}
