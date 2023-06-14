using UnityEngine;

public class DrawingScene : MonoBehaviour
{
    public Color[] colors; // Array of colors that can be used for coloring
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component of the sprite to color

    private Texture2D texture; // Texture used by the sprite
    private bool isTextureUpdated = false; // Flag to track if the texture needs to be updated

    private void Start()
    {
        // Create a new texture with the same dimensions as the sprite's texture
        texture = new Texture2D(
            spriteRenderer.sprite.texture.width,
            spriteRenderer.sprite.texture.height
        );

        // Assign the pixels from the sprite's texture to the new texture
        texture.SetPixels(spriteRenderer.sprite.texture.GetPixels());
        texture.Apply();

        // Assign the new texture to the sprite renderer
        spriteRenderer.sprite = Sprite.Create(
            texture,
            spriteRenderer.sprite.rect,
            new Vector2(0.5f, 0.5f),
            spriteRenderer.sprite.pixelsPerUnit
        );
    }

    private void Update()
    {
        // Check for mouse click or drag
        if (Input.GetMouseButton(0))
        {
            // Convert mouse position to texture coordinates
            Vector2 textureCoords = GetMouseTextureCoordinates();

            // Convert texture coordinates to pixel position
            Vector2Int pixelPosition = new Vector2Int(
                Mathf.RoundToInt(textureCoords.x * texture.width),
                Mathf.RoundToInt(textureCoords.y * texture.height)
            );

            // Color the area around the clicked position
            Color currentColor = GetCurrentColor();
            int brushSize = 5; // Adjust the brush size as desired

            // Get the pixels within the brush area
            Color[] pixels = texture.GetPixels(
                pixelPosition.x - brushSize,
                pixelPosition.y - brushSize,
                brushSize * 2 + 1,
                brushSize * 2 + 1
            );

            // Apply the desired color to each pixel
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = currentColor;
            }

            // Set the modified pixels back to the texture
            texture.SetPixels(
                pixelPosition.x - brushSize,
                pixelPosition.y - brushSize,
                brushSize * 2 + 1,
                brushSize * 2 + 1,
                pixels
            );

            // Mark the texture as updated
            isTextureUpdated = true;
        }

        // Apply the texture updates if needed
        if (isTextureUpdated)
        {
            texture.Apply();
            isTextureUpdated = false;
        }
    }

    // Get the current selected color
    private Color GetCurrentColor()
    {
        if (colors.Length > 0)
        {
            return colors[0];
        }
        return Color.white; // Default color if no colors are available
    }

    // Convert mouse position to texture coordinates
    private Vector2 GetMouseTextureCoordinates()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Normalize the screen coordinates to the range [0, 1]
        Vector2 normalizedCoords = new Vector2(
            mousePosition.x / Screen.width,
            mousePosition.y / Screen.height
        );

        // Convert the normalized screen coordinates to texture coordinates
        return new Vector2(
            normalizedCoords.x * spriteRenderer.sprite.rect.width / spriteRenderer.sprite.texture.width,
            normalizedCoords.y * spriteRenderer.sprite.rect.height / spriteRenderer.sprite.texture.height
        );
    }
}
