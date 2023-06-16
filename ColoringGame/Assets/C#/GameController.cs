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
    public GameObject MenuScreen;

    [Header("Pictures path")]
    public string folderPath = "Picturies";

    public DrowingScene _table;

    private void Start()
    {
        List<Picture> Pictures = GetPictures();

        foreach (Picture picture in Pictures)
        {
            GameObject instantiatedPref = Instantiate(ImageButtonPref);

            instantiatedPref.transform.SetParent(MenuScreen.transform);
            instantiatedPref.transform.localScale = new Vector3(1, 1, 1);
            instantiatedPref.GetComponent<MenuButtonScript>()._picture = picture;
            instantiatedPref.GetComponent<Image>().sprite = picture.MainImage;

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
