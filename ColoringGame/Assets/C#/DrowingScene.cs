using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DrowingScene : MonoBehaviour
{
    public GameObject OneLayerPref;
    public Color NowColor;
    public Color colorToColoring;
    public GameObject coloring_canv;
    public GameObject _gameCanvas;

    public int brushSize;

    public SpriteRenderer spriteRenderer;
    public Sprite _sprite;
    public Texture2D originalTexture;

    public Dictionary<GameObject, Color[]> coloringLayers = new Dictionary<GameObject, Color[]>();

    List<Color> _colors = new List<Color>();

    public TMP_Text finished_text;

    public static bool coloring;

    public bool needUpdateTexture = false;

    public void StartGame(Picture _picture)
    {
        _gameCanvas.SetActive(true);
        coloring_canv.SetActive(true);

        foreach (Texture2D layerPath in _picture.Layers)
        {
            GameObject new_layer = Instantiate(OneLayerPref, new Vector3(0, 0, 0), Quaternion.identity);
            new_layer.name = layerPath.name;

            Sprite _sprite = Sprite.Create(layerPath, new Rect(0, 0, layerPath.width, layerPath.height), Vector2.one * 0.5f);

            new_layer.GetComponent<SpriteRenderer>().sprite = _sprite;
            new_layer.AddComponent<PolygonCollider2D>();
            new_layer.GetComponent<PolygonCollider2D>().autoTiling = true;
            new_layer.GetComponent<PolygonCollider2D>().isTrigger = true;

            coloringLayers.Add(new_layer, new_layer.GetComponent<SpriteRenderer>().sprite.texture.GetPixels());

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + new_layer.name, out _color))
            {
                _colors.Add(_color);
            }
        }

        coloring_canv.GetComponent<ColorsCanvas>().setButtons(_colors);

        foreach (Color _color in _colors)
        {
            coloring_canv.GetComponent<ColorsCanvas>().updateProcentage(_color, GetColoredPercentage(_color));
        }
    }

    public void Set_nowColor(Color _color)
    {
        NowColor = _color;


        foreach (GameObject Layer in coloringLayers.Keys)
        {
            Layer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

            if (Layer.name == ColorUtility.ToHtmlStringRGB(NowColor))
            {
                Layer.GetComponent<SpriteRenderer>().color = NowColor;
            }
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                GameObject clickedObject = hitCollider.gameObject;

                spriteRenderer = clickedObject.GetComponent<SpriteRenderer>();
                _sprite = spriteRenderer.sprite;
                originalTexture = _sprite.texture;

                coloring = true;
            }

            return;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (originalTexture != null) coloring_canv.GetComponent<ColorsCanvas>().updateProcentage(NowColor, GetColoredPercentage(NowColor));
            Update_finished_number();

            coloring = true;

            return;
        }
        else if (Input.GetMouseButton(0))
        {
            Debug.Log("else if (Input.GetMouseButton(0))");
            if (originalTexture != null)
            {
                Debug.Log("if (originalTexture != null)");
                if (spriteRenderer.gameObject.name == ColorUtility.ToHtmlStringRGB(NowColor))
                {
                    Debug.Log("ColorUtility.ToHtmlStringRGB(NowColor)");
                    RaycastHit hit;
                    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) { Debug.Log(hit.point); return; }



                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= spriteRenderer.sprite.texture.width;
                    pixelUV.y *= spriteRenderer.sprite.texture.height;

                    Debug.Log(" RaycastHit hit;");

                    DrawPoint((int)pixelUV.x, (int)pixelUV.y);
                }
            }
        }

        if(needUpdateTexture) StartCoroutine(UpdateTexture());
    }
    IEnumerator UpdateTexture()
    {
        originalTexture.SetPixels(coloringLayers.GetValueOrDefault(spriteRenderer.gameObject));
        originalTexture.Apply();

        yield return null;

        _sprite = Sprite.Create(originalTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = _sprite;

        Debug.Log("UpdateTexture");
    }

    public void DrawPoint(int x, int y)
    {
        int pixel = 0;
        int texWidth = spriteRenderer.sprite.texture.width;
        int texHeight = spriteRenderer.sprite.texture.height;

        // draw fast circle: 
        int r2 = brushSize * brushSize;
        int area = r2 << 2;
        int rr = brushSize << 1;
        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - brushSize;
            int ty = (i / rr) - brushSize;

            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4;

                if(coloringLayers.GetValueOrDefault(spriteRenderer.gameObject)[pixel].a > 0.1f)
                {
                    coloringLayers.GetValueOrDefault(spriteRenderer.gameObject)[pixel] = NowColor;

                    needUpdateTexture = true;

                    Debug.Log("needUpdateTexture = true;");
                }
            }
        }
    }

    public float GetColoredPercentage(Color _color)
    {
        Texture2D _texture = null;

        foreach (GameObject layer in coloringLayers.Keys)
        {
            if (layer.name == ColorUtility.ToHtmlStringRGB(_color))
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
                if (pixels[i] == NowColor)
                {
                    coloredPixelCount++;
                }
            }
        }

        float coloredPercentage = (float)((float)((float)coloredPixelCount / (float)usualPixels) * (float)100f);

        return coloredPercentage;
    }

    public void Update_finished_number()
    {
        int standart_pixels_number = 0;
        int colored_pixes_number = 0;

        foreach (GameObject layer in coloringLayers.Keys)
        {
            Color[] pixels = layer.GetComponent<SpriteRenderer>().sprite.texture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0.1f)
                {
                    standart_pixels_number++;
                    foreach (Color _color in _colors)
                    {
                        if (pixels[i] == _color)
                        {
                            colored_pixes_number++;
                            break;
                        }
                    }
                }
            }
        }

        Debug.Log("standart_pixels_number: " + standart_pixels_number.ToString());
        Debug.Log("colored_pixes_number: " + colored_pixes_number.ToString());

        finished_text.text = "Finished by: " + Math.Round(((float)((float)colored_pixes_number / (float)standart_pixels_number) * (float)100f)).ToString() + " %";
    }
}
