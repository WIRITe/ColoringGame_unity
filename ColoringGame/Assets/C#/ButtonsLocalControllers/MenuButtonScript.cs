using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Picture;

public class MenuButtonScript : MonoBehaviour
{
    public Picture _picture;
    public GameController _gameController;
    public GameObject _loadingScreen;

    private void Start()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _loadingScreen = GameObject.Find("Loading");
    }

    public void Init(Picture picture)
    {
        _picture = picture;

        if (picture.acess == EAcess.Premium)
        {
            gameObject.transform.Find("isPrivate").gameObject.SetActive(true);
            gameObject.GetComponent<Button>().interactable = false;
        }

        UpdatecoloredStat();
    }

    public void UpdatecoloredStat()
    {
        if (FileHandler.is_picture_colored(_picture))
            gameObject.GetComponent<Image>().sprite = Picture.createSprite(_picture.MainImage);
        else
            gameObject.GetComponent<Image>().sprite = Picture.to_Black_White(_picture.MainImage);
    }

    public void setPremium()
    {
        if (_picture.acess == EAcess.Premium)
        {
            transform.Find("isPrivate").gameObject.SetActive(true);
            GetComponent<Button>().interactable = false;
        }
    }

    public void start_coloring()
    {
        _gameController.StartColoring(_picture);
    }
}
