using System.Collections.Generic;
using UnityEngine;

public class DrowingScene : MonoBehaviour
{
    public GameObject OneLayerPref;
    public Color NowColor;
    public Color colorToColoring;
    public GameObject _gameCanvas;

    public float brushSize;

    public SpriteRenderer spriteRenderer;
    public Sprite _sprite;
    public Texture2D originalTexture;

    public List<GameObject> coloringLayers = new List<GameObject>();

    List<Color> _colors = new List<Color>();

    public void StartGame(Picture _picture)
    {
        _gameCanvas.SetActive(true);
        
        foreach (Sprite layer in _picture.Layers)
        {
            GameObject new_layer = Instantiate(OneLayerPref, new Vector3(0, 0, 0), Quaternion.identity);
            new_layer.name = layer.name;
            new_layer.GetComponent<SpriteRenderer>().sprite = layer;
            new_layer.AddComponent<PolygonCollider2D>();
            new_layer.GetComponent<PolygonCollider2D>().autoTiling = true;
            new_layer.GetComponent<PolygonCollider2D>().isTrigger = true;

            coloringLayers.Add(new_layer);

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + layer.name, out _color))
            {
                _colors.Add(_color);
            }
        }

        _gameCanvas.GetComponent<ColorsCanvas>().setButtons(_colors);

        foreach (Color _color in _colors)
        {
            _gameCanvas.GetComponent<ColorsCanvas>().updateProcentage(_color, GetColoredPercentage(_color));
        }
    }

    public void Set_nowColor(Color _color)
    {
        NowColor = _color;
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

                spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                _sprite = spriteRenderer.sprite;
                originalTexture = _sprite.texture;
            }

            return;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(originalTexture!=null) _gameCanvas.GetComponent<ColorsCanvas>().updateProcentage(NowColor, GetColoredPercentage(NowColor));
            firstPress = null;
            return;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, new Vector3(0, 0, 1));

            if (hit.collider != null)
            {
                if (firstPress == null) return;

                Debug.Log(firstPress.gameObject.name);
                if (firstPress.gameObject.name == ColorUtility.ToHtmlStringRGB(NowColor))
                {
                    Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(hit.point);
                    Bounds bounds = _sprite.bounds;

                    float u = (localPosition.x - bounds.min.x) / bounds.size.x;
                    float v = (localPosition.y - bounds.min.y) / bounds.size.y;

                    int texWidth = originalTexture.width;
                    int texHeight = originalTexture.height;

                    int centerX = Mathf.FloorToInt(u * texWidth);
                    int centerY = Mathf.FloorToInt(v * texHeight);

                    Color[] pixels = originalTexture.GetPixels();
                    int brushRadius = Mathf.FloorToInt(brushSize * 2.5f);

                    for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
                    {
                        for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
                        {
                            if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                            {
                                int index = y * texWidth + x;

                                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                                if (distance <= brushRadius)
                                {
                                    if (pixels[index].a > 0.1f)
                                    {
                                        pixels[index] = NowColor;
                                    }
                                }
                            }
                        }
                    }

                    originalTexture.SetPixels(pixels);
                    originalTexture.Apply();

                    _sprite = Sprite.Create(originalTexture, _sprite.rect, new Vector2(0.5f, 0.5f));
                    spriteRenderer.sprite = _sprite;
                }
            }
        }
    }

    public float GetColoredPercentage(Color _color)
    {
        Texture2D _texture = null;

        foreach(GameObject layer in coloringLayers)
        {
            if(layer.name == ColorUtility.ToHtmlStringRGB(_color))
            {
                _texture = layer.GetComponent<SpriteRenderer>().sprite.texture;
            }
        }

        Color[] pixels = _texture.GetPixels();
        int usualPixels = 0;
        int coloredPixelCount = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0.1f)
            {
                usualPixels++;
                if(pixels[i] == NowColor)
                {
                    coloredPixelCount++;
                }
            }
        }

        Debug.Log("coloredPixelCount: " + coloredPixelCount.ToString());
        Debug.Log("usualPixels: " + usualPixels.ToString());

        float coloredPercentage = (float)((float)((float)coloredPixelCount / (float)usualPixels) * (float)100f);

        Debug.Log("coloredPercentage: " + coloredPercentage);

        return coloredPercentage;
    }

    public void SavePictures()
    {

    }
    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}
