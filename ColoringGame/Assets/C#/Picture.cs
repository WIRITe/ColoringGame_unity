using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Picture : MonoBehaviour
{
<<<<<<< HEAD
    public string category;
    public EAcess acess;
=======
    public Enums.ECategory category;
    public Enums.EAcess acess;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)

    public string Name;

    public List<Texture2D> Layers;
    public Texture2D MainImage;

    public float imagescaleMultyplyer;

    public static Sprite to_Black_White(Texture2D _texture)
    {
        int width = _texture.width;
        int height = _texture.height;

        Color[] coloredPixels = _texture.GetPixels();
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

        return createSprite(grayscaleTexture);
    }

    public static Sprite createSprite(Texture2D _texture)
    {
        Sprite _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f);

        return _sprite;
    }

    public static bool is_picture_colored(List<DrowingScene.Layer> _picture)
    {
        int coloredPixelCount = 0;
        int pixelsCount = 0;

        foreach (DrowingScene.Layer layer in _picture)
        {
            Color[] pixels = layer._pixels;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0.01f)
                {
                    if (pixels[i].a == 1.0f)
                    {
                        coloredPixelCount++;
                    }
                    pixelsCount++;
                }
            }
        }

        float coloredPercentage = (float)((float)coloredPixelCount / (float)pixelsCount);

        if (coloredPercentage >= 0.7) return true;
        return false;
    }

    public static Sprite imageToValueForCollider(Texture2D _sprite, float a_value)
    {
        Color[] _pixels = _sprite.GetPixels();

        for (int o = 0; o < _pixels.Length; o++)
        {
            if (_pixels[o].a > 0.1f) _pixels[o].a = a_value;
        }

        _sprite.SetPixels(_pixels);

        return createSprite(_sprite);
    }

    public static void savePicture(Picture nowPicture, List<DrowingScene.Layer> layers)
    {
        Picture pictureToSave = new Picture();

        string _name = nowPicture.Name;
        List<Texture2D> _layers = new List<Texture2D>();

        foreach (DrowingScene.Layer _layer in layers)
        {
            _layers.Add(_layer._spriteRenderer.sprite.texture);
        }

        FileHandler.savePicture(_name, _layers);
    }

    public enum EAcess
    {
        Free,
        Premium
    }
}