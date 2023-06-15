using System.Collections.Generic;
using UnityEngine;

public class DrowingScene : MonoBehaviour
{
    public Texture2D textureToColor;

    public List<Color> colors;
    public List<string> colors_str = new List<string>();

    public Color colorToApply = Color.white;
    public Color colorToColoring;
    public float brushSize = 10f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        foreach(Color _color in colors)
        {
            colors_str.Add(ColorUtility.ToHtmlStringRGB(_color));
        }
    }

    public void StartGame(Picture _picture)
    {

    }

    GameObject firstPress;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                firstPress = hit.collider.gameObject;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            firstPress = null;
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider.gameObject == firstPress && colors_str.Contains(hit.collider.gameObject.name))
            {
                Vector2 localPosition = transform.InverseTransformPoint(hit.point);
                Bounds bounds = spriteRenderer.sprite.bounds;

                float u = (localPosition.x - bounds.min.x) / bounds.size.x;
                float v = (localPosition.y - bounds.min.y) / bounds.size.y;

                int texWidth = textureToColor.width;
                int texHeight = textureToColor.height;

                int centerX = Mathf.FloorToInt(u * texWidth);
                int centerY = Mathf.FloorToInt(v * texHeight);

                Color[] pixels = textureToColor.GetPixels();
                int brushRadius = Mathf.FloorToInt(brushSize);

                for (int y = centerY - brushRadius; y <= centerY + brushRadius; y++)
                {
                    for (int x = centerX - brushRadius; x <= centerX + brushRadius; x++)
                    {
                        if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                        {
                            int index = y * texWidth + x;
                            Color newColor;
                            if(ColorUtility.TryParseHtmlString(hit.collider.gameObject.name, out newColor))
                            {
                                if (pixels[index] != colorToColoring) pixels[index] = newColor;
                            }
                        }
                    }
                }

                textureToColor.SetPixels(pixels);
                textureToColor.Apply();

                spriteRenderer.sprite = Sprite.Create(textureToColor, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
            }
        }
    }
}
