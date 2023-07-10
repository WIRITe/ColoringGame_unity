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
            // Modify the alpha channel of defaultTextureData
            Color[] defaultPixels = _texture.GetPixels();
            for (int i = 0; i < defaultPixels.Length; i++)
            {
                if(defaultPixels[i].a > 0.01f) defaultPixels[i].a = 0.6f;
            }

            Texture2D texture = new Texture2D(_texture.width, _texture.height); // Create a temporary texture
            texture.SetPixels(defaultPixels);
            texture.Apply();

            byte[] defaultTextureData = texture.EncodeToPNG();

            string defaultTextureString = Convert.ToBase64String(defaultTextureData);

            string texturePath = path + _texture.name;

            string textureString = PlayerPrefs.GetString(texturePath, defaultTextureString);

            byte[] textureData = Convert.FromBase64String(textureString);

            Texture2D texture2 = new Texture2D(2, 2);
            texture2.LoadImage(textureData);
            texture2.name = _texture.name;

            _textures.Add(texture2);
        }

        return _textures;
    }

    public static bool deleteAllSavings(Picture _picture)
    {
        string path = _picture.Name + ": ";

        foreach (Texture2D _texture in _picture.Layers)
        {
            string texturePath = path + _texture.name;

            PlayerPrefs.DeleteKey(texturePath);
        }

        return true;
    }

    #endregion
}
