using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorsCanvas : MonoBehaviour
{
    public class _color
    {
        public GameObject _button;
        public float procent = 0.0f;
    }

    public GameObject _buttonPrefab;

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    public List<GameObject> ColorButtons = new List<GameObject>();
=======
    public List<GameObject> ColorButtons;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
=======
    public List<_color> ColorButtons = new List<_color>();
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
=======
    public List<_color> ColorButtons = new List<_color>();
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)

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

            _color color = new _color();
            color._button = button_prefab;

            ColorButtons.Add(color);
        }
    }

    public void SetColorActive(Color _color)
    {
        SetAllNotActive();
        foreach (_color colorButton in ColorButtons)
        {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            if(colorButton.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                StartCoroutine(scaleObj(colorButton.transform.Find("Button").gameObject, 1.2f, 0.5f));
=======
            
            if(colorButton.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                StartCoroutine(scaleObj(colorButton.transform.Find("Button").gameObject, 1.2f, 0.5f));

>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
=======
            
            if(colorButton._button.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                StartCoroutine(scaleObj(colorButton._button.transform.Find("Button").gameObject, 1.2f, 0.5f));
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
=======
            
            if(colorButton._button.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                StartCoroutine(scaleObj(colorButton._button.transform.Find("Button").gameObject, 1.2f, 0.5f));
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
            }
        }
    }

    public void SetAllNotActive()
    {
        foreach(_color colorButton in ColorButtons)
        {
            StartCoroutine(scaleObj(colorButton._button.transform.Find("Button").gameObject, 1, 0.5f));
        }
    }

    public IEnumerator scaleObj(GameObject _object, float To, float Time)
    {
        yield return LeanTween.scale(_object, new Vector3(To, To, To), Time);
    }

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
    public void updateProcentage(Color _color, float procentage)
    {
        foreach (GameObject colorButton in ColorButtons)
        {
            if (colorButton.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                colorButton.transform.Find("procentageText").GetComponent<TMP_Text>().text = Math.Round(procentage, 2).ToString() + " %";
=======
=======
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
    public void updateProcentage(Color _color, float procentage)
    {
        foreach (_color colorButton in ColorButtons)
        {
            if (colorButton._button.gameObject.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                colorButton.procent = procentage;
<<<<<<< HEAD
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
=======
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
            }
        }
    }

<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
=======
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
=======
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
    public void DestroyAllColors()
    {
        foreach(GameObject _obj in ColorButtons)
        {
            Destroy(_obj);
        }
<<<<<<< HEAD

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
        ColorButtons = new List<GameObject>();
=======
        ColorButtons = new List<_color>();
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
=======
        ColorButtons = new List<_color>();
>>>>>>> parent of acbf349 (ready project, without subscribtion and alot API. But working/building correctly)
    }
}
