using System.Collections.Generic;
using UnityEngine;

public class DrowingScene : MonoBehaviour
{
    public GameObject OneLayerPref;
    public Color NowColor;
    public Color colorToColoring;
    public GameObject _gameCanvas;

    public void StartGame(Picture _picture)
    {
        //_gameCanvas.SetActive(true);
        foreach (Sprite layer in _picture.Layers)
        {
            GameObject new_layer = Instantiate(OneLayerPref, new Vector3(0, 0, 0), Quaternion.identity);
            new_layer.name = layer.name;
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
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                firstPress = hit.collider.gameObject;
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
                    Debug.Log(ColorUtility.ToHtmlStringRGB(NowColor));

                    SpriteRenderer spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                    Sprite _sprite = spriteRenderer.sprite;
                    Texture2D originalTexture = _sprite.texture;

                    // Clone the original texture to a new writable texture
                    Texture2D modifiedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
                    modifiedTexture.filterMode = FilterMode.Point;
                    modifiedTexture.wrapMode = TextureWrapMode.Clamp;
                    Graphics.CopyTexture(originalTexture, modifiedTexture);

                    Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(hit.point);
                    Bounds bounds = spriteRenderer.bounds;

                    float u = (localPosition.x - bounds.min.x) / bounds.size.x;
                    float v = (localPosition.y - bounds.min.y) / bounds.size.y;

                    int texWidth = modifiedTexture.width;
                    int texHeight = modifiedTexture.height;

                    int x = Mathf.FloorToInt(u * (texWidth - 1));
                    int y = Mathf.FloorToInt(v * (texHeight - 1));

                    Color pixelColor = modifiedTexture.GetPixel(x, y);
                    if (pixelColor != colorToColoring)
                    {
                        modifiedTexture.SetPixel(x, y, NowColor);
                        modifiedTexture.Apply();

                        Rect spriteRect = _sprite.rect;
                        Vector2 pivot = new Vector2(0.5f, 0.5f);
                        Sprite newSprite = Sprite.Create(modifiedTexture, spriteRect, pivot);

                        spriteRenderer.sprite = newSprite;

                        Debug.Log("colors good");
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
