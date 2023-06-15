using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject ImageButtonPref;
    public GameObject MenuScreen;

    public string folderPath = "PNGs/PicturesPrefabs";

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

            Debug.Log("cfvxz");
        }

        Debug.Log("dfbhdfgn");
    }

    public void StartColoring(Picture _picture)
    {
        print(_picture.Name);
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
