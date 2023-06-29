using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorsCanvas : MonoBehaviour
{
    public GameObject _buttonPrefab;

    public List<GameObject> ColorButtons;

    public void setButtons(List<Color32> _colors)
    {
        foreach (Color32 _color in _colors)
        {
            GameObject button_prefab = Instantiate(_buttonPrefab);
            button_prefab.transform.SetParent(gameObject.transform);

            button_prefab.transform.localScale = new Vector3(1, 1, 1);

            button_prefab.transform.Find("Button").GetComponent<Image>().color = _color;
            button_prefab.name = ColorUtility.ToHtmlStringRGB(_color);
            button_prefab.GetComponent<ColorButton>()._color = _color;

            ColorButtons.Add(button_prefab);
        }
    }

    public void SetColorActive(Color _color)
    {
        SetAllNotActive();
        foreach (GameObject colorButton in ColorButtons)
        {
            
            if(colorButton.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                StartCoroutine(scaleObj(colorButton.transform.Find("Button").gameObject, 1.2f, 0.5f));

            }
        }
    }

    public void SetAllNotActive()
    {
        foreach(GameObject colorButton in ColorButtons)
        {
            StartCoroutine(scaleObj(colorButton.transform.Find("Button").gameObject, 1, 0.5f));
        }
    }

    public IEnumerator scaleObj(GameObject _object, float To, float Time)
    {
        yield return LeanTween.scale(_object, new Vector3(To, To, To), Time);
    }

    public void updateProcentage(Color _color, float procentage)
    {
        foreach (GameObject colorButton in ColorButtons)
        {
            if (colorButton.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                colorButton.transform.Find("procentageText").GetComponent<TMP_Text>().text = Math.Round(procentage, 2).ToString() + " %";
            }
        }
    }

    public void DestroyAllColors()
    {
        foreach(GameObject _obj in ColorButtons)
        {
            Destroy(_obj);
        }
        ColorButtons = new List<GameObject>();
    }
}
