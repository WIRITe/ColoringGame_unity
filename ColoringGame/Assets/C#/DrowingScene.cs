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

            Color _color;
            if (ColorUtility.TryParseHtmlString("#" + layer.name, out _color))
            {
                _colors.Add(_color);
            }
        }

        Debug.Log(_colors);
        _gameCanvas.GetComponent<ColorsCanvas>().setButtons(_colors);
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
                    int brushRadius = Mathf.FloorToInt(brushSize);

                    for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
                    {
                        for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
                        {
                            if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                            {
                                int index = y * texWidth + x;

                                if (pixels[index] == colorToColoring) pixels[index] = NowColor;
                            }
                        }
                    }

                    originalTexture.SetPixels(pixels);
                    originalTexture.Apply();

                    _sprite = Sprite.Create(originalTexture, _sprite.rect, new Vector2(0.5f, 0.5f));
                    spriteRenderer.sprite = _sprite;
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }
    }
}
