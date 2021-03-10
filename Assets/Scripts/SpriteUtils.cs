using UnityEngine;

public static class SpriteUtils
{
    public static bool IsTransparent(this Sprite sprite)
    {
        for (var y = sprite.rect.y; y < sprite.rect.yMax; y++)
        {
            for (var x = sprite.rect.x; x < sprite.rect.xMax; x++)
            {
                var alpha = sprite.texture.GetPixel((int) x, (int) y).a;
                if (alpha > 0)
                    return false;
            }
        }

        return true;
    }

    public static Sprite Mirror(this Sprite sprite)
    {
        var width = (int)sprite.rect.width;
        var mirrored = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
        mirrored.filterMode = FilterMode.Point;
        for (var y = 0; y < sprite.rect.height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = sprite.texture.GetPixel((int) sprite.rect.x + x, (int) sprite.rect.y + y);
                mirrored.SetPixel(width - x - 1, y, color);
            }
        }
        mirrored.Apply();

        var rect = new Rect(0, 0, sprite.rect.width, sprite.rect.height);
        var pivot = Vector2.right - sprite.pivot / sprite.rect.width;
        var mirroredSprite = Sprite.Create(mirrored, rect, pivot, 8);
        return mirroredSprite;
    }
}