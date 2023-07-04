using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("main menu canvas")]
    public GameObject MenuCanvas;

    [Header("Menu objects")]
    public GameObject ImageButtonPref;

    public GameObject animals_category_screen;
    public GameObject cars_category_screen;
    public GameObject peoples_category_screen;
    public GameObject things_category_screen;

    [Header("Pictures path")]
    public string folderPath = "Picturies";

    public DrawingScene _table;

    [Header("categorys screen")]
    public GameObject categorys_parrentObject;
    public GameObject category_screen_prefab;
    public GameObject categorys_toggles_parrentObject;
    public GameObject category_toggle_screen_prefab;
    private Dictionary<string, GameObject> category_screens = new Dictionary<string, GameObject>();

    private void Start()
    {
        List<Picture> Pictures = GetPictures();
        foreach (Picture picture in Pictures)
        {
            GameObject instantiatedPref = Instantiate(ImageButtonPref);

            if (category_screens.ContainsKey(picture.category))
            {
                instantiatedPref.transform.SetParent(category_screens[picture.category].transform.Find("Scroll View/Viewport/content").transform);
            }
            else
            {
                GameObject new_category_screen = Instantiate(category_screen_prefab, categorys_parrentObject.transform);
                category_screens.Add(picture.category, new_category_screen);

                GameObject new_category_toggle_screen = Instantiate(category_toggle_screen_prefab, categorys_toggles_parrentObject.transform);
                new_category_toggle_screen.transform.Find("Background").GetComponent<TMP_Text>().text = picture.category;
                new_category_toggle_screen.GetComponent<Toggle>().group = categorys_toggles_parrentObject.GetComponent<ToggleGroup>();

                new_category_screen.name = picture.category;

                instantiatedPref.transform.SetParent(new_category_screen.transform.Find("Scroll View/Viewport/content").transform);
            }

            if(picture.acess == Enums.EAcess.Premium)
            {
                instantiatedPref.transform.Find("isPrivate").gameObject.SetActive(true);
                instantiatedPref.GetComponent<Button>().interactable = false;
            }

            instantiatedPref.transform.localScale = new Vector3(1, 1, 1);
            instantiatedPref.GetComponent<MenuButtonScript>()._picture = picture;

            if (FileHandler.is_picture_colored(picture))
            {
                Sprite _sprite = Sprite.Create(picture.MainImage, new Rect(0, 0, picture.MainImage.width, picture.MainImage.height), Vector2.one * 0.5f);

                instantiatedPref.GetComponent<Image>().sprite = _sprite;
            }
            else
            {
                int width = picture.MainImage.width;
                int height = picture.MainImage.height;

                Color[] coloredPixels = picture.MainImage.GetPixels();
                Color[] grayscalePixels = new Color[coloredPixels.Length];

                for (int i = 0; i < coloredPixels.Length; i++)
                {
                    Color pixel = coloredPixels[i];
                    float averageIntensity = (pixel.r + pixel.g + pixel.b) / 3f;
                    grayscalePixels[i] = new Color(averageIntensity, averageIntensity, averageIntensity, pixel.a);
                }

                Texture2D grayscaleTexture = new Texture2D(width, height);
                grayscaleTexture.SetPixels(grayscalePixels);
                grayscaleTexture.Apply();

                Sprite _sprite = Sprite.Create(grayscaleTexture, new Rect(0, 0, picture.MainImage.width, picture.MainImage.height), Vector2.one * 0.5f);

                instantiatedPref.GetComponent<Image>().sprite = _sprite;
            }
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
