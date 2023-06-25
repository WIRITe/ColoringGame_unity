using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DrowingScene : MonoBehaviour
{
    public class Layer
    {
        public GameObject _object;
        public int texWidth;
        public int texHeight;
        public Color32[] _pixels;
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

    public void Zoom(Slider _slider)
    {
        Camera.main.orthographicSize = _slider.value;
    }

    public void StartGame(Picture _picture)
    {
        _gameCanvas.SetActive(true);

        foreach (Texture2D layerPath in _picture.Layers)
        {
            GameObject new_layer = Instantiate(OneLayerPref, Vector3.zero, Quaternion.identity);
            new_layer.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            new_layer.name = layerPath.name;

            Sprite _sprite = Sprite.Create(layerPath, new Rect(0, 0, layerPath.width, layerPath.height), Vector2.one * 0.5f);

            SpriteRenderer spriteRenderer = new_layer.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _sprite;
            new_layer.AddComponent<PolygonCollider2D>().isTrigger = true;

            coloringLayers.Add(new Layer
            {
                _object = new_layer,
                texWidth = _sprite.texture.width,
                texHeight = _sprite.texture.height,
                _pixels = _sprite.texture.GetPixels32()
            });

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + new_layer.name, out _color))
            {
                layerColors.Add(new_layer.name, _color);
            }
        }

        List<Color32> _colors = new List<Color32>();

        foreach(Color32 _color in layerColors.Values)
        {
            _colors.Add(_color);
        }

        colorsCanv.GetComponent<ColorsCanvas>().setButtons(_colors);

        foreach (KeyValuePair<string, Color32> layerColor in layerColors)
        {
            colorsCanv.GetComponent<ColorsCanvas>().updateProcentage(layerColor.Value, GetColoredPercentage(layerColor.Key));
        }
    }

    public void Set_nowColor(Color _color)
    {
        NowColor = (Color32)_color;

        foreach (Layer Layer in coloringLayers)
        {
            Layer._object.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

            if (Layer._object.name == ColorUtility.ToHtmlStringRGB(NowColor))
            {
                Layer._object.GetComponent<SpriteRenderer>().color = NowColor;
            }
        }
    }

    public GameObject firstPress;

    private void Update()
    {
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
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (originalTexture != null) colorsCanv.GetComponent<ColorsCanvas>().updateProcentage(NowColor, GetColoredPercentage(firstPress.name));
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
                        Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        Bounds bounds = _sprite.bounds;
                        float u = (localPosition.x - bounds.min.x) / bounds.size.x;
                        float v = (localPosition.y - bounds.min.y) / bounds.size.y;

                        int texWidth = layer.texWidth;
                        int texHeight = layer.texHeight;

                        int centerX = Mathf.FloorToInt(u * texWidth);
                        int centerY = Mathf.FloorToInt(v * texHeight);
                        int brushRadius = Mathf.FloorToInt(brushSize * 2.5f);

                        for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
                        {
                            for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
                            {
                                if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                                {
                                    int index = y * texWidth + x;
                                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                                    if (distance <= brushRadius && layer._pixels[index].a > 0.1f)
                                    {
                                        layer._pixels[index] = NowColor;
                                        layer.isNeedToUpdate = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        StartCoroutine(UpdateTextures());
    }

    private IEnumerator UpdateTextures()
    {
        foreach (Layer layer in coloringLayers)
        {
            if (layer.isNeedToUpdate)
            {
                layer.isNeedToUpdate = false;
                layer._object.GetComponent<SpriteRenderer>().sprite.texture.SetPixels32(layer._pixels);
                layer._object.GetComponent<SpriteRenderer>().sprite.texture.Apply(false);
            }
        }

        yield return null;
    }

    public float GetColoredPercentage(string layerName)
    {
        if (layerColors.TryGetValue(layerName, out Color32 layerColor))
        {
            Layer layer = coloringLayers.Find(l => l._object.name == layerName);
            if (layer != null)
            {
                Texture2D texture = layer._object.GetComponent<SpriteRenderer>().sprite.texture;
                Color32[] pixels = texture.GetPixels32();
                int coloredPixelCount = 0;

                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a > 0.1f && pixels[i].Equals(layerColor))
                    {
                        coloredPixelCount++;
                    }
                }

                float coloredPercentage = (coloredPixelCount / (float)pixels.Length) * 100f;
                return coloredPercentage;
            }
        }

        return 0f;
    }

    public void SavePictures()
    {

    }

    public static void SaveTextureAsPNG(Texture2D texture, string fullPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + fullPath);
    }
}
