using System.Collections.Generic;
using UnityEngine;

public class DrowingScene : MonoBehaviour
{
    public GameObject OneLayerPref;

    public Color NowColor;

    public Color colorToColoring;
    public float brushSize = 10f;

    public GameObject _gameCanvas;

    public void StartGame(Picture _picture)
    {
        //_gameCanvas.SetActive(true);

        foreach (Sprite layer in _picture.Layers)
        {
            GameObject new_layer = Instantiate(OneLayerPref, new Vector3(0, 0, 0), Quaternion.identity);
            new_layer.GetComponent<SpriteRenderer>().sprite = layer;

            new_layer.AddComponent<PolygonCollider2D>();
            new_layer.GetComponent<PolygonCollider2D>().autoTiling = true;
            new_layer.GetComponent<PolygonCollider2D>().isTrigger = true;
        }
    }

    public GameObject firstPress;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    firstPress = hit.collider.gameObject;
                }
            }
            return;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            firstPress = null;
            return;
        }

        else if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(ray.origin, ray.direction);

                Color newColor;
                if (ColorUtility.TryParseHtmlString(hit.collider.gameObject.name, out newColor))
                {
                    if (newColor == NowColor) // Assuming NowColor is defined correctly
                    {
                        SpriteRenderer spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                        Sprite _sprite = spriteRenderer.sprite;
                        Texture2D _texture = _sprite.texture;

                        Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(hit.point);
                        Bounds bounds = _sprite.bounds;

                        float u = (localPosition.x - bounds.min.x) / bounds.size.x;
                        float v = (localPosition.y - bounds.min.y) / bounds.size.y;

                        int texWidth = _texture.width;
                        int texHeight = _texture.height;

                        int centerX = Mathf.FloorToInt(u * texWidth);
                        int centerY = Mathf.FloorToInt(v * texHeight);

                        Color[] pixels = _texture.GetPixels();
                        int brushRadius = Mathf.FloorToInt(brushSize);

                        for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
                        {
                            for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
                            {
                                if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                                {
                                    int index = y * texWidth + x;

                                    if (pixels[index] != colorToColoring) pixels[index] = newColor;
                                }
                            }
                        }

                        _texture.SetPixels(pixels);
                        _texture.Apply();

                        _sprite = Sprite.Create(_texture, _sprite.rect, new Vector2(0.5f, 0.5f));
                        spriteRenderer.sprite = _sprite;
                    }
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }

        }
    }
}
