using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawingScene : MonoBehaviour
{
    public Picture _nowPicture;

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

    public delegate void picture_is_colored();
    public static event picture_is_colored picture_is_colored_event;

    [Header("'a', on color change")]

    public float on_chose_color;

    public Vector2 lastPoint;

    public void StartGame(Picture _picture)
    {
        _nowPicture = _picture;

        _gameCanvas.SetActive(true);

        List<Texture2D> _picture_textures = FileHandler.get_savedLayers(_picture);
        layerColors = new Dictionary<string, Color32>();

        for (int i = 0; i < _picture.Layers.Count; i++)
        {
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
                _spriteRenderer = new_layer.GetComponent<SpriteRenderer>(),
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
        }

        List<Color32> _colors = new List<Color32>();
        foreach (Color32 _color in layerColors.Values)
        {
            _colors.Add(_color);
        }

        colorsCanv.setButtons(_colors);

        is_picture_colored(coloringLayers);
    }

    public void Set_nowColor(Color _color)
    {
        NowColor = (Color)_color;

        foreach (Layer Layer in coloringLayers)
        {
            for (int i = 0; i < Layer._pixels.Length; i++)
            {
                if (Layer._pixels[i].a > 0.1f)
                {
                    if(Layer._pixels[i].a != 1.0f) Layer._pixels[i].a = 0.1f;
                }
            }

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

    private void Update()
    {
        if (Camera_controller.isCameraMoving)
        {
            return;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (is_picture_colored(coloringLayers))
            {
                picture_is_colored_event?.Invoke();
                FileHandler.set_picture_coloring_state(_nowPicture);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Layer layer = coloringLayers.Find(l => l._object.name == ColorUtility.ToHtmlStringRGB(NowColor));

            if (layer != null)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 currentPoint = CalculateTextureCoordinates(mousePosition, layer);

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
            }
        }

        StartCoroutine(UpdateTextures());
    }

    //////\\\\\\
    /// Draw \\\
    //////\\\\\\

    private Vector2 CalculateTextureCoordinates(Vector2 position, Layer _layer)
    {
        Vector2 localPosition = _layer._spriteRenderer.transform.InverseTransformPoint(position);
        Bounds bounds = _layer._spriteRenderer.sprite.bounds;
        float u = (localPosition.x - bounds.min.x) / bounds.size.x;
        float v = (localPosition.y - bounds.min.y) / bounds.size.y;

        int texWidth = _layer.texWidth;
        int texHeight = _layer.texHeight;

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

        if(point.x >= 0 && point.y >= 0) lastPoint = point;
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

    public bool is_picture_colored(List<Layer> _picture)
    {
        int coloredPixelCount = 0;
        int pixelsCount = 0;

        foreach(Layer layer in _picture)
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

        Debug.Log("coloredPercentage: " + coloredPercentage.ToString());

        if (coloredPercentage >= 0.7) return true;
        return false;
    }

    public void fromBeginning()
    {
        FileHandler.deleteAllSavings(_nowPicture);
        FileHandler.set_picture_coloring_state(_nowPicture, false);

        foreach (Layer _layer in coloringLayers)
        {
            Destroy(_layer._object);
        }

        coloringLayers = new List<Layer>();

        colorsCanv.DestroyAllColors();

        StartGame(_nowPicture);
    }

    public void Exit()
    {
        Picture pictureToSave = new Picture();

        pictureToSave.Name = _nowPicture.Name;

        foreach (Layer _layer in coloringLayers)
        {
            pictureToSave.Layers.Add(_layer._object.GetComponent<SpriteRenderer>().sprite.texture);
        }

        FileHandler.savePicture(pictureToSave);
        _nowPicture = null;

        foreach(Layer _layer in coloringLayers)
        {
            Destroy(_layer._object);
        }

        colorsCanv.DestroyAllColors();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnApplicationQuit()
    {
        Exit();
    }
}
