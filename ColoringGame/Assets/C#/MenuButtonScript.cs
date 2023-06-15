using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public Picture _picture;

    public GameController _gameController;

    private void Start()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void LoadPicture()
    {
        _gameController.StartColoring(_picture);
    }
}
