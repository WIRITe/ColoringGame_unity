using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DrowingScene : MonoBehaviour
{
    public Picture _nowPicture;

<<<<<<< HEAD
=======
    public class Layer
    {
        public GameObject _object;
        public int texWidth;
        public int texHeight;
        public Color[] _originalPixels;
        public Color[] _pixels;
        public bool isNeedToUpdate;
    }

>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    public GameObject OneLayerPref;
    public Color32 NowColor;
    public GameObject _gameCanvas;
    public ColorsCanvas colorsCanv;
    public int brushSize;

    public SpriteRenderer spriteRenderer;
    public Sprite _sprite;
    public Texture2D originalTexture;

    public List<Layer> coloringLayers = new List<Layer>();
    public Dictionary<string, Color32> layerColors = new Dictionary<string, Color32>();

<<<<<<< HEAD
    public delegate void picture_is_colored();
    public static event picture_is_colored picture_is_colored_event;

    private Vector2 lastPoint;

    public async Task StartGame(Picture _picture)
=======
    [Header("'a', on color change")]

    public float on_chose_color;

    public void StartGame(Picture _picture)
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    {
        _nowPicture = _picture;

        _gameCanvas.SetActive(true);

        List<Texture2D> _picture_textures = FileHandler.get_savedLayers(_nowPicture);

        layerColors = new Dictionary<string, Color32>();

        for(int i = 0; i < _nowPicture.Layers.Count; i++)
        {
<<<<<<< HEAD
            layerColors.Add(_nowPicture.Layers[i].name, ColorUtility.TryParseHtmlString("#" + _nowPicture.Layers[i].name, out Color result) ? result : Color.white);
=======
            Sprite _originalSprite = Sprite.Create(_picture.Layers[i], new Rect(0, 0, _picture.Layers[i].width, _picture.Layers[i].height), Vector2.one * 0.5f);
            _originalSprite.name = "original sprite";
            Sprite _savedSprite = Sprite.Create(_picture_textures[i], new Rect(0, 0, _picture_textures[i].width, _picture_textures[i].height), Vector2.one * 0.5f);
            _savedSprite.name = "saved sprite: " + _picture_textures[i].name;

            GameObject new_layer = Instantiate(OneLayerPref, Vector3.zero, Quaternion.identity);
            new_layer.transform.localScale = new Vector3(_picture.imagescaleMultyplyer, _picture.imagescaleMultyplyer, _picture.imagescaleMultyplyer);
            new_layer.name = _picture.Layers[i].name;

            Layer _layer = new Layer
            {
                _object = new_layer,
                texWidth = _originalSprite.texture.width,
                texHeight = _originalSprite.texture.height,
                _pixels = _savedSprite.texture.GetPixels(),
                _originalPixels = _originalSprite.texture.GetPixels(),
                isNeedToUpdate = true
            };

            SpriteRenderer spriteRenderer = new_layer.GetComponent<SpriteRenderer>();

            // add polygon collider component \\
            spriteRenderer.sprite = _originalSprite;
            Color[] _pixels = spriteRenderer.sprite.texture.GetPixels();
            for(int o = 0; o < _pixels.Length; o++)
            {
                if(_pixels[o].a > 0.1f) _pixels[o].a = 10.0f;
            }
            spriteRenderer.sprite.texture.SetPixels(_pixels);
            new_layer.AddComponent<PolygonCollider2D>();

            spriteRenderer.sprite = _savedSprite;

            coloringLayers.Add(_layer);

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + new_layer.name, out _color))
            {
                layerColors.Add(new_layer.name, _color);
            }

            _layer.isNeedToUpdate = true;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
        }

        await Task.Run(() =>
        {
            for (int i = 0; i < _nowPicture.Layers.Count; i++)
            {
                Texture2D original_layerTexture = _nowPicture.Layers[i];
                Texture2D saved_layerTexture = _picture_textures[i];

<<<<<<< HEAD
                MainThreadDispatcher.RunOnMainThread(() => CreateLayer(original_layerTexture, saved_layerTexture));
            }

            List<Color32> _colors = new List<Color32>();
            foreach (Color32 _color in layerColors.Values)
            {
                _colors.Add(_color);
            }
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                colorsCanv.setButtons(_colors);
            });

            MainThreadDispatcher.RunOnMainThread(() => { GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().loadingScreen.SetActive(false); });
        });
