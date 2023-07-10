using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Picture;

public class GameController : MonoBehaviour
{
    [Header("main menu canvas")]
    public GameObject MenuCanvas;

    [Header("Menu objects")]
    public GameObject loadingScreen;
    public GameObject ImageButtonPref;
<<<<<<< HEAD
=======
    public GameObject MenuScreen;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)

    [Header("Pictures path")]
    public string[] folderPathes;

    public DrowingScene drowingScene;

<<<<<<< HEAD
    [Header("categorys screen")]
    public GameObject categorys_parrentObject;
    public GameObject category_screen_prefab;
    public GameObject categorys_toggles_parrentObject;
    public GameObject category_toggle_screen_prefab;
    private Dictionary<string, GameObject> category_screens = new Dictionary<string, GameObject>();
    private Dictionary<Picture, GameObject> picturesDictionary = new Dictionary<Picture, GameObject>();

    private void Start()
    {
        List<Picture> Pictures = FileHandler.GetPictures(folderPathes);
        foreach (Picture picture in Pictures)
        {
            GameObject instantiatedPref = Instantiate(ImageButtonPref);
            instantiatedPref.GetComponent<MenuButtonScript>().Init(picture);
            set_buttonCategory(instantiatedPref);

=======
    private void Start()
    {
        List<Picture> Pictures = GetPictures();

        foreach (Picture picture in Pictures)
        {
            GameObject instantiatedPref = Instantiate(ImageButtonPref);

            instantiatedPref.transform.SetParent(MenuScreen.transform);
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
            instantiatedPref.transform.localScale = new Vector3(1, 1, 1);

<<<<<<< HEAD
            picturesDictionary.Add(picture, instantiatedPref);
=======
            Sprite _sprite = Sprite.Create(picture.MainImage, new Rect(0, 0, picture.MainImage.width, picture.MainImage.height), Vector2.one * 0.5f);

            instantiatedPref.GetComponent<Image>().sprite = _sprite;

            instantiatedPref.transform.Find("Name").GetComponent<TMP_Text>().text = picture.Name;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
        }
    }
    public async void StartColoring(Picture _picture)
    {
        loadingScreen.SetActive(true);
        drowingScene.gameObject.SetActive(true);
        await drowingScene.StartGame(_picture);
        MenuCanvas.SetActive(false);
    }

<<<<<<< HEAD
    public void start_coloringFromBeginning()
=======
    public List<Picture> GetPictures()
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    {
        drowingScene.deleteEverything();

        StartColoring(drowingScene._nowPicture);
    }

    

    public void Exit()
    {
        picturesDictionary[drowingScene._nowPicture].GetComponent<MenuButtonScript>().UpdatecoloredStat();
        drowingScene.deletePictureFromScene();

        MenuCanvas.SetActive(true);
    }

    #region category functions
    public void set_buttonCategory(GameObject _buttonOBJ)
    {
        MenuButtonScript _buttonSCR = _buttonOBJ.GetComponent<MenuButtonScript>();

        if (category_screens.ContainsKey(_buttonSCR._picture.category))
        {
            _buttonOBJ.transform.SetParent(category_screens[_buttonSCR._picture.category].transform.Find("Scroll View/Viewport/content").transform);
        }
        else
        {
            GameObject newCategory = CreateNewCategory(_buttonSCR._picture.category, categorys_parrentObject.transform);

            category_screens.Add(_buttonSCR._picture.category, newCategory);

            _buttonOBJ.transform.SetParent(newCategory.transform.Find("Scroll View/Viewport/content").transform);
        }
    }
    public GameObject CreateNewCategory(string categoryName, Transform _parrentObj)
    {
        GameObject new_category_screen = Instantiate(category_screen_prefab, _parrentObj);

        GameObject new_category_toggle_screen = Instantiate(category_toggle_screen_prefab, categorys_toggles_parrentObject.transform);
        new_category_toggle_screen.transform.Find("Background").GetComponent<TMP_Text>().text = categoryName;
        new_category_toggle_screen.GetComponent<Toggle>().group = categorys_toggles_parrentObject.GetComponent<ToggleGroup>();

        new_category_screen.name = categoryName;

        return new_category_screen;
    }
    #endregion
}