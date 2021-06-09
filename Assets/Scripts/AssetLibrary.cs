using System.Collections.Generic;
using System.Xml.Linq;
using Models.Static;
using UnityEngine;
using Utils;

public static class AssetLibrary
{
    private static readonly Dictionary<string, List<CharacterAnimation>> _Animations =
        new Dictionary<string, List<CharacterAnimation>>();
    
    private static readonly Dictionary<string, List<Sprite>> _Images = new Dictionary<string, List<Sprite>>();

    private static readonly Dictionary<int, ObjectDesc> _Type2ObjectDesc = new Dictionary<int, ObjectDesc>();
    private static readonly Dictionary<int, TileDesc> _Type2TileDesc = new Dictionary<int, TileDesc>();
    
    public static void AddAnimations(Texture2D texture, SpriteSheetData data)
    {
        if (!_Animations.ContainsKey(data.Id))
            _Animations[data.Id] = new List<CharacterAnimation>();

        for (var y = texture.height; y >= 0; y -= data.AnimationHeight)
        {
            for (var x = 0; x < data.AnimationWidth; x += data.AnimationWidth)
            {
                var rect = new Rect(x, y, data.AnimationWidth, data.AnimationHeight);
                var frames = SpriteUtils.CreateSprites(texture, rect, data.ImageWidth, data.ImageHeight);
                var animation = new CharacterAnimation(frames, data.StartFacing);
                
                _Animations[data.Id].Add(animation);
            }
        }
    }

    public static void AddImages(Texture2D texture, SpriteSheetData data)
    {
        if (!_Images.ContainsKey(data.Id))
            _Images[data.Id] = new List<Sprite>();

        var rect = new Rect(0, texture.height, texture.width, texture.height);
        _Images[data.Id] = SpriteUtils.CreateSprites(texture, rect, data.ImageWidth, data.ImageHeight);
    }
    
    public static void ParseXml(XElement xml)
    {
        foreach (var objectXml in xml.Elements("Object"))
        {
            var objectDesc = new ObjectDesc(objectXml);
            _Type2ObjectDesc[objectDesc.Type] = objectDesc;
        }

        foreach (var groundXml in xml.Elements("Ground"))
        {
            var tileDesc = new TileDesc(groundXml);
            _Type2TileDesc[tileDesc.Type] = tileDesc;
        }
    }

    public static Sprite GetImage(string sheetName, int index)
    {
        return _Images[sheetName][index];
    }

    public static CharacterAnimation GetAnimation(string sheetName, int index)
    {
        return _Animations[sheetName][index];
    }

    public static ObjectDesc GetObjectDesc(int type)
    {
        return _Type2ObjectDesc[type];
    }

    public static TileDesc GetTileDesc(int type)
    {
        return _Type2TileDesc[type];
    }
}

public readonly struct SpriteSheetData
{
    public readonly string Id;
    public readonly string SheetName;
    public readonly int AnimationWidth;
    public readonly int AnimationHeight;
    public readonly Facing StartFacing;
    public readonly int ImageWidth;
    public readonly int ImageHeight;

    public SpriteSheetData(XElement xml)
    {
        Id = xml.ParseString("@id");
        SheetName = xml.ParseString("@name", Id);

        var animationSize = xml.ParseIntArray("AnimationSize", "x", new [] {0, 0});
        AnimationWidth = animationSize[0];
        AnimationHeight = animationSize[1];
        StartFacing = xml.ParseEnum("StartDirection", Facing.Right);

        var imageSize = xml.ParseIntArray("ImageSize", "x", new [] {0, 0});
        ImageWidth = imageSize[0];
        ImageHeight = imageSize[1];
    }

    public bool IsAnimation()
    {
        return AnimationWidth != 0 && AnimationHeight != 0;
    }
}

public enum Facing
{
    Up,
    Down,
    Left,
    Right
}