=======
        colorsCanv.setButtons(_colors);
        foreach (KeyValuePair<string, Color32> layerColor in layerColors)
        {
            colorsCanv.updateProcentage(layerColor.Value, GetColoredPercentage(ColorUtility.ToHtmlStringRGB(NowColor)));
        }
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    }
    private void CreateLayer(Texture2D original_layerTexture, Texture2D saved_layerTexture)
    {
        Sprite _originalSprite = Picture.createSprite(original_layerTexture);
        _originalSprite.name = "original sprite";
        Sprite _savedSprite = Picture.createSprite(saved_layerTexture);
        _savedSprite.name = "saved sprite: " + saved_layerTexture.name;

        GameObject new_layer = Instantiate(OneLayerPref, Vector3.zero, Quaternion.identity);
        new_layer.transform.localScale = new Vector3(1, 1, 1);
        new_layer.name = original_layerTexture.name;

        Layer _layer = new Layer
        {
            _object = new_layer,
            _spriteRenderer = new_layer.GetComponent<SpriteRenderer>(),
            texWidth = _originalSprite.texture.width,
            texHeight = _originalSprite.texture.height,
            _pixels = _savedSprite.texture.GetPixels(),
            _originalPixels = _originalSprite.texture.GetPixels(),
            isNeedToUpdate = true
        };

        SpriteRenderer spriteRenderer = new_layer.GetComponent<SpriteRenderer>();

        // add polygon collider component \\
        spriteRenderer.sprite = Picture.createSprite(_originalSprite.texture);

        new_layer.AddComponent<PolygonCollider2D>();

        spriteRenderer.sprite = _savedSprite;

        coloringLayers.Add(_layer);

        _layer.isNeedToUpdate = true;
    }


<<<<<<< HEAD
    #region coloring functions
