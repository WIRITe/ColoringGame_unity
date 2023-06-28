using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileHandler : MonoBehaviour
{
    #region saving texture data
    public static bool savePicture(Picture _picture)
    {
        string path = _picture.Name + ": ";

        foreach(Texture2D _texture in _picture.Layers)
        {
            byte[] textureData = _texture.EncodeToPNG();

            string textureString = Convert.ToBase64String(textureData);

            string texturePath = path + _texture.name;

            PlayerPrefs.SetString(texturePath, textureString);
        }

        return true;
    }
    public static List<Texture2D> get_savedLayers(Picture _picture)
    {
        List<Texture2D> _textures = new List<Texture2D>();

        string path = _picture.Name + ": ";

        foreach (Texture2D _texture in _picture.Layers)
        {
            byte[] defalt_textureData = _texture.EncodeToPNG();
            string defalt_textureString = Convert.ToBase64String(defalt_textureData);

            string texturePath = path + _texture.name;

            string textureString = PlayerPrefs.GetString(texturePath, defalt_textureString);

            byte[] textureData = Convert.FromBase64String(textureString);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(textureData);
            texture.name = _texture.name;

            _textures.Add(texture);
        }

        return _textures;
    }
    #endregion
}
