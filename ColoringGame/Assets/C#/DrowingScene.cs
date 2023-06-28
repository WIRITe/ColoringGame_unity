using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DrawingScene : MonoBehaviour
{
    public Picture _nowPicture;

    public class Layer
    {
        public GameObject _object;
        public int texWidth;
        public int texHeight;
        public Color[] _pixels;
        public bool isNeedToUpdate;
    }

    public GameObject OneLayerPref;
    public Color32 NowColor;
    public GameObject _gameCanvas;
    public GameObject colorsCanv;
    public int brushSize;

    public SpriteRenderer spriteRenderer;
    public Sprite _sprite;
    public Texture2D originalTexture;

    public List<Layer> coloringLayers = new List<Layer>();
    public Dictionary<string, Color32> layerColors = new Dictionary<string, Color32>();

    [Header("'a', on color change")]

    public float on_chose_color;

    public void StartGame(Picture _picture)
    {
        _nowPicture = _picture;

        _gameCanvas.SetActive(true);

        List<Texture2D> _picture_textures = FileHandler.get_savedLayers(_picture);
        layerColors = new Dictionary<string, Color32>();

        for (int i = 0; i < _picture.Layers.Count; i++)
        {
            GameObject new_layer = Instantiate(OneLayerPref, Vector3.zero, Quaternion.identity);
            new_layer.transform.localScale = new Vector3(_picture.imagescaleMultyplyer, _picture.imagescaleMultyplyer, _picture.imagescaleMultyplyer);
            new_layer.name = _picture.Layers[i].name;

            Sprite _spriteBasic = Sprite.Create(_picture.Layers[i], new Rect(0, 0, _picture.Layers[i].width, _picture.Layers[i].height), Vector2.one * 0.5f);

            SpriteRenderer spriteRenderer = new_layer.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _spriteBasic;

            new_layer.AddComponent<PolygonCollider2D>().isTrigger = true;


            Sprite _spriteSaved = Sprite.Create(_picture_textures[i], new Rect(0, 0, _picture_textures[i].width, _picture_textures[i].height), Vector2.one * 0.5f);

            spriteRenderer.sprite = _spriteSaved;

            Layer _layer = new Layer
            {
                _object = new_layer,
                texWidth = _spriteSaved.texture.width,
                texHeight = _spriteSaved.texture.height,
                _pixels = _spriteSaved.texture.GetPixels()
            };

            coloringLayers.Add(_layer);

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + new_layer.name, out _color))
            {
                layerColors.Add(new_layer.name, _color);
            }

            for (int j = 0; j < _layer._pixels.Length; j++)
            {
                if (_layer._pixels[j].a > 0.01f)
                {
                    if (_layer._pixels[j].a != 0.6f)
                    {
                        if (_layer._pixels[j].a == 1.0f) _layer._pixels[j].a = 0.6f;
                    }
                }
            }
            _layer.isNeedToUpdate = true;
        }

        List<Color32> _colors = new List<Color32>();

        foreach (Color32 _color in layerColors.Values)
        {
            _colors.Add(_color);
        }

        colorsCanv.GetComponent<ColorsCanvas>().setButtons(_colors);

        foreach (KeyValuePair<string, Color32> layerColor in layerColors)
        {
            colorsCanv.GetComponent<ColorsCanvas>().updateProcentage(layerColor.Value, GetColoredPercentage(ColorUtility.ToHtmlStringRGB(NowColor)));
        }
    }

    public void Set_nowColor(Color _color)
    {
        NowColor = (Color)_color;

        foreach (Layer Layer in coloringLayers)
        {
            for (int i = 0; i < Layer._pixels.Length; i++)
            {
                if (Layer._pixels[i].a > 0.01f)
                {
                    if (Layer._pixels[i].a != 0.1f)
                    {
                        if (Layer._pixels[i].a != 1.0f) Layer._pixels[i].a = 0.1f;
                    }
                }
            }

            if (Layer._object.name == ColorUtility.ToHtmlStringRGB(NowColor))
            {
                for (int i = 0; i < Layer._pixels.Length; i++)
                {
                    if (Layer._pixels[i].a > 0.01f)
                    {
                        if (Layer._pixels[i].a != 0.6f)
                        {
                            if (Layer._pixels[i].a < 1) Layer._pixels[i].a = 0.6f;
                        }
                    }
                }
            }

            Layer.isNeedToUpdate = true;
        }
    }


    public GameObject firstPress;
    private Vector2 lastPoint;

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
            if (originalTexture != null) colorsCanv.GetComponent<ColorsCanvas>().updateProcentage(NowColor, GetColoredPercentage(ColorUtility.ToHtmlStringRGB(NowColor)));
            firstPress = null;
        }
        else if (Input.GetMouseButton(0))
        {
            if (firstPress != null)
            {
                if (firstPress.name == ColorUtility.ToHtmlStringRGB(NowColor))
                {
                    Layer layer = coloringLayers.Find(l => l._object == firstPress);
                    if (layer != null)
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

    //////\\\\\\
    /// Draw \\\
    //////\\\\\\

    private Vector2 CalculateTextureCoordinates(Vector2 position)
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
                {
                    if (pixels[i].a == 1.0f)
                    {
                        coloredPixelCount++;
                    }
                    pixelsCount++;
                }
            }

            float coloredPercentage = (float)((float)coloredPixelCount / (float)pixelsCount) * 100f;
            return coloredPercentage;
        }

        return 0f;
    }



    public void fromBeginning()
    {
        FileHandler.savePicture(_nowPicture);

        foreach (Layer _layer in coloringLayers)
        {
            Destroy(_layer._object);
        }

        coloringLayers = new List<Layer>();

        StartGame(_nowPicture);
    }

    public void Exit()
    {
        _nowPicture.Layers = new List<Texture2D>();

        foreach (Layer _layer in coloringLayers)
        {
            _nowPicture.Layers.Add(_layer._object.GetComponent<SpriteRenderer>().sprite.texture);
        }

        FileHandler.savePicture(_nowPicture);
        _nowPicture = null;


        foreach (Layer _layer in coloringLayers)
        {
            Destroy(_layer._object);
        }

        coloringLayers = new List<Layer>();

        colorsCanv.GetComponent<ColorsCanvas>().DestroyAllColors();
    }

    private void OnDestroy()
    {
        _nowPicture.Layers = new List<Texture2D>();

        foreach (Layer _layer in coloringLayers)
        {
            _nowPicture.Layers.Add(_layer._object.GetComponent<SpriteRenderer>().sprite.texture);
        }

        FileHandler.savePicture(_nowPicture);
    }
}
