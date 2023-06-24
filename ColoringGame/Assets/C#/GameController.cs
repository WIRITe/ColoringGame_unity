using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("main menu canvas")]
    public GameObject MenuCanvas;

    [Header("Menu objects")]
    public GameObject ImageButtonPref;

    public GameObject animals_container;
    public GameObject cars_container;
    public GameObject peoples_container;
    public GameObject things_container;

    [Header("Pictures path")]
    public string folderPath = "Picturies";

    public DrowingScene _table;

    private void Start()
    {
        List<Picture> Pictures = GetPictures();

        foreach (Picture picture in Pictures)
        {
            GameObject instantiatedPref = Instantiate(ImageButtonPref);

            switch (picture.category)
            {
                case Enums.ECategory.Animals:
                    instantiatedPref.transform.SetParent(animals_container.transform);
                    break;
                case Enums.ECategory.Cars:
                    instantiatedPref.transform.SetParent(cars_container.transform);
                    break;
                case Enums.ECategory.Peoples:
                    instantiatedPref.transform.SetParent(peoples_container.transform);
                    break;
                case Enums.ECategory.Things:
                    instantiatedPref.transform.SetParent(things_container.transform);
                    break;
            }

            instantiatedPref.transform.localScale = new Vector3(1, 1, 1);
            instantiatedPref.GetComponent<MenuButtonScript>()._picture = picture;

            Sprite _sprite = Sprite.Create(picture.MainImage, new Rect(0, 0, picture.MainImage.width, picture.MainImage.height), Vector2.one * 0.5f);

            instantiatedPref.GetComponent<Image>().sprite = _sprite;

            instantiatedPref.transform.Find("Name").GetComponent<TMP_Text>().text = picture.Name;
        }
    }

    public void StartColoring(Picture _picture)
    {
        _table.gameObject.SetActive(true);
        _table.StartGame(_picture);
        MenuCanvas.SetActive(false);
    }

    public List<Picture> GetPictures()
    {
        List<Picture> Pictures = new List<Picture>();

        GameObject[] prefabs = Resources.LoadAll<GameObject>(folderPath);

        if (prefabs != null)
        {
            foreach (GameObject prefab in prefabs)
            {
                if (prefab != null)
                {
                    Pictures.Add(prefab.GetComponent<Picture>());
                }
            }
        }

        return Pictures;
    }
}
