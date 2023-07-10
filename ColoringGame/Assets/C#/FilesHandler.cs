using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FilesHandler : MonoBehaviour
{
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        // Create a new Texture2D and load the image data
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }

        return null;
    }

    public static void SaveSprite(Sprite sprite, string path)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }
}
