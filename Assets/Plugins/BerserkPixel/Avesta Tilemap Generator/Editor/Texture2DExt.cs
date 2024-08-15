using UnityEngine;

namespace BerserkPixel.Tilemap_Generator
{
    public static class Texture2DExt
    {
        public static readonly int PREVIEW_SIZE = 200;

        private static Color _normalColor1 = Color.white;
        private static Color _normalColor2 = Color.grey;

        public static Texture2D GetMapTexture(this MapArray map)
        {
            _normalColor1.a = .5f;
            _normalColor2.a = .5f;

            var texture = new Texture2D(map.Width, map.Height);

            for (var y = 0; y < map.Height; y++)
            for (var x = 0; x < map.Width; x++)
            {
                var color = GetCheckersPattern(x, y);

                if (map[x, y] == MapGeneratorConst.TERRAIN_TILE) color = Color.black;

                texture.SetPixel(x, y, color);
            }

            texture.Apply();

            texture = texture.RotateTexture();
            texture = texture.RotateTexture();

            var targetWidth = Mathf.Min(map.Width, PREVIEW_SIZE);
            var targetHeight = Mathf.Min(map.Height, PREVIEW_SIZE);

            return texture.ResizeTexture(targetWidth, targetHeight);
        }

        // checkers grid pattern
        private static Color GetCheckersPattern(int x, int y)
        {
            Color color;
            if (x % 2 == 1)
            {
                if (y % 2 == 0) color = _normalColor1;
                else color = _normalColor2;
            }
            else
            {
                if (y % 2 == 0) color = _normalColor2;
                else color = _normalColor1;
            }

            return color;
        }

        private static Texture2D ResizeTexture(this Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            var rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            var nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return nTex;
        }

        private static Texture2D RotateTexture(this Texture2D image)
        {
            var target =
                new Texture2D(image.height, image.width, image.format,
                    false); //flip image width<>height, as we rotated the image, it might be a rect. not a square image

            var pixels = image.GetPixels32(0);
            pixels = RotateTextureGrid(pixels, image.width, image.height);
            target.SetPixels32(pixels);
            target.Apply();

            return target;
        }

        private static Color32[] RotateTextureGrid(Color32[] tex, int wid, int hi)
        {
            var ret = new Color32[wid * hi]; //reminder we are flipping these in the target

            for (var y = 0; y < hi; y++)
            for (var x = 0; x < wid; x++)
                ret[hi - 1 - y + x * hi] = tex[x + y * wid]; //juggle the pixels around

            return ret;
        }
    }
}