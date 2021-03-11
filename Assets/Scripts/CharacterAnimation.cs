using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CharacterAnimation
{
    private readonly Dictionary<Direction, Dictionary<Action, List<Sprite>>> _directionToAnimation =
        new Dictionary<Direction, Dictionary<Action, List<Sprite>>>();
    
    public CharacterAnimation(Texture2D texture, Rect spriteRect, int imageWidth, int imageHeight,
        Direction startDirection)
    {
        var frames = GetFrames(texture, spriteRect, imageWidth, imageHeight);

        if (startDirection == Direction.Right)
        {
            _directionToAnimation[Direction.Right] = GetDirection(frames, 0, false);
            _directionToAnimation[Direction.Left] = GetDirection(frames, 0, true);
            if (frames.Count >= 14)
            {
                _directionToAnimation[Direction.Down] = GetDirection(frames, 7, false);
                if (frames.Count >= 21)
                {
                    _directionToAnimation[Direction.Up] = GetDirection(frames, 14, false);
                }
            }
        }
        else
        {
            _directionToAnimation[Direction.Down] = GetDirection(frames, 0, false);
            if (frames.Count >= 14)
            {
                _directionToAnimation[Direction.Right] = GetDirection(frames, 7, false);
                _directionToAnimation[Direction.Left] = GetDirection(frames, 7, true);
                if (frames.Count >= 21)
                {
                    _directionToAnimation[Direction.Up] = GetDirection(frames, 14, false);
                }
            }
        }
    }

    public Sprite GetImage(Direction direction, Action action, int frame)
    {
        return _directionToAnimation[direction][action][frame];
    }

    private static List<Sprite> GetFrames(Texture2D texture, Rect spriteRect, int imageWidth, int imageHeight)
    {
        List<Sprite> frames = new List<Sprite>();
        for (var y = spriteRect.y - imageHeight; y >= spriteRect.y - spriteRect.height; y -= imageHeight)
        {
            for (var x = spriteRect.x; x < spriteRect.x + spriteRect.width; x += imageWidth)
            {
                var rect = new Rect(x, y, imageWidth, imageHeight);
                var pivot = new Vector2(0.5f, 0);
                var sprite = Sprite.Create(texture, rect, pivot, 8);

                frames.Add(sprite);
            }
        }

        return frames;
    }

    private static Dictionary<Action, List<Sprite>> GetDirection(List<Sprite> frames, int offset, bool mirror)
    {
        var ret = new Dictionary<Action, List<Sprite>>();
        
        var stand = mirror ? frames[offset].Mirror() : frames[offset];
        var walk1 = mirror ? frames[offset + 1].Mirror() : frames[offset + 1];
        var walk2 = mirror ? frames[offset + 2].Mirror() : frames[offset + 2];
        var attack1 = mirror ? frames[offset + 4].Mirror() : frames[offset + 4];
        var attack2 = frames[offset + 5];
        var attackBit = frames[offset + 6];

        var standAnim = new List<Sprite>
        {
            stand
        };
        ret[Action.Stand] = standAnim;

        var walkAnim = new List<Sprite>
        {
            walk1, 
            walk2.IsTransparent() ? stand : walk2
        };
        ret[Action.Walk] = walkAnim;

        List<Sprite> attackAnim;
        if (attack1.IsTransparent() && attack2.IsTransparent())
        {
            attackAnim = walkAnim;
        }
        else 
        {
            attackAnim = new List<Sprite>
            {
                attack1
            };
            
            if (!attackBit.IsTransparent())
            {
                attack2 = MergeSprites(attack2, attackBit);
            }
            
            attackAnim.Add(mirror ? attack2.Mirror() : attack2);
        }
        ret[Action.Attack] = attackAnim;
        
        return ret;
    }

    private static Sprite MergeSprites(Sprite first, Sprite second)
    {
        var rect = first.rect;
        rect.width += second.rect.width;
        var pivot = new Vector2(0.25f, 0);
        return Sprite.Create(first.texture, rect, pivot, 8);
    }
}

public enum Action
{
    Stand,
    Walk,
    Attack
}