=======
    public GameObject firstPress;
    private Vector2 lastPoint;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)

    private void Update()
    {
        if (Camera_controller.isCameraMoving)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                firstPress = hit.collider.gameObject;
                spriteRenderer = firstPress.GetComponent<SpriteRenderer>();
                _sprite = spriteRenderer.sprite;
                originalTexture = _sprite.texture;
                lastPoint = CalculateTextureCoordinates(mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
<<<<<<< HEAD
            if (Picture.is_picture_colored(coloringLayers))
            {
                picture_is_colored_event?.Invoke();
                FileHandler.set_picture_coloring_state(_nowPicture);
            }
=======
            if (originalTexture != null) colorsCanv.updateProcentage(NowColor, GetColoredPercentage(ColorUtility.ToHtmlStringRGB(NowColor)));
            firstPress = null;
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
        }
        else if (Input.GetMouseButton(0))
        {
            if (firstPress != null)
            {
<<<<<<< HEAD
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 currentPoint = CalculateTextureCoordinates(mousePosition, layer);

                float distance = Vector2.Distance(currentPoint, lastPoint);
                if (distance > brushSize/2)
                {
                    int steps = Mathf.CeilToInt(distance / (brushSize / 2));
                    for (int i = 0; i < steps; i++)
=======
                if (firstPress.name == ColorUtility.ToHtmlStringRGB(NowColor))
                {
                    Layer layer = coloringLayers.Find(l => l._object == firstPress);
                    if (layer != null)
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
                    {
                        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2 currentPoint = CalculateTextureCoordinates(mousePosition);

                        float distance = Vector2.Distance(currentPoint, lastPoint);
                        if (distance > brushSize)
                        {
                            int steps = Mathf.CeilToInt(distance / brushSize);
                            for (int i = 0; i < steps; i++)
                            {
                                float t = i / (float)steps;
                                Vector2 point = Vector2.Lerp(lastPoint, currentPoint, t);
                                DrawPointOnTexture(layer, point);
                            }
                        }
                        else
                        {
                            DrawPointOnTexture(layer, currentPoint);
                        }

                        lastPoint = currentPoint;
                    }
                }
            }
        }

        StartCoroutine(UpdateTextures());
    }

<<<<<<< HEAD
    private Vector2 CalculateTextureCoordinates(Vector2 position, Layer _layer)
=======
    //////\\\\\\
    /// Draw \\\
    //////\\\\\\

    private Vector2 CalculateTextureCoordinates(Vector2 position)
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    {
        Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(position);
        Bounds bounds = _sprite.bounds;
        float u = (localPosition.x - bounds.min.x) / bounds.size.x;
        float v = (localPosition.y - bounds.min.y) / bounds.size.y;

        int texWidth = originalTexture.width;
        int texHeight = originalTexture.height;

        int x = Mathf.FloorToInt(u * texWidth);
        int y = Mathf.FloorToInt(v * texHeight);

        return new Vector2(x, y);
    }

    private void DrawPointOnTexture(Layer layer, Vector2 point)
    {
        int texWidth = layer.texWidth;
        int texHeight = layer.texHeight;
        int brushRadius = Mathf.FloorToInt(brushSize * 2.5f);

        int centerX = Mathf.FloorToInt(point.x);
        int centerY = Mathf.FloorToInt(point.y);

        for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
        {
            for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
            {
                if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                {
                    int index = y * texWidth + x;
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                    if (distance <= brushRadius && layer._pixels[index].a == 0.6f)
                    {
                        if (layer._pixels[index].a != 1) layer._pixels[index].a = 1;
                        layer.isNeedToUpdate = true;
                    }
                }
            }
        }
<<<<<<< HEAD

        lastPoint = point;
=======
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    }

    private IEnumerator UpdateTextures()
    {
        foreach (Layer layer in coloringLayers)
        {
            if (layer.isNeedToUpdate)
            {
                layer.isNeedToUpdate = false;
                layer._object.GetComponent<SpriteRenderer>().sprite.texture.SetPixels(layer._pixels);
                layer._object.GetComponent<SpriteRenderer>().sprite.texture.Apply(false);
            }
        }

        yield return null;
    }

<<<<<<< HEAD
    public void Set_nowColor(Color _color)
    {
        NowColor = (Color)_color;

        foreach (Layer Layer in coloringLayers)
        {
            for (int i = 0; i < Layer._pixels.Length; i++)
            {
                if (Layer._pixels[i].a > 0.1f)
=======
    public float GetColoredPercentage(string layerName)
    {
        Layer layer = coloringLayers.Find(l => l._object.name == layerName);
        if (layer != null)
        {
            Color[] pixels = layer._pixels;
            int coloredPixelCount = 0;
            int pixelsCount = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0.1f)
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
                {
                    if (Layer._pixels[i].a != 1.0f) Layer._pixels[i].a = 0.1f;
                }
            }

<<<<<<< HEAD
            if (Layer._object.name == ColorUtility.ToHtmlStringRGB(NowColor))
            {
                for (int i = 0; i < Layer._pixels.Length; i++)
                {
                    if (Layer._pixels[i].a > 0.01f)
                    {
                        if (Layer._pixels[i].a != 1.0f) Layer._pixels[i].a = 0.6f;
                    }
                }
            }

            Layer.isNeedToUpdate = true;
        }
    }

    #endregion


    public void deleteEverything()
=======
            float coloredPercentage = (float)((float)coloredPixelCount / (float)pixelsCount) * 100f;
            return coloredPercentage;
        }

        return 0f;
    }



    public void fromBeginning()
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    {
        FileHandler.deleteAllSavings(_nowPicture);

        foreach (Layer _layer in coloringLayers)
        {
            Destroy(_layer._object);
        }

        coloringLayers = new List<Layer>();

<<<<<<< HEAD
        colorsCanv.DestroyAllColors();
=======
        StartGame(_nowPicture);
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    }

    public void deletePictureFromScene()
    {
        Picture.savePicture(_nowPicture, coloringLayers);

        foreach (Layer _layer in coloringLayers)
        {
<<<<<<< HEAD
=======
            pictureToSave.Layers.Add(_layer._object.GetComponent<SpriteRenderer>().sprite.texture);
        }

        FileHandler.savePicture(pictureToSave);
        _nowPicture = null;


        foreach (Layer _layer in coloringLayers)
        {
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
            Destroy(_layer._object);
        }

        coloringLayers = new List<Layer>();

<<<<<<< HEAD
        _gameCanvas.SetActive(false);

        layerColors = new Dictionary<string, Color32>();
        coloringLayers = new List<Layer>();
=======
        colorsCanv.GetComponent<ColorsCanvas>().DestroyAllColors();
>>>>>>> parent of 35036dd (finish build, without rustore (subscription) and finish screen)
    }

    public class Layer
    {
        public GameObject _object;
        public SpriteRenderer _spriteRenderer;
        public int texWidth;
        public int texHeight;
        public Color[] _originalPixels;
        public Color[] _pixels;
        public bool isNeedToUpdate;

        public float colored_procentage;
    }
}
