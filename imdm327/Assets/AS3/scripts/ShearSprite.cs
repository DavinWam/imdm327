using UnityEngine;

public class ShearSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float shearAmountX = 0.0f;
    public float shearAmountY = 0.0f;

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not assigned.");
            return;
        }

        // Get the original sprite and its texture
        Sprite originalSprite = spriteRenderer.sprite;
        Texture2D originalTexture = originalSprite.texture;

        // Ensure the texture is readable
        if (!originalTexture.isReadable)
        {
            Debug.LogError("Texture is not readable. Please enable 'Read/Write Enabled' in the texture's import settings.");
            return;
        }

        // Get the pixel data as a byte array
        byte[] originalBytes = originalTexture.GetRawTextureData();

        // Apply shear transformation to the byte array
        byte[] shearedBytes = ApplyShear(originalBytes, originalTexture.width, originalTexture.height, shearAmountX, shearAmountY);

        // Create a new texture and load the sheared byte array
        Texture2D shearedTexture = new Texture2D(originalTexture.width, originalTexture.height, originalTexture.format, false);
        shearedTexture.LoadRawTextureData(shearedBytes);
        shearedTexture.Apply();

        // Create a new sprite with the sheared texture
        Sprite shearedSprite = Sprite.Create(shearedTexture, originalSprite.rect, new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit);

        // Assign the new sprite to the SpriteRenderer
        spriteRenderer.sprite = shearedSprite;
    }

    byte[] ApplyShear(byte[] byteArray, int width, int height, float shearX, float shearY)
    {
        int bytesPerPixel = 4; // Assuming RGBA32 format
        byte[] shearedBytes = new byte[byteArray.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate the new position after shearing
                int newX = Mathf.FloorToInt(x + shearX * y);
                int newY = Mathf.FloorToInt(y + shearY * x);

                // Check if the new position is within bounds
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    // Compute byte indices
                    int originalIndex = (y * width + x) * bytesPerPixel;
                    int newIndex = (newY * width + newX) * bytesPerPixel;

                    // Copy pixel bytes (RGBA)
                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        shearedBytes[newIndex + i] = byteArray[originalIndex + i];
                    }
                }
            }
        }

        return shearedBytes;
    }
}
